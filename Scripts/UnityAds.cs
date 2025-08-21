using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

//[RequireComponent(typeof(Button))]
public class UnityAds : MonoBehaviour
{
    //---------- ONLY NECESSARY FOR ASSET PACKAGE INTEGRATION: ----------//

    #if UNITY_IOS
    private string gameId = "<YOUR_IOS_GAME_ID>";
    #elif UNITY_ANDROID
    private string gameId = "<YOUR_ANDROID_GAME_ID>";
    #else
    private string gameId = "<REDACTED>";
    #endif
    
    //-------------------------------------------------------------------//

    // 1:gift, 2:clock, 3:hint
    public static int use = 0;

    public GameObject message;
    public GameObject adsButton;
    public GameObject canvas;

    private Text messageText;
    private ItemManager item_manager;
    private MainManager main_manager;
    private SleepingManager sleeping_manager;
    private HintManager hint_manager;
    private GameObject failed_popup;
    private string failedPopupPath = "UI/UnityAdsFailed";
    private string gift_message = "プレゼントを開けますか？" +	Environment.NewLine +  "（ＣＭが流れます）";
    private string clock_message = "メガホンを入手しますか？" +	Environment.NewLine +  "（ＣＭが流れます）";
    private string hint_message = "ヒントを見ますか？" +	Environment.NewLine +  "（はじめて見る場合のみＣＭが流れます）";

    public string placementId = "<REWARDED_PLACEMENT_ID>";
    Button m_Button;

    void Start ()
    {
        m_Button = adsButton.GetComponent<Button>();
        if (m_Button) m_Button.onClick.AddListener(ShowAd);

        //---------- ONLY NECESSARY FOR ASSET PACKAGE INTEGRATION: ----------//
        if (Advertisement.isSupported && !string.IsNullOrEmpty(gameId)) {
            Advertisement.Initialize(gameId, true);
        }
        //-------------------------------------------------------------------//
        messageText = message.GetComponent<Text>();
    }

    void Update ()
    {
//        if (m_Button) m_Button.interactable = Advertisement.IsReady(placementId);
    }

    void ShowAd ()
    {
      if (Advertisement.IsReady(placementId)) {
        if (use == 1) {
          if (main_manager == null) {
            main_manager = this.gameObject.GetComponent<MainManager>();
          }
          main_manager.RemoveGift();
        }

        ShowOptions options = new ShowOptions();
        options.resultCallback = HandleShowResult;

        Advertisement.Show(placementId, options);
      } else {
        // Failed Get UnityAds
        failed_popup = Instantiate(Resources.Load<GameObject>(failedPopupPath));
        failed_popup.transform.SetParent(canvas.transform, false);
        failed_popup.SetActive(true);
      }
    }

    public void SetUseNumber(int num) {
		  GameInformation.PopUpFlag = true;
      use = num;
      string text = "";
      switch(use) {
        case 1:
          text = gift_message;
          break;
        case 2:
          // clock
          text = clock_message;
          break;
        case 3:
          // hint
          text = hint_message;
          break;
        default:
          break;
      }
      messageText.text = text;
    }

    public void CloseButton(GameObject obj) {
	    AudioManager.Instance.PlaySE(AUDIO.SE_CLOSE);
		  GameInformation.PopUpFlag = false;
      obj.SetActive(false);
    }

    void HandleShowResult (ShowResult result)
    {
        if(result == ShowResult.Finished) {

          switch(use) {
            case 1:
              // gift
              if (item_manager == null) {
                item_manager = this.gameObject.GetComponent<ItemManager>();
              }
              item_manager.GetGiftItem();

              break;
            case 2:
              // メガホンを取得
              if (sleeping_manager == null) {
                sleeping_manager = this.gameObject.GetComponent<SleepingManager>();
              }
              sleeping_manager.GetMegaphone();
              break;
            case 3:
      		    GameInformation.PopUpFlag = false;
              // hint
              if (hint_manager == null) {
                hint_manager = this.gameObject.GetComponent<HintManager>();
              }
              hint_manager.ShowHint();
              break;
            default:
      		    GameInformation.PopUpFlag = false;
              break;
          }

          Debug.Log("Video completed - Offer a reward to the player");

        }else if(result == ShowResult.Skipped) {
            Debug.LogWarning("Video was skipped - Do NOT reward the player");

        }else if(result == ShowResult.Failed) {
            Debug.LogError("Video failed to show");
        }
        use = 0;
    }
}
