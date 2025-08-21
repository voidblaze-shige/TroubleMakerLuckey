using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemDataBase", menuName="Lucky/ShopItemDataBase")]
public class ShopItemDataBase : ScriptableObject {

	[SerializeField]
	private List<ShopItem> shopItemList = new List<ShopItem>();

	// return the shopItemList
	public List<ShopItem> GetShopItemLists() {
		return shopItemList;
	}
}
