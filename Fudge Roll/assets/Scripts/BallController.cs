using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 /// <summary>
 /// The ball controller controls how the fudge ball rolls around in the world, and how it collects other objects.
 /// </summary>
public class BallController : MonoBehaviour
{
	public Transform cameraTransform;
	public float moveSpeed;
	public float cameraSensitivity;
	
	Transform myTransform;
	Rigidbody myRigidbody;
	SphereCollider myCollider;

	float baseRadius;

	List<Collider> colliders = new List<Collider>();
	Vector3 cameraOffset;
	float multiplier = 1.25f;	// Used to increase movement speed and camera offset
	float collisionOffset = 1f;

	public AudioClip lowRollSound;
	public AudioClip highRollSound;

	AudioSource lowRollSource;
	AudioSource highRollSource;
	AudioSource pickupSource;

	bool moving;

	public enum Stickyness
	{
		Weak,
		Medium,
		Strong
	};
	 
	[System.Serializable]
	public class StickyStage
	{
		public Stickyness stickiness;
		public float speedMultiplier;
		public float radiusIncrease;
		[Range(0f,1f)]
		public float pickupDistanceScale;
		public AudioClip collectSound;
	}

	public StickyStage[] stickyStages;

	Stickyness stickyness = Stickyness.Weak;

	/// <summary>
	/// Logic that runs when the object is initialized.
	/// </summary>
	void Start()
	{
		moving = false;

		myTransform = GetComponent<Transform>();
		myRigidbody = GetComponent<Rigidbody>();
		myCollider = GetComponentInChildren<SphereCollider>();

		baseRadius = myCollider.radius;

		cameraOffset = cameraTransform.position - myTransform.position;

		SetStickyness(0);

		lowRollSource = AudioHelper.CreateAudioSource(gameObject, lowRollSound);
		highRollSource = AudioHelper.CreateAudioSource(gameObject, highRollSound);
	}

	/// <summary>
	/// Logic that runs every frame.
	/// </summary>
	void Update()
	{
		// Process camera movement input if we're allowed to move
		if (GameplayManager.Instance.CanMove())
		{
			float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity;
			cameraOffset = Quaternion.AngleAxis(mouseX, Vector3.up) * cameraOffset;
		}

		cameraTransform.position = myTransform.position + (cameraOffset * Mathf.Min(1.5f, multiplier));
		cameraTransform.rotation = Quaternion.LookRotation(-cameraOffset + Vector3.up);
	}

	/// <summary>
	/// Logic that runs at fixed time intervals regardless of frame time.
	/// </summary>
	void FixedUpdate()
	{
		// Process ball movement if we're allowed to move
		if (GameplayManager.Instance.CanMove())
		{
			Vector3 forward = myTransform.position - cameraTransform.position;
			forward.y = 0f;
			forward.Normalize();

			Vector3 right = Vector3.Cross(Vector3.up, forward);

			// transform input into right and forward movement
			float moveX = Input.GetAxis("Horizontal") * moveSpeed * multiplier;
			float moveZ = Input.GetAxis("Vertical") * moveSpeed * multiplier;
			Vector3 force = (right * moveX) + (forward * moveZ);
			myRigidbody.AddForce(force);
		}

		// Start or stop looping movement sounds if we get slow or fast enough
		if (!moving && myRigidbody.velocity.magnitude > 2f)
		{
			moving = true;
			StartMovementSound();
		}
		else if (moving && myRigidbody.velocity.magnitude <= 1.5f)
		{
			moving = false;
			StopMovementSound();
		}
	}

	/// <summary>
	/// Logic that runs when the ball collides with another object
	/// </summary>
	/// <param name="collision">Collision.</param>
	void OnCollisionEnter(Collision collision)
	{
		// *** Add your source code here ***
	}

	/// <summary>
	/// Determines whether this instance can pick up the specified object.
	/// </summary>
	/// <returns><c>true</c> if the ball can pick up this object; otherwise, <c>false</c>.</returns>
	/// <param name="other">The object to pick up.</param>
	bool CanStick(GameObject other)
	{
		bool stick = false;
		switch (stickyness)
		{
		case Stickyness.Weak:
			stick = other.CompareTag("Small");
			break;
		case Stickyness.Medium:
			stick = other.CompareTag("Medium");
			break;
		case Stickyness.Strong:
			stick = other.CompareTag("Large");
			break;
		}
		return stick;
	}

	/// <summary>
	/// Sets the stickyness.
	/// </summary>
	/// <param name="index">Level of stickiness.</param>
	public void SetStickyness(int index)
	{
		index = Mathf.Clamp(index, 0, stickyStages.Length - 1);

		// Set our stickiness and update speed/collider for the new stage
		stickyness = stickyStages[index].stickiness;
		multiplier = stickyStages[index].speedMultiplier;
		myCollider.radius = baseRadius + stickyStages[index].radiusIncrease;
		collisionOffset = stickyStages[index].pickupDistanceScale;

		// Change audio for new stage
		pickupSource = AudioHelper.CreateAudioSource(gameObject, stickyStages[index].collectSound);
		ChangeMovementSound();
	}

	/// <summary>
	/// Starts audio for ball movement.
	/// </summary>
	void StartMovementSound()
	{
		// We play a combination of two sources depending on which stickiness level we have
		switch (stickyness)
		{
		case Stickyness.Weak:
			lowRollSource.Play();
			break;
		case Stickyness.Medium:
			highRollSource.Play();
			break;
		case Stickyness.Strong:
			lowRollSource.Play();
			highRollSource.Play();
			break;
		}
	}

	/// <summary>
	/// Changes the ball rolling sound to the correct one for our current stickiness.
	/// </summary>
	void ChangeMovementSound()
	{
		// If we're not moving, nothing needs changing
		if (!moving)
		{
			return;
		}

		// When we go from weak to medium, we need to turn off/on some sources
		switch(stickyness)
		{
		case Stickyness.Weak:
			break;
		case Stickyness.Medium:
			lowRollSource.Stop();
			highRollSource.Play();
			break;
		case Stickyness.Strong:
			lowRollSource.Play();
			break;
		}
	}
	
	/// <summary>
	/// Stops ball rolling audio.
	/// </summary>
	void StopMovementSound()
	{
		lowRollSource.Stop();
		highRollSource.Stop();
	}
}
