using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class FlappyBirdController : IGameController
{
    public const string PrependScores = "Score: ";
    public const string KeyLastHighScore = "LastHighScore";
    public static int CurrentScore = 0;

	public enum State
	{
		Start,
		Alive,
		Dead,
		Success,
        StopStory
	}

    public enum DeathStyle
    {
        Still,
        Drop,
        CameraShake,
        Story
    }

	public float flapForce = 1000f;
    public GameObject tapAnimation = null;
	[Header("Rotation Animation")]
	public Vector2 allowedVelocityRange = new Vector2(-5f, 5f);
	public float velocityToAngle = 5f;
	public float lerpValue = 5f;
	[Header("Starting Animation")]
	public float startForce = 10f;
	public float flapIfBelow = -1f;
	[Header("Camera Animation")]
	public Animation cameraAnimation = null;
	public string cameraAnimationName = "cameraRotate";
    [Header("Audio")]
    public AudioMutator flapSound = null;
    public AudioMutator hitSound = null;
    [Header("Other Stuff")]
    public ScrollBackground scroll = null;
    public GameObject[] enableAfterDeath = null;
    public bool playMusic = true;
    public DeathStyle deathStyle = DeathStyle.Still;
    public ParticleSystem featherEffect = null;
    public ParticleSystem trailerEffect = null;
    public ParticleSystemExplosion explosionEffect = null;
    public ParticleSystemExplosion fireEffect = null;
    [Header("Background Flash")]
    public GameObject flash = null;
    public float slowdownFor = 0.5f;
    [Header("Score Stuff")]
    public TextMesh[] scoreHud = null;
    public TextMesh[] scoreIncrement = null;
    public Animator scoreAnimation = null;
    public string scoreAnimationName = "Score";
    public Vector3 scoreDisplayOffset = new Vector3(0, 1, 0);
    public HSBColor scoreColor = new HSBColor(Color.white);
    public float colorChangeSpeed = 0.1f;
    [Header("High Score Stuff")]
    public GameObject lastHighScoreSet = null;
    public TextMesh[] lastHighScore = null;
    public GameObject newHighScoreSet = null;
    public TextMesh[] highScore = null;
    public TextMesh[] newLabels = null;
    [Header("Flag Stuff")]
    public int[] flagThresholds = new int[] { 1000, 5500, 15500, 500000, 1000000 };
    public GameObject nextFlagStartSet = null;
    public TextMesh[] nextFlagStartLabel = null;
    public GameObject nextFlagEndSet = null;
    public TextMesh[] nextFlagEndLabel = null;
    public Animator flagAnimation = null;
    public string flagAnimationName = "Flag";
    public TextMesh[] flagIncrement = null;
    public AudioMutator soundEffect = null;
    [Header("Story Stuff")]
    public Animator storyAnimation = null;
    public string storyAnimationTrigger = "KickOffStory";
    public AudioSource fireAudio = null;
    public float deathSlowdown = 0.2f;
    public Camera storyCamera = null;
    public float cameraPositionLerp = 5f;
    public TextMesh dateLabel;
    public TextMesh traveledLabel;

    float timeGameStarted = 0;
	static State state = State.Start;
	Rigidbody2D body = null;
	Animator animator = null;
	Transform cacheCameraTransform = null;

	Vector2 startingPosition = Vector3.zero;
	
	Vector2 flapForceVector = Vector2.up;
	Vector2 startForceVector = Vector2.up;
	Vector2 obstaclePosition = Vector2.zero;
    Vector2 backgroundPosition = Vector2.zero;
    Vector2 winTriggerPosition = Vector2.zero;

	Vector2? flapTriggered = null;
    Vector3 cameraPosition;

    float slowdownStarted = -1f;
    int lastScore = 0;
    int nextFlag = 0;

	public override string Instructions
	{
		get
		{
			return "Flap!";
		}
	}

	public override bool IsSuccess
	{
		get
		{
			return (state == State.Success);
		}
	}

	public override bool IsAClone
	{
		get
		{
			return true;
		}
	}

    public static State CurrentState
    {
        get
        {
            return state;
        }
    }

    public static int HighScore
    {
        get
        {
            return PlayerPrefs.GetInt(KeyLastHighScore, 0);
        }
        set
        {
            PlayerPrefs.SetInt(KeyLastHighScore, value);
        }
    }

	public void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();

		flapForceVector *= flapForce;
		startForceVector *= startForce;

		cacheCameraTransform = cameraAnimation.transform;

		startingPosition = body.position;

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if(featherEffect != null)
        {
            featherEffect.GetComponent<Renderer>().sortingLayerName = sprite.sortingLayerName;
            featherEffect.GetComponent<Renderer>().sortingOrder = sprite.sortingOrder;
        }
        if(trailerEffect != null)
        {
            trailerEffect.GetComponent<Renderer>().sortingLayerName = sprite.sortingLayerName;
            trailerEffect.GetComponent<Renderer>().sortingOrder = sprite.sortingOrder;
        }
        if(explosionEffect != null)
        {
            foreach(ParticleSystem particle in explosionEffect.AllParticles)
            {
                particle.GetComponent<Renderer>().sortingLayerName = sprite.sortingLayerName;
                particle.GetComponent<Renderer>().sortingOrder = sprite.sortingOrder;
            }
        }
        if(fireEffect != null)
        {
            foreach(ParticleSystem particle in fireEffect.AllParticles)
            {
                particle.GetComponent<Renderer>().sortingLayerName = sprite.sortingLayerName;
                particle.GetComponent<Renderer>().sortingOrder = sprite.sortingOrder;
            }
        }

		gameObject.tag = "Player";
        CurrentScore = 0;
        lastScore = 0;
        Time.timeScale = 1f;
	}

	/// <summary>
	/// Called on the beginning of scene load
	/// </summary>
	public override void Start()
	{
		state = State.Start;
        timeGameStarted = Time.time;

		cameraAnimation.Stop();
		cacheCameraTransform.rotation = Quaternion.identity;

		flapTriggered = null;

		body.velocity = Vector2.zero;
		body.isKinematic = true;
		body.position = startingPosition;
		body.rotation = 0;
		body.isKinematic = false;

        AudioSource backgroundMusic = GlobalGameObject.Get<AudioSource>();
        backgroundMusic.mute = !playMusic;

        tapAnimation.SetActive(true);

        if((lastHighScore != null) && (lastHighScore.Length > 0))
        {
            string labelText = HighScore.ToString("00");
            for(int index = 0; index < lastHighScore.Length; ++index)
            {
                lastHighScore[index].text = labelText;
            }
        }

        if(nextFlagStartSet != null)
        {
            // Check to see which flag user surpassed
            for(nextFlag = 0; nextFlag < flagThresholds.Length; ++nextFlag)
            {
                if(HighScore < flagThresholds[nextFlag])
                {
                    break;
                }
            }

            if(nextFlag < flagThresholds.Length)
            {
                nextFlagStartSet.SetActive(true);
                if((nextFlagStartLabel != null) && (nextFlagStartLabel.Length > 0))
                {
                    string labelText = flagThresholds[nextFlag].ToString();
                    for(int index = 0; index < nextFlagStartLabel.Length; ++index)
                    {
                        nextFlagStartLabel[index].text = labelText;
                    }
                }
            }
            else
            {
                nextFlagStartSet.SetActive(false);
            }
        }
	}
	
	/// <summary>
	/// Called on every frame
	/// </summary>
	public void Update()
	{
		if(state == State.Start)
		{
            if(Input.GetMouseButtonDown(0) == true)
			{
				flapTriggered = flapForceVector;
				state = State.Alive;
                scroll.updateObstacles = true;
                tapAnimation.SetActive(false);
                if(lastHighScoreSet != null)
                {
                    lastHighScoreSet.SetActive(false);
                }
			}
			else if((body.position.y < flapIfBelow) && (body.velocity.y < 0))
			{
				flapTriggered = startForceVector;
			}
		}
        else if(state == State.StopStory)
        {

        }
		else if(state != State.Dead)
		{
			if(Input.GetMouseButtonDown(0) == true)
			{
				flapTriggered = flapForceVector;
			}
            else if((deathStyle != DeathStyle.Story) && (Input.GetKey(KeyCode.Escape) == true))
            {
                Die();
            }
		}
        else
        {
            if(deathStyle == DeathStyle.Story)
            {
                // Move the camera to flappy bird
                if(storyCamera != null)
                {
                    cameraPosition.x = transform.position.x;
                    storyCamera.transform.position = Vector3.Lerp(storyCamera.transform.position, cameraPosition, (Time.unscaledDeltaTime * cameraPositionLerp));
                }
            }
            else if((flash != null) && (slowdownStarted > 0) && (flash.activeSelf == true) && ((Time.unscaledTime - slowdownStarted) > slowdownFor))
            {
                flash.SetActive(false);
                slowdownStarted = -1;
                Time.timeScale = 1;
            }
        }

        if(CurrentScore != lastScore)
        {
            UpdateScoreHud();
        }

        UpdateLabelColor();
        RotationAnimation();
    }

	public void FixedUpdate()
	{
		if(flapTriggered.HasValue == true)
		{
			Flap(flapTriggered.Value);
			flapTriggered = null;
		}
        if(state != State.Dead)
        {
            Vector2 position = body.position;
            position.x = startingPosition.x;
            body.position = position;
        }
    }

	public void OnCollisionEnter2D(Collision2D info)
	{
		if((state == State.Alive) && (info.collider != null) && (info.collider.CompareTag("Harmful") == true))
		{
            Die();
		}
	}

    public void Die()
    {
        // Setup typical states
        state = State.Dead;
        scroll.enabled = false;

        // Change animation based on death style
        switch(deathStyle)
        {
            case DeathStyle.Still:
            {
                body.isKinematic = true;
                break;
            }
            case DeathStyle.CameraShake:
            case DeathStyle.Story:
            {
                cameraAnimation.Stop();
                cameraAnimation.Play(cameraAnimationName);
                goto case DeathStyle.Drop;
            }
            case DeathStyle.Drop:
            {
                if(explosionEffect != null)
                {
                    explosionEffect.Explode();
                }
                if(fireEffect != null)
                {
                    fireEffect.Explode();
                }
                if(trailerEffect != null)
                {
                    trailerEffect.enableEmission = false;
                }
                break;
            }
        }

        bool newHighScore = false;
        if(CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            newHighScore = true;
        }

        // Play Sound
        hitSound.Play();

        // Check if we're telling a story
        if(deathStyle == DeathStyle.Story)
        {
            // Cut out the background music
            AudioSource backgroundMusic = GlobalGameObject.Get<AudioSource>();
            backgroundMusic.mute = true;

            // Make it flash and slow down
            if(flash != null)
            {
                flash.SetActive(true);
                Time.timeScale = deathSlowdown;
            }

            // Play the fire sound
            if(fireAudio != null)
            {
                fireAudio.Play();
            }

            // Grab the camera's position
            if(storyCamera != null)
            {
                cameraPosition = storyCamera.transform.position;
            }

            // Play the story's animation
            if(storyAnimation != null)
            {
                storyAnimation.SetTrigger(storyAnimationTrigger);
            }

            // Update labels
            if(dateLabel != null)
            {
                dateLabel.text = string.Format(dateLabel.text, System.DateTime.Now.ToLongDateString());
            }
            if(traveledLabel != null)
            {
                traveledLabel.text = string.Format(traveledLabel.text, (CurrentScore + 1));
            }
        }
        else
        {
            tapAnimation.SetActive(false);
            for(int index = 0; index < enableAfterDeath.Length; ++index)
            {
                enableAfterDeath[index].SetActive(true);
            }
            if(flash != null)
            {
                flash.SetActive(true);
                slowdownStarted = Time.unscaledTime;
                Time.timeScale = 0;
            }
            
            if(newHighScoreSet != null)
            {
                newHighScoreSet.SetActive(true);
                string labelText = HighScore.ToString("00");
                int index = 0;
                for(; index < highScore.Length; ++index)
                {
                    highScore[index].text = labelText;
                }
                for(index = 0; index < newLabels.Length; ++index)
                {
                    newLabels[index].gameObject.SetActive(newHighScore);
                }
            }
            if(nextFlagEndSet != null)
            {
                if(nextFlag < flagThresholds.Length)
                {
                    nextFlagEndSet.SetActive(true);
                    if((nextFlagEndLabel != null) && (nextFlagEndLabel.Length > 0))
                    {
                        string labelText = flagThresholds[nextFlag].ToString();
                        for(int index = 0; index < nextFlagEndLabel.Length; ++index)
                        {
                            nextFlagEndLabel[index].text = labelText;
                        }
                    }
                }
                else
                {
                    nextFlagEndSet.SetActive(false);
                }
            }
        }
    }

    public void StopStory()
    {
        scroll.enabled = false;

        state = State.StopStory;
    }

	public void TriggerSuccess()
	{
		if(state == State.Alive)
		{
			state = State.Success;
		}
	}

	void Flap(Vector2 flapForce)
	{
        body.velocity = flapForce;
		//body.AddForce(flapForce, ForceMode2D.Impulse);
		animator.ResetTrigger("Flap");
		animator.SetTrigger("Flap");

        if(featherEffect != null)
        {
            featherEffect.Emit(10);
        }

        // Play Sound
        flapSound.Play();
	}

	void RotationAnimation()
	{
        if(state != State.Dead)
        {
            float velocity = body.velocity.y;
            if(velocity < allowedVelocityRange.x)
            {
                velocity = allowedVelocityRange.x;
            }
            else if(velocity > allowedVelocityRange.y)
            {
                velocity = allowedVelocityRange.y;
            }
            body.rotation = Mathf.Lerp(body.rotation, (velocity * velocityToAngle), (Time.deltaTime * lerpValue));
        }
        else
        {
            switch(deathStyle)
            {
                case DeathStyle.Drop:
                case DeathStyle.CameraShake:
                case DeathStyle.Story:
                {
                    body.rotation = Mathf.Lerp(body.rotation, -90f, (Time.deltaTime * lerpValue));
                    break;
                }
            }
        }
	}

    void UpdateLabelColor()
    {
        scoreColor.h += (colorChangeSpeed * Time.deltaTime);
        while (scoreColor.h > 1)
        {
            scoreColor.h -= 1;
        }
        if ((scoreIncrement != null) && (scoreIncrement.Length > 0))
        {
            scoreIncrement[0].color = scoreColor.ToColor();
        }
        if ((newLabels != null) && (newLabels.Length > 0))
        {
            newLabels[0].color = scoreColor.ToColor();
        }
    }

    void UpdateScoreHud()
    {
        if ((scoreHud != null) && (scoreAnimation != null))
        {
            string scoreString = PrependScores + CurrentScore.ToString("00");
            int index = 0;
            for (; index < scoreHud.Length; ++index)
            {
                scoreHud[index].text = scoreString;
            }
            scoreString = "+" + (CurrentScore - lastScore).ToString();
            scoreAnimation.gameObject.SetActive(true);
            scoreAnimation.transform.position = transform.position + scoreDisplayOffset;
            scoreAnimation.Play(scoreAnimationName, -1, 0);
            for (index = 0; index < scoreIncrement.Length; ++index)
            {
                scoreIncrement[index].text = scoreString;
            }
            lastScore = CurrentScore;
        }
        if ((flagAnimation != null) && (nextFlag < flagThresholds.Length) && (CurrentScore >= flagThresholds[nextFlag]))
        {
            // Update label
            if ((flagIncrement != null) && (flagIncrement.Length > 0))
            {
                string labelText = flagThresholds[nextFlag].ToString();
                for (int index = 0; index < flagIncrement.Length; ++index)
                {
                    flagIncrement[index].text = labelText;
                }
            }

            // Show label
            flagAnimation.gameObject.SetActive(true);
            flagAnimation.Play(flagAnimationName, -1, 0);
            soundEffect.Play();

            // Switch to the next threshold
            ++nextFlag;
        }
    }
}
