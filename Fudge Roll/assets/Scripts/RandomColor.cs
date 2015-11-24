using UnityEngine;
using System.Collections;

/// <summary>
/// Script to set a random color on the object it is attached to.
/// </summary>
public class RandomColor : MonoBehaviour
{
	void Start()
	{
		Renderer renderer = GetComponentInChildren<Renderer>();
		if (renderer)
		{
			renderer.material.SetFloat("_Col", Random.Range(0, 10) * 0.1f);
		}
	}
}
