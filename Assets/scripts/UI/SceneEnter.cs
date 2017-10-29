using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEnter : MonoBehaviour {

	// Use this for initialization
public void OnEnter(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }
}
