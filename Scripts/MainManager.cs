using System;
﻿using System.Linq;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utage;

public class MainManager : MonoBehaviour {

	[SerializeField]
	private TroubleEventDataBase troubleEventDataBase;

	AdvEngine Engine { get { return engine ?? (engine = FindObjectOfType<AdvEngine>()); } }
	public AdvEngine engine; // utage
	public GameObject playScreen; // UI
	public GameObject parameter; // パラメータゲージ
	public GameObject dateTextObj; // 日付
	public GameObject luckyParent; // ラッキーの親
	public GameObject troubleItemParent; // 固定トラブルイベントの親
	public GameObject canvas;
	public GiftObjList giftObjList;
	public GameObject whiteBack;
	public FadeScreen fadeScreen;
	public CanvasGroup furnitureGroup; // メイン画面のタップ画像の親
	public ShopManager shop_manager;

 // プレゼントオブジェクト
	[System.SerializableAttribute]
	public class GiftObjList {
		public GameObject giftObject;
		public GameObject giftbox;
		public Animation giftboxAnim;
		public ParticleSystem giftEffect;
	}

	private GameObject morningObj;
	private GameObject lucky;
	private GameObject troubleItem;
	private GameObject namePanel;

	private Text morningText;
	private Text dateText;
	private TrainingManager training_manager;
	private FurnitureManager furniture_manager;
	private Notice notice;
	private bool gift_flag = false;

	private const string morningPath = "Morning/MorningPanel";
	private const string giftboxPath = "gift";
	private const string giftEffectPath = "effect";
	private	const string luckyPath = "Lucky/";
	private const string troubleEventItemPath = "TroubleItem/";
	private const string dateCntText = "ヶ月目";
	private const string label1 = "プロローグ";
	private	const string label2 = "シナリオ";
	private	const string label3 = "トラブル";

	private CanvasGroup playScreenGroup;

	void Awake() {
		// テストモード
		if (TestSenario.isTestMode) {
				GameObject test = Instantiate(Resources.Load<GameObject>("Test/TestPanel"));
				test.transform.SetParent(canvas.transform, false);
				test.name = "TestPanel";
				test.SetActive(true);
		} else {
				if (!SaveData.GetBool(GameInformation.IS_STARTED_KEY, false)) {
					// 課金アイテムを持っていた場合
					if (SaveData.GetBool(GameInformation.SHOP_ITEM_PACK01_KEY, false)) {
						// ５個に初期化
						shop_manager.InitSPItem();
					}
					GameInformation.GAME_DATE = 1;
					GameInformation.STORY_PROGRESS = 0;
					GameInformation.INT = 0;
					GameInformation.BODY = 0;
					GameInformation.LIKE = 0;
					PlayerPrefs.SetInt(GameInformation.INT_KEY, GameInformation.INT);
					PlayerPrefs.SetInt(GameInformation.BODY_KEY, GameInformation.BODY);
					PlayerPrefs.SetInt(GameInformation.LIKE_KEY, GameInformation.LIKE);
					PlayerPrefs.SetInt(GameInformation.GAME_DATE_KEY, GameInformation.GAME_DATE);
					PlayerPrefs.SetInt(GameInformation.STORY_PROGRESS_KEY, GameInformation.STORY_PROGRESS);
					PlayerPrefs.Save();
				}	else {
					// If already playing data exist
					GameInformation.GAME_DATE = PlayerPrefs.GetInt(GameInformation.GAME_DATE_KEY, 1);
					GameInformation.STORY_PROGRESS = PlayerPrefs.GetInt(GameInformation.STORY_PROGRESS_KEY, 0);
				}
		}
	}

	void Start () {
		// テストモード以外
		if (TestSenario.isTestMode) {
			playScreen.SetActive(false);
			parameter.SetActive(false);
		} else {
			InitGame();
		}
	}

