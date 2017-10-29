using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using Player;

public class Rocker : MonoBehaviour {
	public Transform rockerBar;
	public Transform origin;
	public int radius;
	public CameraController cameraController;

	public void FixedUpdate(){
		Vector2 dir = getRockerValue ();
		if (dir.sqrMagnitude != 0) {
//			LobbyManager.localPlayer.GetComponent<PlayerInput> ().OnRockerMoved (dir);			
			cameraController.move (dir);
		}
	}

	public Vector2 getRockerValue(){
		return (rockerBar.position - origin.position) / radius;
	}
}
