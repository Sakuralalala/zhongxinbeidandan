using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.NetworkLobby
{
    public class LobbyPlayer : NetworkLobbyPlayer
    {
		public int PlayerNum = 1;
		public string Name;
		//DEBUG
		void Awake(){
			InputField num = GameObject.Find ("DebugPlayerNum").GetComponent<InputField> ();
			PlayerNum = int.Parse (num.text);
		}
        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();

			
            LobbyPlayerList._instance.AddPlayer(this);
			Debug.Log ("add player");
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
        }

		void Start(){
			StartCoroutine (waitMatchPlayers ());
//			if (isLocalPlayer) {
//				Name = LobbyManager.s_Singleton.localPlayerName;
//				CmdClientUpdateName (Name);
//			}
		}

		[Command]
		public void CmdClientUpdateName(string name){
			RpcServerUpdateName (name);
		}

		[ClientRpc]
		public void RpcServerUpdateName(string name){
			Name = name;
		}

		public IEnumerator waitMatchPlayers(){
			LobbyPlayerList playerList = LobbyPlayerList._instance;
			Vector3 rotation = new Vector3 (0, 0, 1);
			while(playerList.getPlayerNum ()<PlayerNum){
				yield return 0;
				rotation += new Vector3 (0, 0, 1);
			}
			if (isLocalPlayer) {
				Name = LobbyManager.s_Singleton.localPlayerName;
				CmdClientUpdateName (Name);
				Debug.Log ("cmdchangename");
			}
			SendReadyToBeginMessage ();
		}
    }
}