	private void InitGame() {
		if (!SaveData.GetBool(GameInformation.IS_STARTED_KEY, false)) {
			namePanel = Instantiate(Resources.Load<GameObject>("UI/NamePanel"));
			namePanel.transform.SetParent(canvas.transform, false);
			namePanel.SetActive(true);
		} else {
			if (PlayerPrefs.GetInt(GameInformation.STORY_PROGRESS_KEY, 0) == 0) {
					StartPlorogue();
			} else {
				// 家具をセット
				if (furniture_manager == null) {
					furniture_manager = this.gameObject.GetComponent<FurnitureManager>();
				}
				furniture_manager.SetAllFurniture();
				if (PlayerPrefs.GetInt(GameInformation.GAME_DATE_KEY, 1) > 3) {
					GameInformation.SpecialTroubleEventNum = PlayerPrefs.GetInt(GameInformation.SPECIAL_TROUBLE_EVENT_NUM_KEY, 0);
					GameInformation.SpecialTroubleEventFlag = SaveData.GetBool(GameInformation.SPECIAL_TROUBLE_EVENT_FLAG_KEY, false);
				}
				GameInformation.IsSleeping = SaveData.GetBool(GameInformation.IS_SLEEPING_KEY, false);
				// おやすみフラグがFalseのとき
				if (!SaveData.GetBool(GameInformation.IS_SLEEPING_KEY, false)) {
					// 初日以外でトラブルイベント未実行の場合
					if (PlayerPrefs.GetInt(GameInformation.GAME_DATE_KEY, 1) > 1 &&
						PlayerPrefs.GetInt(GameInformation.GAME_DATE_KEY, 1) != PlayerPrefs.GetInt(GameInformation.TROUBLE_EVENT_PROGRESS_KEY, 0)) {
						StartCoroutine(TroubleCoTalk());
					// 育成完了済みの場合
					} else if (PlayerPrefs.GetInt(GameInformation.GAME_DATE_KEY, 1) == PlayerPrefs.GetInt(GameInformation.TRAINING_PROGRESS_KEY, 0)) {
						StartCoroutine(StoryCoTalk());
					} else {
						InitMainContents();
					}
				}
			}
		}
		InsertTextDate();
	}

	public IEnumerator StartSenario(string label) {

		fadeScreen.Fadeout();

    yield return new WaitForSeconds(1);

		if (namePanel != null) {
			Destroy(namePanel);
		}

		whiteBack.SetActive(true);

    yield return new WaitForSeconds(0.5f);

		Engine.JumpScenario(label);

		while(!Engine.IsEndScenario) {
			yield return 0;
		}

		Engine.Config.IsSkip = false;
		whiteBack.SetActive(false);
	}

	// ストーリーイベント共通処理
	public IEnumerator StoryCoTalk() {
		playScreen.SetActive(false);
		parameter.SetActive(false);

		// タップ画像を無効化
		furnitureGroup.blocksRaycasts = false;
		// 通知がある場合消す
		GameInformation.isTalkingStart = true;

		AudioManager.Instance.FadeOutBGM (1);

		int num = PlayerPrefs.GetInt(GameInformation.STORY_PROGRESS_KEY, 0);

		if (num > 12) {
			Debug.Log("STORY_PROGRESSが12を超えています");
		}

		if (!GameInformation.JumpScenarioDate.Contains(num)) {
			string label;

			if (num == 0) {
				label = label1;
			} else {
				switch (num) {
					// 7ヶ月目
					case 7:
						engine.Param.TrySetParameter("kimochi", PlayerPrefs.GetInt(GameInformation.LIKE_KEY, 0));
						break;
					// 12ヶ月目
					case 12:
						EndingManager ending_manager = this.gameObject.GetComponent<EndingManager>();
						engine.Param.TrySetParameter("ending6", ending_manager.IsAchievedEnding6());
						break;
					default:
						break;
				}
				label = label2 + num.ToString();
			}

			yield return StartSenario(label);

			while(!Engine.IsEndScenario) {
				yield return 0;
			}

			// 訪問販売が来たときの処理
			if (PlayerPrefs.GetInt(GameInformation.STORY_PROGRESS_KEY, 0) == GameInformation.SalesPersonMonth) {
				GameInformation.TroubleEventItemNum = (int)engine.Param.GetParameter("trouble_item");
				// もしパラメータイベントに関連するアイテムを取得した場合
				if (GameInformation.TroubleEventItemNum == GameInformation.HunterNum) {
					// パラメータイベントを追加する
					if (training_manager == null) {
						training_manager = this.gameObject.GetComponent<TrainingManager>();
					}
					training_manager.AddParameterEventNodeByTroubleItemNumber(GameInformation.HunterNum);
					GameInformation.SpecialTroubleEventFlag = true;
				} else if (GameInformation.TroubleEventItemNum == GameInformation.HaniwaNum) {
					// パラメータイベントを追加する
					if (training_manager == null) {
						training_manager = this.gameObject.GetComponent<TrainingManager>();
					}
					training_manager.AddParameterEventNodeByTroubleItemNumber(GameInformation.HaniwaNum);
					GameInformation.SpecialTroubleEventFlag = true;
				}
			}
		}

		// エンディング
		if (PlayerPrefs.GetInt(GameInformation.GAME_DATE_KEY, 1) == GameInformation.LAST_DAY) {
			playScreen.SetActive(false);
			parameter.SetActive(false);
			EndingManager ending_manager = this.gameObject.GetComponent<EndingManager>();
			ending_manager.StartEnding();

		} else {
			if (PlayerPrefs.GetInt(GameInformation.STORY_PROGRESS_KEY, 0) > 0) {
				// プロローグ以降

				// おやすみフラグをたてる
				GameInformation.IsSleeping = true;

				PlayerPrefs.SetInt(GameInformation.TROUBLE_EVENT_ITEM_NUM_KEY, GameInformation.TroubleEventItemNum);
				PlayerPrefs.SetInt(GameInformation.STORY_PROGRESS_KEY, PlayerPrefs.GetInt(GameInformation.STORY_PROGRESS_KEY, 0) + 1);
				SaveData.SetBool(GameInformation.SPECIAL_TROUBLE_EVENT_FLAG_KEY, GameInformation.SpecialTroubleEventFlag);
				SaveData.SetBool(GameInformation.IS_SLEEPING_KEY, GameInformation.IsSleeping);
				PlayerPrefs.Save();

			} else {
				// プロローグ
				playScreen.SetActive(true);
				parameter.SetActive(true);
				// メインBGMを流す
				StartMainBGM();

				StartCoroutine(ShowTutorial());
			}
		}
	}

