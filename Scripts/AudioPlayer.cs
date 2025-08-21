using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {

	public void PressDown() {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLICK_DOWN);
	}

	public void PressUp() {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLICK_UP);
	}

	public void WindowOpen() {
		AudioManager.Instance.PlaySE(AUDIO.SE_OPEN);
	}

	public void WindowClose() {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLOSE);
	}

	public void NextPage() {
		AudioManager.Instance.PlaySE(AUDIO.SE_SWITCH_PAGE);
	}

	public void Nice() {
		AudioManager.Instance.PlaySE(AUDIO.SE_NICE);
	}

	public void GrowUpParameter() {
		AudioManager.Instance.PlaySE(AUDIO.SE_PARAMETER_INCREASE);
	}

	public void Delete() {
		AudioManager.Instance.PlaySE(AUDIO.SE_DELETE);
	}

	public void Select() {
		AudioManager.Instance.PlaySE(AUDIO.SE_SLECT);
	}

	public void StartButton() {
		AudioManager.Instance.PlaySE(AUDIO.SE_START);
	}
}
