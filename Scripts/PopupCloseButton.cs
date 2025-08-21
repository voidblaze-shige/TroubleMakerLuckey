using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PopupCloseButton : MonoBehaviour {
	Button  btn;
	EventTrigger eventTrigger;
	EventTrigger.Entry entry;

	void Awake() {
		btn = this.gameObject.GetComponent<Button>();
		eventTrigger = btn.gameObject.AddComponent<EventTrigger>();
		entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback.AddListener((data) => ClickDown());
		eventTrigger.triggers.Add(entry);
	}

	public void ClickDown() {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLICK_DOWN);
	}

	public void WindowClose(GameObject obj) {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLOSE);
		obj.SetActive(false);
		Destroy(obj);
	}

	public void ObjectDestroy(GameObject obj) {
		obj.SetActive(false);
		Destroy(obj);
	}
}