	private IEnumerator ShowTutorial() {
		PlayerPrefs.SetInt(GameInformation.STORY_PROGRESS_KEY, PlayerPrefs.GetInt(GameInformation.STORY_PROGRESS_KEY, 0) + 1);
		PopUpManager popupmanager = this.gameObject.GetComponent<PopUpManager>();

		yield return new WaitForSeconds(0.8f);

		GameInformation.first_tutorial_flag = true;
		AudioManager.Instance.PlaySE(AUDIO.SE_OPEN);
		popupmanager.OpenTutorial();
	}

  // トラブルイベント共通処理
	private IEnumerator TroubleCoTalk() {
		int troubleNum = GetTroubleEventNumber();
		playScreen.SetActive(false);
		parameter.SetActive(false);

		if (GameInformation.special_ending_flag) {
			// エンディング
			EndingManager ending_manager = this.gameObject.GetComponent<EndingManager>();
			if (ending_manager.StartEnding() == 0) {
				PlayerPrefs.SetInt(GameInformation.SPECIAL_TROUBLE_EVENT_NUM_KEY, GameInformation.SpecialTroubleEventNum);
				// エンディング条件を満たさなかった場合、再度トラブル番号を取得
				troubleNum = GetTroubleEventNumber();
			} else {
				// エンディングを迎えた場合、番号は0にする
				troubleNum = 0;
			}
		}

		if (troubleNum > 0) {

			AudioManager.Instance.FadeOutBGM (1);

			string label = label3 + troubleNum.ToString();

			yield return StartSenario(label);

			// パラメータ増減
			TroubleEvent te = GetTroubleEvent(troubleNum);
			int selectNum = (int)engine.Param.GetParameter("select_num");
			List<int> list = te.GetValueList(selectNum);

			parameter.SetActive(true);
			yield return new WaitForSeconds(0.3f);

			int value = 0;
			int sum = 0;

			for (int type = 1; type <= 3; type++) {
				value = list[type-1];
				sum += value;

				if (training_manager == null) {
					training_manager = this.gameObject.GetComponent<TrainingManager>();
				}
				training_manager.GrowUp(value, type);
			}
			// セーブ
			GameInformation.TroubleEventProgress = PlayerPrefs.GetInt(GameInformation.GAME_DATE_KEY, 1);
			PlayerPrefsX.SetIntArray(GameInformation.TROUBLE_EVENT_LIST_KEY, GameInformation.TroubleEventList.ToArray());
			PlayerPrefs.SetInt(GameInformation.TROUBLE_EVENT_PROGRESS_KEY, GameInformation.TroubleEventProgress);
			PlayerPrefs.SetInt(GameInformation.SPECIAL_TROUBLE_EVENT_NUM_KEY, GameInformation.SpecialTroubleEventNum);
			SaveData.SetBool(GameInformation.SPECIAL_TROUBLE_EVENT_FLAG_KEY, GameInformation.SpecialTroubleEventFlag);

			// パラメータが上がった場合、下がった場合のSE
			if (sum > 0) {
				AudioManager.Instance.PlaySE(AUDIO.SE_PARAMETER_TROUBLE_UP);
			} else if (sum < 0 ) {
				AudioManager.Instance.PlaySE(AUDIO.SE_PARAMETER_TROUBLE_DOWN);
			}

			if (sum != 0) {
				yield return new WaitForSeconds(0.8f);
			}
			InitMainContents();
		}
	}

