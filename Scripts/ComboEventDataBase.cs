using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ComboEventDataBase", menuName="Lucky/ComboEventDataBase")]
public class ComboEventDataBase : ScriptableObject {

	[SerializeField]
	private List<ComboEvent> comboEventList = new List<ComboEvent>();

	// return the ComboEventList
	public List<ComboEvent> GetComboEventLists() {
		return comboEventList;
	}
}
