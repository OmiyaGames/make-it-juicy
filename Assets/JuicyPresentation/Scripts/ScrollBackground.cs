using UnityEngine;
using System.Collections;

public class ScrollBackground : MonoBehaviour
{
    public Transform flappyBird;
	public float scrollSpeed = 5f;
    public int incrementScore = 1;
    [Header("Background")]
	public Transform[] background;
	public int backgroundAt0 = 0;
    public Vector3 shiftBackgroundBy = new Vector3();
    public bool updateBackground = false;
    [Header("Foreground")]
	public Rigidbody2D[] foreground;
	public int foregroundAt0 = 1;
    public Vector2 shiftForegroundBy = new Vector2();
    public bool updateForeground = false;
    [Header("Obstacles")]
    public Rigidbody2D[] obstacle;
    public int obstacleAt0 = 1;
    public Vector2 shiftObstacleBy = new Vector2();
    public Vector2 allowedYRange = new Vector2(-2, 2);
    public bool cycleObstacles = true;
    public bool updateObstacles = false;
    [Header("Debris")]
    public ScatterDebris[] debris;
    public ParticleSystem backgroundPartiles = null;
    public SpriteRenderer matchOrder = null;
    [Header("CameraAnimation")]
    public Animation cameraAnimation = null;
    public string scoreAnimation = "cameraShake";
    public GameObject rockFlash = null;
    public float pauseSeconds = 0.2f;
    [Header("Diamonds")]
    public Diamond[] diamonds = null;
    public int interval = 3;
    public Vector2 allowedDiamondXRange = new Vector2(2f, 4.5f);
    public Vector2 allowedDiamondYRange = new Vector2(-2.5f, 2.5f);
    [Header("Difficulty")]
    public bool increaseDifficulty = false;
    public float increaseSpeedBy = 0.1f;
    public int increaseSpeedAfterLevels = 5;

	int index, debrisIndex, diamondsIndex, levels = 1;
	Vector3 left = new Vector3(-1, 0, 0);
	Vector2 cache;
    Vector3 shiftObstaclesBy3;
    float flashStarted = -1f;
    int diamondPlacement = 0;

    void Start()
    {
        debrisIndex = 0;
        diamondPlacement = 0;
        levels = 1;
        obstacle[0].position = PositionObstacle(obstacle[obstacle.Length - 1].position + shiftObstacleBy);
        for(index = 0; index < obstacle.Length; ++index)
        {
            obstacle[index].position = PositionObstacle(obstacle[index].position);
        }
        if((diamonds != null) && (diamonds.Length > 0))
        {
            PositionDiamond(obstacle[0].position);
        }
        shiftObstaclesBy3 = shiftObstacleBy;
        if(backgroundPartiles != null)
        {
            backgroundPartiles.GetComponent<Renderer>().sortingLayerID = matchOrder.sortingLayerID;
            backgroundPartiles.GetComponent<Renderer>().sortingOrder = matchOrder.sortingOrder + 1;
        }
    }

	// Update is called once per frame
	void Update ()
	{
        if(enabled == true)
        {
            if(updateBackground == true)
    		{
    			// Translate all the elements
    			left.x = scrollSpeed * Time.deltaTime * -1f;
    			for(index = 0; index < background.Length; ++index)
    			{
    				background[index].Translate(left);
    			}

    			// Sort the list
    			if(background[backgroundAt0 + 1].position.x < 0)
    			{	
                    background[0].position = background[background.Length - 1].position + shiftBackgroundBy;
    				ReSortArray<Transform>(background);
    			}
    		}
            if((debris.Length > 0) && (flappyBird != null))
            {
                for(index = 0; index < (obstacleAt0 + 1); ++index)
                {
                    if((obstacle[index].gameObject.activeSelf == true) && (obstacle[index].position.x < flappyBird.position.x))
                    {
                        obstacle[index].gameObject.SetActive(false);
                        FlappyBirdController.CurrentScore += incrementScore;
                        if(increaseDifficulty == true)
                        {
                            if((levels % increaseSpeedAfterLevels) == 0)
                            {
                                scrollSpeed += increaseSpeedBy;
                            }
                            ++levels;
                        }
                        if(rockFlash == null)
                        {
                            AnimateRocks(obstacle[index].position);
                        }
                        else
                        {
                            AnimateFlash(obstacle[index].position);
                        }
                        break;
                    }
                }
            }
            if((rockFlash != null) && (rockFlash.activeSelf == true) && ((Time.unscaledTime - flashStarted) > pauseSeconds))
            {
                // Hide the flash
                rockFlash.SetActive(false);

                // Return the time
                Time.timeScale = 1;
                flashStarted = -1f;

                // Animate the rocks
                AnimateRocks(rockFlash.transform.position);
            }
        }
	}

