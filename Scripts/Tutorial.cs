using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {
	public int maxPage;
	public GameObject content;
	public GameObject star;
	public GameObject[] guide;

	private int selectPage = 1;
	private Vector2 content_pos;
	private Vector2[] page_pos;
	private float init_x = 0;
	private bool flag = false;

	private MainManager manager;

	void Start() {
		if (GameInformation.first_tutorial_flag) {
			GameObject obj = GameObject.FindGameObjectWithTag("MainManager");
			manager = obj.GetComponent<MainManager>();
		}
	}

	public void ClickDown() {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLICK_DOWN);
	}

	public void CloseTutorial() {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLOSE);
		this.gameObject.SetActive(false);
		Destroy(this.gameObject);
	}

	public void Next() {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLICK_UP);
		if (selectPage < maxPage) {
			selectPage++;
		} else {
			selectPage = 1;
		}
		SlidePage();
	}

	public void Back() {
		AudioManager.Instance.PlaySE(AUDIO.SE_CLICK_UP);
		if (selectPage > 1) {
			selectPage--;
		} else {
			selectPage = maxPage;
		}
		SlidePage();
	}

	private void SlidePage() {
		if (!flag) {
			GetInitPosition();
			flag = true;
		}
		float x = 0;
		x = page_pos[selectPage - 1].x;
		star.transform.SetParent(guide[selectPage - 1].transform, false);

		StartCoroutine(MoveToPage(-1*(x - init_x)));
	}

	private IEnumerator MoveToPage(float target) {
		content_pos = content.GetComponent<RectTransform>().anchoredPosition;
		Vector2 target_pos = new Vector2(target, content_pos.y);
		float current_pos = 0;
		while(current_pos <= 1) {
			current_pos+=0.1f;
			content.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(content_pos, target_pos, current_pos);
			yield return null;
		}
	}

	private void GetInitPosition() {
		content_pos = content.GetComponent<RectTransform>().anchoredPosition;
 		page_pos = new Vector2[maxPage];
		for (int i = 0; i < maxPage; i++) {
			page_pos[i] = this.gameObject.transform.Find("Explain/Mask/Content/" + (i+1).ToString()).GetComponent<RectTransform>().anchoredPosition;
		}
		init_x = page_pos[0].x;
	}

	void OnDestroy() {
		if (GameInformation.first_tutorial_flag) {
			GameInformation.first_tutorial_flag = false;
			if (manager != null) {
				manager.InitMainContents();
			}
		}
	}

	public void SetContent(int page, GameObject content, GameObject star, GameObject[] guide) {
		maxPage = page;
		this.content = content;
		this.star = star;
		this.guide= guide;
	}

	public IEnumerator SetPage(int selectPage) {
		yield return new WaitForSeconds(0.2f);
		this.selectPage = selectPage;
		SlidePage();
	}
}
