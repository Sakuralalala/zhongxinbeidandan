using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsEnter : MonoBehaviour {

    public void OnSettings(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }

}
