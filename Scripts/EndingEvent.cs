using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "EndingEvent", menuName="Lucky/EndingEvent")]
public class EndingEvent : ScriptableObject {

	// エンディングID
	[SerializeField]
	private int id;

	// エンディング番号
	[SerializeField]
	private int number;

	[SerializeField]
	private string title;

	// 画像
	[SerializeField]
	private Sprite image;

	// パラメータ条件
	[SerializeField]
	private int atama;

	// パラメータ条件
	[SerializeField]
	private int karada;

	// パラメータ条件
	[SerializeField]
	private int kimochi;

	// 関連するトラブルイベント
	[SerializeField]
	private int troubleNumber;

	// 優先度
	[SerializeField]
	private int priority;

	public int GetId(){
		return id;
	}

	public int GetNumber() {
		return number;
	}

	public string GetTitle() {
		return title;
	}

	public Sprite GetImage() {
		return image;
	}

	public int GetAtamaRate() {
		return atama;
	}

	public int GetKaradaRate() {
		return karada;
	}

	public int GetKimochiRate() {
		return kimochi;
	}

	public int GetTroubleNumber() {
		return troubleNumber;
	}

	public int GetPriority() {
		return priority;
	}
}
