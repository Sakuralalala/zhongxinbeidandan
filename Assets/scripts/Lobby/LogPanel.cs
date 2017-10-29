using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;
using MySocket;
using System.Net.Sockets;
using Prototype.NetworkLobby;
using UnityEngineInternal;
using System.Threading;

public class LogPanel : MonoBehaviour {
	public RectTransform UsernameInput;
	public RectTransform PasswordInput;
	public LoadingNum loadingNum;

	public LobbyManager lobbyManager;
	public RectTransform logError;
	public RectTransform alreadyLogin;
	public RectTransform matching;
	public RectTransform linking;

	private ClientSocket client;
	private string serverIp;
	private string response = "";
	void Start(){
		serverIp = "138.68.18.64";
	}

	public void OnClickLogin(){
		//==========================================================================================================
		// 连接的过程一共分为三步
		// 1. 向服务器发送登录请求，直到获得回应  																							----------------------------------10%
		// 2. 获得服务器回应后，（若需要）获取房间列表，直到得到房间列表											----------------------------------30%
		// 3. 获取房间列表后，新建房间或者加入房间，直到进入房间															----------------------------------60%
		// 4. 等待玩家准备，直到开始游戏																												----------------------------------100%
		StartCoroutine (tryLogin ());
	}

	public IEnumerator tryLogin(){
		loadingNum.countFromAndTo (0,10);
		client = new ClientSocket ();
		Thread connect = new Thread(Connect);
		connect.Start ();
		while(!client.IsConnected){
			yield return 0;
		}

		ChangeTipTo (linking);
		Debug.Log ("successfully connect server");
		string username = UsernameInput.GetComponent<InputField> ().text;
		LobbyManager.s_Singleton.localPlayerName = username;
		string password = PasswordInput.GetComponent <InputField> ().text;
		client.SendMessage (new Item (username,password,"online").formatRecord ());
		Debug.Log ("request to log in");
		yield return 0;

		Thread getResponse = new Thread (GetResponse);
		getResponse.Start ();
		while (response.Equals ("")) {
			yield return 0;
		}
		Debug.Log ("get response from server");

		if (response.Contains ("success")) {
//		if(true){
			OnStartMatch ();
		} else if (response.Contains ("fail")) {
			ChangeTipTo (logError);
			StartCoroutine (backtoLog ());
		} else {
			ChangeTipTo (alreadyLogin);
			StartCoroutine (backtoLog ());
		}
	}

	private void Connect(){
		client.ConnectServer (serverIp,8088);
	}

	private void GetResponse(){
		response = client.ReceiveMessage ();
	}	

	public void OnStartMatch(){
		loadingNum.countFromAndTo (10,30);
		lobbyManager.StartMatchMaker ();
		lobbyManager.matchMaker.ListMatches (0,6,"",true,0,0,OnListGet);
	}

	private void OnListGet(bool success, string extendedInfo, List<MatchInfoSnapshot> matches){
		loadingNum.countFromAndTo (30,60);
		if (matches.Count == 0) {
			lobbyManager.matchMaker.CreateMatch(
				"game",
				(uint)lobbyManager.maxPlayers,
				true,
				"", "", "", 0, 0,
				lobbyManager.OnMatchCreate);
			lobbyManager._isMatchmaking = true;				
		} else {
			foreach (MatchInfoSnapshot room in matches)
				if (room.currentSize < room.maxSize) {
					lobbyManager.matchMaker.JoinMatch(room.networkId, "", "", "", 0, 0, lobbyManager.OnMatchJoined);
					lobbyManager._isMatchmaking = true;
				}
		}
	}

	private RectTransform current;
	private void ChangeTipTo(RectTransform target){
		if (current)
			current.gameObject.SetActive (false);
		current = target;
		if(current)
			current.gameObject.SetActive (true);
	}

	public void OnMatching(){
			ChangeTipTo (matching);
	}

	private IEnumerator backtoLog(){
		float time = 0;
		while(time<4){
			time += Time.deltaTime;
			yield return 0;
		}
		ChangeTipTo (null);
		GetComponent <SetActive>().OnClick (true);
	}
}
