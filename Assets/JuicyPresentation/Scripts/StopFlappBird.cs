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
        heartParticles.renderer.sortingLayerID = femaleFlappyBird.sortingLayerID;
        heartParticles.renderer.sortingOrder = femaleFlappyBird.sortingOrder;
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
