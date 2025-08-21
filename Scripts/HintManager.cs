using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour {
	private string hintPath = "UI/Hint";
	private string hintConfirmPath = "UI/HintConfirm";
	private string hintConfirmYESPath = "MenuBack/Buttons/Contents/YES";
	private string titlePath = "Window/TextBack/Title";
	private string messagePath = "Window/Narration/Fukidashi/Message";
	private string hintMessagePath = "Window/HintText";
	private Text title;
	private Text message;
	private Text hintMessage;
	private UnityAds unityAds;
	private bool addFlag = false;
	private int hintNumber = 0;

	[SerializeField]
	private HintDataBase hintDatabase;

	public GameObject canvas;
	public GameObject unityAdsPopup;

	public void OpenConfirm(int num) {
		addFlag = false;
		hintNumber = num;

		List<int> list = new List<int>(PlayerPrefsX.GetIntArray(GameInformation.HINT_KEY, 0, 0));
		// もし、一度ヒントを見たことがある場合はポップアップをCMの流れないポップアップを表示する
		if (list.Contains(num)) {
			GameObject obj = Instantiate(Resources.Load<GameObject>(hintConfirmPath));
			obj.transform.SetParent(canvas.transform, false);
			Button yesButton = obj.transform.Find(hintConfirmYESPath).GetComponent<Button>();
			yesButton.onClick.AddListener( () => ShowHint() );
		} else {
			// まだCMを見たことがないときはCMを流す
			addFlag = true;

			if (unityAds == null) {
				unityAds = this.gameObject.GetComponent<UnityAds>();
			}
			unityAds.SetUseNumber(3);
			unityAdsPopup.SetActive(true);
		}
	}

	public void ShowHint() {
		if (addFlag) {
			// 保存する
			List<int> list = new List<int>(PlayerPrefsX.GetIntArray(GameInformation.HINT_KEY, 0, 0));
			list.Add(hintNumber);
			PlayerPrefsX.SetIntArray(GameInformation.HINT_KEY, list.ToArray());
			addFlag = false;
		}

		AudioManager.Instance.PlaySE(AUDIO.SE_OPEN);
		GameObject obj = Instantiate(Resources.Load<GameObject>(hintPath));
		obj.transform.SetParent(canvas.transform, false);
		title = obj.transform.Find(titlePath).GetComponent<Text>();
		message = obj.transform.Find(messagePath).GetComponent<Text>();
		hintMessage = obj.transform.Find(hintMessagePath).GetComponent<Text>();
		Hint hint = GetHint(hintNumber);
		title.text = hint.GetTitle();
		hintMessage.text = hint.GetHint();
		message.text = hint.GetMessage();

		obj.SetActive(true);

		hintNumber = 0;
	}

	// Get hint information by number
	public Hint GetHint(int searchNum) {
	 return hintDatabase.GetHintLists().Find(number => number.GetNumber() == searchNum);
	}
}