    void FixedUpdate()
    {
        if(enabled == true)
        {
            left.x = scrollSpeed * Time.deltaTime * -1f;
            if(updateForeground == true)
            {
                // Translate all the elements
                for(index = 0; index < foreground.Length; ++index)
                {
                    cache = foreground[index].position;
                    cache.x += left.x;
                    foreground[index].MovePosition(cache);
                }

                // Sort the list
                if(foreground[foregroundAt0 + 1].position.x < 0)
                {   
                    foreground[0].position = foreground[foreground.Length - 1].position + shiftForegroundBy;
                    ReSortArray<Rigidbody2D>(foreground);
                }
            }

            if(updateObstacles == true)
            {
                for(index = 0; index < obstacle.Length; ++index)
                {
                    cache = obstacle[index].position;
                    cache.x += left.x;
                    obstacle[index].MovePosition(cache);
                }

                if((cycleObstacles == true) && (obstacle[obstacleAt0 + 1].position.x < 0))
                {
                    if(obstacle[0].gameObject.activeSelf == true)
                    {
                        obstacle[0].position = PositionObstacle(obstacle[obstacle.Length - 1].position + shiftObstacleBy);
                    }
                    else
                    {
                        obstacle[0].transform.position = PositionObstacle(obstacle[obstacle.Length - 1].transform.position + shiftObstaclesBy3);
                    }
                    ReSortArray<Rigidbody2D>(obstacle);
                    obstacle[obstacle.Length - 1].gameObject.SetActive(true);
                    if((diamonds != null) && (diamonds.Length > 0))
                    {
                        PositionDiamond(obstacle[obstacle.Length - 1].position);
                    }
                }

                if((diamonds != null) && (diamonds.Length > 0))
                {
                    for(index = 0; index < diamonds.Length; ++index)
                    {
                        cache = diamonds[index].Body.position;
                        cache.x += left.x;
                        diamonds[index].Body.MovePosition(cache);
                    }
                }
            }
        }
    }

	void ReSortArray<T>(T[] originalList)
	{
		T backup = originalList[0];
		for(index = 1; index < originalList.Length; ++index)
		{
			originalList[index - 1] = originalList[index];
		}
		originalList[originalList.Length - 1] = backup;
	}

    Vector2 PositionObstacle(Vector2 position)
    {
        position.y = Random.Range(allowedYRange.x, allowedYRange.y);
        return position;
    }

    Vector3 PositionObstacle(Vector3 position)
    {
        position.y = Random.Range(allowedYRange.x, allowedYRange.y);
        return position;
    }

    void AnimateFlash(Vector2 position)
    {
        // Show the flash
        rockFlash.SetActive(true);
        rockFlash.transform.position = new Vector3(position.x, position.y, 0);

        // Stop the time
        Time.timeScale = 0;
        flashStarted = Time.unscaledTime;
    }

    void AnimateRocks(Vector2 position)
    {
        debris[debrisIndex].Scatter(position);
        ++debrisIndex;
        if (debrisIndex >= debris.Length)
        {
            debrisIndex = 0;
        }
        if (cameraAnimation != null)
        {
            cameraAnimation.Stop();
            cameraAnimation.Play(scoreAnimation);
        }
    }

    void PositionDiamond(Vector2 position)
    {
        if((diamondPlacement % interval) == 0)
        {
            // Reset the diamond's condition
            diamonds[diamondsIndex].gameObject.SetActive(true);
            diamonds[diamondsIndex].Reset();

            // Position the diamond
            position.x += Random.Range(allowedDiamondXRange.x, allowedDiamondXRange.y);
            position.y = Random.Range(allowedDiamondYRange.x, allowedDiamondYRange.y);
            diamonds[diamondsIndex].Body.position = position;

            // Shift to the next diamond
            ++diamondsIndex;
            if(diamondsIndex >= diamonds.Length)
            {
                diamondsIndex = 0;
            }
        }
        ++diamondPlacement;
    }
}
