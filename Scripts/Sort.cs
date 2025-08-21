using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sort : MonoBehaviour {

	public void SortItem(GameObject parent) {
		List<Transform> objList = new List<Transform>();

		var childCount = parent.transform.childCount;
		for( int i = 0; i < childCount ; i++) {
			objList.Add(parent.transform.GetChild(i));
		}

		// ソート
		objList.Sort((obj1, obj2) => int.Parse(obj1.name) - int.Parse(obj2.name));

		foreach ( var obj in objList) {
			obj.SetSiblingIndex(childCount - 1);
		}
	}
}
