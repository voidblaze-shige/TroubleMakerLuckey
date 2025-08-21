using UnityEngine;
using UnityEngine.UI;
using Utage;


/// <summary>
/// ADV用SendMessageコマンドから送られたメッセージを受け取る処理のサンプル
/// </summary>
[AddComponentMenu("Utage/ADV/Examples/UtageRecieveMessageSample")]
public class UtageRecieveMessage : MonoBehaviour
{
    public AdvEngine engine;            //Advエンジン本体
		public FadeScreen fadeScreen;       //フェードアウト

    //SendMessageコマンドが実行されたタイミング
    void OnDoCommand(AdvCommandSendMessage command)
    {
        switch (command.Name)
        {
            case "DebugLog":
                DebugLog(command);
                break;
            case "FadeOut":
                FadeOut();
                break;
            default:
                Debug.Log("Unknown Message:" + command.Name );
                break;
        }
    }


    //デバッグログを出力
    void DebugLog(AdvCommandSendMessage command)
    {
        Debug.Log(command.Arg2);
    }

		void FadeOut() {
			fadeScreen.Fadeout();
		}
}
