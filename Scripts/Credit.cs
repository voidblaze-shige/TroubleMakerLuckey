using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credit : MonoBehaviour {
	private string creditPath = "UI/Credit";
	private string canvasTag = "Canvas";

	public void OpenCredit(bool isTitle) {

		GameObject credit = Instantiate(Resources.Load<GameObject>(creditPath));
		GameObject parent = GameObject.FindGameObjectWithTag(canvasTag).gameObject;
	 	credit.transform.SetParent(parent.transform, false);
	 	credit.SetActive(true);
	}

	public void Exit() {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLOSE);
		Destroy(this.gameObject);
	}

	public void ClickDown() {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLICK_DOWN);
	}
}
