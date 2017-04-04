using UnityEngine;
using OmiyaGames;

[RequireComponent(typeof(SoundEffect))]
public class ParticleSystemExplosion : MonoBehaviour
{
	// a simple script to scale the size, speed and lifetime of a particle system
	public float multiplier = 1f;

    ParticleSystem[] allParticles = null;
    Transform cachedTransform = null;
    SoundEffect cachedAudio = null;

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

    public SoundEffect CachedAudio
    {
        get
        {
            if(cachedAudio == null)
            {
                cachedAudio = GetComponent<SoundEffect>();
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
                ParticleSystem.MainModule main;
                ParticleSystem.MinMaxCurve curve;
                foreach(ParticleSystem system in AllParticles)
                {
                    main = system.main;

                    curve = main.startSize;
                    curve.constant *= multiplier;
                    main.startSize = curve;

                    curve = main.startSpeed;
                    curve.constant *= multiplier;
                    main.startSpeed = curve;

                    curve = main.startLifetime;
                    curve.constant *= Mathf.Lerp(multiplier, 1, 0.5f);
                    main.startLifetime = curve;
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
