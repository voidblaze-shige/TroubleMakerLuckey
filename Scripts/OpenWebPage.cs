using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenWebPage : MonoBehaviour {

	public GameObject confirm;
	public Button button;
	public Text messageText;

	private string dev_message = "開発者ページに移動しますか？" + Environment.NewLine + "（ブラウザが起動します）";
	private string pri_message = "プライバシーポリシーのページ" +	Environment.NewLine + "に移動しますか？" +	Environment.NewLine + "（ブラウザが起動します）";
	private string tok_message = "特定商取引法に基づく表記のページ" + Environment.NewLine + "に移動しますか？" +	Environment.NewLine + "（ブラウザが起動します）";

	public void OpenDeveloperPage() {
		OpenConfirm("http://mb-vil.com/", dev_message);
	}

	public void OpenPrivacyPage() {
		OpenConfirm("http://mb-vil.com/privacypolicy.html", pri_message);
	}

	public void OpenTokuteiPage() {
		OpenConfirm("http://mb-vil.com/tokutei.html", tok_message);
	}

	public void OpenConfirm(string url, string message) {
		// set method
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(() => OpenURL(url));
		AudioManager.Instance.PlaySE(AUDIO.SE_OPEN);
		messageText.text = message;
		confirm.SetActive(true);
	}

	private void OpenURL(string url) {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLICK_UP);
		Application.OpenURL(url);
		confirm.SetActive(false);
	}
}
