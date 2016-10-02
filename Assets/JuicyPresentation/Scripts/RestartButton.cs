using UnityEngine;
using System.Collections;

public class RestartButton : IButton
{
	public override bool OnClick()
	{
		Application.LoadLevel(Application.loadedLevel);
		return true;
	}
}
