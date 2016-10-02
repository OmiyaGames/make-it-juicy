using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioMutator))]
public class ParticleSystemExplosion : MonoBehaviour
{
	// a simple script to scale the size, speed and lifetime of a particle system
	public float multiplier = 1f;

    ParticleSystem[] allParticles = null;
    Transform cachedTransform = null;
    AudioMutator cachedAudio = null;

    public Transform CachedTransform
    {
        get
        {
            if(cachedTransform == null)
            {
                cachedTransform = transform;
            }
            return cachedTransform;
        }
    }

    public Vector2 Position
    {
        get
        {
            Vector3 position = CachedTransform.position;
            return new Vector2(position.x, position.y);
        }
    }

    public AudioMutator CachedAudio
    {
        get
        {
            if(cachedAudio == null)
            {
                cachedAudio = GetComponent<AudioMutator>();
            }
            return cachedAudio;
        }
    }

    public ParticleSystem[] AllParticles
    {
        get
        {
            if(allParticles == null)
            {
                allParticles = GetComponentsInChildren<ParticleSystem>();
                foreach(ParticleSystem system in AllParticles)
                {
                    system.startSize *= multiplier;
                    system.startSpeed *= multiplier;
                    system.startLifetime *= Mathf.Lerp(multiplier, 1, 0.5f);
                }
            }
            return allParticles;
        }
    }

	public void Explode()
	{
        foreach(ParticleSystem system in AllParticles)
        {
            system.Clear();
            system.Stop();
            system.Play();
        }
        if(CachedAudio != null)
        {
            CachedAudio.Play();
        }
    }
}
