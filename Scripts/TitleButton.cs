using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleButton : MonoBehaviour {

	public void BackToTitle() {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLICK_UP);
		GameObject obj = GameObject.FindGameObjectWithTag("SceneController").gameObject;
		SceneController sceneController = obj.GetComponent<SceneController>();
		sceneController.LoadTitle();
	}

	public void ClickDown() {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLICK_DOWN);
	}

	public void WindowClose() {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLOSE);
		this.gameObject.SetActive(false);
		Destroy(this.gameObject);
	}
}
