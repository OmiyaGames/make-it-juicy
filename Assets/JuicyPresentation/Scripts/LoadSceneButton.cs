using UnityEngine;
using System.Collections;

public class LoadSceneButton : IButton
{
    [Header("Scene")]
    public string scene = "00Menu";
    [Header("Description")]
    public WrapTextMesh descriptionText = null;
    [TextArea(5, 20)]
    public string descriptionString = "This is an important description";

	public override bool OnClick()
	{
        Application.LoadLevel(scene);
		return true;
	}

    public override void OnHover()
    {
        if(descriptionText != null)
        {
            descriptionText.Text = descriptionString;
        }
    }
}
