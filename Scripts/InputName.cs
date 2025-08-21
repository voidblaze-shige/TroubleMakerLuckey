using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utage;

public class InputName : MonoBehaviour {
  public AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>() as AdvEngine); } }
  [SerializeField]
  AdvEngine engine;

  public InputField inputField;
  public Button button;

  private bool inputflag = false;

  void Start() {
    if (SaveData.GetBool(GameInformation.IS_CLEAR_KEY, false)) {
      inputField.text = PlayerPrefs.GetString(GameInformation.PLAYER_NAME_KEY);
      if (inputField.text.Length > 0 && inputField.text.Length <= 10) {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => SavePlayerName());
        inputflag = true;
      } else {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => BuzzerButton());
        inputflag = false;
      }

    } else {
      button.onClick.RemoveAllListeners();
      button.onClick.AddListener(() => BuzzerButton());
    }
  }

  public void inputText() {

    inputField.onValidateInput += ValidateInput;

    if (inputField.text.Length > 0 && inputField.text.Length <= 10) {
      if (!inputflag) {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => SavePlayerName());
        inputflag = true;
      }
    } else {
      if (inputflag){
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => BuzzerButton());
        inputflag = false;
      }
    }
  }

	public void SavePlayerName() {
    StartCoroutine(SaveNameAndStartGame());
  }

  private IEnumerator SaveNameAndStartGame() {
    AudioManager.Instance.PlaySE(AUDIO.SE_START);

    inputField.interactable = false;
    string name = inputField.text;

    //もし10文字以上の文字列が入ってきてしまった場合
    if (name.Length > 10) {
       name = name.Remove(10);
    }

    Engine.Param.TrySetParameter("player_name", name);
    PlayerPrefs.SetString(GameInformation.PLAYER_NAME_KEY, name);
    SaveData.SetBool(GameInformation.IS_STARTED_KEY, true);

    MainManager manager = GameObject.FindGameObjectWithTag("MainManager").GetComponent<MainManager>();

    yield return new WaitForSeconds(0.5f);
    manager.StartPlorogue();
  }

  public char ValidateInput(string text, int charIndex, char addedChar) {
    if (char.IsSurrogate (addedChar)) {
      // サロゲートペアの場合には削除
      addedChar = '\0';
    }
    return addedChar;
  }

  public void BuzzerButton() {
    AudioManager.Instance.PlaySE(AUDIO.SE_BUZZER);
  }
}
