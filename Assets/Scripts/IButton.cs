using UnityEngine;
using System.Collections;

public abstract class IButton : MonoBehaviour
{
	private enum Trigger
	{
		Up,
		Down,
		Disable
	}
	public string onDownTrigger = "onDown";
	public string onUpTrigger = "onUp";
	public string onDisableTrigger = "onDisable";
	public AudioClip onPressDown = null;
	public AudioClip onPressUp = null;

	private Animator buttonAnimator = null;
	private AudioSource buttonSound = null;
	private bool isEnabled = true;
	private bool isMouseDown = false;
	private bool isHovering = false;
	private bool isDownGraphics = false;
	private bool wasDownGraphics = false;

	public abstract bool OnClick();
    public virtual void OnHover() {}

	public bool IsMouseDown
	{
		get
		{
			return isMouseDown;
		}
	}
	
	public bool IsHovering
	{
		get
		{
			return isHovering;
		}
	}

	public bool IsEnabled
	{
		get
		{
			return isEnabled;
		}
		set
		{
			if(isEnabled != value)
			{
				isEnabled = value;
				isMouseDown = false;
				isHovering = false;
				if(isEnabled == true)
				{
					TriggerAnimation(Trigger.Up);
				}
				else
				{
					TriggerAnimation(Trigger.Disable);
				}
			}
		}
	}

	protected virtual void Awake()
	{
		buttonAnimator = GetComponent<Animator>();
		buttonSound = GetComponent<AudioSource>();
	}

	private void UpdateButtonGraphics()
	{
		isDownGraphics = false;
		if((isHovering == true) && (isMouseDown == true))
		{
			isDownGraphics = true;
		}
		if(isDownGraphics != wasDownGraphics)
		{
			if(isDownGraphics == true)
			{
				buttonSound.PlayOneShot(onPressDown);
				TriggerAnimation(Trigger.Down);
			}
			else
			{
				buttonSound.PlayOneShot(onPressUp);
				TriggerAnimation(Trigger.Up);
			}
			wasDownGraphics = isDownGraphics;
		}
	}

	protected void OnMouseDown()
	{
		if((isHovering == true) && (isEnabled == true))
		{
			isMouseDown = true;
			UpdateButtonGraphics();
		}
	}

	protected void OnMouseExit()
	{
		if(isEnabled == true)
		{
			isHovering = false;
			UpdateButtonGraphics();
		}
	}
	
	protected void OnMouseEnter()
	{
		if(isEnabled == true)
		{
			isHovering = true;
            OnHover();
			UpdateButtonGraphics();
		}
	}

	protected void Update()
	{
		if((Input.GetMouseButtonUp(0) == true) && (isMouseDown == true))
		{
			isMouseDown = false;
			UpdateButtonGraphics();
			if(isHovering == true)
			{
				OnClick();
			}
		}
	}

	private void TriggerAnimation(Trigger trigger)
	{
		switch(trigger)
		{
			case Trigger.Up:
				buttonAnimator.ResetTrigger(onDownTrigger);
				buttonAnimator.ResetTrigger(onDisableTrigger);
				buttonAnimator.SetTrigger(onUpTrigger);
				break;
			case Trigger.Down:
				buttonAnimator.ResetTrigger(onUpTrigger);
				buttonAnimator.ResetTrigger(onDisableTrigger);
				buttonAnimator.SetTrigger(onDownTrigger);
				break;
			case Trigger.Disable:
				buttonAnimator.ResetTrigger(onDownTrigger);
				buttonAnimator.ResetTrigger(onUpTrigger);
				buttonAnimator.SetTrigger(onDisableTrigger);
				break;
		}
	}
}
