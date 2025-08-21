using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : SingletonMonoBehaviour<SceneController> {
	private float interval = 0.5f;
	private float time = 0;
  private AsyncOperation async = null;
	private static bool isLoading = false;
	private bool isSecondLoadMain = false;
	private string loadingPath = "UI/LoadCanvas";
	private GameObject loading;

	void Awake() {
			if (this != Instance) {
				Destroy (this);
				return;
			}
			DontDestroyOnLoad(this);
	}

	private void InitLoading() {
		if (loading == null) {
			loading = Instantiate(Resources.Load<GameObject>(loadingPath));
			DontDestroyOnLoad(loading);
		}
		loading.GetComponent<CanvasGroup>().alpha = 0;
	}

	public void LoadMain() {
		if (!isSecondLoadMain){
			Invoke("LoadMain", 0.5f);
			isSecondLoadMain = true;
		} else {
			MoveTo("Main");
		}
	}

	public void LoadTitle() {
		MoveTo("Title");
	}

	private void MoveTo(string scenename) {
		if (!isLoading) {
			InitLoading();
			isLoading = true;
			loading.SetActive(true);
//			async = SceneManager.LoadSceneAsync(scenename);
//			async.allowSceneActivation = false;
			StartCoroutine(LoadAnimCoroutine(scenename));
		}
	}

	private IEnumerator LoadAnimCoroutine(string scenename) {
		time = 0;
		while (time <= interval) {
			loading.GetComponent<CanvasGroup>().alpha = Mathf.Lerp (0f, 1f, time / interval);
			time += Time.deltaTime;
			yield return null;
		}

/*
		while (!async.isDone) {
			if (async.progress >= 0.9f) {
						async.allowSceneActivation = true;
			}
			yield return null;
		}
		*/
		SceneManager.LoadScene(scenename);

		time = 0;
		while (time <= interval) {
			loading.GetComponent<CanvasGroup>().alpha = Mathf.Lerp (1f, 0f, time / interval);
			time += Time.deltaTime;
			yield return null;
		}

		yield return async;

		Destroy(loading);
		isLoading = false;
	}
}
