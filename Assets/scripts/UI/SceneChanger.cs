using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour {

    public void OnStartGame(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }

}