	// トラブルイベントの番号を取得
	private int GetTroubleEventNumber() {
		int num = 1;

		// リスト作成
		SetRandomTroubleEventList();

		// 固定トラブルイベントが起きるかどうか
 		// バイヤー
		if (PlayerPrefs.GetInt(GameInformation.TROUBLE_EVENT_ITEM_NUM_KEY, 0) == GameInformation.PrincessNum
		 	&& ((PlayerPrefs.GetInt(GameInformation.GAME_DATE_KEY, 1) - GameInformation.SalesPersonMonth) % 2 == 1)
			&& PlayerPrefs.GetInt(GameInformation.SPECIAL_TROUBLE_EVENT_NUM_KEY, 0) < (CalcTroubleEventNumber(GameInformation.PrincessNum) + 3)) {

			if (PlayerPrefs.GetInt(GameInformation.SPECIAL_TROUBLE_EVENT_NUM_KEY, 0) == 0) {
				num = GameInformation.SpecialTroubleEventNum = CalcTroubleEventNumber(GameInformation.PrincessNum);
			} else {
				num = GameInformation.SpecialTroubleEventNum = PlayerPrefs.GetInt(GameInformation.SPECIAL_TROUBLE_EVENT_NUM_KEY, 0) + 1;
				if (num == (CalcTroubleEventNumber(GameInformation.PrincessNum) + 3)) {
					// エンディング
					GameInformation.special_ending_flag = true;
				}
			}
			// ハンター
		}  else if (PlayerPrefs.GetInt(GameInformation.TROUBLE_EVENT_ITEM_NUM_KEY, 0) == GameInformation.HunterNum && SaveData.GetBool(GameInformation.SPECIAL_TROUBLE_EVENT_FLAG_KEY, false)
			&& PlayerPrefs.GetInt(GameInformation.SPECIAL_TROUBLE_EVENT_NUM_KEY, 0) < (CalcTroubleEventNumber(GameInformation.HunterNum) + 3)) {
			if (PlayerPrefs.GetInt(GameInformation.SPECIAL_TROUBLE_EVENT_NUM_KEY, 0) == 0) {
				num = GameInformation.SpecialTroubleEventNum = CalcTroubleEventNumber(GameInformation.HunterNum);
			} else {
				num = GameInformation.SpecialTroubleEventNum = PlayerPrefs.GetInt(GameInformation.SPECIAL_TROUBLE_EVENT_NUM_KEY, 0) + 1;
				if (num == (CalcTroubleEventNumber(GameInformation.HunterNum) + 3)) {
					// エンディング
					GameInformation.special_ending_flag = true;
				}
			}
			GameInformation.SpecialTroubleEventFlag = false;
			// ハニワ
		}	else if (PlayerPrefs.GetInt(GameInformation.TROUBLE_EVENT_ITEM_NUM_KEY, 0) == GameInformation.HaniwaNum && SaveData.GetBool(GameInformation.SPECIAL_TROUBLE_EVENT_FLAG_KEY, false)
	 	&& PlayerPrefs.GetInt(GameInformation.SPECIAL_TROUBLE_EVENT_NUM_KEY, 0) < (CalcTroubleEventNumber(GameInformation.HaniwaNum) + 3)) {
	 	if (PlayerPrefs.GetInt(GameInformation.SPECIAL_TROUBLE_EVENT_NUM_KEY, 0) == 0) {
	 		num = GameInformation.SpecialTroubleEventNum = CalcTroubleEventNumber(GameInformation.HaniwaNum);
	 	} else {
	 		num = GameInformation.SpecialTroubleEventNum = PlayerPrefs.GetInt(GameInformation.SPECIAL_TROUBLE_EVENT_NUM_KEY, 0) + 1;
	 		if (num == (CalcTroubleEventNumber(GameInformation.HaniwaNum) + 3)) {
	 			// エンディング
	 			GameInformation.special_ending_flag = true;
	 		}
	 	}
	 	GameInformation.SpecialTroubleEventFlag = false;
	 } else {
			// リストの先頭から番号取り出し
			num = GameInformation.TroubleEventList[0];
			GameInformation.TroubleEventList.RemoveAt(0);
		}

		return num;
	}

