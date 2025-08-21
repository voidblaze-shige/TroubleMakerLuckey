using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InsertTestTitle : MonoBehaviour {

	[SerializeField]
	Button btn;

	void Start() {
		btn = GetComponent<Button>();
		btn.onClick.AddListener(() => SetTitle());
	}

	public void SetTitle() {
		GameObject test = GameObject.Find("/Canvas/TestPanel").gameObject;
		TestSenario testSenario = test.GetComponent<TestSenario>();
		Text title = this.gameObject.GetComponentInChildren<Text>();
		testSenario.SetTitle(title.text);
	}
}
