using System.Linq;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureManager : MonoBehaviour {

	[SerializeField]
	private ParameterEventDataBase parameterDataBase;
	public TrainingManager training_manager;
	public GameObject parent;
	public AnimationController anim_controller;

	private List<ParameterEvent> parameterEventList;
	private string furniturePath = "Furniture/";
	private string tapImagePath = "tapImage";
	private string effectPath = "Effect";
	private string findMarkPath = "Exclamation_mark";
	private GameObject TV;
	private Notice notice;

	void Start() {
		training_manager = this.gameObject.GetComponent<TrainingManager>();
		notice = this.gameObject.GetComponent<Notice>();
	}

	// ゲーム起動時にゲーム日付までに追加された家具を表示する
	public void SetAllFurniture() {
		// データベースからパラメータイベントのリストを取得
		if (parameterEventList == null) {
			parameterEventList = parameterDataBase.GetParameterEventLists();
		}

		foreach(ParameterEvent e in parameterEventList) {
			// ゲーム日付までに表示されているべき家具の場合
		if (e.GetTabEventFlag() && e.GetLiverationPeriod() > 0
			&& e.GetLiverationPeriod() <= PlayerPrefs.GetInt(GameInformation.GAME_DATE_KEY, 1)) {
				// テレビの場合はその月の時のみ表示
				if (e.GetKindOfTap() == "TV") {
					// テレビの場合
					TV = CreateFurniture(e);
				} else {
					CreateFurniture(e);
				}
			}
		}
	}

	// 家具を置く(MainManagerから呼ぶ)
	public void SetFurniture() {
		// データベースからパラメータイベントのリストを取得
		if (parameterEventList == null) {
			parameterEventList = parameterDataBase.GetParameterEventLists();
		}

		// もし前の月にテレビを表示していた場合、削除
		if (TV != null) {
				Destroy(TV);
		}

		foreach(ParameterEvent e in parameterEventList) {
			// ゲーム日付と同じ出現条件の家具がないか
			if (e.GetTabEventFlag()
				&& PlayerPrefs.GetInt(GameInformation.GAME_DATE_KEY, 1) == e.GetLiverationPeriod()) {
				if (e.GetKindOfTap() == "TV") {
					// テレビの場合
					TV = CreateFurniture(e);
				} else {
					CreateFurniture(e);
				}
 			}
		}
	}

	private GameObject CreateFurniture(ParameterEvent e) {
		GameObject furniture = Instantiate(Resources.Load<GameObject>(furniturePath + tapImagePath));
		furniture.transform.SetParent(parent.transform, false);
		Image furnitureImage = furniture.GetComponent<Image>();
		furniture.name = e.GetNumber().ToString();
		furnitureImage.sprite = e.GetTapSprite();
		furnitureImage.SetNativeSize();
		RectTransform rect = furniture.GetComponent<RectTransform>();
		rect.localPosition = e.GetPosition();
		// エフェクトの追加
		SetEffect(furniture, e.GetNumber());
		furniture.SetActive(true);

		return furniture;
	}

	// これまでタップしたことがあるか
	private bool CheckAlreadyTap(int num) {
		// タップしたことのある家具リストを取得
		List<int> list = new List<int>(PlayerPrefsX.GetIntArray(GameInformation.FURNITURE_LIST_KEY, 0, 0));
		return list.Contains(num);
	}

	// エフェクトの追加
	private void SetEffect(GameObject obj, int num) {
		// 今までタップしたことがない場合
		if (!CheckAlreadyTap(num)) {
			GameObject effect = Instantiate(Resources.Load<GameObject>(furniturePath + effectPath));
			effect.transform.SetParent(obj.transform, false);
			effect.name = effectPath;
			effect.SetActive(true);
		}
	}

	public void TapFurniture(int  num, GameObject obj) {
		// タップアイテムをリストに格納
		List<int> list = new List<int>(PlayerPrefsX.GetIntArray(GameInformation.FURNITURE_LIST_KEY, 0, 0));
		// 今までタップしたことがない場合
		if (!list.Contains(num)) {
			list.Add(num);

			// エフェクトを消す
			GameObject effect = obj.transform.Find(effectPath).gameObject;
			Destroy(effect);

			// ビックリマーク表示
			AudioManager.Instance.PlaySE(AUDIO.SE_FIND);
			StartCoroutine(ShowFindMark(obj));

			if (training_manager == null) {
				training_manager = this.gameObject.GetComponent<TrainingManager>();
			}
			// 通知GameObjectが未定義の場合
			if (notice == null) {
				notice = this.gameObject.GetComponent<Notice>();
			}

			// パラメータイベントを追加
			string title = training_manager.AddParameterEventNodeByNumber(num);
			// セーブ
			PlayerPrefsX.SetIntArray(GameInformation.FURNITURE_LIST_KEY, list.ToArray());
			// 通知
			StartCoroutine(notice.ShowNotice(title));
		}
	}

	public void TapLucky(int  num, GameObject obj) {
		// タップアイテムをリストに格納
		List<int> list = new List<int>(PlayerPrefsX.GetIntArray(GameInformation.FURNITURE_LIST_KEY, 0, 0));
		// 今までタップしたことがない場合
		if (!list.Contains(num)) {

			// ビックリマーク表示
			AudioManager.Instance.PlaySE(AUDIO.SE_FIND);
			StartCoroutine(ShowFindMark(obj));

			if (training_manager == null) {
				training_manager = this.gameObject.GetComponent<TrainingManager>();
			}
			
			// パラメータイベントを追加
			string title = training_manager.AddParameterEventNodeByNumber(num);

			// 通知GameObjectが未定義の場合
			if (notice == null) {
				notice = this.gameObject.GetComponent<Notice>();
			}

			// 通知
			StartCoroutine(notice.ShowNotice(title));
		}
	}

	private IEnumerator ShowFindMark(GameObject obj) {
		GameObject find_mark = Instantiate(Resources.Load<GameObject>(furniturePath + findMarkPath));
		find_mark.transform.SetParent(obj.transform, false);
		find_mark.SetActive(true);
		Animation anim = find_mark.GetComponent<Animation>();

		anim.Play();

		yield return anim_controller.WaitForAnimation(anim);

		Destroy(find_mark);
	}
}
