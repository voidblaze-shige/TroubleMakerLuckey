using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataBase", menuName="Lucky/ItemDataBase")]
public class ItemDataBase : ScriptableObject {

	[SerializeField]
	private List<Item> itemList = new List<Item>();

	// return the ItemList
	public List<Item> GetItemLists() {
		return itemList;
	}
}
