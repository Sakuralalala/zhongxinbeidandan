using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSettings : MonoBehaviour {

    public void OnStartGame(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }

}
