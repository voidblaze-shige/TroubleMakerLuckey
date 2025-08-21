using System.Linq;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utage;

public class EndingManager : MonoBehaviour {

	AdvEngine Engine { get { return engine ?? (engine = FindObjectOfType<AdvEngine>()); } }
	public AdvEngine engine; // utage
	public GameObject endingListParent;
	public GameObject playScreen;
	public GameObject parameter;
	public CanvasGroup furnitureGroup; // メイン画面のタップ画像の親

	[SerializeField]
	private EndingEventDataBase endingEventDataBase;

	private const string nodePath = "Ending/EndingNode";
	private const string endPath = "END";
	private const string endingButtonPath = "Button";
	private const string lockImagePath = "LOCK";
	private const string hintButtonPath = "HINT";
	private string loadingPath = "UI/LoadCanvas";
	private HintManager hint_manager;
	private TrainingManager training_manager;

	private int ending_number = 0;
	private const string label = "エンディング0";

	void Start () {
		GameInformation.EndingList = new List<int>(PlayerPrefsX.GetIntArray(GameInformation.ENDING_KEY, 0, 9));
		hint_manager = this.gameObject.GetComponent<HintManager>();
		training_manager = this.gameObject.GetComponent<TrainingManager>();

		// エンディングリスト作成
		CreateEndingList();
	}

	private void CreateEndingList() {
		// これまで辿りついたエンディングがあるか
		List<EndingEvent> list = endingEventDataBase.GetEndingEventLists();
		int num = 0;
		foreach(var item in list.Select((Value, Index) => new { Value, Index })) {
			EndingEvent e = item.Value;
			int count = item.Index + 1;
			if (num != e.GetNumber()) {
				num = e.GetNumber();
				GameObject node = Instantiate(Resources.Load<GameObject>(nodePath));
				node.transform.SetParent(endingListParent.transform, false);
				node.name = count.ToString();
				GameObject endingButton = node.transform.Find(endPath).gameObject;
				Image endingButtonImage = endingButton.GetComponent<Image>();
				endingButtonImage.sprite = e.GetImage();
				// ボタンにエンディング閲覧メソッド追加
				Button button = endingButton.transform.Find(endingButtonPath).GetComponent<Button>();
				button.onClick.AddListener(() => ShowEndingFromList(e.GetNumber()));
				// ヒント設置
				if (hint_manager == null) {
					hint_manager = this.gameObject.GetComponent<HintManager>();
				}
				Button hintButton = node.transform.Find(hintButtonPath).GetComponent<Button>();
				hintButton.onClick.AddListener(() => hint_manager.OpenConfirm(e.GetNumber()));
				// エンディングリストの中身がある場合
				if (GameInformation.EndingList.Any()) {
					if (GameInformation.EndingList.Contains(e.GetNumber())) {
						GameObject lockObj = node.transform.Find(lockImagePath).gameObject;
						lockObj.SetActive(false);
					}
				}
			}
		}
	}

	private EndingEvent GetEndingEvent(int searchNum) {
	 	return endingEventDataBase.GetEndingEventLists().Find(number => number.GetNumber() == searchNum);
	}

	public int StartEnding() {
		// エンディングの判別フラグ
		ending_number = 0;

		if (GameInformation.special_ending_flag) {
			ending_number = GetEndingNumber(PlayerPrefs.GetInt(GameInformation.TROUBLE_EVENT_ITEM_NUM_KEY, 0));
			GameInformation.special_ending_flag = false;
		} else {
			ending_number = GetEndingNumber();

			if (ending_number == 0) {
				// 本来ありえない
				Debug.Log("条件に合わないパターンが発生");
				ending_number = 1;
			}
		}

		if (ending_number > 0) {
			if (GameInformation.EndingList == null) {
				GameInformation.EndingList = new List<int>(PlayerPrefsX.GetIntArray(GameInformation.ENDING_KEY, 0, 9));
			}
			GameInformation.EndingList.Add(ending_number);
			engine.Param.TrySetParameter("ending_number", ending_number);
			// エンディングスタート
			StartCoroutine(EndlingCoTalk());
		}

		return ending_number;
	}

	private int GetEndingNumber(int trouble_number = 0) {

		List<EndingEvent> endingList = endingEventDataBase.GetEndingEventLists();

		EndingEvent tempEvent = null;
		foreach(EndingEvent e in endingList) {
			if (e.GetTroubleNumber() == trouble_number) {
				if ((PlayerPrefs.GetInt(GameInformation.INT_KEY, 0) >= e.GetAtamaRate() &&
				PlayerPrefs.GetInt(GameInformation.BODY_KEY, 0) >= e.GetKaradaRate() &&
				PlayerPrefs.GetInt(GameInformation.LIKE_KEY, 0) >= e.GetKimochiRate())) {
					if (tempEvent == null || tempEvent.GetPriority() <= e.GetPriority()) {
						tempEvent = e;
					}
				}
			}
		}
		return tempEvent != null ? tempEvent.GetNumber() : 0;
	}

