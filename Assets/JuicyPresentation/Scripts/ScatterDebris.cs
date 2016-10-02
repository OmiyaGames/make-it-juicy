using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScatterDebris : MonoBehaviour
{
    [Header("Top Pillars")]
    public string topPillarNames = "";
    public SpriteRenderer[] topPillarSprites;
    public Rigidbody2D[] topPillarBodies;
    [Header("Bottom Pillars")]
    public string bottomPillarNames = "";
    public SpriteRenderer[] bottomPillarSprites;
    public Rigidbody2D[] bottomPillarBodies;
    [Header("Explosions")]
    public ParticleSystemExplosion topExplosionCenter;
    public ParticleSystemExplosion bottomExplosionCenter;
    public string explosionLayer = "Foreground";
    public int explosionOrder = 7;
    [Header("Timing")]
    public float disappearAfter = 2f;
    [Header("Forces")]
    public Vector2 velocity = new Vector2(-10, 0);

    float timeTriggered = -1f;
    Transform[] allTransforms;
    Vector3[] allLocalPositions;
    Quaternion[] allLocalRotations;
    GameObject objectCache;
    Transform transformCache;

    public GameObject CachedObject
    {
        get
        {
            if(objectCache == null)
            {
                objectCache = gameObject;
            }
            return objectCache;
        }
    }

    public Transform CachedTransform
    {
        get
        {
            if(transformCache == null)
            {
                transformCache = transform;
            }
            return transformCache;
        }
    }

    public void Scatter(Vector3 location)
    {
        Deactivate();
        CachedTransform.position = location;
        CachedObject.SetActive(true);

        int index = 0;
        for(; index < topPillarBodies.Length; ++index)
        {
            topPillarBodies[index].velocity = velocity;
        }
        for(index = 0; index < bottomPillarBodies.Length; ++index)
        {
            bottomPillarBodies[index].velocity = velocity;
        }

        topExplosionCenter.Explode();
        bottomExplosionCenter.Explode();

        StartCoroutine(DelayDeactivation());
    }

    public void Deactivate()
    {
        CachedObject.SetActive(false);
        if(allTransforms == null)
        {
            SetupTransforms();
        }
        int index = 0;
        for(; index < allTransforms.Length; ++index)
        {
            allTransforms[index].localPosition = allLocalPositions[index];
            allTransforms[index].localRotation = allLocalRotations[index];
        }
    }

	// Use this for initialization
	void SetupTransforms()
    {
        int index = 0;
        allTransforms = new Transform[topPillarBodies.Length + bottomPillarBodies.Length];
        allLocalPositions = new Vector3[allTransforms.Length];
        allLocalRotations = new Quaternion[allTransforms.Length];
        for(; index < allTransforms.Length; ++index)
        {
            if(index < topPillarBodies.Length)
            {
                allTransforms[index] = topPillarBodies[index].transform;
            }
            else
            {
                allTransforms[index] = bottomPillarBodies[index - topPillarBodies.Length].transform;
            }
            allLocalPositions[index] = allTransforms[index].localPosition;
            allLocalRotations[index] = allTransforms[index].localRotation;
        }
        foreach(ParticleSystem particle in topExplosionCenter.AllParticles)
        {
            particle.renderer.sortingLayerID = topPillarSprites[0].sortingLayerID;
            particle.renderer.sortingOrder = topPillarSprites[0].sortingOrder;
        }
        foreach(ParticleSystem particle in bottomExplosionCenter.AllParticles)
        {
            particle.renderer.sortingLayerID = topPillarSprites[0].sortingLayerID;
            particle.renderer.sortingOrder = topPillarSprites[0].sortingOrder;
        }
	}

    IEnumerator DelayDeactivation()
    {
        yield return new WaitForSeconds(disappearAfter);
        Deactivate();
    }

    [ContextMenu("Setup")]
    void Setup()
    {
        List<SpriteRenderer> topPillarSpritesList = new List<SpriteRenderer>();
        List<Rigidbody2D> topPillarBodiesList = new List<Rigidbody2D>();
        List<SpriteRenderer> bottomPillarSpritesList = new List<SpriteRenderer>();
        List<Rigidbody2D> bottomPillarBodiesList = new List<Rigidbody2D>();

        foreach(Transform child in transform)
        {
            if(child.name.StartsWith(topPillarNames) == true)
            {
                topPillarSpritesList.Add(child.GetComponent<SpriteRenderer>());
                topPillarBodiesList.Add(child.GetComponent<Rigidbody2D>());
            }
            else if(child.name.StartsWith(bottomPillarNames) == true)
            {
                bottomPillarSpritesList.Add(child.GetComponent<SpriteRenderer>());
                bottomPillarBodiesList.Add(child.GetComponent<Rigidbody2D>());
            }
        }
        topPillarSprites = topPillarSpritesList.ToArray();
        topPillarBodies = topPillarBodiesList.ToArray();
        bottomPillarSprites = bottomPillarSpritesList.ToArray();
        bottomPillarBodies = bottomPillarBodiesList.ToArray();
    }
}
