using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "Item", menuName="Lucky/Item")]
public class Item : ScriptableObject {

	public enum KindOfType {
		Atama,
		Karada,
		Kimochi,
		All,
		Other
	}

	// アイテム番号
	[SerializeField]
	private int number;

	// アイコン
	[SerializeField]
	private Sprite icon;

	// アイテム取得画像
	[SerializeField]
	private Sprite getImage;

	// タイプ
	[SerializeField]
	private KindOfType kindOfType;

	// アイテム名
	[SerializeField]
	private string itemName;

	// 上昇する値
	[SerializeField]
	private int increaseValue;

	// アイテム詳細
	[SerializeField]
	private string information;

	// 課金アイテムフラグ
	[SerializeField]
	private bool specialFlag;

	// 持越し可能フラグ
	[SerializeField]
	private bool carryOverFlag;

	public int GetNumber() {
		return number;
	}

	public Sprite GetIcon() {
		return icon;
	}

	public Sprite GetImage() {
		return getImage;
	}

	public int GetKindOfType() {
		int num = 0;
		switch (kindOfType) {
			case KindOfType.Atama:
				num = 1;
				break;
			case KindOfType.Karada:
				num = 2;
				break;
			case KindOfType.Kimochi:
				num = 3;
				break;
/*
			case KindOfType.All:
				num = 4;
				break;
			case KindOfType.Other:
				num = 5;
				break;
*/
			default:
				break;
		}
		return num;
	}

	public int GetIncreaseValue() {
		return increaseValue;
	}

	public string GetItemName() {
		return itemName;
	}

	public string GetInformation() {
		return information;
	}

	public bool GetSpecialFlag () {
		return specialFlag;
	}

	public bool GetCarryOverFlag () {
		return carryOverFlag;
	}
}
