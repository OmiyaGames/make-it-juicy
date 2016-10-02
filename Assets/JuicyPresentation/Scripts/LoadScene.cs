using UnityEngine;
using System.Collections;

public class LoadScene : MonoBehaviour
{
    public string nextSceneToLoad = "";

    public void LoadNextScene()
    {
        Application.LoadLevelAsync(nextSceneToLoad);
    }
}
