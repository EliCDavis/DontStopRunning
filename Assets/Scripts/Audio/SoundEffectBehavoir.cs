using UnityEngine;
using System.Collections;

namespace EliDavis.Audio {

	/// <summary>
	/// Helper class for the UI that allows buttons to play sound effects through
	/// the event system setup.
	/// </summary>
	public class SoundEffectBehavoir : MonoBehaviour {

		public void playSound(string soundEffect){

			Audio.SoundEffects.Play((Audio.SoundEffectType) System.Enum.Parse(typeof(Audio.SoundEffectType), soundEffect, true));

		}

	}

}
