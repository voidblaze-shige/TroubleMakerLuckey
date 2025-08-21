using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HintDataBase", menuName="Lucky/HintDataBase")]
public class HintDataBase : ScriptableObject {

	[SerializeField]
	private List<Hint> hintList = new List<Hint>();

	// return the hintList
	public List<Hint> GetHintLists() {
		return hintList;
	}
}
