using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animation))]
public class ObstacleHit : MonoBehaviour
{
    public bool wiggle = false;
    Animation animationCache;

	// Use this for initialization
	void Start ()
    {
        animationCache = GetComponent<Animation>();
	}
	
    void OnCollisionEnter2D(Collision2D info)
    {
        if((wiggle == true) && (info.collider.CompareTag("Player") == true))
        {
            animationCache.Play();
        }
    }
}
