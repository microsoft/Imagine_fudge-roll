using UnityEngine;
using System.Collections;

/// <summary>
/// Controller that contains logic for finishing the game when hitting the ribbon on the goal.
/// </summary>
public class GoalController : MonoBehaviour
{
	public AudioClip goalSound;
	AudioSource goalSource;

	public GameObject winAnim;

	void Start()
	{
		//Set up audio
		goalSource = AudioHelper.CreateAudioSource(gameObject, goalSound);
	}

	/// <summary>
	/// Called when the ball collides with the ribbon.
	/// It's a trigger because we don't want to apply physics to the collision.
	/// </summary>
	/// <param name="other">Other object that collided with us.</param>
	void OnTriggerEnter(Collider other)
	{
		// *** Add your source code here ***
	}
}
