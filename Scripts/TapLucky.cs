using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapLucky : MonoBehaviour {
	private int tap_count = 0;
	// 踊るのパラメータイベント番号
	private int eventNumber = 208;
	private string manager_tag = "MainManager";

	public void Tap() {
		// ラッキー鳴く
		AudioManager.Instance.PlaySE(AUDIO.SE_LUCKY1);

		// ラッキーを５回タップするを達成してない場合
		if (!SaveData.GetBool(GameInformation.LUCKY_TAP_5_KEY, false)) {
			tap_count++;
			if (tap_count >= 5) {
				// ５回タップした記録を保存
				SaveData.SetBool(GameInformation.LUCKY_TAP_5_KEY, true);
				FurnitureManager furniture_manager = GameObject.FindGameObjectWithTag(manager_tag).GetComponent<FurnitureManager>();
				furniture_manager.TapLucky(eventNumber, this.gameObject);
			}
		}
	}
}
