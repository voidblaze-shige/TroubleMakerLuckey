using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : SingletonMonoBehaviour<DataManager> {
	public void Awake() {
		if (this != Instance) {
			Destroy (this);
			return;
		}
		DontDestroyOnLoad (this.gameObject);
	}

	public void Delete() {
		bool audio_flg = SaveData.GetBool(GameInformation.AUDIOSWITCH_KEY,false);
		bool notice_flg = SaveData.GetBool(GameInformation.PUSH_OFF_KEY, false);
		bool item01 = SaveData.GetBool(GameInformation.SHOP_FULL_PACK01_KEY, false);
		bool item02 = SaveData.GetBool(GameInformation.SHOP_ITEM_PACK01_KEY, false);
		bool item03 = SaveData.GetBool(GameInformation.SHOP_MEGAPHONE_PACK01_KEY, false);

		PlayerPrefs.DeleteAll();
		SaveData.SetBool(GameInformation.AUDIOSWITCH_KEY, audio_flg);
		SaveData.SetBool(GameInformation.PUSH_OFF_KEY, notice_flg);
		SaveData.SetBool(GameInformation.SHOP_FULL_PACK01_KEY, item01);
		SaveData.SetBool(GameInformation.SHOP_ITEM_PACK01_KEY, item02);
		SaveData.SetBool(GameInformation.SHOP_MEGAPHONE_PACK01_KEY, item03);
		PlayerPrefs.Save();
	}
}
