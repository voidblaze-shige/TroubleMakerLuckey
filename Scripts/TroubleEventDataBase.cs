using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TroubleEventDataBase", menuName="Lucky/TroubleEventDataBase")]
public class TroubleEventDataBase : ScriptableObject {

	[SerializeField]
	private List<TroubleEvent> troubleEventList = new List<TroubleEvent>();

	// return the ParameterEventList
	public List<TroubleEvent> GetTroubleEventLists() {
		return troubleEventList;
	}
}
