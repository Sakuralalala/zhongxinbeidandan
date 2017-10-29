using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;


namespace Prototype.NetworkLobby
{
    public class LobbyManager : NetworkLobbyManager 
    {

        static public LobbyManager s_Singleton;
		static public GameObject localPlayer;
		public string localPlayerName;
		public LogPanel logPanel;
		public LoadingNum loadingNum;


        [Header("Unity UI Lobby")]
        [Tooltip("Time in second between all players ready & match start")]
        public float prematchCountdown = 5.0f;

        public LobbyCountdownPanel countdownPanel;
        protected RectTransform currentPanel;

        //Client numPlayers from NetworkManager is always 0, so we count (throught connect/destroy in LobbyPlayer) the number
        //of players, so that even client know how many player there is.
        [HideInInspector]
        public int _playerNumber = 0;

        //used to disconnect a client properly when exiting the matchmaker
        [HideInInspector]
        public bool _isMatchmaking = false;

        protected bool _disconnectServer = false;
        
        protected ulong _currentMatchID;

        protected LobbyHook _lobbyHooks;

        void Start()
        {
            s_Singleton = this;
            _lobbyHooks = GetComponent<Prototype.NetworkLobby.LobbyHook>();

            DontDestroyOnLoad(gameObject);
        }

        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
			Debug.Log ("scene");
//			logPanel.gameObject.SetActive (false);
        }

		public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
		{
			loadingNum.countFromAndTo (60,100);
			base.OnMatchCreate(success, extendedInfo, matchInfo);
            _currentMatchID = (System.UInt64)matchInfo.networkId;
			logPanel.OnMatching ();
		}

		public override void OnMatchJoined (bool success, string extendedInfo, MatchInfo matchInfo){
			loadingNum.countFromAndTo (60,100);
			base.OnMatchJoined (success,extendedInfo,matchInfo);
			logPanel.OnMatching ();
		}

		public override void OnDestroyMatch(bool success, string extendedInfo)
		{
			base.OnDestroyMatch(success, extendedInfo);
			if (_disconnectServer)
            {
                StopMatchMaker();
                StopHost();
            }
        }

        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
		{
			string name = lobbyPlayer.GetComponent<LobbyPlayer> ().Name;
			Debug.Log ("OnLobbyServerSceneLoadedForPlayer,get lobby player name is "+name);
			gamePlayer.GetComponent<PlayerMessage> ().Name = name;
            return true;
        }

    }
}
