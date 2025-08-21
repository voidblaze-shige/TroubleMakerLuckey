using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGetPopup : MonoBehaviour {
	public void RemovePopup(GameObject obj) {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLOSE);
		obj.SetActive(false);
		GameInformation.PopUpFlag = false;
		Destroy(obj);
	}
}
