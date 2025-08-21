using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "ShopItem", menuName="Lucky/ShopItem")]
public class ShopItem : ScriptableObject {

	// アイテム番号
	[SerializeField]
	private int number;


	// アイテム名
	[SerializeField]
	private string itemName;
/*
	// アイテム詳細
	[SerializeField]
	private string description;
*/

	// アイテム画像
	[SerializeField]
	private Sprite image;

	public int GetNumber() {
		return number;
	}

	public Sprite GetImage() {
		return image;
	}

	public string GetItemName() {
		return itemName;
	}
}
