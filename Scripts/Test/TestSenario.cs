using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Utage;

public class TestSenario : MonoBehaviour {
	AdvEngine Engine { get { return engine ?? (engine = FindObjectOfType<AdvEngine>()); } }
	public AdvEngine engine; // utage
	public static bool isTestMode = false;
	public Text titleText;
	public GameObject whitePanel;
	public CanvasGroup group;

	public void TestModeOn() {
		isTestMode = true;
		GameObject sceneController = GameObject.Find("SceneController").gameObject;
		SceneController sc = sceneController.GetComponent<SceneController>();
		sc.LoadMain();
	}

	public void SetTitle(string title) {
			titleText.text = title;
	}

	public void StartSenario()
	{
		if (titleText.text.Length > 0)
		{
			StartCoroutine(CoTalk(titleText.text));
		}
	}

	public IEnumerator CoTalk(string label) {
		AudioManager.Instance.FadeOutBGM (1);
		group.alpha = 0;
		group.interactable = false;
		group.blocksRaycasts = false;

		if (whitePanel == null) {
			GameObject canvas = GameObject.Find("/Canvas").gameObject;
			whitePanel = canvas.transform.Find("WhitePanel").gameObject;
		}
		whitePanel.SetActive(true);

		Engine.JumpScenario(label);

		while(!Engine.IsEndScenario) {
			yield return 0;
		}

		whitePanel.SetActive(false);
		group.alpha = 1;
		group.interactable = true;
		group.blocksRaycasts = true;
		AudioManager.Instance.PlayBGM(AUDIO.BGM_MAIN_01);
	}
 }
