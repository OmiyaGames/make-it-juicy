using UnityEngine;
using System.Collections;

public class Diamond : MonoBehaviour
{
    public int incrementScore = 500;
    public SpriteRenderer diamond = null;
    public ParticleSystem sparklingDust = null;

    bool isTriggered = false;
    Rigidbody2D body = null;
    AudioMutator mutator;

    public Rigidbody2D Body
    {
        get
        {
            if(body == null)
            {
                body = rigidbody2D;
            }
            return body;
        }
    }

	// Use this for initialization
	void Start ()
    {
        mutator = GetComponent<AudioMutator>();
        ParticleSystem[] allParticles = GetComponentsInChildren<ParticleSystem>();
        for(int index = 0; index < allParticles.Length; ++index)
        {
            allParticles[index].renderer.sortingLayerID = diamond.sortingLayerID;
            allParticles[index].renderer.sortingOrder = diamond.sortingOrder;
        }
    }

    public void Reset()
    {
        isTriggered = false;
        diamond.enabled = true;
        sparklingDust.Play();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if((isTriggered == false) && (FlappyBirdController.CurrentState == FlappyBirdController.State.Alive) && (other.CompareTag("Player") == true))
        {
            isTriggered = true;
            diamond.enabled = false;
            mutator.Play();
            FlappyBirdController.CurrentScore += incrementScore;
            sparklingDust.Stop();
        }
    }
}
