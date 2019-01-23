using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Collections.Generic;
using PowerUI;
using System.Collections;
using System;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using System.Linq;

namespace GangaGame
{
    public static class GameInfo
    {
        public const string PLAYER_READY = "IsPlayerReady";
        public const string PLAYER_TEAM = "PlayerTeam";
        public const string PLAYER_LOADED_LEVEL = "LoadedLevel";
        public const string MAP_SEED = "MapSeed";
        public const string NPC_INFO = "NPCInfo";
        public const string PLAYER_SPECTATOR = "PlayerSpectator";
        public const string MAP_SIZE = "MapSize";

        public static bool isPlayerReady = false;
        public static int playerTeam = 1;
        public static bool playerSpectate = false;
        public static List<int> NPCList = new List<int>();

        public const int defaultMapSize = 3;
        public static int mapSize = defaultMapSize;

        public static bool IsMasterClient()
        {
            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    return true;
                return false;
            }
            else
                return true;
        }
        
        public static void SetReady(bool newState)
        {
            isPlayerReady = newState;
            if (PhotonNetwork.InRoom)
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { GameInfo.PLAYER_READY, isPlayerReady } });
        }
        public static void SetTeam(int newTeam)
        {
            playerTeam = newTeam;
            if (PhotonNetwork.InRoom)
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { GameInfo.PLAYER_TEAM, playerTeam } });
        }
        public static void SetSpectator(bool newState)
        {
            playerSpectate = newState;
            if (PhotonNetwork.InRoom)
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { GameInfo.PLAYER_SPECTATOR, playerSpectate } });
        }
        public static void SetMapSize(int newMapSize)
        {
            mapSize = newMapSize;
            if (PhotonNetwork.InRoom)
                PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { GameInfo.MAP_SIZE, mapSize } });
        }
        public static List<int> GetNPCInfo()
        {
            if (PhotonNetwork.InRoom)
            {
                List<int> roomNPCInfo = new List<int>();
                object oldNPCListObj;
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameInfo.NPC_INFO, out oldNPCListObj))
                {
                    foreach (Match match in new Regex(@"\((\d+)\)").Matches((string)oldNPCListObj))
                    {
                        var regex = new Regex(@"(\d+)").Match(match.Value);
                        int NPCTeam = int.Parse(regex.Groups[1].Value);
                        roomNPCInfo.Add(NPCTeam);
                    }
                }
                return roomNPCInfo;
            }
            else
                return NPCList;
        }
        public static void UpdateNPCInfo(List<int> newNPCList)
        {
            string newNPCListAsString = "";
            foreach(int NPCTeam in newNPCList)
                newNPCListAsString += String.Format("({0})", NPCTeam);

            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameInfo.NPC_INFO, newNPCListAsString } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }

        public static void ChangeNPCTeam(int index, int newTeam)
        {
            if (PhotonNetwork.InRoom)
            {
                List<int> oldNPCList = GetNPCInfo();
                oldNPCList[index] = newTeam;
                UpdateNPCInfo(oldNPCList);
            }
            NPCList[index] = newTeam;
        }
        public static void CreateNPC(int team)
        {
            if (PhotonNetwork.InRoom)
            {
                List<int> oldNPCList = GetNPCInfo();
                oldNPCList.Add(team);
                UpdateNPCInfo(oldNPCList);
            }
            NPCList.Add(team);
        }
        public static void DeleteNPC(int index)
        {
            if (PhotonNetwork.InRoom)
            {
                List<int> oldNPCList = GetNPCInfo();
                oldNPCList.RemoveAt(index);
                UpdateNPCInfo(oldNPCList);
            }
            NPCList.RemoveAt(index);
        }
        public static string GetNickName()
        {
            if (PhotonNetwork.IsConnected)
                return PhotonNetwork.LocalPlayer.NickName;

            string username = PlayerPrefs.GetString("username");
            if (username == "")
                username = "Player";
            return username;
        }
    }

    public class MenuBehavior : MonoBehaviourPunCallbacks
    {
        string gameVersion = "0.2";

        private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
        private List<Player> playerListEntries = new List<Player>();

        public TextAsset MenuHTMLFile;
        public TextAsset LobbyHTMLFile;
        public TextAsset RoomHTMLFile;
        public TextAsset SettingsHTMLFile;

        public AudioClip menuMusic;
        public float musicDelay = 0.0f;

        public AudioClip clickSound;

        private float timeToStart = 10.0f;
        private float timerToStart = 0.0f;
        private float sendMessageTimer = 0.0f;
        private bool timerActive = false;

        private bool gameStarted = false;

        AudioSource musicSource;
        AudioSource soundsSource;
        private void Start()
        {
            musicSource = GetComponent<AudioSource>();
            soundsSource = GetComponentInChildren<AudioSource>();

            musicSource.clip = menuMusic;
            musicSource.volume = PlayerPrefs.GetFloat("musicMenuVolume");
            musicSource.PlayDelayed(musicDelay);
        }

        private string loadedLevel = "";
        private bool singleplayer = true;
        private float timerToLoad = 1.0f;

        private void Update()
        {
            if(loadedLevel != "")
            {
                timerToLoad -= Time.fixedDeltaTime;
                if(timerToLoad <= 0)
                {
                    musicSource.Stop();
                    if (singleplayer)
                        SceneManager.LoadScene(loadedLevel);
                    else
                        PhotonNetwork.LoadLevel(loadedLevel);
                    loadedLevel = "";
                }
                return;
            }

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

                    if (PhotonNetwork.InRoom)
                    {
                        PhotonNetwork.CurrentRoom.IsOpen = false;
                        PhotonNetwork.CurrentRoom.IsVisible = false;
                    }

                    UI.document.Run("CreateLoadingScreen", "Level initialization");
                    loadedLevel = "Levels/Map1/Map1";
                    return;
                }
            }
            string className = "";
            var element = PowerUI.CameraPointer.All[0].ActiveOver;
            if (element != null)
                className = element.className;

            if (UnityEngine.Input.GetMouseButtonUp(0) || UnityEngine.Input.GetKeyDown(KeyCode.Return))
            {
                bool changed = SettingsScript.ChangeTabOrSaveSettings(className, windowSettings: "settingsContainer", saveClassName: "saveSettings", errorClassName: "messageSettings", mainMenu: true);
                if (changed)
                {
                    musicSource.volume = PlayerPrefs.GetFloat("musicMenuVolume");
                    return;
                }

                if (className.Contains("singleplayer"))
                {
                    singleplayer = true;
                    soundsSource.PlayOneShot(clickSound, PlayerPrefs.GetFloat("interfaceVolume"));
                    GameInfo.playerSpectate = false;
                    GameInfo.NPCList.Clear();
                    UI.document.innerHTML = RoomHTMLFile.text;
                    GameInfo.SetReady(false);
                    GameInfo.mapSize = GameInfo.defaultMapSize;
                    UpdateRoomView();
                    return;
                }
                if (className.Contains("multiplayer"))
                {
                    singleplayer = false;
                    soundsSource.PlayOneShot(clickSound, PlayerPrefs.GetFloat("interfaceVolume"));
                    if (PlayerPrefs.GetString("username") == "")
                        CreateConnectDialog();
                    else
                        Connect(PlayerPrefs.GetString("username"));
                }
                if (className.Contains("menuElement settings"))
                {
                    soundsSource.PlayOneShot(clickSound, PlayerPrefs.GetFloat("interfaceVolume"));
                    UI.document.innerHTML = SettingsHTMLFile.text;
                    SettingsScript.CreateSettings("settingsContainer", true);
                    return;
                }
                if (className.Contains("exit"))
                {
                    soundsSource.PlayOneShot(clickSound, PlayerPrefs.GetFloat("interfaceVolume"));
                    Application.Quit();
                    return;
                }
                else if (className.Contains("ConnectDialog") || UI.document.getElementsByClassName("ConnectDialog").length > 0 && UnityEngine.Input.GetKeyDown(KeyCode.Return))
                {
                    string error = SettingsScript.SaveSettings();
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

                // Singleplayer
                else if (singleplayer && className.Contains("backToMenu"))
                {
                    gameStarted = false;
                    UI.document.innerHTML = MenuHTMLFile.text;
                }
                else if (singleplayer && className.Contains("setReady"))
                {
                    GameInfo.SetReady(!GameInfo.isPlayerReady);
                    UpdateRoomView();
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
                        DeleteDialog();
                        CreateMessage("Creating...");
                        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = (byte)int.Parse(maxplayers) }, TypedLobby.Default);
                    }
                }

                else if ((PhotonNetwork.InRoom || singleplayer) && className.Contains("backToMenu"))
                {
                    timerActive = false;
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
                    GameInfo.SetReady(!GameInfo.isPlayerReady);
                }

                // Create NPC
                else if (GameInfo.IsMasterClient() && className.Contains("CreateNPC"))
                {
                    if (GameInfo.NPCList.Count >= 9)
                    {
                        UI.document.Run("CreateChatMessage", "You can not create more bots than 9");
                        return;
                    }
                    else
                    {
                        GameInfo.CreateNPC(GameInfo.NPCList.Count + 1);
                        if (singleplayer)
                            UpdateRoomView();
                        return;
                    }
                }
                // Kick
                else if (GameInfo.IsMasterClient() && className.Contains("kick"))
                {
                    if (element.parentElement.className.Contains("NPC"))
                    {
                        int index = int.Parse(new Regex(@"NPCINDEX(\d+)").Match(element.className).Groups[1].Value);
                        GameInfo.DeleteNPC(index);
                        if (singleplayer)
                            UpdateRoomView();
                        return;
                    }
                    else
                    {
                        string kickUsername = element.parentElement.textContent;
                        AddMessasgeToChat(String.Format("{0} kicked user {1}", PhotonNetwork.LocalPlayer.NickName, kickUsername));
                        GetComponent<PhotonView>().RPC("KickPlayer", PhotonTargets.All, kickUsername);
                        return;
                    }
                }
                if (className.Contains("spectator") && element.parentElement.innerText.Contains(GameInfo.GetNickName()))
                {
                    GameInfo.SetSpectator(!GameInfo.playerSpectate);
                    if (singleplayer)
                        UpdateRoomView();
                    return;
                }

                // Change map size
                var mapElements = UI.document.getElementsByClassName("MapSize");
                if (mapElements.length > 0)
                {
                    int newSize = int.Parse(mapElements[0].id);
                    if (newSize != GameInfo.mapSize)
                    {
                        GameInfo.SetMapSize(newSize);
                        if (singleplayer)
                            UpdateRoomView();
                        return;
                    }
                }

                // Change team
                if (UI.document.getElementsByAttribute("name", "teamSelect").length > 0)
                {
                    foreach (Dom.Element selectElement in UI.document.getElementsByAttribute("name", "teamSelect"))
                    {
                        int teamSelect = int.Parse(selectElement.id);
                        if (selectElement.className.Contains(GameInfo.GetNickName()))
                        {
                            if (GameInfo.playerTeam != teamSelect)
                            {
                                GameInfo.SetTeam(teamSelect);
                            }
                        }
                        else if (GameInfo.IsMasterClient() && selectElement.parentElement.className.Contains("NPC"))
                        {
                            // Debug.Log("NPC (" + selectElement.parentElement.className + "): " + teamSelect);
                            int index = int.Parse(new Regex(@"NPCINDEX(\d+)").Match(selectElement.parentElement.className).Groups[1].Value);
                            if (GameInfo.NPCList[index] != teamSelect)
                                GameInfo.ChangeNPCTeam(index, teamSelect);
                        }
                    }
                }
            }
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
        void CreateUser(string username, int playerTeam, bool playerReady, bool canEdit, bool canKick = false, int value = 0, string className = "", bool canChangeSpectate = false, bool spectateActive = false)
        {
            if (UI.document.getElementsByClassName("players").length <= 0)
                return;
            
            UI.document.Run("CreateUser", username, playerTeam, playerReady, canEdit, canKick, value, className, canChangeSpectate, spectateActive);
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
        [PunRPC]
        public void KickPlayer(string kickUsername)
        {
            if (PhotonNetwork.LocalPlayer.NickName == kickUsername)
            {
                PhotonNetwork.LeaveRoom();
                CreateMessage("Leaving...");
            }
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
            Debug.Log("UpdateLobbyListView: " + cachedRoomList.Count);
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
            GameInfo.isPlayerReady = false;
            GameInfo.playerSpectate = false;
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                playerListEntries.Add(p);
            }
            UI.document.innerHTML = RoomHTMLFile.text;
            GameInfo.SetReady(false);
            UpdateRoomView();

            if (GameInfo.IsMasterClient())
            {
                GameInfo.mapSize = GameInfo.defaultMapSize;
                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() {
                    { GameInfo.MAP_SEED, UnityEngine.Random.Range(0, 1000) }, { GameInfo.MAP_SIZE, GameInfo.mapSize }
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }

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

            List<int> roomNPCList = GameInfo.GetNPCInfo();
            bool allPlayersIsReady = true;
            string masterClientNickname = "";
            if (PhotonNetwork.InRoom)
            {
                object mapSizeObd;
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameInfo.MAP_SIZE, out mapSizeObd))
                    GameInfo.mapSize = (int)mapSizeObd;

                foreach (Player user in playerListEntries)
                {
                    object userReadyObd;
                    bool userReady = false;
                    if (user.CustomProperties.TryGetValue(GameInfo.PLAYER_READY, out userReadyObd))
                        userReady = (bool)userReadyObd;

                    object userTeamObd;
                    int userTeam = 1;
                    if (user.CustomProperties.TryGetValue(GameInfo.PLAYER_TEAM, out userTeamObd))
                        userTeam = (int)userTeamObd;

                    object userSpectateObd;
                    bool userSpectate = false;
                    if (user.CustomProperties.TryGetValue(GameInfo.PLAYER_SPECTATOR, out userSpectateObd))
                        userSpectate = (bool)userSpectateObd;

                    bool canEdit = false;
                    if (user == PhotonNetwork.LocalPlayer)
                        canEdit = true;

                    CreateUser(user.NickName, userTeam, userReady, canEdit, canKick: GameInfo.IsMasterClient() && !canEdit, canChangeSpectate: canEdit, spectateActive: userSpectate);

                    if (!userReady)
                        allPlayersIsReady = false;

                    if (user.IsMasterClient)
                        masterClientNickname = user.NickName;
                }
            }
            else
            {
                masterClientNickname = GameInfo.GetNickName();
                allPlayersIsReady = GameInfo.isPlayerReady;
                CreateUser(GameInfo.GetNickName(), GameInfo.playerTeam, GameInfo.isPlayerReady, canEdit: true, canChangeSpectate: true, spectateActive: GameInfo.playerSpectate);
            }
            int index = 0;
            foreach (int NPCTeam in roomNPCList)
            {
                index++;
                CreateUser(
                    String.Format("NPC: {0}", index), NPCTeam, true, canEdit: GameInfo.IsMasterClient(), 
                    canKick: GameInfo.IsMasterClient(), value: index - 1, className: "NPC",
                    canChangeSpectate: false, spectateActive: false
                    );
            }

            if (GameInfo.IsMasterClient())
            {
                UI.document.Run("CreateAddButton");
            }
            if (allPlayersIsReady)
            {
                if (PhotonNetwork.InRoom)
                    timerToStart = timeToStart;
                else
                    timerToStart = 3.0f;

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

            if (PhotonNetwork.InRoom)
            {
                AddInfo(String.Format("Room name: {0}", PhotonNetwork.CurrentRoom.Name));
                AddInfo(String.Format("Room owner: {0}", masterClientNickname));
                AddInfo(String.Format("Max players: {0}", PhotonNetwork.CurrentRoom.MaxPlayers));
            }
            AddInfo(String.Format("NPC count: {0}", roomNPCList.Count));
            
            List<string> mapNames = new List<string>(new string[] { "Very small", "Small", "Medium", "Big", "Very big" });
            if (GameInfo.IsMasterClient())
            {
                List<string> optionsArgs = new List<string>(new string[] { "Map size:", "MapSize", GameInfo.mapSize.ToString() });
                UI.document.Run("AddSelectOption", optionsArgs.Concat(mapNames).ToArray());
            }
            else
            {
                AddInfo(String.Format("Map size: {0}", mapNames[GameInfo.mapSize]));
            }
        }

        void Awake()
        {
            SettingsScript.SetDefaultSettingsIfNotSetted();
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