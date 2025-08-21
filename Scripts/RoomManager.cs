using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour {

	public Image roomImage;
	public Sprite room1;
	public Sprite room2;
	public MainManager main_manager;
	public FadeScreen fadeScreen;

	// Use this for initialization
	void Start () {
		ChangeBackGround();
	}

	public void SwitchRoom(int num) {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLICK_UP);
		fadeScreen.Fadeout();
		PlayerPrefs.SetInt(GameInformation.ROOM_KEY, num);
		StartCoroutine(ShowRoom());
	}

	private IEnumerator ShowRoom() {
		yield return new WaitForSeconds(0.8f);
		main_manager.StartMainBGM();
		ChangeBackGround();
	}

	private void ChangeBackGround() {
		switch(PlayerPrefs.GetInt(GameInformation.ROOM_KEY, 1)) {
			case 1:
				roomImage.sprite = room1;
				break;
			case 2:
				roomImage.sprite = room2;
				break;
			default:
				roomImage.sprite = room1;
				break;
		}
	}
}
