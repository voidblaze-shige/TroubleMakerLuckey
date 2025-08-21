using System;
using System.Linq;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour {

	[SerializeField]
	private ItemDataBase itemDataBase;

	private const string itemGetPopupNodePath = "Item/ItemGet";
	private const string popupTextPath = "MenuBack/TextBack/Text";
	private const string popupImagePath = "MenuBack/Image";
	private const string inventoryNodePath = "Item/InvBox";
	private const string inventoryButtonPath = "Button";
	private const string inventoryFramePath = "Frame";
	private const string inventoryNumberPath = "numbg/Text";
	private const string empty_name = "999";
	private const int inventory_max = 6;

	private const int initNumber = 1;
	private static int useItemNum = 0;
	private static int selectedItemNum = 0;

	private GameObject selectItemObj;
	private TrainingManager training_manager;
	private Text useNumText;
	private Sort sort;

	private List<int> giftItemNumberList = new List<int>();

	public DetailObjList detailobjectlist;
	// 操作するアイテム詳細オブジェクトリスト
	[System.SerializableAttribute]
	public class DetailObjList {
		public GameObject imageObj;
		public GameObject nameObj;
		public GameObject textObj;
		public GameObject useNumObj;
	}

	// アイテムを表示する親
	public GameObject itemParent;
	// アイテム詳細を表示するオブジェクト
	public GameObject detailObj;
	// アイテム取得画面
	public GameObject canvas;

	void Start () {
		// アイテムの在庫を取得
		GameInformation.InventoryArray = PlayerPrefsX.GetIntArray(GameInformation.INVENTORY_KEY, 0, 6);

		// アイテム画面に反映
		CreateInventoryNode();
	}

	// アイテム詳細の使用アイテム数の変更
	public void ChangeNumber (bool flag) {
		if (selectedItemNum > 0) {
			// 在庫取得
			int num = GameInformation.InventoryArray[selectedItemNum-1];

			// 増減フラグ trueで増やす
			if (flag) {
				if (useItemNum < num) {
					AudioManager.Instance.PlaySE(AUDIO.SE_CLICK_UP);
					useItemNum++;
				} else {
					AudioManager.Instance.PlaySE(AUDIO.SE_BUZZER);
				}
			} else {
				if (useItemNum > 1) {
					AudioManager.Instance.PlaySE(AUDIO.SE_CLICK_UP);
					useItemNum--;
				} else {
					AudioManager.Instance.PlaySE(AUDIO.SE_BUZZER);
				}
			}
			useNumText.text = useItemNum.ToString();
		} else {
			AudioManager.Instance.PlaySE(AUDIO.SE_BUZZER);
		}
	}

	// アイテム使用確認画面
	public void OpenItemConfirm(GameObject obj) {
		if (selectedItemNum > 0) {
			AudioManager.Instance.PlaySE(AUDIO.SE_OPEN);
			obj.SetActive(true);
		} else {
			AudioManager.Instance.PlaySE(AUDIO.SE_BUZZER);
		}
	}

	// アイテム使用ボタンの処理
	public void UseItem() {
		if (selectedItemNum > 0) {
			AudioManager.Instance.PlaySE(AUDIO.SE_CLICK_UP);

			// 選択中のアイテム情報を取得
			Item item = GetItem(selectedItemNum);

			// 使用した分を減らす
			int currentNumber = GameInformation.InventoryArray[selectedItemNum - 1] - useItemNum;

			if (currentNumber < 0) {
				// あり得ないが念のため
				currentNumber = 0;
				Debug.Log("アイテム数がマイナスになっています");
			}

			SetItemNumber(selectItemObj, currentNumber);

			// 現在の在庫を保存
			GameInformation.InventoryArray[selectedItemNum - 1] = currentNumber;
			PlayerPrefsX.SetIntArray(GameInformation.INVENTORY_KEY, GameInformation.InventoryArray);
			// パラメータ処理
			if (training_manager == null) {
				training_manager = this.gameObject.GetComponent<TrainingManager>();
			}
			training_manager.GrowUp(item.GetIncreaseValue()*useItemNum , item.GetKindOfType());

			AudioManager.Instance.PlaySE(AUDIO.SE_PARAMETER_INCREASE);

			//もし在庫が0場合は画像削除
			if (currentNumber == 0) {
				RemoveSelectItem();
				selectItemObj = null;
				selectedItemNum = 0;
				// 詳細画面の初期化
				InitDetail();
			} else {
				useNumText.text = initNumber.ToString();
				useItemNum = initNumber;
			}

		} else {
			AudioManager.Instance.PlaySE(AUDIO.SE_BUZZER);
		}
	}

	// アイテムボタンを選択したとき
	public void SelectItem(GameObject obj) {
		selectItemObj = obj.transform.parent.gameObject;
		// アイテムの番号
		int num = int.Parse(selectItemObj.name);
		selectedItemNum = num;
		if (num != 0) {
			SetDetail(num);
		}
	}

	// アイテム詳細を表示
	private void SetDetail(int num) {
		Image detailImage = detailobjectlist.imageObj.GetComponent<Image>();
		Text detailText = detailobjectlist.textObj.GetComponent<Text>();
		Text nameText = detailobjectlist.nameObj.GetComponent<Text>();
		if (useNumText == null) {
			useNumText = detailobjectlist.useNumObj.GetComponent<Text>();
		}

		Item item = GetItem(num);
		detailImage.sprite = item.GetIcon();
		nameText.text = item.GetItemName();
		detailText.text = item.GetInformation();
		useNumText.text = initNumber.ToString();
		useItemNum = initNumber;

		detailobjectlist.imageObj.SetActive(true);
	}

	// アイテム詳細画面を初期化
	public void InitDetail() {
		Text detailText = detailobjectlist.textObj.GetComponent<Text>();
		Text nameText = detailobjectlist.nameObj.GetComponent<Text>();
		if (useNumText == null) {
			useNumText = detailobjectlist.useNumObj.GetComponent<Text>();
		}

		nameText.text = "";
		detailText.text = "";
		useNumText.text = (initNumber - 1).ToString();
		useItemNum = initNumber - 1;

		detailobjectlist.imageObj.SetActive(false);
	}

	private void CreateInventoryNode() {
		for (int num = 0; num < inventory_max; num++) {
			GameObject node = Instantiate(Resources.Load<GameObject>(inventoryNodePath));

			node.transform.SetParent(itemParent.transform, false);

			// 在庫あり
			if (GameInformation.InventoryArray[num] > 0) {
				node.name = (num + 1).ToString();
				Item item = GetItem(num+1);
				GameObject button = node.transform.Find(inventoryButtonPath).gameObject;
				GameObject frame = node.transform.Find(inventoryFramePath).gameObject;
				Image buttonImage = button.GetComponent<Image>();

				buttonImage.sprite = item.GetIcon();
				SetItemNumber(node, GameInformation.InventoryArray[num]);
				frame.SetActive(false);
				button.SetActive(true);
			} else {
				// 在庫なし
				node.name = empty_name;
			}
			node.SetActive(true);
		}
		// ソート
		if (sort == null) {
			sort = this.gameObject.GetComponent<Sort>();
		}
		sort.SortItem(itemParent);
	}

	private void RemoveSelectItem() {
		RemoveItem(selectItemObj);
		AddEmptyFrame();
	}

	private void RemoveItem(GameObject obj) {
		if (obj != null) {
			Destroy(obj);
		}
	}

	private void SetItemNumber(GameObject obj, int num) {
	 	GameObject number = obj.transform.Find(inventoryButtonPath + "/" + inventoryNumberPath).gameObject;
		Text numberText = number.GetComponent<Text>();
		numberText.text = num.ToString();
	}

	private void AddEmptyFrame() {
		GameObject node = Instantiate(Resources.Load<GameObject>(inventoryNodePath));
		node.transform.SetParent(itemParent.transform, false);
		// 在庫なし
		node.name = empty_name;
	}

	// アイテムを追加 (アイテム番号)
	public void AddItem(int num, int value = 1) {
		int inventoryNum = GameInformation.InventoryArray[num-1];
		// アイテム上限を越さない
		if (GameInformation.InventoryArray[num-1] + value < 99) {
			GameInformation.InventoryArray[num-1] += value;
		} else {
			GameInformation.InventoryArray[num-1] = 99;
		}

		// 所持していないアイテムを取得した場合
		if (inventoryNum == 0) {
			SetItemNode(num);
		} else {
			// アイテムの所持数のテキストを更新
			GameObject node = itemParent.transform.Find(num.ToString()).gameObject;
			SetItemNumber(node, GameInformation.InventoryArray[num-1]);
		 }
		PlayerPrefsX.SetIntArray(GameInformation.INVENTORY_KEY, GameInformation.InventoryArray);
	}

	// 指定したアイテム数に設定する
	public void SetConstantNumOfInventory(int num, int value) {
		int inventoryNum = GameInformation.InventoryArray[num-1];
		GameInformation.InventoryArray[num-1] = value;
		// 所持していないアイテムを取得した場合
		if (inventoryNum == 0) {
			SetItemNode(num);
		} else {
			// アイテムの所持数のテキストを更新
			GameObject node = itemParent.transform.Find(num.ToString()).gameObject;
			SetItemNumber(node, GameInformation.InventoryArray[num-1]);
		}
		PlayerPrefsX.SetIntArray(GameInformation.INVENTORY_KEY, GameInformation.InventoryArray);
	}

	private void SetItemNode(int num) {
		GameObject node = Instantiate(Resources.Load<GameObject>(inventoryNodePath));
		node.transform.SetParent(itemParent.transform, false);
		node.name = num.ToString();
		Item item = GetItem(num);
		GameObject button = node.transform.Find(inventoryButtonPath).gameObject;
		GameObject frame = node.transform.Find(inventoryFramePath).gameObject;
		Image buttonImage = button.GetComponent<Image>();

		buttonImage.sprite = item.GetIcon();
		frame.SetActive(false);
		button.SetActive(true);
		SetItemNumber(node, GameInformation.InventoryArray[num-1]);

		GameObject empty_frame = itemParent.transform.Find(empty_name).gameObject;
		if (empty_frame != null) {
			// 空きフレームを削除
			RemoveItem(empty_frame);
		}
		node.SetActive(true);
		// ソート
		if (sort == null) {
			sort = this.gameObject.GetComponent<Sort>();
		}
		sort.SortItem(itemParent);
	}

	// Get item information by item number
	public Item GetItem(int searchNum) {
	 return itemDataBase.GetItemLists().Find(number => number.GetNumber() == searchNum);
	}

	// プレゼントアイテムリストをゲット
	private List<int> GetGiftItemNumberList() {
		List<int> list = new List<int>();
		List<Item> itemList = itemDataBase.GetItemLists();
		foreach(Item item in itemList) {
			if ( !item.GetSpecialFlag() ) {
					list.Add(item.GetNumber());
			}
		}
		return list;
	}

	// ランダムなプレゼントアイテム番号を取得
	public int GetRandamGiftItemNumber() {
		// リストが空のとき
		if (!giftItemNumberList.Any()){
			giftItemNumberList = GetGiftItemNumberList();
		}
		giftItemNumberList = giftItemNumberList.OrderBy(x=>Guid.NewGuid()).ToList();
		return giftItemNumberList[0];
	}

	public void GetGiftItem() {
		// アイテム取得
		AddItem(GameInformation.GiftItemNumber);

		// プレゼント開封フラグ
		GameInformation.GiftOpenFlag = true;
		SaveData.SetBool(GameInformation.GIFT_OPEN_FLAG_KEY, GameInformation.GiftOpenFlag);

		// プレゼント月削除
		for(int i = 0; i < GameInformation.GiftAppearanceMonthArray.Length; i++) {
			if (GameInformation.GiftAppearanceMonthArray[i] == PlayerPrefs.GetInt(GameInformation.GAME_DATE_KEY, 1)) {
				GameInformation.GiftAppearanceMonthArray[i] = 0;
				PlayerPrefsX.SetIntArray(GameInformation.GIFT_MONTH_KEY, GameInformation.GiftAppearanceMonthArray);
				break;
			}
		}

		// ポップアップ表示
		ShowItemGetPopup(GameInformation.GiftItemNumber);
		GameInformation.GiftItemNumber = 0;

		AudioManager.Instance.PlaySE(AUDIO.SE_NICE);
	}

	private void ShowItemGetPopup(int num) {
		GameObject popup = Instantiate(Resources.Load<GameObject>(itemGetPopupNodePath));
		popup.transform.SetParent(canvas.transform, false);
		// テキストにアイテム名を入れる
		// アイテム画像を入れる
		GameObject textObj = popup.transform.Find(popupTextPath).gameObject;
		GameObject imageObj = popup.transform.Find(popupImagePath).gameObject;
		Text text = textObj.GetComponent<Text>();
		Image image = imageObj.GetComponent<Image>();
		Item item = GetItem(num);
		text.text = "「" + item.GetItemName() + "」を入手しました！";
		image.sprite = item.GetImage();

		popup.SetActive(true);
	}
}