	private int CalcTroubleEventNumber(int num) {
		return (num - 1)*4 + 101;
	}

	public void StartPlorogue() {
		StartCoroutine(StoryCoTalk());
	}

	// メイン画面の初期化
	public void InitMainContents() {

		SetLukcy();
		SetTroubleEventItem();

		// 通知GameObjectが未定義の場合
		if (notice == null) {
			notice = this.gameObject.GetComponent<Notice>();
		}

		// 前回のオブジェクトが残ってる場合を考えてNoticeの子を削除
		notice.DestroyAllChildren();
		playScreen.SetActive(true);

		if (playScreenGroup == null) {
			playScreenGroup = playScreen.GetComponent<CanvasGroup>();
		}

		playScreenGroup.blocksRaycasts = true;
		parameter.SetActive(true);
		// メインBGMを流す
		StartMainBGM();
		// 通知がある場合通知を入れる
		GameInformation.isTalkingStart = false;
		GameInformation.noticeList = new List<int> (PlayerPrefsX.GetIntArray(GameInformation.NOTICE_KEY, 0, 0));
		// 通知リストが空じゃないとき
		if (GameInformation.noticeList.Any()) {
			StartCoroutine(notice.ShowNoticeList());
		}
		// プレゼント出現処理
		AppearGift();

		furnitureGroup.blocksRaycasts = true;
	}

	// メイン画面にプレゼントを表示
	private void AppearGift() {

		// セーブデータからデータを取り出す
		if (GameInformation.GiftAppearanceMonthArray[0] == 0 ) {
			 int[] month_array = PlayerPrefsX.GetIntArray(GameInformation.GIFT_MONTH_KEY, 0, 0);
 			// セーブデータにも存在しないとき
			if (month_array.Length == 0 ) {

				// ランダムに出現する月を設定
				List<int> list = new List<int>();
				for (int i = 0; i < 12; i++ ) {
					list.Add( i + 1 );
				}
				list = list.OrderBy( x => Guid.NewGuid() ).ToList();

				ItemManager item_manager = this.gameObject.GetComponent<ItemManager>();

				for (int i = 0; i < GameInformation.GIFT_NUM; i++ ) {
					GameInformation.GiftAppearanceMonthArray[i] = list[i];
					// 出現するプレゼントアイテム番号（ランダム）を取得
					GameInformation.GiftItemNumberArray[i] = item_manager.GetRandamGiftItemNumber();
				}
				// 出現月を保存
				PlayerPrefsX.SetIntArray(GameInformation.GIFT_MONTH_KEY, GameInformation.GiftAppearanceMonthArray);
				// 出現するアイテムリストを保存
				PlayerPrefsX.SetIntArray(GameInformation.GIFT_ITEM_NUMBER_KEY, GameInformation.GiftItemNumberArray);
			} else {
				GameInformation.GiftAppearanceMonthArray = month_array;
				GameInformation.GiftItemNumberArray = PlayerPrefsX.GetIntArray(GameInformation.GIFT_ITEM_NUMBER_KEY, 0 ,0);
			}
		}

		// プレゼント出現月か
		gift_flag = false;

		int idx = 0;
		foreach (var item in GameInformation.GiftAppearanceMonthArray.Select((Value, Index) => new { Value, Index })) {
			int m = item.Value;
			if (m == PlayerPrefs.GetInt(GameInformation.GAME_DATE_KEY, 1)) {
					gift_flag = true;
					idx = item.Index;
					break;
			}
		}

		// プレゼント出現月の場合
		if (gift_flag) {
			if (SaveData.GetBool(GameInformation.GIFT_OPEN_FLAG_KEY, false)) {
				GameInformation.GiftOpenFlag = false;
				SaveData.SetBool(GameInformation.GIFT_OPEN_FLAG_KEY, false);
			}

			// プレゼント出現月の配列と同じインデックスのアイテム番号を取得
			int itemNum = GameInformation.GiftItemNumberArray[idx];
			GameInformation.GiftItemNumber = itemNum;
			if ( itemNum > 0 ) {
				// アイテムが上限に達しているか確認する(99未満のとき)
				if (GameInformation.InventoryArray[itemNum-1] < GameInformation.ITEM_MAX) {
					// メイン画面にプレゼントを表示
					giftObjList.giftObject.SetActive(true);
					giftObjList.giftbox.SetActive(true);
					AudioManager.Instance.PlaySE(AUDIO.SE_GIFT);
					giftObjList.giftEffect.Play();
					giftObjList.giftboxAnim.Play();
				}
			}
			// 上限に達している場合は何もしない
		}
	}

