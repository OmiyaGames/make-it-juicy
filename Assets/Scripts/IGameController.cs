using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class IGameController : MonoBehaviour
{
	public const float SnappingDistance = 0.01f;

	/// <summary>
	/// Called on the beginning of scene load
	/// </summary>
	public abstract void Start();

	/// <summary>
	/// Gets the instructions.
	/// </summary>
	/// <value>The instructions.</value>
	public abstract string Instructions
	{
		get;
	}

	public abstract bool IsSuccess
	{
		get;
	}

	public abstract bool IsAClone
	{
		get;
	}
}
