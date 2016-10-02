using UnityEngine;
using System.Collections;

public class MenuButton : IButton
{
    private static int menuIndex = 0;

    public enum Direction
    {
        Up,
        Down
    }

    [Header("Menu")]
    public Direction direction = Direction.Down;
    public Animator menuAnimation = null;
    public string indexField = "Index";

    void Start()
    {
        int index = menuAnimation.GetInteger(indexField);
        if(menuIndex != index)
        {
            menuAnimation.SetInteger(indexField, menuIndex);
        }
    }

	public override bool OnClick()
	{
        if(menuAnimation != null)
        {
            int index = menuAnimation.GetInteger(indexField);
            if(direction == Direction.Down)
            {
                ++index;
            }
            else
            {
                --index;
            }
            menuAnimation.SetInteger(indexField, index);
            menuIndex = index;
        }
		return true;
	}
}
