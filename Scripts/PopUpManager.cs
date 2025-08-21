using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour {
	private string titlePath = "UI/TitleConfirm";
	private string tutorialPath = "UI/Tutorial";
	private string megaphonePath = "UI/MegaphoneConfirm";

	public GameObject canvas;

	public void OpenTitleBackConfirm() {
		CreatePopUp(titlePath);
	}

	public void OpenTutorial() {
		CreatePopUp(tutorialPath);
	}

	public void OpenMegaphoneConfirm() {
		GameInformation.PopUpFlag = true;
		AudioManager.Instance.PlaySE(AUDIO.SE_OPEN);
		CreatePopUp(megaphonePath);
	}

	private void CreatePopUp(string path) {
		GameObject obj = Instantiate(Resources.Load<GameObject>(path));
		obj.transform.SetParent(canvas.transform, false);
	 	obj.SetActive(true);
	}
}
