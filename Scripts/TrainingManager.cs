using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingManager : MonoBehaviour {

	[SerializeField]
	private ParameterEventDataBase parameterDataBase;

	[SerializeField]
	private ComboEventDataBase comboEventDataBase;

	// path
	private string trainingNodePath = "Training/TrainingNode";
	private string comboNodePath = "Training/ComboNode";
	private string resultTextPath = "Window/ResultText";
	private string nodeTextPath = "Button/Text";
	private string nodeButtonPath = "Button";
	private string nodeNewPath = "new";
	private string trainingDonePath = "Training/TrainingDone";
	private string comboPath = "Training/Combo";

	// parameter event list item number
	private const int INT_MAX = 11;
	private const int BODY_MAX = 11;
	private const int LIKE_MAX = 7;
	private const int MAX_EVENT_NUM = 3;

	// ソート
	private Sort sort;
	// main_manager
	private MainManager main_manager;
	// 選択中の育成ボタン
	private GameObject selectEvent;
	// 選択している育成項目オブジェクトリスト
	private GameObject[] selectedItemArray = new GameObject[MAX_EVENT_NUM];
	// 選択中の育成項目[3]
	private ParameterEvent[] trainingArray = new ParameterEvent[MAX_EVENT_NUM];
	// 選択中のコンボオブジェクト
	private GameObject selectedComboEvent;
	// 選択中のコンボ番号
	private int selectedComboNum = 0;

	// コンボ番号
	private int comboNum = 0;
	// コンボイベント
	private ComboEvent comboEvent;

	// コンボと育成完了を表示するための親
	public GameObject canvas;
	// 育成項目リストの親オブジェクト
	public ParameterListObject parameterList;

	[System.SerializableAttribute]
	public class ParameterListObject {
		public GameObject int_list_parent;
		public GameObject body_list_parent;
		public GameObject like_list_parent;
		public GameObject combo_list_parent;
	}

	// 育成項目リストのパネル
	public GameObject trainingPanel;
	// 育成結果
	public GameObject resultPanel;
	// 育成選択ボタン各3つ
	public List<GameObject> selectButton;
	// 育成ボタンの未選択画像
	public Sprite notSelectImage;

	// 育成一覧項目のボタン背景
	public TrainingItemImages trainingItemImages;
	[System.SerializableAttribute]
	public class TrainingItemImages {
		public Sprite default1;
		public Sprite default2;
		public Sprite selected;
	}

	// 育成選択画面のコンボエフェクト
	public GameObject comboEffect;
	// ページ初期化用
	public GameObject trainingObj;
	// パラメータの値表示用
	public ParameterManager parameter_manager;
	// アニメーションコントローラー
	public AnimationController anim_controller;
	// エフェクト表示時に無効化するUI
	public CanvasGroup playScreenGroup;

	void Start () {
		// 育成項目作成
		// ゲーム開始
		if (!SaveData.GetBool(GameInformation.IS_STARTED_KEY, false)) {
			// クリア済み（二週目以降）
			if (SaveData.GetBool(GameInformation.IS_CLEAR_KEY, false)) {
				// 既プレイの場合はパラメータリストからイベントを取得
				SetAllParameterEventList();
				// TrainingNodeインスタンスを作成
				CreateAllParameterList();
				// コンボリスト作成
				CreateComboList();
			} else {
				// はじめて（一週目）の場合はパラメータリストを作成
				SetParameterItemList();
			}
		} else {
			// 既プレイの場合はパラメータリストからイベントを取得
			SetAllParameterEventList();
			// TrainingNodeインスタンスを作成
			CreateAllParameterList();
			// コンボリスト作成
			CreateComboList();
		}
		main_manager = this.gameObject.GetComponent<MainManager>();
	}

	public void StartTrainingButton(GameObject confirmWindow) {
		bool flag = true;
		for (int i = 0; i < trainingArray.Length; i++) {
			if (trainingArray[i] == null) {
				flag = false;
			}
		}

		if (flag) {
			// 開く音
			AudioManager.Instance.PlaySE(AUDIO.SE_OPEN);
			confirmWindow.SetActive(true);

		}	else {
			// ブザー音
			AudioManager.Instance.PlaySE(AUDIO.SE_BUZZER);
		}
	}

	// 育成実行
	public void RunTrain() {
		playScreenGroup.blocksRaycasts = false;

		comboEffect.SetActive(false);

		foreach (ParameterEvent e in trainingArray) {
			if (e.GetSpecialFlag()) {
				// if include special parameter event, go next event
				SetNextSpecialParameterEvent(e);

				// トラブルイベントフラグを立てる
				GameInformation.SpecialTroubleEventFlag = true;
			 	break;
			}
		}

		// コンボの時, コンボを表示
		if ( comboNum > 0 ) {
			StartCoroutine(ShowCombo());
		} else {
			// 育成内容表示
			ShowResult();
		}
	}

	// コンボ表示
	private IEnumerator ShowCombo() {
		AudioManager.Instance.PlaySE(AUDIO.SE_NICE);
		GameObject combo_effect = Instantiate(Resources.Load<GameObject>(comboPath));
		combo_effect.transform.SetParent(canvas.transform, false);
		combo_effect.SetActive(true);

		Animation anim_back = combo_effect.transform.Find("Effect_back").GetComponent<Animation>();
		Animation anim_star = combo_effect.transform.Find("Effect_star").GetComponent<Animation>();
		anim_back.Play();
		anim_star.Play();

		yield return anim_controller.WaitForAnimation(anim_back);
		yield return anim_controller.WaitForAnimation(anim_star);

		combo_effect.SetActive(false);
		Destroy(combo_effect);
		// 育成内容表示
		ShowResult();
	}

	private void ShowResult() {
		string resultSentence ="" ;

		resultPanel.SetActive(true);
		playScreenGroup.blocksRaycasts = true;

		if ( comboNum > 0 ) {
			comboEvent = GetComboEvent(comboNum);
			resultSentence = comboEvent.GetResultInformation();
		} else if (trainingArray[0].GetSpecialFlag()) {
			resultSentence = trainingArray[0].GetResultInformation();
		}	else {
			resultSentence = trainingArray[0].GetResultInformation() +
				Environment.NewLine +
				trainingArray[1].GetResultInformation()+
				Environment.NewLine +
				trainingArray[2].GetResultInformation();
		}

		GameObject text_obj = resultPanel.transform.Find(resultTextPath).gameObject;
		Text resultText = text_obj.GetComponentInChildren<Text>();
		resultText.text = resultSentence;
		AudioManager.Instance.PlaySE(AUDIO.SE_RESULT_LIST);
	}

	// 育成完了
	public void ShowTrainingDone() {
		playScreenGroup.blocksRaycasts = false;
		// 育成完了表示
		StartCoroutine(ResultEffect());

		int int_value = 0;
		int body_value = 0;
		int like_value = 0;

		// パラメータ成長
		// コンボの場合
		if (comboNum > 0) {
			if (GameInformation.ComboList == null) {
				GameInformation.ComboList = new List<int>(PlayerPrefsX.GetIntArray(GameInformation.COMBO_KEY, 0, 0));
			}
			if (!GameInformation.ComboList.Contains(comboNum)) {
				// 未発見の場合リストに追加
			 	GameInformation.ComboList.Add(comboNum);
	 			AddComboNode(GetComboEvent(comboNum), true);
			}

			if (comboEvent == null) {
				comboEvent = GetComboEvent(comboNum);
			}
			int_value = comboEvent.GetAtamaValue();
			body_value = comboEvent.GetKaradaValue();
			like_value = comboEvent.GetKimochiValue();

			comboEvent = null;
		} else {
		// コンボ以外
			for (int i=0; i<3; i++) {
				switch(trainingArray[i].GetKindOfType()) {
					case 1:
						int_value += trainingArray[i].GetIncreaseValue();
						break;
					case 2:
						body_value += trainingArray[i].GetIncreaseValue();
						break;
					case 3:
						like_value += trainingArray[i].GetIncreaseValue();
						break;
					default:
						break;
				}
				// 固定トラブルイベントのパラメータイベントの場合は、同じイベントなので
				// ひとつだけ値を代入して終わり
				if (trainingArray[i].GetSpecialFlag()) break;
			}
		}
		// コンボリスト取得
		if (GameInformation.ComboList == null) {
			GameInformation.ComboList = new List<int>(PlayerPrefsX.GetIntArray(GameInformation.COMBO_KEY, 0, 0));
		}
		GameInformation.ComboList.Sort();
		// セーブ
		GameInformation.TrainingProgress = GameInformation.GAME_DATE;
		PlayerPrefs.SetInt(GameInformation.TRAINING_PROGRESS_KEY, GameInformation.TrainingProgress);
		SaveData.SetBool(GameInformation.SPECIAL_TROUBLE_EVENT_FLAG_KEY, GameInformation.SpecialTroubleEventFlag);
		PlayerPrefsX.SetIntArray(GameInformation.COMBO_KEY, GameInformation.ComboList.ToArray());

		GrowUp(int_value, 1);
		GrowUp(body_value, 2);
		GrowUp(like_value, 3);
		ClearAllButtonBackground();
		ClearTrainingButton();
		ClearComboNode();
	}

	// 育成結果効果
	private IEnumerator ResultEffect () {
		AudioManager.Instance.PlaySE(AUDIO.SE_PARAMETER_INCREASE);

		yield return new WaitForSeconds(0.5f);

		GameObject result_effect = Instantiate(Resources.Load<GameObject>(trainingDonePath));
		result_effect.transform.SetParent(canvas.transform, false);
		result_effect.SetActive(true);
		AudioManager.Instance.PlaySE(AUDIO.SE_CHEER);

		Animation anim_back = result_effect.transform.Find("Effect_back").GetComponent<Animation>();
		Animation anim_star = result_effect.transform.Find("Effect_star").GetComponent<Animation>();
		anim_back.Play();
		anim_star.Play();

		yield return new WaitForSeconds(1.5f);

		result_effect.SetActive(false);

		// start story mode
		StartCoroutine(main_manager.StoryCoTalk());
	}

	// 育成項目リストを開く
	public void OpenTrainingList(GameObject obj) {
		AudioManager.Instance.PlaySE (AUDIO.SE_OPEN);
		selectEvent = obj;
		trainingPanel.SetActive(true);
	}

	// 育成項目を選択した時の処理（育成項目のボタン）
	public void SetEvent(int num, GameObject itemObj) {
		// 選択済みのイベントかどうか判定
		if (IsAlreadySelected(num)) {
			// 選択できない
			AudioManager.Instance.PlaySE (AUDIO.SE_BUZZER);
		} else {
			// 選択中の育成ボタンの番号を取得
			int buttonNum = int.Parse(selectEvent.name) - 1;
			// 選択中のパラメータイベントを取得
			ParameterEvent pe = GetParameterEvent(num);

			ClearComboNode();

			// もし特殊イベントだったら
			if (pe.GetSpecialFlag()) {

				for (int i = 0; i < MAX_EVENT_NUM; i++) {
					// 選択したパラメータイベントで三つのアイコンをすべて埋める
					selectButton[i].GetComponent<Image>().sprite = pe.GetIcon();
					// 同じイベントを設定
					trainingArray[i] = pe;
					// 選択していた全ての育成項目の背景を初期化
					if (selectedItemArray[i] != null) {
						ClearButtonBackground(selectedItemArray[i], trainingItemImages.default1);
					}
				}
				Array.Clear(selectedItemArray, 0 , selectedItemArray.Length);

				comboNum = 0;
				// コンボエフェクトを消す
				if (comboEffect.activeSelf) {
		 			comboEffect.SetActive(false);
				}
			} else {
				// すでに特殊イベントが選択されている場合、全解除をまず行う
				if (trainingArray[0] != null && trainingArray[0].GetSpecialFlag()) {
					ClearTrainingButton();

					for (int i = 0; i < selectedItemArray.Length; i++) {
						// 選択していた特殊イベントのボタンの背景を初期化
						if (selectedItemArray[buttonNum] != null) {
								ClearButtonBackground(selectedItemArray[buttonNum], trainingItemImages.default2);
						}
					}

				} else {
					if (selectedItemArray[buttonNum] != null) {
						// 選択していた通常イベントのボタンの背景を初期化
						ClearButtonBackground(selectedItemArray[buttonNum], trainingItemImages.default1);
					}
				}

				// 選択したアイコンを表示する
				Image buttonImage = selectEvent.GetComponent<Image>();
				buttonImage.sprite = pe.GetIcon();
				// 配列に選択した項目を追加する
				// 既に選択したことのあるボタンを選択していた場合は上書き
				trainingArray[buttonNum] = pe;
				// コンボ番号取得
				comboNum = GetComboNum();

				if (comboNum > 0) {
					// コンボ条件を満たした時星マークを表示
					comboEffect.SetActive(true);
				} else {
					comboEffect.SetActive(false);
				}
			}
			// 選択中の育成項目オブジェクトを格納
			selectedItemArray[buttonNum] = itemObj;

			trainingPanel.SetActive(false);

			// ボタンの背景を変更する
			// 初めて選択した場合、Newのアイコンを消す
			SelectParameterEventNode(num, itemObj);

			// 開いていた育成項目リストの初期化
			SwitchPage sp = trainingObj.GetComponent<SwitchPage>();
			sp.InitPage();
		}
 }

 // コンボ一覧からコンボを選んだ時の処理
 public void SetEventByCombo(int num, GameObject obj) {

 	 	// 選択済みのコンボかチェックする
	 	if (selectedComboNum == num) {
	 		AudioManager.Instance.PlaySE (AUDIO.SE_BUZZER);
	 	} else {
			ClearComboNode();

			// 選択したコンボを格納
			selectedComboEvent = obj;
			selectedComboNum = num;
	 		// コンボ番号を設定
		 	comboNum = num;
		 	// コンボ番号からパラメータイベントのリストを取得
		 	List<ParameterEvent> list = GetParameterEventsByComboNum(num);
		 	// 既に選択済みのパラメータイベントがあっても上書きする
		 	for (int i = 0; i < MAX_EVENT_NUM; i++) {
	 		 	// すべての育成ボタンにパラメータイベントのアイコンを設定する
				 // スプライトを設定
			 	 selectButton[i].GetComponent<Image>().sprite = list[i].GetIcon();
				 // 同じイベントを設定
				 trainingArray[i] = list[i];
				 // 選択していた全ての育成項目の背景を初期化
				 if (selectedItemArray[i] != null) {
					 ClearButtonBackground(selectedItemArray[i], trainingItemImages.default1);
				 }
		 	}
		 	Array.Clear(selectedItemArray, 0 , selectedItemArray.Length);
		 	// コンボエフェクトを表示
		 	comboEffect.SetActive(true);

			// コンボ内容のパラメータイベントのオブジェクトを取得、選択配列に格納
			foreach(var item in list.Select((Value, Index) => new { Value, Index })) {
				ParameterEvent e = item.Value;
				int idx = item.Index;
				num = e.GetNumber();
				string objectPath = num.ToString() + "/" + num.ToString();
				if (num < 200) {
					selectedItemArray[idx] = parameterList.int_list_parent.transform.Find(objectPath).gameObject;
				} else if (num < 300) {
					selectedItemArray[idx] = parameterList.body_list_parent.transform.Find(objectPath).gameObject;
				} else if (num < 400) {
					selectedItemArray[idx] = parameterList.like_list_parent.transform.Find(objectPath).gameObject;
				}
				// 選択したパラメータイベントの背景を変更
	 	 		SelectParameterEventNode(num, selectedItemArray[idx]);
				// 選択したコンボの背景を変更
				SelectComboNode(obj);
				// 選択したコンボの背景を変更
			}

			// 育成画面を非アクティブ化
		 	trainingPanel.SetActive(false);

			// 開いていた育成項目リストの初期化
 		 	SwitchPage sp = trainingObj.GetComponent<SwitchPage>();
 		 	sp.InitPage();
	 }
 }

 //	既に選択済みのパラメータイベントかチェック
 private bool IsAlreadySelected(int num) {
	 for (int i = 0; i < trainingArray.Length; i++) {
		 	if (trainingArray[i] != null) {
				if (trainingArray[i].GetNumber() == num) {
					return true;
				}
			}
 	 }
	 return false;
 }

 // 選択したパラメータイベントの組み合わせからコンボ番号を取得
 private int GetComboNum() {
	 if (trainingArray[0] != null && trainingArray[1] != null && trainingArray[2] != null) {

		 List<int> list1 = trainingArray[0].GetComboNums();
		 List<int> list2 = trainingArray[1].GetComboNums();
		 List<int> list3 = trainingArray[2].GetComboNums();

		 if ( list1.Any() && list2.Any() && list3.Any() ) {
			 foreach(int i in list1) {
				 if (list2.Contains(i) && list3.Contains(i)) {
					 	return i;
				 }
			 }
		 }
	 }
	 return 0;
	}

	// コンボ番号からパラメータイベントを取得
	public List<ParameterEvent> GetParameterEventsByComboNum(int num) {
		List<ParameterEvent> allList = parameterDataBase.GetParameterEventLists();
		List<ParameterEvent> list = new List<ParameterEvent>();

		foreach (ParameterEvent e in allList) {
			// パラメータイベントにコンボ番号が含まれる場合、リストに追加
			if (e.GetComboNums().Contains(num)) {
				list.Add(e);
			}
		}
		return list;
	}

	// Get parameter event information by parameter number
	public ParameterEvent GetParameterEvent(int searchNum) {
	 return parameterDataBase.GetParameterEventLists().Find(number => number.GetNumber() == searchNum);
	}

	// Get parameter event information by trouble event number
	public ParameterEvent GetParameterEventByTroubleNumber(int searchNum) {
	 return parameterDataBase.GetParameterEventLists().Find(number => number.GetTroubleEventNumber() == searchNum);
	}

	// Get combo event information by combo number
	public ComboEvent GetComboEvent(int searchNum) {
	 return comboEventDataBase.GetComboEventLists().Find(number => number.GetNumber() == searchNum);
	}

	public void SetAllParameterEventList() {
		ClearParameterLists();

		// get int ParameterEvent
		GameInformation.TrainingINTArray = PlayerPrefsX.GetIntArray(GameInformation.TrainingINT_KEY, 0, 0);
		SetParameterEventList(GameInformation.TrainingINTArray, 101, GameInformation.IntList, INT_MAX);

	 	// get body ParameterEvent
		GameInformation.TrainingBODYArray = PlayerPrefsX.GetIntArray(GameInformation.TrainingBODY_KEY, 0 ,0);
	 	SetParameterEventList(GameInformation.TrainingBODYArray, 201, GameInformation.BodyList, BODY_MAX);

	 	// get like ParameterEvent
		GameInformation.TrainingLIKEArray = PlayerPrefsX.GetIntArray(GameInformation.TrainingLIKE_KEY, 0, 0);
	 	SetParameterEventList(GameInformation.TrainingLIKEArray, 301, GameInformation.LikeList, LIKE_MAX);
	}

	private void ClearParameterLists() {
		if (GameInformation.IntList != null ) {
			GameInformation.IntList.Clear();
		}
		if (GameInformation.BodyList != null) {
			GameInformation.BodyList.Clear();
		}
		if (GameInformation.LikeList != null){
			GameInformation.LikeList.Clear();
		}
	}

	// はじめてパラメータイベント一覧を作る（一週目）
	private void SetParameterItemList() {
		List<ParameterEvent> allList = new List<ParameterEvent>();
		allList = parameterDataBase.GetParameterEventLists();

		foreach(ParameterEvent e in allList) {
			// ゲーム内の日付よりも前の解放条件かつ特殊以外のイベントを取得
			if ( GameInformation.GAME_DATE >= e.GetLiverationPeriod()
			&& !e.GetSpecialFlag() && !e.GetTabEventFlag() && !e.GetEndingAddFlag()) {
				// デフォルトのイベントにはnewをつけない
				SetParameterEvent(e, 2);
			}
		}
	}

	public void AddParameterItemList() {
		List<ParameterEvent> allList = new List<ParameterEvent>();
		allList = parameterDataBase.GetParameterEventLists();
		bool int_flag = false;
		bool body_flag = false;
		bool like_flag = false;

		foreach(ParameterEvent e in allList) {
			// ゲーム内の日付と同じ解放条件かつ特殊以外のイベントを取得
			if ( GameInformation.GAME_DATE == e.GetLiverationPeriod() && !e.GetSpecialFlag() && !e.GetTabEventFlag()) {

				int type = 0;
				// どのタイプか
				if (e.GetNumber() < 200) {
					int_flag = true;
					type = 1;
				} else if (e.GetNumber() < 300) {
					body_flag = true;
					type = 2;
				} else if (e.GetNumber() < 400) {
					like_flag = true;
					type = 3;
				}

				// すでに追加されているパラメータイベントの場合、スキップする
				if (!CheckAlreadyExistEvent(type , e.GetNumber())) {
					SetParameterEvent(e);
					// 通知リストに追加
					GameInformation.noticeList.Add(e.GetNumber());
					PlayerPrefsX.SetIntArray(GameInformation.NOTICE_KEY, GameInformation.noticeList.ToArray());
				}
			}
		}

		// ソート
		if (sort == null) {
			sort = this.gameObject.GetComponent<Sort>();
		}
		if (int_flag) {
			sort.SortItem(parameterList.int_list_parent);
		}
		if (body_flag) {
			sort.SortItem(parameterList.body_list_parent);
		}
		if (like_flag) {
			sort.SortItem(parameterList.like_list_parent);
		}
	}

	// クリア後に追加されるイベント
	public void AddParamterEventByClear() {
		// 通知一覧初期化
		GameInformation.noticeList.Clear();

		List<ParameterEvent> allList = new List<ParameterEvent>();
		allList = parameterDataBase.GetParameterEventLists();

		foreach(ParameterEvent e in allList) {
			// クリア後に追加されるイベントを取得
			if (e.GetEndingAddFlag()) {

				int type = 0;
				// どのタイプか
				if (e.GetNumber() < 200) {
					type = 1;
				} else if (e.GetNumber() < 300) {
					type = 2;
				} else if (e.GetNumber() < 400) {
					type = 3;
				}

				// すでに追加されているパラメータイベントの場合、スキップする
				if (!CheckAlreadyExistEvent(type , e.GetNumber())) {
					SetParameterEvent(e);

					// 通知リストに追加
					GameInformation.noticeList.Add(e.GetNumber());
					PlayerPrefsX.SetIntArray(GameInformation.NOTICE_KEY, GameInformation.noticeList.ToArray());
				}
			}
		}
	}

	// 追加済みのパラメータイベントかチェック
	private bool CheckAlreadyExistEvent(int type, int num) {
		switch(type) {
			case 1:
			 	return GameInformation.TrainingINTArray[num-101] > 0;
			case 2:
				return GameInformation.TrainingBODYArray[num-201] > 0;
			case 3:
				return GameInformation.TrainingLIKEArray[num-301] > 0;
			default:
				break;
		}
		return false;
	}

	private void SetParameterEvent(ParameterEvent e, int value = 1) {
		int type = e.GetKindOfType();
		int num = e.GetNumber();
		GameObject obj = null;
		bool _newflag = value == 1;
		switch(type) {
			case 1:
				GameInformation.TrainingINTArray[num-101] = value;
				PlayerPrefsX.SetIntArray(GameInformation.TrainingINT_KEY, GameInformation.TrainingINTArray);
				GameInformation.IntList.Add( new TrainingData {newFlag = _newflag, praEvent = e} );
				obj = parameterList.int_list_parent;
				break;
			case 2:
				GameInformation.TrainingBODYArray[num-201] = value;
				PlayerPrefsX.SetIntArray(GameInformation.TrainingBODY_KEY, GameInformation.TrainingBODYArray);
				GameInformation.BodyList.Add( new TrainingData {newFlag = _newflag, praEvent = e} );
				obj = parameterList.body_list_parent;
				break;
			case 3:
				GameInformation.TrainingLIKEArray[num-301] = value;
				PlayerPrefsX.SetIntArray(GameInformation.TrainingLIKE_KEY, GameInformation.TrainingLIKEArray);
				GameInformation.LikeList.Add( new TrainingData {newFlag = _newflag, praEvent = e} );
				obj = parameterList.like_list_parent;
				break;
			default:
				break;
		}
		AddParameterEventNode(e, obj, _newflag);
	}

	private void SetNextSpecialParameterEvent(ParameterEvent e) {
		if (e.GetSpecialFlag() && (e.GetLiverationPeriod() < e.GetNumber())) {

			int num = e.GetNumber();
			ParameterEvent new_event = GetParameterEvent(num + 1);

			// 次のパラメータイベントに進める
			switch(e.GetKindOfType()) {
				case 1:
					GameInformation.TrainingINTArray[num - 101] = 0;
					RemoveParameterEventNode(num, parameterList.int_list_parent);
					break;
				case 2:
					GameInformation.TrainingBODYArray[num - 201] = 0;
					RemoveParameterEventNode(num, parameterList.body_list_parent);
					break;
				case 3:
					GameInformation.TrainingLIKEArray[num - 301] = 0;
					RemoveParameterEventNode(num, parameterList.like_list_parent);
					break;
				default:
					break;
			}
			SetParameterEvent(new_event);
			// 通知リストに追加
			GameInformation.noticeList.Add(new_event.GetNumber());
			PlayerPrefsX.SetIntArray(GameInformation.NOTICE_KEY, GameInformation.noticeList.ToArray());
		}
	}

	// トラブルアイテム番号からパラメータイベントを設定
	public void AddParameterEventNodeByTroubleItemNumber(int num) {
		ParameterEvent e = GetParameterEventByTroubleNumber(num);
		AddParamterEvent(e);
	}

	// パラメータイベント番号からパラメータイベントを設定
	public string AddParameterEventNodeByNumber(int num) {
		ParameterEvent e = GetParameterEvent(num);
		AddParamterEvent(e, false);
		return e.GetTrainingName();
	}

	private void AddParamterEvent(ParameterEvent e, bool notice_flag = true) {
		SetParameterEvent(e);
		if (notice_flag) {
			// 通知リストに追加
			GameInformation.noticeList.Add(e.GetNumber());
			PlayerPrefsX.SetIntArray(GameInformation.NOTICE_KEY, GameInformation.noticeList.ToArray());
		}

		// ソート
		if (sort == null) {
			sort = this.gameObject.GetComponent<Sort>();
		}
		if (e.GetNumber() < 200) {
			sort.SortItem(parameterList.int_list_parent);
		} else if (e.GetNumber() < 300) {
			sort.SortItem(parameterList.body_list_parent);
		}	else if (e.GetNumber() < 400) {
			sort.SortItem(parameterList.like_list_parent);
		}
	}

	// set parameter event by save data
	private void SetParameterEventList(int[] array, int baseNum, List<TrainingData> list, int max) {
		int num = 0;
		bool flag = false;
		ParameterEvent temp_event;

		foreach(int i in array) {
			if (i != 0){
				// 新規データかどうか
				flag = (i == 1);
				temp_event = GetParameterEvent(num + baseNum);
				list.Add(new TrainingData {newFlag = flag, praEvent = temp_event});
			}
			if (max == (num + 1)) {
				break;
			}
			num++;
		}
	}

	// パラメータ項目リストのノード作成
	private void CreateAllParameterList() {
		// Create INT list
		CreateParameterList(GameInformation.IntList, parameterList.int_list_parent);
		// Create BODY list
		CreateParameterList(GameInformation.BodyList, parameterList.body_list_parent);
		// Create LIKE list
		CreateParameterList(GameInformation.LikeList, parameterList.like_list_parent);
	}

	private void CreateParameterList (List<TrainingData> list, GameObject parentObj) {
		foreach(TrainingData data in list) {
			AddParameterEventNode(data.praEvent, parentObj, data.newFlag);
		}
	}

	private void CreateComboList () {
		// コンボリスト取得
		GameInformation.ComboList = new List<int>(PlayerPrefsX.GetIntArray(GameInformation.COMBO_KEY, 0, 0));
		foreach(int num in GameInformation.ComboList) {
			AddComboNode(GetComboEvent(num));
		}
	}

	// パラメータ一覧にパラメータノードを追加
	private void AddParameterEventNode(ParameterEvent pe, GameObject parentObj, bool newFlag = true) {
		if (parentObj != null) {
			GameObject node;
			if (pe.GetSpecialFlag()) {
				 node = Instantiate(Resources.Load<GameObject>(trainingNodePath + "2"));
			} else {
				 node = Instantiate(Resources.Load<GameObject>(trainingNodePath));
			}

			node.transform.SetParent(parentObj.transform, false);
			node.SetActive(true);
			GameObject textObj = node.transform.Find(nodeTextPath).gameObject;
			if (textObj != null) {
				Text itemName = textObj.GetComponentInChildren<Text>();
				itemName.text = pe.GetTrainingName();
			} else {
				Debug.Log("Do not exist Training Node Text!!");
			}
			// 項目番号ごとにノードに名前とnewフラグを反映させる
			GameObject buttonObj = node.transform.Find(nodeButtonPath).gameObject;
			buttonObj.name = node.name = pe.GetNumber().ToString();
			if (!newFlag) {
				GameObject newObj = buttonObj.transform.Find(nodeNewPath).gameObject;
				if (newObj != null) {
					newObj.SetActive(false);
				}
			}
		}
	}

	// コンボ一覧にコンボノードを追加
	private void AddComboNode(ComboEvent combo, bool sort_flag = false) {
		GameObject node = Instantiate(Resources.Load<GameObject>(comboNodePath));
		node.transform.SetParent(parameterList.combo_list_parent.transform, false);
		node.SetActive(true);

		GameObject textObj = node.transform.Find(nodeTextPath).gameObject;
		Text itemName = textObj.GetComponentInChildren<Text>();
		itemName.text = combo.GetTitle();

		// 項目番号ごとにノードに名前を反映させる
		GameObject buttonObj = node.transform.Find(nodeButtonPath).gameObject;
		buttonObj.name = node.name = combo.GetNumber().ToString();

		if (sort_flag) {
			// ソート
			if (sort == null) {
				sort = this.gameObject.GetComponent<Sort>();
			}
			sort.SortItem(parameterList.combo_list_parent);
		}
	}

	// パラメータイベントを選択中にする
	private void SelectParameterEventNode(int num, GameObject obj) {
		int array_num = 0;
		bool newFlag = false;

		if (num < 200) {
				array_num = num - 101;
				if (GameInformation.TrainingINTArray[array_num] == 1) {
					GameInformation.TrainingINTArray[array_num] = 2;
					PlayerPrefsX.SetIntArray(GameInformation.TrainingINT_KEY, GameInformation.TrainingINTArray);
					newFlag = true;
				}
		} else if (num < 300) {
				array_num = num - 201;
				if (GameInformation.TrainingBODYArray[array_num] == 1) {
					GameInformation.TrainingBODYArray[array_num] = 2;
					PlayerPrefsX.SetIntArray(GameInformation.TrainingBODY_KEY, GameInformation.TrainingBODYArray);
					newFlag = true;
				}
		} else if (num < 400){
				array_num = num - 301;
				if (GameInformation.TrainingLIKEArray[array_num] == 1) {
					GameInformation.TrainingLIKEArray[array_num] = 2;
					PlayerPrefsX.SetIntArray(GameInformation.TrainingLIKE_KEY, GameInformation.TrainingLIKEArray);
					newFlag = true;
				}
		}

		// 今まで選択していなかった項目を選択したときNEW画像を非アクティブに変更
		if (newFlag) {
			GameObject newObj = obj.transform.Find(nodeNewPath).gameObject;
			if (newObj != null) {
				newObj.SetActive(false);
			}
		}
		// 背景色変更
		Image itemImage = obj.GetComponent<Image>();
		itemImage.sprite = trainingItemImages.selected;
	}

	// コンボを選択中にする
	private void SelectComboNode(GameObject obj) {
		// 背景色変更
		Image itemImage = obj.GetComponent<Image>();
		itemImage.sprite = trainingItemImages.selected;
	}

	// 選択したコンボを初期化
	public void ClearComboNode() {
		// 背景色変更
		if (selectedComboEvent != null) {
			Image itemImage = selectedComboEvent.GetComponent<Image>();
			itemImage.sprite = trainingItemImages.default1;
			selectedComboEvent = null;
		}
		selectedComboNum = 0;
	}

	private void RemoveParameterEventNode(int num, GameObject parentObj) {
		if (parentObj != null) {
			GameObject obj = parentObj.transform.Find(num.ToString()).gameObject;
			if (obj != null) {
				Destroy(obj);
			}
		}
	}

	public void RemoveSpecialParameterEvent() {
		GameInformation.TrainingINTArray = PlayerPrefsX.GetIntArray(GameInformation.TrainingINT_KEY, 0, 0);
		GameInformation.TrainingBODYArray = PlayerPrefsX.GetIntArray(GameInformation.TrainingBODY_KEY, 0, 0);
		GameInformation.TrainingLIKEArray = PlayerPrefsX.GetIntArray(GameInformation.TrainingLIKE_KEY, 0, 0);

		List<ParameterEvent> list = parameterDataBase.GetParameterEventLists();
		foreach(ParameterEvent e in list) {
			if (e.GetSpecialFlag()) {
				int num = e.GetNumber();
				if (num < 200) {
					GameInformation.TrainingINTArray[num-101] = 0;
					PlayerPrefsX.SetIntArray(GameInformation.TrainingINT_KEY, GameInformation.TrainingINTArray);
				} else if (num < 300) {
					GameInformation.TrainingBODYArray[num-201] = 0;
					PlayerPrefsX.SetIntArray(GameInformation.TrainingBODY_KEY, GameInformation.TrainingBODYArray);
				} else if (num < 400) {
					GameInformation.TrainingLIKEArray[num-301] = 0;
					PlayerPrefsX.SetIntArray(GameInformation.TrainingLIKE_KEY, GameInformation.TrainingLIKEArray);
				}
			}
		}
	}

	// パラメータ成長処理
	public void GrowUp(int value, int type) {
		switch (type) {
			case 1:
				if (GameInformation.INT + value > GameInformation.Parameter_MAX) {
					GameInformation.INT = GameInformation.Parameter_MAX;
				} else if (GameInformation.INT + value < 0) {
					GameInformation.INT	= 0;
				}	else {
					GameInformation.INT += value;
				}
				PlayerPrefs.SetInt(GameInformation.INT_KEY, GameInformation.INT);
				StartCoroutine(parameter_manager.ShowParameterUpDownIcon(value, 1));
				Debug.Log("+++++++++あたま変動値:" + value);
				break;
			case 2:
				if (GameInformation.BODY + value > GameInformation.Parameter_MAX) {
					GameInformation.BODY = GameInformation.Parameter_MAX;
				} else if (GameInformation.BODY + value < 0) {
					GameInformation.BODY = 0;
				} else {
					GameInformation.BODY += value;
				}
				PlayerPrefs.SetInt(GameInformation.BODY_KEY, GameInformation.BODY);
				StartCoroutine(parameter_manager.ShowParameterUpDownIcon(value, 2));
				Debug.Log("+++++++++からだ変動値:" + value);
				break;
			case 3:
				if (GameInformation.LIKE + value > GameInformation.Parameter_MAX) {
					GameInformation.LIKE = GameInformation.Parameter_MAX;
				} else if (GameInformation.LIKE + value < 0) {
					GameInformation.LIKE = 0;
				} else {
					GameInformation.LIKE+=value;
				}
				PlayerPrefs.SetInt(GameInformation.LIKE_KEY, GameInformation.LIKE);
				StartCoroutine(parameter_manager.ShowParameterUpDownIcon(value, 3));
				Debug.Log("+++++++++きもち変動値:" + value);
				break;
			default:
				break;
		}
	}

	private void ClearButtonBackground(GameObject obj, Sprite initSprite) {
		// 背景色変更
		Image itemImage = obj.GetComponent<Image>();
		itemImage.sprite = initSprite;
	}

	// 育成項目背景すべて初期化
	public void ClearAllButtonBackground() {
		for(int i = 0; i < selectedItemArray.Length; i++) {
			if (selectedItemArray[i] != null && trainingArray[i] != null) {
				if (trainingArray[i].GetSpecialFlag()) {
					ClearButtonBackground(selectedItemArray[i], trainingItemImages.default2);
				} else {
					ClearButtonBackground(selectedItemArray[i], trainingItemImages.default1);
				}
			}
		}
		Array.Clear(selectedItemArray, 0, selectedItemArray.Length);
	}

	// 育成選択画面のボタンをすべて初期化
	public void ClearTrainingButton() {
		foreach(GameObject o in selectButton) {
			o.GetComponent<Image>().sprite = notSelectImage;
		}
		Array.Clear(trainingArray, 0, trainingArray.Length);
	}
}
