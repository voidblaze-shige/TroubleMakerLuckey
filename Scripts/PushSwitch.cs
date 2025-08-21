using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PushSwitch : MonoBehaviour {
	Image buttonImage;
	public Sprite imageON;
	public Sprite imageOFF;
	public GameObject pushSwitch;
	public GameObject titleOption;

	void Awake() {
		buttonImage = pushSwitch.GetComponent<Image>();
		InitButton();
	}

	public void ClickSwitch() {
		// プッシュ通知がONのとき
		if (!SaveData.GetBool(GameInformation.PUSH_OFF_KEY, false)) {
			buttonImage.sprite = imageOFF;
			SaveData.SetBool(GameInformation.PUSH_OFF_KEY, true);
		} else {
		// プッシュ通知がOFFのとき
			buttonImage.sprite = imageON;
			SaveData.SetBool(GameInformation.PUSH_OFF_KEY, false);
		}
	}

	public void InitButton() {
		// プッシュ通知がONのとき
		if (!SaveData.GetBool(GameInformation.PUSH_OFF_KEY, false)) {
			buttonImage.sprite = imageON;
		} else {
			buttonImage.sprite = imageOFF;
		}
	}
}
