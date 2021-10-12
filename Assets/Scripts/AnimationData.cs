using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AnimationData : ScriptableObject
{
	public int priority = 0;
	public bool loop = true;
	public bool playBackwards = false;
	[Tooltip("Frames per second")]
	public float defaultSpeed = 15f;
	public int startingFrame = 0;
	public Sprite[] sprites;
}
