using UnityEngine;
using System.Collections;

public class LoadingDisplay : MonoBehaviour
{
	public GUISkin style;

	private IButton[] allButtons = null;
	private Rect screenSize;
	private bool isDisplayingLoading = false;

	public bool IsDisplayingLoading
	{
		set
		{
			if(isDisplayingLoading != value)
			{
				isDisplayingLoading = value;
				foreach(IButton button in allButtons)
				{
					button.IsEnabled = !value;
				}
			}
		}
	}
	// Use this for initialization
	void Start ()
	{
		allButtons = GetComponentsInChildren<IButton>();
		LoadExecutable[] scripts = GetComponentsInChildren<LoadExecutable>();
		foreach(LoadExecutable script in scripts)
		{
			script.LoadingScript = this;
		}
		screenSize = new Rect(0, 0, Screen.width, Screen.height);
	}
	
	// Update is called once per frame
	void OnGUI()
	{
		if(isDisplayingLoading == true)
		{
			GUI.skin = style;
			GUI.Box(screenSize, "Loading...");
		}
	}

	void OnApplicationFocus(bool flag)
	{
		if(flag == false)
		{
			IsDisplayingLoading = false;
		}
	}

	public void DisplayLoading()
	{
		IsDisplayingLoading = true;
	}
}
