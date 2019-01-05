using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Collections.Generic;
using PowerUI;
using System.Collections;
using System;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

namespace GangaGame
{
    public class GameInfo
    {
        public const string PLAYER_READY = "IsPlayerReady";
        public const string PLAYER_TEAM = "PlayerTeam";
        public const bool PLAYER_LOADED_LEVEL = false;
    }

    public class MenuBehavior : MonoBehaviourPunCallbacks
    {
        string gameVersion = "0.1";

        private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
        private List<Player> playerListEntries = new List<Player>();

        public TextAsset MenuHTMLFile;
        public TextAsset LobbyHTMLFile;
        public TextAsset RoomHTMLFile;
        public TextAsset SettingsHTMLFile;

        private float timeToStart = 10.0f;
        private float timerToStart = 0.0f;
        private float sendMessageTimer = 0.0f;
        private bool timerActive = false;

        private bool isPlayerReady = false;
        private bool gameStarted = false;
        private int playerTeam = 1;

        AudioSource audioSource;
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (timerActive && !gameStarted)
            {
                timerToStart -= Time.deltaTime;
                sendMessageTimer -= Time.deltaTime;
                if (sendMessageTimer <= 0)
                {
                    AddMessasgeToChat(String.Format("Game will start in {0:F0}", timerToStart));
                    sendMessageTimer = 1.0f;
                }
                if (timerToStart <= 0)
                {
                    AddMessasgeToChat(String.Format("Game started..."));
                    gameStarted = true;

                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    PhotonNetwork.CurrentRoom.IsVisible = false;

                    PhotonNetwork.LoadLevel("Levels/Map1/Map1");
                }
            }
            string className = "";
            var element = PowerUI.CameraPointer.All[0].ActiveOver;
            if (element != null)
                className = element.className;

