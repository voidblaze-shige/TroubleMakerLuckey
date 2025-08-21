using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTrainingButton : MonoBehaviour {
	private GameObject obj;
	private TrainingManager manager;
	private string managerTag = "MainManager";

	public void StartTraining(GameObject confirmWindow) {
		if(manager == null) {
			obj = GameObject.FindGameObjectWithTag(managerTag);
			manager = obj.GetComponent<TrainingManager>();
		}
		manager.StartTrainingButton(confirmWindow);
	}
}
