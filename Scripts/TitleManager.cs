using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour {

	private bool isFinishedSplashScreen = false;
	private bool active_flag = false;
	private GameObject sceneController; // scene controller
	public GameObject touchPanel;

	void Awake() {
		if (PlayerPrefs.GetInt(GameInformation.AUDIOSWITCH_KEY, 0) == 0) {
			AudioListener.volume = 1f;
		} else {
			AudioListener.volume = 0f;
		}
	}

	void Start () {
		AudioManager.Instance.PlayBGM (AUDIO.BGM_TITLE);
		sceneController = GameObject.FindGameObjectWithTag("SceneController").gameObject;
	}

	void Update() {
		if (!active_flag) {
			if (isFinishedSplashScreen) {
					Invoke("ActiveTouchPanel", 0.2f);
			}

			if (UnityEngine.Rendering.SplashScreen.isFinished) {
				isFinishedSplashScreen = true;
			}
		}
	}

	private void ActiveTouchPanel() {
		touchPanel.GetComponent<Image>().raycastTarget = true;
		active_flag = true;
	}

	public void StartGame() {
		SceneController sc = sceneController.GetComponent<SceneController>();
		sc.LoadMain();
	}

	public void DeleteAllData() {
		DataManager.Instance.Delete();
		AudioManager.Instance.PlaySE(AUDIO.SE_DELETE);
	}
}
