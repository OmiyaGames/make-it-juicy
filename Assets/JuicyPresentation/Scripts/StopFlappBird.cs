using UnityEngine;
using System.Collections;

public class StopFlappBird : MonoBehaviour
{
    public FlappyBirdController controller;
    public SpriteRenderer femaleFlappyBird;
    public ParticleSystem heartParticles;
    public Animator endAnimation;
    public string triggerName = "KickOffStory";

    void Start()
    {
        heartParticles.GetComponent<Renderer>().sortingLayerID = femaleFlappyBird.sortingLayerID;
        heartParticles.GetComponent<Renderer>().sortingOrder = femaleFlappyBird.sortingOrder;
        heartParticles.Stop();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") == true)
        {
            controller.StopStory();
            heartParticles.Play();
            endAnimation.SetTrigger(triggerName);
        }
    }
}
