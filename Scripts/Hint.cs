using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "Hint", menuName="Lucky/Hint")]
public class Hint : ScriptableObject {

	// ヒント番号
	[SerializeField]
	private int number;

	// エンディング
	[SerializeField]
	private string title;

	// ヒント内容
	[SerializeField]
	private string hint;

	// ナレーションメッセージ
	[SerializeField]
	private string message;

	public int GetNumber() {
		return number;
	}

	public string GetTitle() {
		return title;
	}

	public string GetHint() {
		return hint;
	}

	public string GetMessage() {
		return message;
	}
}
