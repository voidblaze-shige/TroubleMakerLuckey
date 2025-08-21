using System;
using System.IO;
﻿using System.Collections;
using UnityEngine;
using SocialConnector;

public class ShareController : MonoBehaviour {

	public void Share() {
			StartCoroutine(ShareScreenShot());
	}

	IEnumerator ShareScreenShot() {
			//スクリーンショット画像の保存先を設定。ファイル名が重複しないように実行時間を付与
			string fileName = String.Format("image_{0:yyyyMMdd_Hmmss}.png", DateTime.Now);
			string imagePath = Application.persistentDataPath + "/" + fileName;

			//スクリーンショットを撮影
			ScreenCapture.CaptureScreenshot(fileName);
			yield return new WaitForEndOfFrame();

			// Shareするメッセージを設定
			string text ="トラブルメイカーラッキーをプレイしています\n#トラブルメイカーラッキー ";
			string URL = "https://play.google.com/store/apps/details?id=com.example.app";
			yield return new WaitForSeconds(1);

			//Shareする
			SocialConnector.SocialConnector.Share(text, URL, imagePath);
	}
}
