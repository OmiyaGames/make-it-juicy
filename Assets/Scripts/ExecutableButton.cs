using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LoadExecutable))]
public class ExecutableButton : IButton
{
	private LoadExecutable executable = null;

	public override bool OnClick()
	{
		bool returnFlag = false;
		if(executable != null)
		{
			returnFlag = executable.Execute();
		}
		return returnFlag;
	}
	
	protected override void Awake()
	{
		base.Awake();	
		executable = GetComponent<LoadExecutable>();
	}
}
