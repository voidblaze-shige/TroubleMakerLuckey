using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "TroubleEvent", menuName="Lucky/TroubleEvent")]
public class TroubleEvent : ScriptableObject {

	// トラブル番号
	[SerializeField]
	private int number;

	[SerializeField]
	private ValueArray[] valueArray;

	[System.SerializableAttribute]
	public class ValueArray {
		// あたま：増減するパラメータの値
		public int atama;

		// からだ：増減するパラメータの値
		public int karada;

		// きもち：増減するパラメータの値
		public int kimochi;
	}

	// 固定イベントフラグ
	[SerializeField]
	private bool specialFlag;

	// 次の固定イベントの番号
	[SerializeField]
	private int nextNumber;

	// 関連番号
	[SerializeField]
	private int specialNumber;

	public int GetNumber() {
		return number;
	}

	public int GetNextNumber() {
		return nextNumber;
	}

	public int GetSpecialNumber() {
		return specialNumber;
	}

	public List<int> GetValueList(int num) {
		List<int> list = new List<int>();

		list.Add(valueArray[num-1].atama);
		list.Add(valueArray[num-1].karada);
		list.Add(valueArray[num-1].kimochi);
		return list;
	}

	public bool GetSpecialFlag () {
		return specialFlag;
	}
}
