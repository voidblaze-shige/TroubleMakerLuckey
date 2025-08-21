using UnityEngine;
#if UNITY_IOS
using System;
using UnityEngine.iOS;
#endif

public class ReviewManager : MonoBehaviour
{
  [SerializeField]
  private string appleId = "com.mb-vil.troublemakerlucky";

  public void Request()
  {
#if UNITY_ANDROID
    string url = "market://details?id=" + Application.identifier;
    Application.OpenURL(url);
#elif UNITY_IOS
    Version iosVersion = new Version(Device.systemVersion);
    Version minVersion = new Version("10.3");
    if (iosVersion >= minVersion)
    {
      iOSReviewRequest.Request();
    }
    else
    {
      string url = "itms-apps://itunes.apple.com/jp/app/id" + appleId + "?mt=8&action=write-review";
      Application.OpenURL(url);
    }
#endif
  }
}
