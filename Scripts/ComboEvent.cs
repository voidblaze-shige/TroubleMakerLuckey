using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "ComboEvent", menuName="Lucky/ComboEvent")]
public class ComboEvent : ScriptableObject {

	// コンボ番号
	[SerializeField]
	private int number;

	// タイトル
	[SerializeField]
	private string title;

	// 上昇する値:あたま
	[SerializeField]
	private int atama_Value;

	// 上昇する値:からだ
	[SerializeField]
	private int karada_Value;

	// 上昇する値:きもち
	[SerializeField]
	private int kimochi_Value;

	// 結果詳細
	[SerializeField]
	private string resultInformation;

	// アイコン
	[SerializeField]
	private Sprite icon;

	public int GetNumber() {
		return number;
	}

	public string GetTitle() {
		return title;
	}

	public int GetAtamaValue() {
		return atama_Value;
	}

	public int GetKaradaValue() {
		return karada_Value;
	}

	public int GetKimochiValue() {
		return kimochi_Value;
	}

	public string GetResultInformation() {
		return resultInformation;
	}

	public Sprite GetIcon() {
		return icon;
	}
}
