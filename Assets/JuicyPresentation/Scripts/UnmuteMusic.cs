using UnityEngine;
using OmiyaGames;

public class UnmuteMusic : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        BackgroundMusic.GlobalMute = false;
	}
}
