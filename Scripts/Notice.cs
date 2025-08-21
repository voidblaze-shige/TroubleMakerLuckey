using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notice : MonoBehaviour {
	private string noticeNodePath = "Notice/NoticeNode";
  private string textPath = "Content/Text/Title";
  private string contentPath = "Content";
  // 通知表示時間
	private int noticeShowTime = 1;
	private TrainingManager training_manager;

  public GameObject noticePanel;

	void Start() {
		training_manager = this.gameObject.GetComponent<TrainingManager>();
	}

	public IEnumerator ShowNoticeList() {
		DestroyAllChildren();

		List<int> removeList = new List<int>();
		if (GameInformation.noticeList == null || !GameInformation.noticeList.Any()) {
			GameInformation.noticeList = new List<int>(PlayerPrefsX.GetIntArray(GameInformation.NOTICE_KEY, 0, 0));
		}
		foreach(int num in GameInformation.noticeList) {
			// 通知中に会話がはじまったとき
			if (GameInformation.isTalkingStart) {
				break;
			}

			if (training_manager == null) {
				training_manager = this.gameObject.GetComponent<TrainingManager>();
			}
			ParameterEvent e = training_manager.GetParameterEvent(num);
			if (e != null) {
				StartCoroutine(ShowNotice(e.GetTrainingName()));
  			yield return new WaitForSeconds(1);
				removeList.Add(num);
			}
		}
		// 通知中に会話がはじまったとき
		if (GameInformation.isTalkingStart) {
				GameInformation.noticeList.Clear();
		} else {
			foreach(int num in removeList) {
				GameInformation.noticeList.Remove(num);
			}
		}

		PlayerPrefsX.SetIntArray(GameInformation.NOTICE_KEY, GameInformation.noticeList.ToArray());
	}

	public IEnumerator ShowNotice(string title) {
  	GameObject resultNode = Instantiate(Resources.Load<GameObject>(noticeNodePath));
  	resultNode.transform.SetParent(noticePanel.transform, false);

    GameObject textObj = resultNode.transform.Find(textPath).gameObject;
    Text noticeText = textObj.GetComponent<Text>();

    GameObject contentObj = resultNode.transform.Find(contentPath).gameObject;
		Animation anim = contentObj.GetComponent<Animation>();
		anim.Play("NoticeNode_Fadein");

    noticeText.text = "「"+ title +"」";

  	AudioManager.Instance.PlaySE(AUDIO.SE_NICE);
    resultNode.SetActive(true);


  	int count = 0;

  	while (true) {
			// 通知中に会話がはじまったとき
			if (GameInformation.isTalkingStart) {
				yield break;
			} else {
  			yield return new WaitForSeconds(1);
			}

  		if (count < noticeShowTime) {
    			count++;
  		} else if (count >= noticeShowTime) {

				// ノードの大きさを変える
				yield return StartCoroutine(ChangeScaleYToZero(resultNode));

  			resultNode.SetActive(false);
  			Destroy(resultNode);
  			yield break;
  		}
  	}
  }

	public void DestroyAllChildren() {
		foreach(Transform child in noticePanel.transform){
    	Destroy(child.gameObject);
		}
	}

	private IEnumerator ChangeScaleYToZero(GameObject obj) {
		float scaleValue = 1;
		while(scaleValue > 0) {
			scaleValue = obj.transform.localScale.y - 0.1f;
			obj.transform.localScale = new Vector3(obj.transform.localScale.x, scaleValue, obj.transform.localScale.z);
			yield return null;
		}
	}
}
