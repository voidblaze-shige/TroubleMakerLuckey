using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SleepingManager : MonoBehaviour {
	private int[] sleeping_time = {180,300,480,780,1260,1800,2400,3000,3600,4200,4800,0};
	private Text timeText;
	private bool is_start_sleeping = false;
	private GameObject managerObj;
	private MainManager manager;
	private string managerTag = "MainManager";
	private string sleepingZPath = "Sleeping/Lucky_z";
	private const string itemGetPopupNodePath = "Item/ItemGet";
	private const string popupTextPath = "MenuBack/TextBack/Text";
	private const string popupImagePath = "MenuBack/Image";

	public GameObject sleepingPanel;
	public GameObject timeObj;
	public GameObject sleepingZParent;
	public GameObject present;
	public GameObject megaphone;
	public GameObject magicMegaphone;
	public Text megaphone_number;
	public Sprite megaphoneImage;
	public GameObject canvas;
	public FadeScreen fadeScreen;

	void Update () {

		if (GameInformation.IsSleeping && !is_start_sleeping) {
			// 開始フラグをたてる
			is_start_sleeping = true;
			StartCreateSleepingZ();
			// 待ち時間取得
			GameInformation.SleepingTime = PlayerPrefs.GetInt(GameInformation.SleepingTime_KEY, -1);
			// ストーリー進行度によるおやすみ時間の取得
			if (GameInformation.SleepingTime < 0) {
				GameInformation.SleepingTime = sleeping_time[GameInformation.GAME_DATE - 1];
				PlayerPrefs.SetInt(GameInformation.SleepingTime_KEY, GameInformation.SleepingTime);
				//フェードアウト
				fadeScreen.Fadeout();
				Invoke("StartSleeping" , 0.8f);
			} else {
				// 前回起動した時刻から残り時間を計算
				string timestring = PlayerPrefs.GetString(GameInformation.LAST_TIME_KEY, System.DateTime.Now.ToString());
				System.DateTime datetime = System.DateTime.Parse(timestring);
				System.TimeSpan span = System.DateTime.Now - datetime;
				double spantime = span.TotalSeconds;
				GameInformation.SleepingTime -= (int)spantime;
				if (GameInformation.SleepingTime < 0) {
					GameInformation.SleepingTime = 0;
				}
				StartSleeping();
			}
		}

		if (GameInformation.IsSleeping) {

			if (GameInformation.SleepingTime == 0) {
				megaphone.GetComponent<Button>().interactable = false;
				magicMegaphone.GetComponent<Button>().interactable = false;
				present.GetComponent<Button>().interactable = false;
			}

			// 魔法のメガホンを持っていない
			if (!SaveData.GetBool(GameInformation.SHOP_MEGAPHONE_PACK01_KEY, false)) {
				// メガホンの残り数を表示
				if (megaphone.activeSelf && PlayerPrefs.GetInt(GameInformation.MEGAPHONE_KEY, 0) != int.Parse(megaphone_number.text)) {
					megaphone_number.text = PlayerPrefs.GetInt(GameInformation.MEGAPHONE_KEY, 0).ToString();
				}
			}
		}
	}

	public void StartSleeping() {
		// おやすみ画面を立ち上げる
		int megaphone_number = PlayerPrefs.GetInt(GameInformation.MEGAPHONE_KEY, 0);

		// 魔法のメガホンを持っているか
		if (SaveData.GetBool(GameInformation.SHOP_MEGAPHONE_PACK01_KEY, false)) {
			if (megaphone.activeSelf) {
				megaphone.SetActive(false);
			}
			magicMegaphone.GetComponent<Button>().interactable = true;
			magicMegaphone.SetActive(true);

		} else {
			megaphone.GetComponent<Button>().interactable = true;

			if (megaphone_number > 0) {
				megaphone.SetActive(true);
				present.SetActive(false);
			} else {
				present.GetComponent<Button>().interactable = true;
				present.SetActive(true);
				megaphone.SetActive(false);
			}
		}

		sleepingPanel.SetActive(true);
		AudioManager.Instance.PlayBGM (AUDIO.BGM_SLEEPING, AudioManager.BGM_FADE_SPEED_RATE_HIGH);

		timeText = timeObj.GetComponentInChildren<Text>();
		InsertTimetoText(GameInformation.SleepingTime, timeText);

		PlayerPrefs.SetString(GameInformation.LAST_TIME_KEY, System.DateTime.Now.ToString());
		Invoke("Sleeping" , 1);
	}

	private void Sleeping () {
		System.TimeSpan span = System.DateTime.Now - System.DateTime.Parse(PlayerPrefs.GetString(GameInformation.LAST_TIME_KEY, System.DateTime.Now.ToString()));
		double spantime = span.TotalSeconds;
		if ((int)spantime > 1) {
			GameInformation.SleepingTime -= (int)spantime;
			if (GameInformation.SleepingTime < 0) {
				GameInformation.SleepingTime = 0;
			}
		} else {
			GameInformation.SleepingTime -= 1;
		}

		PlayerPrefs.SetInt(GameInformation.SleepingTime_KEY, GameInformation.SleepingTime);
		InsertTimetoText(GameInformation.SleepingTime , timeText);

		if (GameInformation.SleepingTime > 0) {
			Invoke("Sleeping" , 1);
			// 現在時刻を保存
			System.DateTime now = System.DateTime.Now;
			PlayerPrefs.SetString(GameInformation.LAST_TIME_KEY, now.ToString());

		} else if (GameInformation.SleepingTime <= 0 && !GameInformation.PopUpFlag) {
			GameInformation.SleepingTime = -1;
			// おやすみフラグ
			GameInformation.IsSleeping = false;
			SaveData.SetBool(GameInformation.IS_SLEEPING_KEY, GameInformation.IsSleeping);
			PlayerPrefs.SetInt(GameInformation.SleepingTime_KEY, GameInformation.SleepingTime);

			AudioManager.Instance.FadeOutBGM (0.1f);

			is_start_sleeping = false;
			sleepingPanel.SetActive(false);

			// 次の日の処理
			if (manager == null) {
				managerObj = GameObject.FindGameObjectWithTag(managerTag);
				manager = managerObj.GetComponent<MainManager>();
			}
			manager.NextDay();
		} else {
			Invoke("Sleeping" , 1);
		}
	}

	private void InsertTimetoText(int time, Text obj) {
		if (time < 0) time = 0;
		int minutes = time / 60;
		int seconds = time % 60;
		obj.text = String.Format("{0:D2}", minutes) + ":" + String.Format("{0:D2}", seconds);
	}

	private void StartCreateSleepingZ( ) {
		if (GameInformation.IsSleeping) {
			StartCoroutine(CreateZ());
			Invoke("StartCreateSleepingZ", 1.5f);
		}
	}

	private IEnumerator CreateZ() {
		GameObject obj = Instantiate(Resources.Load<GameObject>(sleepingZPath));
		obj.transform.SetParent(sleepingZParent.transform, false);
		obj.SetActive(true);

		yield return new WaitForSeconds(4);

		obj.SetActive(false);
		Destroy(obj);
	}

	public void GetMegaphone() {
		// 2から4の値を取得
		int num = UnityEngine.Random.Range(0, 3) + 2;
		PlayerPrefs.SetInt(GameInformation.MEGAPHONE_KEY, num);
		present.SetActive(false);
		megaphone.SetActive(true);
		ShowItemGetPopup(num);
		AudioManager.Instance.PlaySE(AUDIO.SE_NICE);
	}

	private void ShowItemGetPopup(int num) {
		GameInformation.PopUpFlag = true;
		GameObject popup = Instantiate(Resources.Load<GameObject>(itemGetPopupNodePath));
		popup.transform.SetParent(canvas.transform, false);
		// テキストにアイテム名を入れる
		// アイテム画像を入れる
		GameObject textObj = popup.transform.Find(popupTextPath).gameObject;
		GameObject imageObj = popup.transform.Find(popupImagePath).gameObject;
		Text text = textObj.GetComponent<Text>();
		Image image = imageObj.GetComponent<Image>();
		text.text = "「" + "メガホン" + "」を"+ num + "個入手しました！";
		image.sprite = megaphoneImage;

		popup.SetActive(true);
	}

	public void SwitchMegaphoneInteractable(bool flag) {
		if (SaveData.GetBool(GameInformation.SHOP_MEGAPHONE_PACK01_KEY, false)) {
			magicMegaphone.GetComponent<Button>().interactable = flag;
		} else {
			megaphone.GetComponent<Button>().interactable = flag;
		}
	}

	public void SwitchGiftInteractable(bool flag) {
		present.GetComponent<Button>().interactable = flag;
	}

}