	// プレゼント非表示にする
	public void RemoveGift() {
		giftObjList.giftbox.SetActive(false);
		giftObjList.giftObject.SetActive(false);

	}

	// for sleeping manager
	public void NextDay () {
		StartCoroutine(MoveToNextDayWithPanelAnimation());
	}

	// Trigger Method by GameObject
	public void NextDayButton() {
		StartCoroutine(StoryCoTalk());
	}

	// 次の日のバックグラウンドアニメーション処理
	private IEnumerator MoveToNextDayWithPanelAnimation() {
		// プレゼント処理
		if (gift_flag) {
			if (SaveData.GetBool(GameInformation.GIFT_OPEN_FLAG_KEY, false)) {
				GameInformation.GiftOpenFlag = false;
			} else {
				// プレゼントが表示されたままの場合削除
				RemoveGift();
			}
			gift_flag = false;
		}

		// 次の月に進む
		GameInformation.GAME_DATE = PlayerPrefs.GetInt(GameInformation.GAME_DATE_KEY, 1) + 1;
		// セーブ
		PlayerPrefs.SetInt(GameInformation.GAME_DATE_KEY, GameInformation.GAME_DATE);
		SaveData.SetBool(GameInformation.GIFT_OPEN_FLAG_KEY, GameInformation.GiftOpenFlag);

		// 家具をセット
		if (furniture_manager == null) {
			furniture_manager = this.gameObject.GetComponent<FurnitureManager>();
		}
		furniture_manager.SetFurniture();

		AudioManager.Instance.FadeOutBGM (1);
		if (morningObj == null) {
			morningObj = Instantiate(Resources.Load<GameObject>(morningPath));
		 	morningText = morningObj.GetComponentInChildren<Text>();
		}

		morningObj.transform.SetParent(canvas.transform, false);

		morningText.text = GameInformation.GAME_DATE + dateCntText;

		morningObj.SetActive(true);
		AudioManager.Instance.PlaySE(AUDIO.SE_MORNING);

		yield return new WaitForSeconds(1);

		// 時間経過で追加されるパラメータイベント
		if (training_manager == null) {
			training_manager = this.gameObject.GetComponent<TrainingManager>();
		}
		training_manager.AddParameterItemList();

		yield return new WaitForSeconds(1);

		morningObj.SetActive(false);
		morningText.text = "";

		InsertTextDate();
		// トラブルイベント
		StartCoroutine(TroubleCoTalk());
	}

	// メイン画面の日付にゲーム時間の日付を入れる
	private void InsertTextDate () {
		if (dateText == null) {
			dateText = dateTextObj.GetComponent<Text>();
		}
		dateText.text = PlayerPrefs.GetInt(GameInformation.GAME_DATE_KEY, 1) + dateCntText;
	}

	// メイン画面にラッキーを設置
	private void SetLukcy() {
		if (lucky != null ) {
			Destroy(lucky);
		}
		int num = UnityEngine.Random.Range(0, GameInformation.MaxLuckyNum) + 1;
		lucky = Instantiate(Resources.Load<GameObject>(luckyPath + num.ToString()));
		lucky.transform.SetParent(luckyParent.transform, false);
		lucky.SetActive(true);
	}

