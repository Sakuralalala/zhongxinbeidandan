using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Prototype.NetworkLobby;
using System.Net.NetworkInformation;

public class TestPlayer : MonoBehaviour {
	public GameObject player;
	private PlayerInput playerInput;
	public List<GameObject> allPlanets;
	public static TestPlayer instance;

	public static GameObject getStar(int ID){
		return instance.allPlanets [ID];
	}

	void OnEnable(){
		instance = this;
		GameObject stars = GameObject.Find ("DebugStars");
		allPlanets = new List<GameObject> ();
		for (int i = 0; i < stars.transform.childCount; i++) {
			allPlanets.Add (stars.transform.GetChild (i).gameObject);
		}

	}
	void Update(){
		if(!playerInput)
		if(LobbyManager.localPlayer)
			playerInput = LobbyManager.localPlayer.GetComponent<PlayerInput> ();		
		bool protect = Input.GetKeyDown ("p");
		int[] planets = new int[7]{ 0, 1, 2, 3, 4, 5, 6 };
		if (protect) 
			playerInput.OnProtectionClick (planets);		
	}
}
