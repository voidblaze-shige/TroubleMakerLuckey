using System.Linq;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour {
	[SerializeField]
	private ComboEventDataBase comboEventDataBase;

	// list
	private string windowPath = "Combo/ComboHintList";	// ウィンドウのパス
	private string nodePath = "Combo/ComboHintNode"; // ノードのパス
	private string contentPath = "MenuBack/ScrollPanel/ScrollView/Viewport(Panel)/Content"; // ノードの親
	private string textPath = "Text"; // ノードのテキスト（コンボタイトル）
	private string imagePath = "Image"; // ノードのアイコン
	private string buttonPath = "Button";

	// detail
	private string comboDetailPath = "Combo/ComboDetail";
	private string detailContentPath = "Explain/Mask/Content";
	private string detailPagePath = "Combo/DetailPage";
	private string detailIconPath = "ComboImage/";
	private string detailTitlePath = "Title/Text";
	private string detailParamPath = "Param/";
	private string detailGuidePath = "Line/Guide";
	private string guidePath = "Combo/Guide";
	private string starPath = "Combo/PointStar";
	private string[] paramName = {"Atama", "Karada", "Kimochi"};

	private const int MAX_STAR = 4;
	private int[] STAR_RATE = {0, 5, 10, 20, 100};

	private List<ComboEvent> allComboList;
	private int numOfCombo = 0;

	public GameObject parentObj;
	public TrainingManager training_manager;
	public Sprite foundImage; // 発見済みノード画像

	public void OpenComboHint() {
		GameObject window = Instantiate(Resources.Load<GameObject>(windowPath));
		window.transform.SetParent(parentObj.transform, false);

		CreateComboHintList(window.transform.Find(contentPath).gameObject);

		AudioManager.Instance.PlaySE(AUDIO.SE_OPEN);
		window.SetActive(true);
	}

	private void CreateComboHintList(GameObject parentObj) {
		if (allComboList == null)  {
			allComboList = comboEventDataBase.GetComboEventLists();
			numOfCombo = allComboList.Count;
		}
		List<int> comboList = new List<int>(PlayerPrefsX.GetIntArray(GameInformation.COMBO_KEY, 0, 0));
		bool existfoundCombo = comboList.Any();

		foreach (var item in allComboList.Select((Value, Index) => new { Value, Index })) {
			ComboEvent e = item.Value;
			int idx = item.Index;

			GameObject node = Instantiate(Resources.Load<GameObject>(nodePath));
			node.transform.SetParent(parentObj.transform, false);
			Text nodeText = node.transform.Find(textPath).GetComponent<Text>();
			Image nodeImage = node.transform.Find(imagePath).GetComponent<Image>();
			GameObject nodeButtonObj = node.transform.Find(buttonPath).gameObject;
			Button nodeButton = nodeButtonObj.GetComponent<Button>();
			nodeText.text = e.GetTitle();
			nodeImage.sprite = e.GetIcon();
			nodeButton.onClick.AddListener(() => OpenComboDetail(idx+1));

			// もし発見済みコンボの場合ピンク
			if (existfoundCombo && comboList.Contains(e.GetNumber())) {
				Image nodeButtonImage = nodeButtonObj.GetComponent<Image>();
				nodeButtonImage.sprite = foundImage;
			}

			node.SetActive(true);
		}
	}

	public void OpenComboDetail(int num) {
		// コンボ詳細を開くメソッド
		GameObject detailWindow = Instantiate(Resources.Load<GameObject>(comboDetailPath));
		detailWindow.transform.SetParent(parentObj.transform, false);

		GameObject detailContent = detailWindow.transform.Find(detailContentPath).gameObject;
		GameObject guideParent = detailWindow.transform.Find(detailGuidePath).gameObject;

		GameObject[] guide = new GameObject[numOfCombo];

		foreach(var item in comboEventDataBase.GetComboEventLists().Select((Value, Index) => new { Value, Index })) {
			ComboEvent e = item.Value;
			int idx = item.Index;

			guide[idx] = Instantiate(Resources.Load<GameObject>(guidePath));
			guide[idx].transform.SetParent(guideParent.transform, false);
			guide[idx].name = (idx+1).ToString();
			guide[idx].SetActive(true);

			GameObject page = CreateComboDetailPage(e);
			page.transform.SetParent(detailContent.transform, false);
			page.name = (idx+1).ToString();
			page.SetActive(true);
		}

		GameObject pointStar = Instantiate(Resources.Load<GameObject>(starPath));

		pointStar.transform.SetParent(guide[0].transform, false);
		pointStar.SetActive(true);

		// ウィンドウの初期設定
		Tutorial tutorial = detailWindow.GetComponent<Tutorial>();
		tutorial.SetContent(numOfCombo, detailContent, pointStar, guide);

		AudioManager.Instance.PlaySE(AUDIO.SE_OPEN);
		detailWindow.SetActive(true);

		// 指定のページを設定
		StartCoroutine(tutorial.SetPage(num));
	}

	// コンボ詳細画面の各ページ内容を作成
	private GameObject CreateComboDetailPage(ComboEvent e) {
		GameObject page = Instantiate(Resources.Load<GameObject>(detailPagePath));
		Text pageTitle = page.transform.Find(detailTitlePath).GetComponent<Text>();
		pageTitle.text = e.GetTitle();
		List<ParameterEvent> parameterList = GetParameterEventsByComboNum(e.GetNumber());

		int[] intAry = PlayerPrefsX.GetIntArray(GameInformation.TrainingINT_KEY, 0, 0);
		int[] bodyAry = PlayerPrefsX.GetIntArray(GameInformation.TrainingBODY_KEY, 0, 0);
		int[] likeAry = PlayerPrefsX.GetIntArray(GameInformation.TrainingLIKE_KEY, 0, 0);

		// コンボイベントの内容のパラメータイベントの画像を設定
		foreach (var item in parameterList.Select((Value, Index) => new { Value, Index })) {
			// 現在取得済みのパラメータイベントのみ
			ParameterEvent pe = item.Value;
			int idx = item.Index;
			bool showFlag = false;

			switch(pe.GetKindOfType()) {
				case 1:
					showFlag = intAry[pe.GetNumber() - 101] > 0;
					break;
				case 2:
					showFlag = bodyAry[pe.GetNumber() - 201] > 0;
					break;
				case 3:
					showFlag = likeAry[pe.GetNumber() - 301] > 0;
					break;
				default:
					break;
			}

			if (showFlag) {
				Image icon = page.transform.Find(detailIconPath + (idx+1).ToString()).GetComponent<Image>();
				icon.sprite = pe.GetIcon();
			}
		}

		// コンボイベントの効果を記述
		int[] comboValue = {e.GetAtamaValue(), e.GetKaradaValue(), e.GetKimochiValue()};

		foreach (var item in comboValue.Select((Value, Index) => new { Value, Index })) {
			int value = item.Value;
			int idx = item.Index;
			int star_rate = 0;
			string starText = "";

			for (int i = 0; i < MAX_STAR; i++) {
				if (value <= STAR_RATE[i]) {
					break;
				}
				star_rate++;
			}

			// 星設定
			for (int i = 1; i <= MAX_STAR; i++) {
				if (i <= star_rate) {
					starText = starText + "★";
				} else {
					starText = starText + "☆";
				}
			}

			// テキストに代入
			Text rateText = page.transform.Find(detailParamPath + paramName[idx] + "/Star").GetComponent<Text>();
			rateText.text = starText;
		}

		return page;
	}

	// コンボ番号からコンボイベントを取得
	private ComboEvent GetComboEvent(int num) {
		return training_manager.GetComboEvent(num);
	}

	// コンボ番号から３つのパラメータイベントを取得
	private List<ParameterEvent> GetParameterEventsByComboNum(int num)	{
		return training_manager.GetParameterEventsByComboNum(num);
	}
}
