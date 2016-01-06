using UnityEngine;  
using System.Collections;

namespace Audio {


	/// <summary>
	/// Go to class for instantiating sound effects into the scene.
	/// </summary>
	public static class SoundEffects  {  


		/// <summary>
		/// Plays the specified sound effect that the player can hear
		/// no matter where the object is in the world.
		/// </summary>
		/// <param name="effectToPlay">Effect to play.</param>
		public static void Play(SoundEffectType effectToPlay){

			GetAudioSource().clip = GetSoundEffect(effectToPlay);  
			GetAudioSource().Play();  
		
		}  


		/// <summary>
		/// Plays the 3D sound effect in the world.
		/// </summary>
		/// <returns>The gameobject that contains the audio source.</returns>
		/// <param name="effectToPlay">Effect to play.</param>
		/// <param name="positionToPlayAt">Position to play at.</param>
		public static GameObject PlayInWorld(SoundEffectType effectToPlay, Vector3 positionToPlayAt){

			// Create a gameobject to play the sound
			soundObjectInstance = new GameObject("Sound Effect Object");  
			soundObjectInstance.transform.position = positionToPlayAt;  

			// Set up the audio source to play the clip
			soundObjectInstance.AddComponent<AudioSource>();  
			soundObjectInstance.GetComponent<AudioSource>().clip = GetSoundEffect(effectToPlay);  
			soundObjectInstance.GetComponent<AudioSource>().Play();  
			soundObjectInstance.GetComponent<AudioSource>().loop = false;  
			soundObjectInstance.GetComponent<AudioSource>().priority = 1;

			// Delete the gameobject after the audio has finnished playing
			Object.Destroy (soundObjectInstance, soundObjectInstance.GetComponent<AudioSource>().clip.length);

			return soundObjectInstance;  
		
		}



		static GameObject soundObjectInstance = null; 


		/// <summary>
		/// Gets the audio source that we're using to play the sound effect
		/// </summary>
		/// <returns>The audio source.</returns>
		static AudioSource GetAudioSource(){

			//if it's null, build the object  
			if(soundObjectInstance == null){  
				soundObjectInstance = new GameObject("Sound Effect Object");  
				soundObjectInstance.AddComponent<AudioSource>();  
			}

			return soundObjectInstance.GetComponent<AudioSource>();

		}  


		/// <summary>
		/// Sets the volume for the sound effects
		/// </summary>
		/// <param name="newVolume">New volume for the sound effects</param>
		public static void SetVolume(float newVolume){ 

			GetAudioSource().volume = newVolume; 

		}  


		/// <summary>
		/// Gets the current volume that the sound effects play at
		/// </summary>
		/// <returns>The current volume.</returns>
		public static float GetCurrentVolume(){ 

			return GetAudioSource().volume;  

		}  


		/// <summary>
		/// Grabs the audioclip sounds effect from resources based on
		/// the effect passed in.
		/// </summary>
		/// <returns>The sound effect.</returns>
		/// <param name="effectToPlay">Effect to play.</param>
		static AudioClip GetSoundEffect(SoundEffectType effectToPlay){  

			switch(effectToPlay){  

			case SoundEffectType.NotEnoughBoostError:  
				return Resources.Load("Audio/FX/Error") as AudioClip;

			case SoundEffectType.MenuSelect:  
				return Resources.Load("Audio/FX/Select") as AudioClip;

			case SoundEffectType.MechChainGunShot:
				return Resources.Load("Audio/FX/Gunshot") as AudioClip;

			}

			return null;  

		}  

	}

}