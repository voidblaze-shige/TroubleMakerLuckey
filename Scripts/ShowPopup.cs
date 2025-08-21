using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ShowPopup : MonoBehaviour {
	private string popupPath = "UI/Popup";
	private string buttonPath = "Common/Window/Buttons/Contents/Button";
	private string textPath = "Common/Window/TextBack/Text";
	private	EventTrigger eventTrigger;
	private UnityAction<BaseEventData> unityAction1;
	private UnityAction<BaseEventData> unityAction2;
	private	EventTrigger.Entry entry1;
	private	EventTrigger.Entry entry2;

	public GameObject parentObject;

	public void CreatePopup(string message) {
		GameObject popup = Instantiate(Resources.Load<GameObject>(popupPath));
		popup.transform.SetParent(parentObject.transform, false);
		GameObject text_obj = popup.transform.Find(textPath).gameObject;
		Text popup_text = text_obj.GetComponent<Text>();
		popup_text.text = message;

		GameObject button_obj = popup.transform.Find(buttonPath).gameObject;
		eventTrigger = button_obj.AddComponent<EventTrigger>();

		unityAction1 = new UnityAction<BaseEventData> (OnMyPointerDown);
		unityAction2 = new UnityAction<BaseEventData> (OnMyPointerClick);

		// pointer down
		entry1 = new EventTrigger.Entry();
		entry1.eventID = EventTriggerType.PointerDown;
		entry1.callback.AddListener (unityAction1);
		eventTrigger.triggers.Add(entry1);

		// pointer click
		entry2 = new EventTrigger.Entry();
		entry2.eventID = EventTriggerType.PointerClick;
		entry2.callback.AddListener (unityAction2);
		eventTrigger.triggers.Add(entry2);

		popup.SetActive(true);
		AudioManager.Instance.PlaySE(AUDIO.SE_OPEN);		
	}

	private void OnMyPointerDown(BaseEventData data) {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLICK_DOWN);
	}

	private void OnMyPointerClick(BaseEventData data) {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLOSE);
	}
}