	// メイン画面にトラブルイベントアイテムを設置
	private void SetTroubleEventItem() {
		if (PlayerPrefs.GetInt(GameInformation.STORY_PROGRESS_KEY, 0) > 3 && troubleItem == null ) {
			GameInformation.TroubleEventItemNum = PlayerPrefs.GetInt(GameInformation.TROUBLE_EVENT_ITEM_NUM_KEY, 0);
			int num = GameInformation.TroubleEventItemNum;
			if (num > 0) {
				troubleItem = Instantiate(Resources.Load<GameObject>(troubleEventItemPath + num.ToString()));
				troubleItem.transform.SetParent(troubleItemParent.transform, false);
				troubleItem.SetActive(true);
			}
		}
	}

  // Get trouble event information by number
	public TroubleEvent GetTroubleEvent(int searchNum) {
		return troubleEventDataBase.GetTroubleEventLists().Find(number => number.GetNumber() == searchNum);
	}

	// ランダムなトラブルイベント番号リストの設定
	public void SetRandomTroubleEventList() {
		// 次回リリース時に追加 --ここから--
		/*
		if (GameInformation.SalesPersonMonth == (GameInformation.GAME_DATE - 1)) {
			// Add trouble Event
			foreach(TroubleEvent data in troubleEventDataBase.GetTroubleEventLists()) {
				if (data.GetSpecialNumber() != 0 && data.GetSpecialNumber() != GameInformation.TroubleEventItemNum && !data.GetSpecialFlag()) {
					GameInformation.TroubleEventList.Add(data.GetNumber());
				}
			}
			// ランダムに並べ替え
			GameInformation.TroubleEventList = GameInformation.TroubleEventList.OrderBy( x => Guid.NewGuid() ).ToList();
			PlayerPrefsX.SetIntArray(GameInformation.TROUBLE_EVENT_LIST_KEY, GameInformation.TroubleEventList.ToArray());
 		}  else
		*/
		// 次回リリース時に追加 --ここまで--
		// 初めてトラブルイベントが発生する日
		if (GameInformation.GAME_DATE == 2) {
			if (GameInformation.TroubleEventList != null) {
				GameInformation.TroubleEventList.Clear();
			}
			// トラブルイベント一覧を作成
			foreach(TroubleEvent data in troubleEventDataBase.GetTroubleEventLists()) {
				if (data.GetSpecialNumber() == 0 && !data.GetSpecialFlag()) {
					GameInformation.TroubleEventList.Add(data.GetNumber());
				}
			}
			// ランダムに並べ替え
			GameInformation.TroubleEventList = GameInformation.TroubleEventList.OrderBy( x => Guid.NewGuid() ).ToList();
			PlayerPrefsX.SetIntArray(GameInformation.TROUBLE_EVENT_LIST_KEY, GameInformation.TroubleEventList.ToArray());
		} else {
			// セーブデータから取得
			GameInformation.TroubleEventList = new List<int>(PlayerPrefsX.GetIntArray(GameInformation.TROUBLE_EVENT_LIST_KEY, 0, 0));
			// もしデータがない場合
			if (!GameInformation.TroubleEventList.Any()) {
				// トラブルイベント一覧を作成
				foreach(TroubleEvent data in troubleEventDataBase.GetTroubleEventLists()) {
					if (data.GetSpecialNumber() == 0 && !data.GetSpecialFlag()) {
						GameInformation.TroubleEventList.Add(data.GetNumber());
					}
				}
			}
		}
	}

	public void StartMainBGM() {
		switch(PlayerPrefs.GetInt(GameInformation.ROOM_KEY, 1)) {
		case 1:
			AudioManager.Instance.PlayBGM (AUDIO.BGM_MAIN_01, AudioManager.BGM_FADE_SPEED_RATE_HIGH);
			break;
		case 2:
			AudioManager.Instance.PlayBGM (AUDIO.BGM_MAIN_02, AudioManager.BGM_FADE_SPEED_RATE_HIGH);
			break;
		default:
			AudioManager.Instance.PlayBGM (AUDIO.BGM_MAIN_01, AudioManager.BGM_FADE_SPEED_RATE_HIGH);
			break;
		}
	}
}
