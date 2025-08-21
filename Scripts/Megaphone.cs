using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Megaphone : MonoBehaviour {

	public void UseMegaphone() {
		if (GameInformation.SleepingTime > 1) {
			SleepingManager sleeping_manager = GameObject.FindGameObjectWithTag("MainManager").GetComponent<SleepingManager>();
			sleeping_manager.SwitchMegaphoneInteractable(false);

			AudioManager.Instance.PlaySE(AUDIO.SE_MEGAPHONE);

			// 魔法のメガホンを持っていないとき数を減らす
			if (!SaveData.GetBool(GameInformation.SHOP_MEGAPHONE_PACK01_KEY, false)) {
				int num = PlayerPrefs.GetInt(GameInformation.MEGAPHONE_KEY, 0) - 1;

				if (num < 0) {
					Debug.Log("メガホンの数がマイナスになっています");
					num = 0;
				}

				PlayerPrefs.SetInt(GameInformation.MEGAPHONE_KEY, num);
			}

	    GameInformation.SleepingTime = 1;
			this.gameObject.SetActive(false);
			GameInformation.PopUpFlag = false;
			Destroy(this.gameObject);
		} else {
			AudioManager.Instance.PlaySE(AUDIO.SE_BUZZER);
		}
	}

	public void ClickDown() {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLICK_DOWN);
	}

	public void WindowClose() {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLOSE);
		this.gameObject.SetActive(false);
		GameInformation.PopUpFlag = false;
		Destroy(this.gameObject);
	}
}
