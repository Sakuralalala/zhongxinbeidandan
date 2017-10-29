using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Switch : MonoBehaviour {
    public GameObject isOnGameObject;
    public GameObject isOffGameObject;
    private Toggle toggle;

    // Use this for initialization
    void Start () {
        toggle = GetComponent<Toggle>();
        
    }
	
	// Update is called once per frame
	void Update () {
        OnValueChange(toggle.isOn);
    }

    public void OnValueChange(bool isOn)
    {
        isOnGameObject.SetActive(isOn);
        isOffGameObject.SetActive(!isOn);
    }
}
