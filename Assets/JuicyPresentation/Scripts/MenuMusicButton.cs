using UnityEngine;
using System.Collections;

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
            AudioSource backgroundMusic = GlobalGameObject.Get<AudioSource>();
            if(value == true)
            {
                PlayerPrefs.SetInt(MusicKey, 1);
                backgroundMusic.mute = false;
                buttonLabel.text = MusicOnText;
            }
            else
            {
                PlayerPrefs.SetInt(MusicKey, 0);
                backgroundMusic.mute = true;
                buttonLabel.text = MusicOffText;
            }
        }
    }

    // Use this for initialization
    void Start ()
    {
        AudioSource backgroundMusic = GlobalGameObject.Get<AudioSource>();
        if(IsMusicOn == true)
        {
            backgroundMusic.mute = false;
            buttonLabel.text = MusicOnText;
        }
        else
        {
            backgroundMusic.mute = true;
            buttonLabel.text = MusicOffText;
        }
    }

    public override bool OnClick()
    {
        IsMusicOn = !IsMusicOn;
        return true;
    }
}
