using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTrainingEvent : MonoBehaviour {
	private GameObject obj;
	private TrainingManager manager;
	private string managerTag = "MainManager";

	public void SetTraining() {
		int num =	int.Parse(this.gameObject.name);

		if (manager == null){
			obj = GameObject.FindGameObjectWithTag(managerTag);
			manager = obj.GetComponent<TrainingManager>();
		}
		manager.SetEvent(num, this.gameObject);
	}

	public void SetCombo() {
		int num =	int.Parse(this.gameObject.name);

		if (manager == null){
			obj = GameObject.FindGameObjectWithTag(managerTag);
			manager = obj.GetComponent<TrainingManager>();
		}
		manager.SetEventByCombo(num, this.gameObject);
	}
}
