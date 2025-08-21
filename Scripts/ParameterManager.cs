using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParameterManager : MonoBehaviour {

	public Slider intSlider;
	public Slider bodySlider;
	public Slider likeSlider;
	public GameObject intFarame;
	public GameObject bodyFarame;
	public GameObject likeFarame;
	public AnimationController anim_controller;
	private float difValue = 0.2f;
	private string upArrowPath = "UI/ArrowUp";
	private string downArrowPath = "UI/ArrowDown";

	void Start () {
		InitParameter();
	}

	void Update () {
		if (intSlider.value != GameInformation.INT) {
				StartCoroutine(GrowUpAnimation(intSlider, GameInformation.INT, intSlider.value));

 		}
		if (bodySlider.value != GameInformation.BODY) {
				StartCoroutine(GrowUpAnimation(bodySlider, GameInformation.BODY, bodySlider.value));
 		}
		if (likeSlider.value != GameInformation.LIKE) {
				StartCoroutine(GrowUpAnimation(likeSlider, GameInformation.LIKE, likeSlider.value));
 		}
	}

	private IEnumerator GrowUpAnimation(Slider slider, int toValue, float currentValue) {
		// マイナスの場合
		if (toValue < currentValue) {
			while(toValue < currentValue) {
				slider.value = currentValue;
				currentValue -= difValue;
				if (currentValue < 0) {
					break;
				}
				yield return new WaitForSeconds(0.01f);
			}
		} else {
			// プラスの場合
			while(toValue > currentValue) {
				slider.value = currentValue;
				currentValue += difValue;
				if (currentValue > GameInformation.Parameter_MAX) {
					break;
				}
				yield return new WaitForSeconds(0.01f);
			}
		}
	}

	private void InitParameter() {
		// 最大値を設定
		intSlider.maxValue = GameInformation.Parameter_MAX;
		bodySlider.maxValue = GameInformation.Parameter_MAX;
		likeSlider.maxValue = GameInformation.Parameter_MAX;
		GameInformation.INT = PlayerPrefs.GetInt(GameInformation.INT_KEY, 0);
		GameInformation.BODY = PlayerPrefs.GetInt(GameInformation.BODY_KEY, 0);
		GameInformation.LIKE = PlayerPrefs.GetInt(GameInformation.LIKE_KEY, 0);
		// 値を設定
		intSlider.value = GameInformation.INT;
		bodySlider.value = GameInformation.BODY;
		likeSlider.value = GameInformation.LIKE;
	}

	public IEnumerator ShowParameterUpDownIcon(int value, int type) {
		switch(type) {
			case 1:
				StartCoroutine(ShowArrow(intFarame, value));
				break;
			case 2:
				StartCoroutine(ShowArrow(bodyFarame, value));
				break;
			case 3:
				StartCoroutine(ShowArrow(likeFarame, value));
				break;
			default:
				break;
		}
		yield return null;
	}

	private IEnumerator ShowArrow(GameObject parentObj, int num) {

		// 上昇値減少値が0でないとき
		if (num != 0) {
			string path = "";
			if (num > 0) {
				path = upArrowPath;
			} else {
				path = downArrowPath;
			}
			GameObject arrow = Instantiate(Resources.Load<GameObject>(path));
			arrow.transform.SetParent(parentObj.transform, false);
			Animation anim = arrow.GetComponent<Animation>();

			arrow.SetActive(true);

			yield return new WaitForSeconds(0.5f);

			anim.Play();

			yield return anim_controller.WaitForAnimation(anim);

			arrow.SetActive(false);
		}
	}
}
