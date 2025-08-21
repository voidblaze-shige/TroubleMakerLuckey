using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EndingEventDataBase", menuName="Lucky/EndingEventDataBase")]
public class EndingEventDataBase : ScriptableObject {

	[SerializeField]
	private List<EndingEvent> endingEventList = new List<EndingEvent>();

	// return the EndingEventList
	public List<EndingEvent> GetEndingEventLists() {
		return endingEventList;
	}
}
