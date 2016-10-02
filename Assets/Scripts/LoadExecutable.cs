using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Text;

public class LoadExecutable : MonoBehaviour
{
	public string applicationName = "";

	private StringBuilder pathBuilder = new StringBuilder();
	private LoadingDisplay loading = null;

	public LoadingDisplay LoadingScript
	{
		set
		{
			loading = value;
		}
	}

	public bool Execute()
	{
		bool returnFlag = false;
		if(string.IsNullOrEmpty(applicationName) == false)
		{
			// Determine the path
			pathBuilder.Length = 0;
			pathBuilder.Append(Application.dataPath);
			switch(Application.platform)
			{
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.OSXPlayer:
				pathBuilder.Append("/../../../");
				break;
			default:
				pathBuilder.Append("/../../");
				break;
			}
			pathBuilder.Append(applicationName);
			pathBuilder.Append('/');
			pathBuilder.Append(applicationName);
			switch(Application.platform)
			{
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.OSXPlayer:
				pathBuilder.Append(".app");
				break;
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.WindowsPlayer:
				pathBuilder.Append(".exe");
				break;
			}

			// Create a new process
			Process game = new Process();
			game.StartInfo.FileName = pathBuilder.ToString();
			game.StartInfo.Arguments = "-single-instance";
			game.Start();

			// Display loading
			loading.DisplayLoading();
			returnFlag = true;
		}
		return returnFlag;
	}
}
