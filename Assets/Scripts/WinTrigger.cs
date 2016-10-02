using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WinTrigger : MonoBehaviour
{
	public FlappyBirdController flappyBird = null;

    Collider2D thisCollider = null;

    Collider2D ThisCollider
    {
        get
        {
            if(thisCollider == null)
            {
                thisCollider = GetComponentInChildren<Collider2D>();
            }
            return thisCollider;
        }
    }

    public void Reset()
    {
    }
    	
	///<summary>
	/// Called on every frame
	///</summary>
	public void OnTriggerEnter2D(Collider2D other)
	{
		if((other != null) && (other.CompareTag("Player") == true))
		{
            bool results = true;
			if(flappyBird != null)
			{
				flappyBird.TriggerSuccess();
			}
		}
	}
}
