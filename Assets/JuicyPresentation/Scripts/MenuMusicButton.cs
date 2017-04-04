using UnityEngine;
using OmiyaGames;

public class MenuMusicButton : IButton
{
    public const string MusicKey = "MenuMusicOn";
    public const string MusicOnText = "Turn Music Off";
    public const string MusicOffText = "Turn Music On";

    public TextMesh buttonLabel = null;

    bool IsMusicOn
    {
        get
        {
            return (PlayerPrefs.GetInt(MusicKey, 0) != 0);
        }
        set
        {
            if(value == true)
            {
                PlayerPrefs.SetInt(MusicKey, 1);
                BackgroundMusic.GlobalMute = false;
                buttonLabel.text = MusicOnText;
            }
            else
            {
                PlayerPrefs.SetInt(MusicKey, 0);
                BackgroundMusic.GlobalMute = true;
                buttonLabel.text = MusicOffText;
            }
        }
    }

    // Use this for initialization
    void Start ()
    {
        if(IsMusicOn == true)
        {
            BackgroundMusic.GlobalMute = false;
            buttonLabel.text = MusicOnText;
        }
        else
        {
            BackgroundMusic.GlobalMute = true;
            buttonLabel.text = MusicOffText;
        }
    }

    public override bool OnClick()
    {
        IsMusicOn = !IsMusicOn;
        return true;
    }
}
