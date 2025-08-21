using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetItem : MonoBehaviour {
	private GameObject obj;
	private ItemManager manager;
	private string managerTag = "MainManager";

	public void SetSelectItem () {
		if (manager == null) {
			obj = GameObject.FindGameObjectWithTag(managerTag);
			manager = obj.GetComponent<ItemManager>();
		}
		manager.SelectItem(this.gameObject);
	}
}
