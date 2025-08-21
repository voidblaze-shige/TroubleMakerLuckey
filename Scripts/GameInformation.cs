using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInformation : MonoBehaviour {

	// training flag
	private static bool isSleeping = false;
	public static bool IsSleeping {
		get { return isSleeping; }
		set { isSleeping = value; }
	}

	// rest sleeping time
	public static int sleepingTime = -1;
	public static int SleepingTime {
		get { return sleepingTime;}
		set { sleepingTime = value; }
	}

	// story progress
	public static int STORY_PROGRESS {get; set;}

	// game date
 	public static int GAME_DATE {get; set;}

	// last day
	private static int last_day = 12;
	public static int LAST_DAY {
		 get {return last_day; }
  }

	// last Playing date
	public static string CURRENT_DATE {get; set;}

	// Parameter MAX value
	private static int parameter_max = 100;
	public static int Parameter_MAX {
		 get {return parameter_max; }
  }

	// Trouble events MAX number
	private static int maxTroubleEventsNum = 26;
	public static int MaxTroubleEventsNum {
		 get {return maxTroubleEventsNum; }
  }

	// シナリオがない月
	private static int[] jumpScenarioDate = {6,8,9,10};
	public static int[] JumpScenarioDate {
		get {return jumpScenarioDate; }
	}

	// Lucky MAX number
	private static int maxLuckyNum = 6;
	public static int MaxLuckyNum {
		 get {return maxLuckyNum; }
  }

	// Sales Person
	private static int salesPersonMonth = 3;
	public static int SalesPersonMonth {
		 get {return salesPersonMonth; }
  }

	// 固定トラブルイベントの番号 1:バイヤー, 2:ハンター
	private static int troubleEventItemNum = 0;
	public static int TroubleEventItemNum {
		get { return troubleEventItemNum; }
		set { troubleEventItemNum = value; }
	}

	// バイヤー
	private static int princessNum = 1;
	public static int PrincessNum {
		 get {return princessNum; }
  }

	// ハンター
	private static int hunterNum = 2;
	public static int HunterNum {
		 get {return hunterNum; }
  }

	// ハニワ
	private static int haniwaNum = 3;
	public static int HaniwaNum {
		 get {return haniwaNum; }
  }

	// 特殊トラブルイベントフラグ
	private static bool specialTroubleEventFlag = false;
	public static bool SpecialTroubleEventFlag {
		get { return specialTroubleEventFlag; }
		set { specialTroubleEventFlag = value; }
	}

	// Trouble Event List
	public static List<int> TroubleEventList = new List<int>();

	// 進行中の固定トラブルイベント番号
	private static int specialTroubleEventNum = 0;
	public static int SpecialTroubleEventNum {
		get { return specialTroubleEventNum; }
		set { specialTroubleEventNum = value; }
	}

	// Parameter value
	public static int INT { get; set; }
	public static int BODY { get; set; }
	public static int LIKE { get; set; }

	// Find Combo List
	public static List<int> ComboList {get; set;}

	// Parameter Event List 新しいパラメータイベントを格納するリスト
	public static List<TrainingData> IntList = new List<TrainingData>();
	public static List<TrainingData> BodyList = new List<TrainingData>();
	public static List<TrainingData> LikeList = new List<TrainingData>();

	// Training Array for save training number ゲームを終了しても値を保持する配列
	// 0: no data, 1: new, 2:old
	public static int[] TrainingINTArray = new int[50];
	public static int[] TrainingBODYArray = new int[50];
	public static int[] TrainingLIKEArray = new int[50];

	// 持っているアイテムを保持する変数
	public static int[] InventoryArray = new int[6];

	// プレゼント出現回数
	public static int GIFT_NUM = 4;
	// プレゼント出現月を保存する配列
	public	static int[] GiftAppearanceMonthArray = new int[GIFT_NUM];
	// プレゼントアイテムの中身の番号を保存する配列
	public static int[] GiftItemNumberArray = new int[GIFT_NUM];
	// プレゼントを開けてから一時的にアイテム番号を格納する変数
	public static int GiftItemNumber = 0;

	// アイテムの最大所持数
	private static int item_max = 99;
	public static int ITEM_MAX {
		 get {return item_max; }
  }

	// gift open flag
	public static bool GiftOpenFlag = false;

	// これまで辿りついたエンディングリスト
	public static List<int> EndingList;

	public static bool special_ending_flag = false;

	// トラブルイベントの進捗（実行した日にちを入れる）
	// 初日はトラブルイベントが発生しないため、初期値を1とする
	public static int TroubleEventProgress = 1;
	// トレーニングの進捗
	public static int TrainingProgress = 0;

	// クリアフラグ
	public static bool IsClear = false;

	// 通知用リスト
	public static List<int> noticeList = new List<int>();

	// ポップアップ立ち上げ中フラグ
	public static bool PopUpFlag = false;

	// イベント中フラグ
	public static bool isTalkingStart = false;

	// プロローグ後に表示されるチュートリアルのフラグ
	public static bool first_tutorial_flag = false;

	// 家具一覧（パラメータイベントの番号）
	public static List<int> furniture_list = new List<int>();

	//---------- PlayerPrefs key -----------
	private static string is_started_key = "IS_STARTED";
	public static string IS_STARTED_KEY {
		get { return is_started_key; }
	}

	private static string is_sleeping_key = "IS_SLEEPING";
	public static string IS_SLEEPING_KEY {
		get { return is_sleeping_key; }
	}

	private static string trainingINT_key = "TRAINING_INT";
	public static string TrainingINT_KEY {
		get { return trainingINT_key; }
	}

	private static string trainingBODY_key = "TRAINING_BODY";
	public static string TrainingBODY_KEY {
		get { return trainingBODY_key; }
	}

	private static string trainingLIKE_key = "TRAINING_LIKE";
	public static string TrainingLIKE_KEY {
		get { return trainingLIKE_key; }
	}

	private static string sleepingTime_key = "SLEEPING_TIME";
	public static string SleepingTime_KEY {
		get { return sleepingTime_key; }
	}

	private static string audioSwitch_key  = "AUDIO_SWITCH";
	public static string AUDIOSWITCH_KEY {
		get { return audioSwitch_key; }
	}

	private static string game_date_key = "GAME_DATE";
	public static string GAME_DATE_KEY {
		get { return game_date_key; }
	}

	private static string current_date_key = "CURRENT_DATE";
	public static string CURRENT_DATE_KEY {
		get { return current_date_key; }
	}

	private static string int_key = "INT";
	public static string INT_KEY {
		get { return int_key; }
	}

	private static string body_key = "BODY";
	public static string BODY_KEY {
		get { return body_key; }
	}

	private static string like_key = "LIKE";
	public static string LIKE_KEY {
		get { return like_key; }
	}

	private static string story_progress_key = "STORY_PROGRESS";
	public static string STORY_PROGRESS_KEY {
		get { return story_progress_key; }
	}

	private static string trouble_event_item_num_key = "TROUBLE_EVENT_NUM";
	public static string TROUBLE_EVENT_ITEM_NUM_KEY {
		get { return trouble_event_item_num_key; }
	}

	private static string trouble_event_list_key = "TROUBLE_EVENT_LIST";
	public static string TROUBLE_EVENT_LIST_KEY {
		get { return trouble_event_list_key; }
	}

	private static string special_trouble_event_num_key = "SPECIAL_TROUBLE_EVENT_NUM";
	public static string SPECIAL_TROUBLE_EVENT_NUM_KEY {
		get {return special_trouble_event_num_key; }
	}

	private static string special_trouble_event_flag_key = "SPECIAL_TROUBLE_EVENT_FLAG";
	public static string SPECIAL_TROUBLE_EVENT_FLAG_KEY {
		get {return special_trouble_event_flag_key; }
	}

	private static string inventory_key = "INVENTORY";
	public static string  INVENTORY_KEY {
		get {return inventory_key; }
	}

	private static string gift_month_key = "GIFT_MONTH";
	public static string GIFT_MONTH_KEY {
		get {return gift_month_key; }
	}

	private static string gift_item_number_key = "GIFT_ITEM_NUMBER";
	public static string GIFT_ITEM_NUMBER_KEY {
		get {return gift_item_number_key; }
	}

	private static string gift_open_flag_key = "GIFT_OPEN_FLAG";
	public static string GIFT_OPEN_FLAG_KEY {
		get {return gift_open_flag_key; }
	}

	private static string ending_key = "ENDING";
	public static string ENDING_KEY {
		get {return ending_key; }
	}

	private static string player_name_key = "PLAYER_NAME";
	public static string PLAYER_NAME_KEY {
		get {return player_name_key; }
	}

	private static string trouble_event_progress_key = "TROUBLE_EVENT_PROGRESS";
	public static string TROUBLE_EVENT_PROGRESS_KEY {
		get {return trouble_event_progress_key; }
	}

	private static string training_progress_key = "TRAINING_PROGRESS";
	public static string TRAINING_PROGRESS_KEY {
		get {return training_progress_key; }
	}

	private static string is_clear_key = "IS_CLEAR";
	public static string IS_CLEAR_KEY {
		get {return is_clear_key; }
	}

	private static string last_time_key = "LAST_TIME";
	public static string LAST_TIME_KEY {
		get {return last_time_key; }
	}

	private static string notice_key = "NOTICE";
	public static string NOTICE_KEY {
		get {return notice_key; }
	}

	private static string combo_key = "COMBO";
	public static string COMBO_KEY {
		get {return combo_key; }
	}

	private static string megaphone_key = "MEGAPHONE";
	public static string MEGAPHONE_KEY {
		get {return megaphone_key; }
	}

	private static string push_off_key = "PUSH_OFF";
	public static string PUSH_OFF_KEY {
		get {return push_off_key; }
	}

	private static string hint_key = "HINT";
	public static string HINT_KEY {
		get {return hint_key; }
	}

	private static string room_key = "ROOM";
	public static string ROOM_KEY {
		get {return room_key; }
	}

	// shop key
	private static string shop_full_pack01_key = "SHOP_FULL_PACK01";
	public static string SHOP_FULL_PACK01_KEY {
		get {return shop_full_pack01_key; }
	}

	private static string shop_item_pack01_key = "SHOP_ITEM_PACK01";
	public static string SHOP_ITEM_PACK01_KEY {
		get {return shop_item_pack01_key; }
	}

	private static string shop_megaphone_pack01_key = "SHOP_MEGAPHONE_PACK01";
	public static string SHOP_MEGAPHONE_PACK01_KEY {
		get {return shop_megaphone_pack01_key; }
	}

	private static string furniture_list_key = "FURNITURE_LIST";
	public static string FURNITURE_LIST_KEY {
		get {return furniture_list_key; }
	}

	private static string lucky_tap_5_key = "LUCKY_TAP_5";
	public static string LUCKY_TAP_5_KEY {
		get {return lucky_tap_5_key; }
	}
}
