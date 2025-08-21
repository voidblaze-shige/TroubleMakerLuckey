using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {

	public IEnumerator WaitForAnimation ( Animation animation )
	{
		if (animation != null) {
			do
			{
					if (GameInformation.isTalkingStart) {
						yield break;
					}

					yield return null;
			} while ( animation.isPlaying );
		}
	}
}
