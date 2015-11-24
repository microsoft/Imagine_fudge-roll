using UnityEngine;
using System.Collections;

/// <summary>
/// Audio related helper functions. These are static functions and can be call directly using
/// AudioHelper.FunctionName() syntax without having to attach to any GameObject.
/// </summary>
public class AudioHelper
{
	/// <summary>
	/// Creates and attaches a new audio source component to the given game object. The
	/// audio source will play the audio clip provided.
	/// </summary>
	/// <returns>The new audio source component.</returns>
	/// <param name="obj">The game object the audio source component is attached to.</param>
	/// <param name="clip">The audio clip for the audio source component.</param>
	public static AudioSource CreateAudioSource(GameObject obj, AudioClip clip)
	{
		AudioSource defaultSource = AudioManager.Instance.GetSourceWithClip(clip.name);
		AudioSource source;
		if (defaultSource == null)
		{
			source = obj.AddComponent<AudioSource>();
			source.clip = clip;
		}
		else
		{
			source = obj.AddComponent<AudioSource>(defaultSource);
		}
		return source;
	}
}
