using UnityEngine;
using System.Collections;

public class UnmuteMusic : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        AudioSource backgroundMusic = GlobalGameObject.Get<AudioSource>();
        backgroundMusic.mute = false;
	}
}
