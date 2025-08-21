using System.Linq;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class ShopManager : MonoBehaviour {

	[SerializeField]
	private ShopItemDataBase shopItemDataBase;

	public ItemManager item_manager;
	public List<GameObject> itemList;
	public GameObject canvas;
	public Sprite purchasedImage;

	private const string itemGetPopupNodePath = "Item/ItemGet";
	private const string popupTextPath = "MenuBack/TextBack/Text";
	private const string popupImagePath = "MenuBack/Image";
	private const string purchasedPath = "ItemImage/Purchased";
	private const string buttonPath = "Button";

	void Awake() {
		// 購入済みの項目を押下できないようにする
		SetDisableItems();
	}

	public void ShowItemGetPopup(int num) {
		GameObject popup = Instantiate(Resources.Load<GameObject>(itemGetPopupNodePath));
		popup.transform.SetParent(canvas.transform, false);
		// テキストにアイテム名を入れる
		// アイテム画像を入れる
		GameObject textObj = popup.transform.Find(popupTextPath).gameObject;
		GameObject imageObj = popup.transform.Find(popupImagePath).gameObject;
		Text text = textObj.GetComponent<Text>();
		Image image = imageObj.GetComponent<Image>();
		ShopItem item = GetShopItem(num);
		text.text = "「" + item.GetItemName() + "」を入手しました！";
		image.sprite = item.GetImage();

		popup.SetActive(true);
		AudioManager.Instance.PlaySE(AUDIO.SE_NICE);
	}

	// 育成アイテムパック取得処理
	public void SetSPItem() {
		// アイテム数を5個に設定
		item_manager.SetConstantNumOfInventory(1, 5);
		item_manager.SetConstantNumOfInventory(2, 5);
		item_manager.SetConstantNumOfInventory(3, 5);
	}

	public void InitSPItem() {
		GameInformation.InventoryArray[0] = 5;
		GameInformation.InventoryArray[1] = 5;
		GameInformation.InventoryArray[2] = 5;
		PlayerPrefsX.SetIntArray(GameInformation.INVENTORY_KEY, GameInformation.InventoryArray);
	}

	public void SetDisableItems() {
		if (SaveData.GetBool(GameInformation.SHOP_FULL_PACK01_KEY, false)) {
			GameObject button1 = itemList[0].transform.Find(buttonPath).gameObject;
			GameObject button2 = itemList[1].transform.Find(buttonPath).gameObject;
			GameObject button3 = itemList[2].transform.Find(buttonPath).gameObject;
			button1.GetComponent<Button>().interactable = false;
			button1.GetComponent<Image>().sprite = purchasedImage;
			itemList[0].transform.Find(purchasedPath).gameObject.SetActive(true);
			button2.GetComponent<Button>().interactable = false;
			button2.GetComponent<Image>().sprite = purchasedImage;
			itemList[1].transform.Find(purchasedPath).gameObject.SetActive(true);
			button3.GetComponent<Button>().interactable = false;
			button3.GetComponent<Image>().sprite = purchasedImage;
			itemList[2].transform.Find(purchasedPath).gameObject.SetActive(true);
		} else {
			// アイテム
			if (SaveData.GetBool(GameInformation.SHOP_ITEM_PACK01_KEY, false)) {
				GameObject button1 = itemList[0].transform.Find(buttonPath).gameObject;
				GameObject button2 = itemList[1].transform.Find(buttonPath).gameObject;
				button2.GetComponent<Button>().interactable = false;
				button2.GetComponent<Image>().sprite = purchasedImage;
				itemList[1].transform.Find(purchasedPath).gameObject.SetActive(true);
				// いずれか購入するとフルパックは買えない
				button1.GetComponent<Button>().interactable = false;
				button1.GetComponent<Image>().sprite = purchasedImage;
//				itemList[0].transform.Find(purchasedPath).gameObject.SetActive(true);
			}

			// メガホン
			if (SaveData.GetBool(GameInformation.SHOP_MEGAPHONE_PACK01_KEY, false)) {
				GameObject button1 = itemList[0].transform.Find(buttonPath).gameObject;
				GameObject button2 = itemList[1].transform.Find(buttonPath).gameObject;
				GameObject button3 = itemList[2].transform.Find(buttonPath).gameObject;

				button3.GetComponent<Button>().interactable = false;
				button3.GetComponent<Image>().sprite = purchasedImage;
				itemList[2].transform.Find(purchasedPath).gameObject.SetActive(true);
				// いずれか購入するとフルパックは買えない
				button1.GetComponent<Button>().interactable = false;
				button1.GetComponent<Image>().sprite = purchasedImage;
//				itemList[0].transform.Find(purchasedPath).gameObject.SetActive(true);
			}
		}
	}

	private ShopItem GetShopItem(int searchNum) {
		return shopItemDataBase.GetShopItemLists().Find(number => number.GetNumber() == searchNum);
	}
}
