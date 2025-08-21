using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ParameterEventDataBase", menuName="Lucky/ParameterEventDataBase")]
public class ParameterEventDataBase : ScriptableObject {

	[SerializeField]
	private List<ParameterEvent> parameterEventList = new List<ParameterEvent>();

	// return the ParameterEventList
	public List<ParameterEvent> GetParameterEventLists() {
		return parameterEventList;
	}
}
