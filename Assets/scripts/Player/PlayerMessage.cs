using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMessage : NetworkBehaviour {
	public TextMesh PlayerNameText;

	[SyncVar(hook = "OnSyncNameChanged")]
	public string Name = "";

	public void OnSyncNameChanged(string name){
		Debug.Log ("SyncVar playerName changed to " + name);
		PlayerNameText.text = name;
	}

	void Start(){
		Invoke ("PatchChangeName",1);
	}

	void PatchChangeName(){
		PlayerNameText.text = Name;
	}
}
