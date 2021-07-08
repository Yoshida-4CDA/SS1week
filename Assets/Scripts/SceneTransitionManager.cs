using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public void LoaeTo(string sceneName)
    {
        // SoundManager.instance.PlayBGM(sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
