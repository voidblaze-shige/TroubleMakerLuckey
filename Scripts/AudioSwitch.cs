using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AudioSwitch : MonoBehaviour {
    Image buttonImage;
		public Sprite imageON;
		public Sprite imageOFF;
    public GameObject audioSwitch;
    private string key = GameInformation.AUDIOSWITCH_KEY;

    void Awake() {
      buttonImage = audioSwitch.GetComponent<Image>();
      InitButton();
    }

    public void ClickSwitch()
    {
      if (PlayerPrefs.GetInt(key, 0) == 0) {
				// Mute
    		buttonImage.sprite = imageOFF;
        AudioListener.volume = 0f;
        PlayerPrefs.SetInt(key, 1);
      } else {
    		buttonImage.sprite = imageON;
        AudioListener.volume = 1f;
        PlayerPrefs.SetInt(key, 0);
      }
    }

    public void InitButton() {
      if (PlayerPrefs.GetInt(key, 0) == 0) {
				buttonImage.sprite = imageON;
        AudioListener.volume = 1f;
      } else {
				buttonImage.sprite = imageOFF;
        AudioListener.volume = 0f;
      }      
    }
}