            if (UnityEngine.Input.GetMouseButtonUp(0) || UnityEngine.Input.GetKeyDown(KeyCode.Return))
            {
                if (className.Contains("singleplayer"))
                {
                    audioSource.Play(0);
                    SceneManager.LoadScene("Levels/Map1/Map1");
                }
                if (className.Contains("multiplayer"))
                {
                    audioSource.Play(0);
                    if (PlayerPrefs.GetString("username") == "")
                        CreateConnectDialog();
                    else
                        Connect(PlayerPrefs.GetString("username"));
                }
                if (className.Contains("menuElement settings"))
                {
                    audioSource.Play(0);
                    UI.document.innerHTML = SettingsHTMLFile.text;
                    UI.document.Run("CreateSettings", "settingsContainer");
                    SetSettings();
                    return;
                }
                if (className.Contains("exit"))
                {
                    audioSource.Play(0);
                    Application.Quit();
                    return;
                }
                else if (className.Contains("ConnectDialog") || UI.document.getElementsByClassName("ConnectDialog").length > 0 && UnityEngine.Input.GetKeyDown(KeyCode.Return))
                {
                    string error = SaveSettings();
                    if (error != "")
                        UI.document.getElementsByClassName("error")[0].innerText = error;
                    else
                    {
                        DeleteDialog();
                        Connect(PlayerPrefs.GetString("username"));
                    }
                }
                else if (className.Contains("backToMenu") && UI.document.getElementsByClassName("settingsBlock").length > 0)
                {
                    UI.document.innerHTML = MenuHTMLFile.text;
                    return;
                }
                else if (className.Contains("saveSettings") && UI.document.getElementsByClassName("saveSettings").length > 0)
                {
                    string error = SaveSettings();
                    if (error != "")
                        UI.document.getElementsByClassName("messageSettings")[0].innerHTML = error;
                    else
                        UI.document.getElementsByClassName("messageSettings")[0].innerHTML = "Settings saved!";
                    return;
                }
                else if (className.Contains("backToMenu") && !PhotonNetwork.InRoom)
                {
                    PhotonNetwork.Disconnect();
                    CreateMessage("Disconnecting...");
                }
                else if (className.Contains("refreshMenu"))
                    UpdateLobbyListView();

                else if (className.Contains("createRoomDialog"))
                    CreateRoomMenu();

                else if (className.Contains("roomInLobby"))
                {
                    CreateMessage(String.Format("Connecting to \"{0}\"...", element.innerText));
                    PhotonNetwork.JoinRoom(element.innerText);
                }

                else if (className.Contains("deleteDialog"))
                    DeleteDialog();

                else if (className.Contains("RoomDialog") || UI.document.getElementsByClassName("inputMaxplayers").length > 0 && UnityEngine.Input.GetKeyDown(KeyCode.Return))
                {
                    string roomName = UI.document.getElementsByClassName("inputName")[0].innerText;
                    string maxplayers = UI.document.getElementsByClassName("inputMaxplayers")[0].innerText;
                    if (!Regex.Match(maxplayers, @"^[2-9][0-9]*$", RegexOptions.IgnoreCase).Success)
                    {
                        UI.document.getElementsByClassName("error")[0].innerText = "Wrong maxplayers value";
                    }
                    else if (roomName.Length <= 0)
                    {
                        UI.document.getElementsByClassName("error")[0].innerText = "Empty room name";
                    }
                    else
                    {
                        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = (byte)int.Parse(maxplayers) }, TypedLobby.Default);
                        DeleteDialog();
                        CreateMessage("Creating...");
                    }
                }
                else if (PhotonNetwork.InRoom && className.Contains("backToMenu"))
                {
                    PhotonNetwork.LeaveRoom();
                    CreateMessage("Leaving...");
                }
                else if (PhotonNetwork.InRoom && (UnityEngine.Input.GetKeyDown(KeyCode.Return) || className.Contains("chatSend")))
                {
                    string chatInput = UI.document.getElementsByClassName("chatInput")[0].getAttribute("value");
                    if (chatInput != null && chatInput.Length > 0)
                    {
                        UI.document.getElementsByClassName("chatInput")[0].setAttribute("value", "");
                        GetComponent<PhotonView>().RPC(
                            "AddMessasgeToChat", PhotonTargets.All, new string[1] { String.Format("{0}: {1}", PhotonNetwork.NickName, chatInput) }
                            );
                    }
                }
                else if (PhotonNetwork.InRoom && className.Contains("setReady"))
                {
                    SetReady(!isPlayerReady);
                }
                else if (PhotonNetwork.InRoom && UI.document.getElementsByAttribute("name", "teamSelect").length > 0)
                {
                    var select = UI.document.getElementsByAttribute("name", "teamSelect")[0];
                    int teamSelect = int.Parse(select.id);
                    if (playerTeam != teamSelect)
                    {
                        SetTeam(teamSelect);
                    }
                }
            }
        }

        public void SetSettings()
        {
            if (UI.document.getElementsByClassName("username").length > 0)
                UI.document.getElementsByClassName("username")[0].innerText = PlayerPrefs.GetString("username");
        }

        public string SaveSettings()
        {
            if (UI.document.getElementsByClassName("username").length > 0)
            {
                string username = UI.document.getElementsByClassName("username")[0].innerText;
                if (username.Length <= 0)
                    return "Wrong username";

                PlayerPrefs.SetString("username", username);
            }
            PlayerPrefs.Save();
            return "";
        }

        void CreateConnectDialog()
        {
            UI.document.Run("CreateConnectDialog", "ConnectDialog");
        }
        void CreateRoomMenu()
        {
            UI.document.Run("CreateRoomDialog", "RoomDialog");
        }
        void DeleteDialog()
        {
            UI.document.getElementsByClassName("crateRoomForm")[0].remove();
        }
        void CreateMessage(string message)
        {
            UI.document.Run("CreateMessage", new string[1] { message });
        }
        void CreateErrorMessage(string message)
        {
            if (UI.document.getElementsByClassName("crateRoomForm").length > 0)
                UI.document.getElementsByClassName("crateRoomForm")[0].remove();

            UI.document.Run("CreateErrorMessage", new string[1] { message });
        }
        void AddInfo(string info)
        {
            UI.document.Run("AddInfo", new string[1] { info });
        }
        void CreateUser(string username, int playerTeam, bool playerReady, bool canEdit)
        {
            if (UI.document.getElementsByClassName("players").length <= 0)
                return;
            
            UI.document.Run("CreateUser", username, playerTeam, playerReady, canEdit);
            //var select = UI.document.getElementsByAttribute("name", "teamSelect")[0];
            // select.childNodes[playerTeam - 1].setAttribute("selected", "selected");
        }
        [PunRPC]
        public void AddMessasgeToChat(string message)
        {
            if (UI.document.getElementsByClassName("chat").length <= 0)
                return;

            UI.document.Run("CreateChatMessage", new string[1] { message });
        }
        
        private void UpdateLobbyListView()
        {
            var menuBlock = UI.document.getElementsByClassName("menu")[0];
            menuBlock.innerText = "";

            foreach (RoomInfo info in cachedRoomList.Values)
            {
                var roomBlock = UI.document.createElement("div");
                roomBlock.className = "menuElement roomInLobby";
                roomBlock.innerText = info.Name;
                menuBlock.appendChild(roomBlock);
            }

        }

        public override void OnConnectedToMaster()
        {
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            UpdateCachedRoomList(roomList);
            UpdateLobbyListView();
        }
        public override void OnJoinedLobby()
        {
            UI.document.innerHTML = LobbyHTMLFile.text;
        }
        public override void OnLeftLobby()
        {
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            gameStarted = false;
            UI.document.innerHTML = MenuHTMLFile.text;
        }
        
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            UpdateRoomView();
        }

        public override void OnJoinedRoom()
        {
            isPlayerReady = false;
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                playerListEntries.Add(p);
            }
            UI.document.innerHTML = RoomHTMLFile.text;
            SetReady(false);
            UpdateRoomView();

            //Hashtable props = new Hashtable
            //{
            //    {AsteroidsGame.PLAYER_LOADED_LEVEL, false}
            //};
            //PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public override void OnLeftRoom()
        {
            gameStarted = false;
            playerListEntries.Clear();

            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
            UI.document.innerHTML = LobbyHTMLFile.text;
            UpdateLobbyListView();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            AddMessasgeToChat(string.Format("{0} connected to room", newPlayer.NickName));
            playerListEntries.Add(newPlayer);
            UpdateRoomView();
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            AddMessasgeToChat(string.Format("{0} disconnected from room", otherPlayer.NickName));
            playerListEntries.Remove(otherPlayer);
            UpdateRoomView();
        }
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("OnCreateRoomFailed: " + message);
            CreateErrorMessage(String.Format("Romm create failed: {0} ({1})", message, returnCode.ToString()));
        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRoomFailed: " + message);
            CreateErrorMessage(String.Format("Join failed: {0} ({1})", message, returnCode.ToString()));
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRandomFailed: " + message);
            CreateErrorMessage(String.Format("Join failed: {0} ({1})", message, returnCode.ToString()));
        }
        public override void OnCustomAuthenticationFailed(string debugMessage)
        {
            Debug.Log("OnCustomAuthenticationFailed: " + debugMessage);
            CreateErrorMessage(String.Format("Authentication failed: {0}", debugMessage));
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            UpdateRoomView();
        }

        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            Debug.Log("UpdateCachedRoomList: " + roomList.Count);
            foreach (RoomInfo roomInfo in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!roomInfo.IsOpen || !roomInfo.IsVisible || roomInfo.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(roomInfo.Name))
                    {
                        cachedRoomList.Remove(roomInfo.Name);
                    }
                    continue;
                }

                // Update cached room info
                if (cachedRoomList.ContainsKey(roomInfo.Name))
                {
                    cachedRoomList[roomInfo.Name] = roomInfo;
                }
                // Add new room info to cache
                else
                {
                    cachedRoomList.Add(roomInfo.Name, roomInfo);
                }
            }
        }

        private void UpdateRoomView()
        {
            if (UI.document.getElementsByClassName("players").length <= 0)
                return;
            else
                UI.document.getElementsByClassName("players")[0].innerHTML = "";

            bool allPlayersIsReady = true;
            foreach (Player player in playerListEntries)
            {
                object playerReadyObd;
                bool playerReady = false;
                if (player.CustomProperties.TryGetValue(GameInfo.PLAYER_READY, out playerReadyObd))
                    playerReady = (bool)playerReadyObd;

                object playerTeamObd;
                int playerTeam = 1;
                if (player.CustomProperties.TryGetValue(GameInfo.PLAYER_TEAM, out playerTeamObd))
                    playerTeam = (int)playerTeamObd;

                bool canEdit = false;
                if (player == PhotonNetwork.LocalPlayer)
                    canEdit = true;

                CreateUser(player.NickName, playerTeam, playerReady, canEdit);

                if (!playerReady)
                    allPlayersIsReady = false;
            }
            if (allPlayersIsReady)
            {
                timerToStart = timeToStart;
                AddMessasgeToChat(String.Format("All players is ready, game will start in {0:F0} seconds", timerToStart));
                sendMessageTimer = 0.0f;
                timerActive = true;
            }
            else if(timerActive)
            {
                AddMessasgeToChat(String.Format("Timer is stopped"));
                timerActive = false;
            }

            if (UI.document.getElementsByClassName("roomInfo").length <= 0)
                return;
            else
                UI.document.getElementsByClassName("roomInfo")[0].innerHTML = "";

            AddInfo(String.Format("Room name: {0}", PhotonNetwork.CurrentRoom.Name));
            AddInfo(String.Format("Max players: {0}", PhotonNetwork.CurrentRoom.MaxPlayers));
        }

        void SetReady(bool newState)
        {
            isPlayerReady = newState;
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() {
                { GameInfo.PLAYER_READY, isPlayerReady }//,  { GameInfo.PLAYER_LOADED_LEVEL, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        void SetTeam(int newTeam)
        {
            playerTeam = newTeam;
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() {
                { GameInfo.PLAYER_TEAM, playerTeam}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        
        public void Connect(string userName)
        {
            if (PhotonNetwork.IsConnected)
            {
                OnConnectedToMaster();
            }
            else
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.NickName = userName;
                PhotonNetwork.ConnectUsingSettings();
                CreateMessage("Connecting...");
            }
        }

        private bool CheckPlayersReady()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                object playerReady;
                if (player.CustomProperties.TryGetValue(GameInfo.PLAYER_READY, out playerReady))
                {
                    if (!(bool)playerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}