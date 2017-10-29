using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetActive : MonoBehaviour {
    public GameObject Username;
    public GameObject Password;
    public GameObject Loading;
    public GameObject ButtonLogin;
	// Use this for initialization
	void Start () {
      
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClick(bool isOn)
    {
        Username.SetActive(isOn);
        Password.SetActive(isOn);
        ButtonLogin.SetActive(isOn);
        Loading.SetActive(!isOn);
    }
}