	// エンディング６判定
	public bool IsAchievedEnding6 () {
		EndingEvent e = GetEndingEvent(6);
		return (PlayerPrefs.GetInt(GameInformation.INT_KEY, 0) >= e.GetAtamaRate() &&
		PlayerPrefs.GetInt(GameInformation.BODY_KEY, 0) >= e.GetKaradaRate() &&
		PlayerPrefs.GetInt(GameInformation.LIKE_KEY, 0) >= e.GetKimochiRate());
	}

	// エンディングイベント処理
	private IEnumerator EndlingCoTalk() {
		AudioManager.Instance.FadeOutBGM (1);
		MainManager main_manager = this.gameObject.GetComponent<MainManager>();

		yield return main_manager.StartSenario(label);

		GameObject blackBackGround = Instantiate(Resources.Load<GameObject>(loadingPath));
		blackBackGround.GetComponent<CanvasGroup>().alpha = 1;
		blackBackGround.SetActive(true);

		// ゲームデータ初期化
		PlayerPrefsX.SetIntArray(GameInformation.ENDING_KEY, GameInformation.EndingList.ToArray());
		// 変数など初期化
		InitDataForRestart();

		// 固定トラブルイベントの初期化
		training_manager.RemoveSpecialParameterEvent();
		// パラメータイベント追加
		training_manager.AddParamterEventByClear();
		// セーブ
		PlayerPrefs.Save();
		// タイトルへ戻る
		GameObject obj = GameObject.FindGameObjectWithTag("SceneController").gameObject;
		SceneController sc = obj.GetComponent<SceneController>();
		sc.LoadTitle();
	}

	public void InitDataForRestart() {
		GameInformation.special_ending_flag = false;

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

		SaveData.SetBool(GameInformation.IS_CLEAR_KEY, true);		// クリアフラグ
		PlayerPrefs.SetInt(GameInformation.TROUBLE_EVENT_ITEM_NUM_KEY, 0);		// 固定トラブルイベントの選択番号
		PlayerPrefs.SetInt(GameInformation.TROUBLE_EVENT_PROGRESS_KEY, 1);		// トラブルイベントの進捗状況（日数）
		GameInformation.SpecialTroubleEventNum = 0;		// 固定トラブルイベントのトラブル番号
		PlayerPrefs.SetInt(GameInformation.SPECIAL_TROUBLE_EVENT_NUM_KEY, 0);
		PlayerPrefs.SetInt(GameInformation.TRAINING_PROGRESS_KEY	, 0);		// 育成進捗状況（日数）
		GameInformation.SpecialTroubleEventFlag = false;  // 固定トラブル発生判定フラグ
		SaveData.SetBool(GameInformation.SPECIAL_TROUBLE_EVENT_FLAG_KEY, false);
		SaveData.SetBool(GameInformation.IS_STARTED_KEY, false);		// 開始フラグ

		GameInformation.IsSleeping = false;		// おやすみ判定フラグ
		SaveData.SetBool(GameInformation.IS_SLEEPING_KEY, GameInformation.IsSleeping);
		SaveData.SetBool(GameInformation.GIFT_OPEN_FLAG_KEY, false);		// プレゼント開封フラグ

		int[] ary = new int[6];		// プレゼント出現月を格納する配列
		PlayerPrefsX.SetIntArray(GameInformation.GIFT_MONTH_KEY, ary);
		PlayerPrefsX.SetIntArray(GameInformation.GIFT_ITEM_NUMBER_KEY, ary);		// プレゼントのアイテムリスト

		List<int> list = new List<int>();		// 発生するトラブルイベント番号のリスト
		PlayerPrefsX.SetIntArray(GameInformation.TROUBLE_EVENT_LIST_KEY, list.ToArray());
		PlayerPrefsX.SetIntArray(GameInformation.NOTICE_KEY, list.ToArray());		// 通知リスト
	}

	public void ShowEndingFromList(int num) {
		engine.Param.TrySetParameter("ending_number", num);
		StartCoroutine(EndlingCoTalkForList());
	}

	private IEnumerator EndlingCoTalkForList() {
		furnitureGroup.blocksRaycasts = false;
		parameter.SetActive(false);
		playScreen.SetActive(false);
		AudioManager.Instance.FadeOutBGM (1);
		MainManager main_manager = this.gameObject.GetComponent<MainManager>();
		yield return main_manager.StartSenario(label);
		playScreen.SetActive(true);
		parameter.SetActive(true);
		main_manager.StartMainBGM();
		furnitureGroup.blocksRaycasts = true;
	}
}
