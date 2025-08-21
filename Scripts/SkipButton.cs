using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utage;

public class SkipButton : MonoBehaviour {
	/// <summary>ADVエンジン</summary>
	public virtual AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>()); } }
	[SerializeField]
	protected AdvEngine engine;

	public void OnTapSkip( bool isOn )
	{
		Engine.Config.IsSkip = isOn;
	}
}
