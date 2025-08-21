using System.Linq;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchPage : MonoBehaviour {

	[SerializeField]
	public List<GameObject> pageList;

	public List<ScrollRectList> scrollRectList;
	public bool isMenu;

	// スクロールビューのあるページリスト
	[System.SerializableAttribute]
	public class ScrollRectList {
		public int pageNum;
		public ScrollRect scrollRect;
	}

	private GameObject _currentPage;
	private ItemManager item_manager;

	void Awake () {
		// Insert ItemPage Object
		_currentPage = pageList[0];
	}

 //Switch Page
	public void SwitchPageItem(GameObject o) {
		if ( _currentPage != o ) {
			SetInActiveAll();
		// Play SE
		AudioManager.Instance.PlaySE(AUDIO.SE_SWITCH_PAGE);
		o.SetActive(true);
		_currentPage = o;
		}
	}

	private void SetInActiveAll() {
		if (isMenu) {
			if (item_manager == null) {
				item_manager = GameObject.FindGameObjectWithTag("MainManager").GetComponent<ItemManager>();
			}
			item_manager.InitDetail();
		}

		foreach(var item in pageList.Select((Value, Index) => new { Value, Index })) {
			GameObject o = item.Value;
			int count = item.Index + 1;
			foreach(ScrollRectList srlist in scrollRectList) {
				if (srlist.pageNum == count) {
					ResetScrollRect(srlist.scrollRect);
				}
 			}
			if (o.activeSelf) {
				o.SetActive(false);
			}
		}
	}

	//はじめのページだけアクティブにする
	public void InitPage() {
		SetInActiveAll();
		pageList[0].SetActive(true);
		_currentPage = pageList[0];
	}

	// ScrollRectを初期位置に戻す
	private void ResetScrollRect (ScrollRect scrollRect) {
		scrollRect.verticalNormalizedPosition = 1;
	}
}
