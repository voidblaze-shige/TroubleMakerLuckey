using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "ParameterEvent", menuName="Lucky/ParameterEvent")]
public class ParameterEvent : ScriptableObject {

	public enum KindOfType {
		Atama,
		Karada,
		Kimochi
	}

	public enum KindOfTap {
		None,
		PictureBook,
		BuildingBlock,
		Laptop,
		AstronomicalTelescope,
		SportsShoes,
		RuckSack,
		Calendar,
		Lucky,
		TV
	}

	// 育成番号
	[SerializeField]
	private int number;

	// アイコン
	[SerializeField]
	private Sprite icon;

	// 育成タイプ
	[SerializeField]
	private KindOfType kindOfType;

	// 育成名
	[SerializeField]
	private string trainingName;

	// 上昇する値
	[SerializeField]
	private int increaseValue;

	// コンボNo.
	[SerializeField]
	private List<int> comboNums;

	// 結果詳細
	[SerializeField]
	private string resultInformation;

	// 特殊イベントフラグ
	[SerializeField]
	private bool specialFlag;

	// エンディング追加フラグ
	[SerializeField]
	private bool endingAddFlag;

	// タップイベントフラグ
	[SerializeField]
	private bool tapEventFlag;

	// オブジェクト/テレビ/ラッキー
	[SerializeField]
	private KindOfTap kindOfTap;

	// タップする画像
	[SerializeField]
	private Sprite tapSprite;

	// タップする画像の位置: x
	[SerializeField]
	private int image_x;

	// タップする画像の位置: y
	[SerializeField]
	private int image_y;

	// 解放期間+特殊イベントの場合は前提育成のNo
	[SerializeField]
	private int liberationPeriod;

	// 関連するトラブルイベント番号
	[SerializeField]
	private int troubleEventNumber;

	public int GetNumber() {
		return number;
	}

	public Sprite GetIcon() {
		return icon;
	}

	public int GetKindOfType() {
		switch (kindOfType) {
			case KindOfType.Atama:
				return 1;
			case KindOfType.Karada:
 				return 2;
			case KindOfType.Kimochi:
				return 3;
			default:
				break;
		}
		return 0;
	}

	public int GetIncreaseValue() {
		return increaseValue;
	}

	public string GetTrainingName() {
		return trainingName;
	}

	public string GetResultInformation() {
		return resultInformation;
	}

	public List<int> GetComboNums() {
		return comboNums;
	}

	public bool GetSpecialFlag () {
		return specialFlag;
	}

	public int GetLiverationPeriod () {
		return liberationPeriod;
	}

	public int GetTroubleEventNumber() {
		return troubleEventNumber;
	}

	public bool GetEndingAddFlag() {
		return endingAddFlag;
	}

	public bool GetTabEventFlag() {
		return tapEventFlag;
	}

	public string GetKindOfTap() {
		switch (kindOfTap) {
			case KindOfTap.PictureBook:
				return "PictureBook";
			case KindOfTap.BuildingBlock:
				return "BuildingBlock";
			case KindOfTap.Laptop:
				return "Laptop";
			case KindOfTap.AstronomicalTelescope:
				return "AstronomicalTelescope";
			case KindOfTap.SportsShoes:
				return "SportsShoes";
			case KindOfTap.RuckSack:
				return "RuckSack";
			case KindOfTap.Calendar:
				return "Calendar";
			case KindOfTap.Lucky:
				return "Lucky";
			case KindOfTap.TV:
				return "TV";
			default:
				break;
		}
		return "None";
	}

	public Sprite GetTapSprite() {
		return tapSprite;
	}

	public Vector2 GetPosition() {
		return new Vector2(image_x, image_y);
	}
}
