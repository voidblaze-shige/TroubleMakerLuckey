using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapFurniture : MonoBehaviour {
	public string manager_tag = "MainManager";
	public void Tap() {
		FurnitureManager furniture_manager = GameObject.FindGameObjectWithTag(manager_tag).GetComponent<FurnitureManager>();
		furniture_manager.TapFurniture(int.Parse(this.gameObject.name), this.gameObject);
	}
}
