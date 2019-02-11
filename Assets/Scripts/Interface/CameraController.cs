using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PowerUI;
using Photon.Pun;
using Photon.Realtime;
using UISpace;
using GangaGame;
using CI.QuickSave;
using UnityEngine.SceneManagement;

namespace GangaGame
{
    public class CameraController : MonoBehaviourPunCallbacks
    {
        [Header("Resources")]
        public Dictionary<BaseBehavior.ResourceType, float> resources = new Dictionary<BaseBehavior.ResourceType, float>();

        public enum MusicType { None, Normal };
        [System.Serializable]
        public class SoundInfo
        {
            public MusicType type = MusicType.None;
            public List<AudioClip> soundList = new List<AudioClip>();
            public int lastSoundIndex = -1;
        }
        [Header("Sounds Info")]
        public SoundInfo[] soundsInfo;
        public AudioClip clickSound;
        AudioSource audioSource;
        public AudioSource interfaceSource;

        [Header("Select info")]
        public bool isSelecting = false;
        private Vector3 mousePosition1;

        public int team = 1;
        public string userId = "123";
        private int userNumber = 0;
        [HideInInspector]
        private int workersIterate = 0;

        [System.Serializable]
        public class TagToSelect
        {
            public string name;
            public bool healthVisibleOnlyWhenSelect;
        }
        public List<TagToSelect> tagsToSelect = new List<TagToSelect>();

        static Texture2D _whiteTexture;
        public static Texture2D WhiteTexture
        {
            get
            {
                if (_whiteTexture == null)
                {
                    _whiteTexture = new Texture2D(1, 1);
                    _whiteTexture.SetPixel(0, 0, Color.white);
                    _whiteTexture.Apply();
                }

                return _whiteTexture;
            }
        }

        [HideInInspector]
        public GameObject buildedObject;
        [HideInInspector]
        private GameObject selectedObject;

        [System.Serializable]
        public class SpawnUnitInfo
        {
            public string prefabName;
            public int number;
            public float distance = 3.0f;
        }
        [Header("Spawn info")]
        public string createSpawnBuildingName;
        public List<SpawnUnitInfo> startUnitsInfo = new List<SpawnUnitInfo>();

        private float clickTimer = 0.0f;

        [Header("Map info")]
        public Texture mapTexture;

        Dictionary<string, List<Vector3>> spawnData;
        static bool[,] chanksView = new bool[0, 0];
        float chunkSize = 11.0f;
        public bool debugVisionGrid = false;
        [HideInInspector]
        public int uniqueIdIndex = 0;

        [Header("Effects")]
        public GameObject rangeProjector;

        // Stuff
        [HideInInspector]
        public List<GameObject> selectedObjects = new List<GameObject>();

        [HideInInspector]
        public Dictionary<KeyCode, List<GameObject>> unitsBinds = new Dictionary<KeyCode, List<GameObject>>();
        private KeyCode tempKey;
        private float keyTimer = 0.0f;
        private int keyPressed;

        UIBaseScript cameraUIBaseScript;
        [HideInInspector]
        public TerrainGenerator terrainGenerator;
        public static float clickLoadTimer = 0.0f;

        RTS_Cam.RTS_Camera RTS_Camera;
        HtmlElement gameInfoBlock;

        void Start()
        {
            resources[BaseBehavior.ResourceType.Food] = 200.0f;
            resources[BaseBehavior.ResourceType.Gold] = 0.0f;
            resources[BaseBehavior.ResourceType.Wood] = 150.0f;
            resources[BaseBehavior.ResourceType.Favor] = 0.0f;

            GameMenuBehavior.selectedWindowType = GameMenuBehavior.WindowType.None;
            gameInfoBlock = (HtmlElement)UI.document.getElementsByClassName("gameInfo")[0];

            terrainGenerator = Terrain.activeTerrain.GetComponent<TerrainGenerator>();
            RTS_Camera = GetComponent<RTS_Cam.RTS_Camera>();
            cameraUIBaseScript = Camera.main.GetComponent<UIBaseScript>();
            audioSource = GetComponent<AudioSource>();
            interfaceSource = GetComponentInChildren<AudioSource>();

            MapScript.CreateOrUpdateMaps(ref mapCache, update: true);

            UI.document.Run("CreateLoadingScreen", "Retrieving data");

            for (int number = 1; number <= 9; number++)
                unitsBinds.Add(KeyCode.Alpha0 + number, new List<GameObject>());

            int mapSize = GameInfo.mapSize;
            int playerCount = 0;
            // Multiplayer
            if (PhotonNetwork.InRoom)
            {
                // Getting userNumber
                int index = 0;
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    if (player == PhotonNetwork.LocalPlayer)
                        userNumber = index;
                    index += 1;

                    bool isPlayerSpec = false;
                    object playerSpecObd;
                    if (player.CustomProperties.TryGetValue(GameInfo.PLAYER_SPECTATOR, out playerSpecObd))
                        isPlayerSpec = (bool)playerSpecObd;

                    if (!isPlayerSpec)
                        playerCount += 1;
                }

                object playerTeamObd;
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(GameInfo.PLAYER_TEAM, out playerTeamObd))
                    team = (int)playerTeamObd;

                object mapSeedObd;
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameInfo.MAP_SEED, out mapSeedObd))
                    GameInfo.mapSeed = (int)mapSeedObd;

                object mapSizeObd;
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameInfo.MAP_SIZE, out mapSizeObd))
                    mapSize = (int)mapSizeObd;

                userId = PhotonNetwork.LocalPlayer.UserId;
                // Debug.Log("userNumber: " + userNumber + " team: " + team + " userId: " + userId + " mapSeed: " + mapSeed);
            }
            // Singleplayer
            else
            {
                if (!GameInfo.playerSpectate)
                    playerCount += 1;

                PhotonNetwork.OfflineMode = true;
            }
            RTS_Camera.SetHeight();

            UI.document.Run("UpdateLoadingDescription", "Create terrain");
            if (terrainGenerator)
            {
                terrainGenerator.Generate(GameInfo.mapSeed, mapSize);
                UpdateViewChunks();
                spawnData = terrainGenerator.GetSpawnData(
                    spawnCount: playerCount + GameInfo.GetNPCInfo().Count, maxTrees: 1000 * mapSize,
                    goldCountOnRow: 5 * mapSize, goldRows: mapSize,
                    animalsCountOnRow: 5 * mapSize, animalsRows: mapSize);
                
                LoadLevel(mapSize, playerCount);

                if (!GameInfo.playerSpectate && !terrainGenerator.initBlindTexture)
                    terrainGenerator.UpdateBlindTexture(init: true);

                if (PhotonNetwork.InRoom)
                {
                    ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameInfo.PLAYER_LOADED_LEVEL, true } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                    UI.document.Run("UpdateLoadingDescription", "Waiting for other players");
                }
                else
                {
                    StartGame();
                }
            }
            else
            {
                UpdateViewChunks();
                StartGame();
            }
        }

        public void LoadLevel(int mapSize, int playerCount)
        {
            // Load save game
            if (LoadSaveScript.selectedFile != "")
            {
                InstantiateObjects(
                    spawnData, maxTrees: 1250 * mapSize,
                    treePrefabs: terrainGenerator.treePrefabs, goldPrefabs: terrainGenerator.goldPrefabs);

                LoadSaveScript.RestoreGame();
            }
            // Create new game
            else
            {
                InstantiateObjects(
                    spawnData, maxTrees: 1250 * mapSize,
                    treePrefabs: terrainGenerator.treePrefabs, goldPrefabs: terrainGenerator.goldPrefabs, animalPrefabs: terrainGenerator.animalsPrefabs);

                // Create bots
                if (GameInfo.IsMasterClient())
                {
                    int indexNPC = 0;
                    foreach (int NPCTeam in GameInfo.GetNPCInfo())
                    {
                        InstantiateSpawn(spawnData, newUserNumber: playerCount + indexNPC, newUserId: (playerCount + indexNPC).ToString(), NewTeam: NPCTeam);
                        indexNPC++;
                    }
                }

                if (!GameInfo.playerSpectate)
                    InstantiateSpawn(spawnData, newUserNumber: userNumber, newUserId: userId, NewTeam: team);
                else
                {
                    MapScript.MoveCameraToPoint(spawnData["spawn"][0]);
                    team = -10;
                    GameObject DLight = GameObject.FindGameObjectWithTag("Light");
                    DLight.GetComponent<Light>().intensity = 1.3f;
                }
            }
            if (clickLoadTimer > 0.0f)
                clickLoadTimer -= Time.fixedDeltaTime;
        }
        
        public static void SelectLoadNote(MouseEvent mouseEvent)
        {
            if (clickLoadTimer > 0.0f && LoadSaveScript.selectedFile == mouseEvent.srcElement.id)
            {
                UI.document.Run("CreateLoadingScreen", "Loading: " + LoadSaveScript.selectedFile);
                LoadSaveScript.LoadFileSettings();

                LoadSaveScript.loadLevelTimer = 0.5f;
                LoadSaveScript.loadLevel = "Levels/Map1";
            }
            else
            {
                clickLoadTimer = 0.4f;
                LoadSaveScript.selectedFile = mouseEvent.srcElement.id;
                LoadSaveScript.UpdateSaveList();
            }
        }

        // Cache
        private Vector2 viewCameraPosition = new Vector2(0, 0);
        private float cameraPositionX = 0.0f;
        private float cameraPositionZ = 0.0f;
        public Dom.Element activeOver;

        private Dictionary<BaseBehavior.ResourceType, int> workedOn = new Dictionary<BaseBehavior.ResourceType, int>();
        private float countWorkersTimer = 0.0f;
        bool selectedObjectsChanged = false;
        Quaternion cameraRotation;
        public static bool isHotkeysBlocked = false;
        // Update is called once per frame
        void Update()
        {
            isHotkeysBlocked = false;
            if (UI.document.getElementsByClassName("chatInput").length > 0)
                isHotkeysBlocked = true;
            else if (GameMenuBehavior.selectedWindowType != GameMenuBehavior.WindowType.None)
                isHotkeysBlocked = true;

            activeOver = InputPointer.All[0].ActiveOver;

            if (activeOver != null && activeOver.className.Contains("units"))
                MapScript.MapEvent((HtmlDivElement)activeOver);

            UnityEngine.Profiling.Profiler.BeginSample("p Update vision and map"); // Profiler
            cameraPositionX = transform.position.x;
            cameraPositionZ = transform.position.z;
            cameraRotation = transform.rotation;

            Vector2 newViewCameraPosition = GetChunkByPosition(transform.position + new Vector3(transform.forward.normalized.x, 0, transform.forward.normalized.z) * GetCameraOffset() * -1.0f);
            if (viewCameraPosition.x != newViewCameraPosition.x && viewCameraPosition.y != newViewCameraPosition.y)
            {
                viewCameraPosition = newViewCameraPosition;
                UnityEngine.Profiling.Profiler.BeginSample("p UpdateVisionChunks"); // Profiler
                UpdateVisionChunks();
                UnityEngine.Profiling.Profiler.EndSample(); // Profiler
            }

            countWorkersTimer -= Time.deltaTime;
            if (countWorkersTimer <= 0)
            {
                UnityEngine.Profiling.Profiler.BeginSample("p CreateOrUpdateMaps"); // Profiler
                MapScript.CreateOrUpdateMaps(ref mapCache);
                UnityEngine.Profiling.Profiler.EndSample(); // Profiler

                UpdateStatistic();
                countWorkersTimer = 1.0f;
            }
            UpdateMapsCamera();

            UnityEngine.Profiling.Profiler.BeginSample("p Update resources stats"); // Profiler
            if (Time.frameCount % 15 == 0)
            {
                foreach (string typeName in Enum.GetNames(typeof(BaseBehavior.ResourceType)))
                {
                    if (typeName == "None")
                        continue;

                    BaseBehavior.ResourceType resourceType = (BaseBehavior.ResourceType)Enum.Parse(typeof(BaseBehavior.ResourceType), typeName, true);
                    UI.Variables[typeName] = new StringBuilder().AppendFormat("{0:F0} ({1})", resources[resourceType], workedOn[resourceType]).ToString();
                }
            }
            UnityEngine.Profiling.Profiler.EndSample(); // Profiler

            UnityEngine.Profiling.Profiler.EndSample(); // Profiler

            UnityEngine.Profiling.Profiler.BeginSample("p Update events"); // Profiler

            // Chat
            if (PhotonNetwork.InRoom)
                UpdateChat(activeOver);

            // Select free workers
            UpdateSelectFreeWorkers();

            bool isClickGUI = false;
            if (activeOver != null && activeOver.className.Contains("clckable"))
                isClickGUI = true;

            SelectBinds();

            bool objectPlaced = PlaceBuildingOnTerrainUpdate(activeOver);

            if (!objectPlaced && buildedObject == null)
                SelectUnitsUpdate(isClickGUI);

            if (!audioSource.isPlaying)
                PlayMusic(targetMusicType, delay: 0.0f, loop: false, dropCount: false);

            if (selectedObjectsChanged)
            {
                cameraUIBaseScript.UpdateUI();
                selectedObjectsChanged = false;
            }

            if (LoadSaveScript.loadLevel != "")
            {
                LoadSaveScript.loadLevelTimer -= Time.fixedDeltaTime;
                if(LoadSaveScript.loadLevelTimer <= 0.0f)
                {
                    SceneManager.LoadSceneAsync(LoadSaveScript.loadLevel);
                    LoadSaveScript.loadLevel = "";
                }
            }

            UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        }

        private MusicType targetMusicType;
        public void PlayMusic(MusicType musicType, float delay = 0.0f, bool loop = false, bool dropCount = false)
        {
            targetMusicType = musicType;
            if (audioSource == null)
                return;

            float volume = PlayerPrefs.GetFloat("musicVolume");
            foreach (var soundInfo in soundsInfo)
            {
                if (dropCount)
                    soundInfo.lastSoundIndex = -1;

                if (soundInfo.type == musicType && soundInfo.soundList.Count > 0 && !audioSource.isPlaying)
                {
                    if (soundInfo.lastSoundIndex == -1)
                        soundInfo.lastSoundIndex = UnityEngine.Random.Range(0, soundInfo.soundList.Count);

                    if (soundInfo.lastSoundIndex >= soundInfo.soundList.Count)
                        soundInfo.lastSoundIndex = 0;

                    audioSource.clip = soundInfo.soundList[soundInfo.lastSoundIndex];
                    audioSource.loop = loop;
                    audioSource.PlayDelayed(delay);
                    soundInfo.lastSoundIndex++;
                }
            }
        }

        public void UpdateSettings()
        {
            var unitInfo = tagsToSelect.Find(x => x.name == "Unit");
            unitInfo.healthVisibleOnlyWhenSelect = PlayerPrefs.GetInt("isUnitHealthAlwaysSeen") == 1 ? false : true;
            var buildInfo = tagsToSelect.Find(x => x.name == "Building");
            buildInfo.healthVisibleOnlyWhenSelect = PlayerPrefs.GetInt("isBuildingHealthAlwaysSeen") == 1 ? false : true;
            audioSource.volume = PlayerPrefs.GetFloat("musicVolume");

            RTS_Camera.usePanning = PlayerPrefs.GetInt("usePanning") == 1 ? true : false;
            RTS_Camera.useKeyboardInput = PlayerPrefs.GetInt("useKeyboardInput") == 1 ? true : false;

            RTS_Camera.useKeyboardInput = PlayerPrefs.GetInt("useKeyboardInput") == 1 ? true : false;
            RTS_Camera.keyboardMovementSpeed = PlayerPrefs.GetFloat("keyboardMovementSpeed");

            RTS_Camera.useScreenEdgeInput = PlayerPrefs.GetInt("useScreenEdgeInput") == 1 ? true : false;
            RTS_Camera.screenEdgeMovementSpeed = PlayerPrefs.GetFloat("screenEdgeMovementSpeed");

            RTS_Camera.useScrollwheelZooming = PlayerPrefs.GetInt("useScrollwheelZooming") == 1 ? true : false;
            RTS_Camera.scrollWheelZoomingSensitivity = PlayerPrefs.GetFloat("scrollWheelZoomingSensitivity");
        }

        public void UpdateSelectFreeWorkers()
        {
            UnityEngine.Profiling.Profiler.BeginSample("p UpdateSelectFreeWorkers"); // Profiler
            if (UnityEngine.Input.GetKeyDown(KeyCode.F1))
            {
                List<GameObject> freeWorkers = new List<GameObject>();
                foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
                {
                    UnitBehavior unitUnitBehavior = unit.GetComponent<UnitBehavior>();
                    if (unitUnitBehavior.team == team && unitUnitBehavior.ownerId == userId &&
                        unitUnitBehavior.resourceGatherInfo.Count > 0 && unitUnitBehavior.IsIdle())
                        freeWorkers.Add(unit);
                }
                if (freeWorkers.Count > 0)
                {
                    if (freeWorkers.Count <= workersIterate)
                        workersIterate = 0;
                    DeselectAllUnits();
                    selectedObjects.Add(freeWorkers[workersIterate]);
                    selectedObjectsChanged = true;

                    UnitSelectionComponent selection = freeWorkers[workersIterate].GetComponent<UnitSelectionComponent>();
                    selection.SetSelect(true);
                    MoveCaeraToUnit(freeWorkers[workersIterate]);
                    workersIterate++;
                }
            }
            UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        }

        public void UpdateChat(Dom.Element activeOver)
        {
            bool chatInput = UI.document.getElementsByClassName("chatInput").length > 0;

            UnityEngine.Profiling.Profiler.BeginSample("p UpdateChat"); // Profiler
            if ((activeOver != null && activeOver.className.Contains("chatSend") && UnityEngine.Input.GetMouseButtonDown(0) ||
                UnityEngine.Input.GetKeyDown(KeyCode.Return) || (UnityEngine.Input.GetKeyDown(KeyCode.Escape) && chatInput)))
            {
                if (chatInput && !UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                {
                    interfaceSource.PlayOneShot(clickSound, PlayerPrefs.GetFloat("interfaceVolume"));

                    var chatElement = UI.document.getElementsByClassName("chatBox")[0];
                    var inputElement = UI.document.getElementsByClassName("chatInput")[0];
                    var buttonElement = UI.document.getElementsByClassName("chatSend")[0];
                    string message = inputElement.innerText;
                    inputElement.remove();
                    buttonElement.remove();
                    if (message.Length > 0)
                    {
                        if (PhotonNetwork.InRoom)
                            GetComponent<PhotonView>().RPC("SendChatMessage", PhotonTargets.All, String.Format("<b>{0}</b>: {1}", PhotonNetwork.LocalPlayer.NickName, message));
                        else
                            SendChatMessage(String.Format("<b>{0}</b>: {1}", PhotonNetwork.LocalPlayer.NickName, message));
                    }
                    chatInput = false;
                }
                else
                {
                    UI.document.Run("CreateInput");
                    chatInput = true;
                }
            }
            UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        }

        public void UpdateViewChunks()
        {
            int chanksSizeX = (int)(Terrain.activeTerrain.terrainData.size.x / chunkSize);
            int chanksSizeY = (int)(Terrain.activeTerrain.terrainData.size.z / chunkSize);
            chanksView = new bool[chanksSizeX, chanksSizeY];
        }

        List<GameObject> objects = new List<GameObject>();
        public void UpdateIsInCameraView(int x, int y, bool newState)
        {
            BaseBehavior.GetObjectsInRange(ref objects, GetPositionByChunk(x, y), chunkSize, units: false);
            foreach (GameObject objectinChunk in objects)
            {
                BaseBehavior baseBehaviorComponent = objectinChunk.GetComponent<BaseBehavior>();
                baseBehaviorComponent.UpdateIsInCameraView(newState);
            }
        }
        public void UpdateVisionChunks()
        {
            UnityEngine.Profiling.Profiler.BeginSample("p UpdateVisionChunks"); // Profiler
            for (int x = 0; x < chanksView.GetLength(0); x++)
                for (int y = 0; y < chanksView.GetLength(1); y++)
                {
                    float distance = Vector2.Distance(viewCameraPosition, new Vector2(x, y));
                    bool newState = false;

                    float distanceDraw = transform.position.y / 4.0f;
                    if (distanceDraw < 4.0f)
                        distanceDraw = 4.0f;

                    if (distance < distanceDraw)
                        newState = true;

                    if (chanksView[x, y] != newState)
                        UpdateIsInCameraView(x, y, newState);

                    chanksView[x, y] = newState;
                }
            UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        }

        public Vector3 GetPositionByChunk(int x, int y)
        {
            return new Vector3(x * chunkSize, 0, y * chunkSize);
        }

        public static Vector2 GetChunkByPosition(Vector3 position)
        {
            if (position == null)
                return new Vector2(0, 0);

            int x = (int)(chanksView.GetLength(0) * (position.x / Terrain.activeTerrain.terrainData.size.x));
            int y = (int)(chanksView.GetLength(1) * (position.z / Terrain.activeTerrain.terrainData.size.z));
            if (x < 0)
                x = 0;
            if (x > chanksView.GetLength(0) - 1)
                x = chanksView.GetLength(0) - 1;
            if (y < 0)
                y = 0;
            if (y > chanksView.GetLength(1) - 1)
                y = chanksView.GetLength(1) - 1;
            return new Vector2(x, y);
        }

        public static bool IsInCameraView(Vector3 position)
        {
            Vector2 viewPosition = GetChunkByPosition(position);
            return chanksView[(int)viewPosition.x, (int)viewPosition.y];
        }

        void OnDrawGizmos()
        {
            if (spawnData != null)
            {
                if (Debug.isDebugBuild)
                {
                    Gizmos.color = Color.white;
                    foreach (Vector3 spawnPoint in spawnData["spawn"])
                        Gizmos.DrawSphere(spawnPoint, 0.4f);

                    // Gizmos.color = Color.green;
                    // foreach (Vector3 treePoint in spawnData["trees"])
                    //     Gizmos.DrawSphere(treePoint, 0.4f);

                    Gizmos.color = Color.yellow;
                    foreach (Vector3 treePoint in spawnData["gold"])
                        Gizmos.DrawSphere(treePoint, 0.4f);

                    Gizmos.color = Color.blue;
                    foreach (Vector3 treePoint in spawnData["animals"])
                        Gizmos.DrawSphere(treePoint, 0.4f);
                }
                if (debugVisionGrid)
                {
                    Vector2 cameraPos = GetChunkByPosition(transform.position);
                    for (int x = 0; x < chanksView.GetLength(0); x++)
                        for (int y = 0; y < chanksView.GetLength(1); y++)
                        {
                            Gizmos.color = Color.gray;
                            if (chanksView[x, y])
                                Gizmos.color = Color.white;
                            if (cameraPos.x == x && cameraPos.y == y)
                                Gizmos.color = Color.yellow;
                            Gizmos.DrawCube(GetPositionByChunk(x, y), new Vector3(chunkSize, 0.5f, chunkSize));
                        }
                }
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            int countLoadedPlayers = 0;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                object playerLoadedObd;
                bool playerLoaded = false;
                if (player.CustomProperties.TryGetValue(GameInfo.PLAYER_LOADED_LEVEL, out playerLoadedObd))
                    playerLoaded = (bool)playerLoadedObd;
                if (playerLoaded)
                    countLoadedPlayers++;
            }
            if (countLoadedPlayers >= PhotonNetwork.PlayerList.Length)
                StartGame();
        }

        public void InstantiateObjects(Dictionary<string, List<Vector3>> newSpawnData, List<GameObject> treePrefabs, List<GameObject> goldPrefabs, List<GameObject> animalPrefabs = null, int maxTrees = 0)
        {
            int index = 0;
            foreach (Vector3 objectPosition in newSpawnData["trees"])
            {
                if (maxTrees > 0 && index > maxTrees)
                {
                    Debug.Log(new StringBuilder(50).AppendFormat("Trees limit reached: {0} ({1})", maxTrees, newSpawnData["trees"].Count).ToString());
                    break;
                }

                GameObject newTree = Instantiate(treePrefabs[UnityEngine.Random.Range(0, treePrefabs.Count)], objectPosition, new Quaternion(0, 0, 0, 0));
                newTree.transform.eulerAngles = new Vector3(0, UnityEngine.Random.Range(0.0f, 360.0f), 0);
                newTree.transform.SetParent(Terrain.activeTerrain.transform);
                index++;

                BaseBehavior baseBehaviorComponent = newTree.GetComponent<BaseBehavior>();
                baseBehaviorComponent.uniqueId = uniqueIdIndex++;
            }
            foreach (Vector3 objectPosition in newSpawnData["gold"])
            {
                GameObject prefab = goldPrefabs[UnityEngine.Random.Range(0, goldPrefabs.Count)];
                GameObject newGold = Instantiate(prefab, objectPosition, prefab.transform.rotation);

                BaseBehavior baseBehaviorComponent = newGold.GetComponent<BaseBehavior>();
                baseBehaviorComponent.uniqueId = uniqueIdIndex++;

                // newGold.transform.SetParent(Terrain.activeTerrain.transform);
            }

            if (GameInfo.IsMasterClient() && animalPrefabs != null)
            {
                foreach (Vector3 objectPosition in newSpawnData["animals"])
                {
                    GameObject prefab = animalPrefabs[UnityEngine.Random.Range(0, animalPrefabs.Count)];
                    for (int i = 0; i < 7; i++)
                    {
                        GameObject newAnimal = PhotonNetwork.Instantiate(prefab.name, BaseBehavior.GetRandomPoint(objectPosition, 10.0f), prefab.transform.rotation);
                        BaseBehavior baseBehaviorComponent = newAnimal.GetComponent<BaseBehavior>();
                        baseBehaviorComponent.prefabName = prefab.name;
                        baseBehaviorComponent.uniqueId = uniqueIdIndex++;
                    }
                }
            }
        }

        public void InstantiateSpawn(Dictionary<string, List<Vector3>> newSpawnData, int newUserNumber, string newUserId, int NewTeam)
        {
            GameObject createdUnit = null;

            createdUnit = PhotonNetwork.Instantiate(createSpawnBuildingName, newSpawnData["spawn"][newUserNumber], new Quaternion(), 0);
            if (createdUnit == null)
            {
                cameraUIBaseScript.DisplayMessage("Could not find free spawn place.", 3000);
            }
            else
            {
                BaseBehavior baseBehaviorComponent = createdUnit.GetComponent<BaseBehavior>();
                baseBehaviorComponent.prefabName = createSpawnBuildingName;
                if (PhotonNetwork.InRoom)
                    createdUnit.GetComponent<PhotonView>().RPC("ChangeOwner", PhotonTargets.All, newUserId, NewTeam);
                else
                    baseBehaviorComponent.ChangeOwner(newUserId, NewTeam);

                MoveCaeraToUnit(createdUnit);
            }
            foreach (var startUnitInfo in startUnitsInfo)
            {
                BuildingBehavior createdUnitBuildingBehavior = createdUnit.GetComponent<BuildingBehavior>();
                createdUnitBuildingBehavior.ProduceUnit(startUnitInfo.prefabName, startUnitInfo.number, startUnitInfo.distance);
            }
        }

        public void StartGame()
        {
            GC.Collect();

            UpdateSettings();
            UI.document.Run("DeleteLoadingScreen");
            PlayMusic(MusicType.Normal, delay: 1.0f, dropCount: true);
        }

        GameObject objectUnderMouse;
        RaycastHit hit;
        public void SelectUnitsUpdate(bool isClickGUI)
        {
            UnityEngine.Profiling.Profiler.BeginSample("p SelectUnitsUpdate"); // Profiler
            clickTimer -= Time.deltaTime;
            bool selectObject = false;
            bool raycast = Physics.Raycast(Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition), out hit, 100);
            if (raycast)
            {
                if (objectUnderMouse != null)
                    objectUnderMouse.gameObject.GetComponent<UnitSelectionComponent>().SetOutline(false);

                BaseBehavior baseBehaviorComponent = hit.transform.gameObject.GetComponent<BaseBehavior>();
                UnitSelectionComponent hitUnitSelectionComponent = hit.transform.gameObject.GetComponent<UnitSelectionComponent>();
                if (hitUnitSelectionComponent != null && baseBehaviorComponent != null && baseBehaviorComponent.IsVisible())
                {
                    hitUnitSelectionComponent.SetOutline(true);
                    objectUnderMouse = hit.transform.gameObject;
                }
            }
            else if (objectUnderMouse != null)
                objectUnderMouse.gameObject.GetComponent<UnitSelectionComponent>().SetOutline(false);

            if (UnityEngine.Input.GetMouseButtonUp(0) || UnityEngine.Input.GetMouseButtonDown(1))
            {
                if (!isClickGUI && raycast)
                {
                    UnityEngine.Profiling.Profiler.BeginSample("p Left click select"); // Profiler
                    if (UnityEngine.Input.GetMouseButtonUp(0))
                        if (Vector3.Distance(mousePosition1, UnityEngine.Input.mousePosition) < 0.5)
                            if (hit.collider != null && tagsToSelect.Exists(x => (x.name == hit.transform.gameObject.tag)))
                            {
                                bool isCanBeSelected = true;
                                BaseBehavior baseBehaviorComponent = hit.transform.gameObject.GetComponent<BaseBehavior>();
                                if (baseBehaviorComponent != null && baseBehaviorComponent.canBeSelected == false)
                                    isCanBeSelected = false;

                                if (isCanBeSelected && baseBehaviorComponent.IsVisible())
                                {
                                    DeselectAllUnits();

                                    // If user click twice
                                    if (clickTimer > 0.0f)
                                    {
                                        foreach (GameObject selectedUnit in GetSelectUnitsOnScreen(hit.transform.gameObject))
                                        {
                                            UnitSelectionComponent selection = selectedUnit.GetComponent<UnitSelectionComponent>();
                                            selection.SetSelect(true);
                                            selectedObjects.Add(selectedUnit);
                                            selectedObjectsChanged = true;
                                        }
                                    }
                                    else
                                    {
                                        clickTimer = 0.4f;
                                        UnitSelectionComponent selectionComponent = hit.transform.gameObject.GetComponent<UnitSelectionComponent>();
                                        selectionComponent.SetSelect(true);
                                        selectedObjects.Add(hit.transform.gameObject);
                                        selectedObjectsChanged = true;
                                    }
                                    selectObject = true;
                                    isSelecting = false;
                                }
                            }
                    UnityEngine.Profiling.Profiler.EndSample(); // Profiler

                    UnityEngine.Profiling.Profiler.BeginSample("p Right click"); // Profiler
                    if (UnityEngine.Input.GetMouseButtonDown(1))
                    {
                        bool sendToQueue = UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift);
                        Dictionary<GameObject, float> formationList = new Dictionary<GameObject, float>();

                        foreach (GameObject unit in selectedObjects)
                        {
                            BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
                            UnitSelectionComponent selectionComponent = unit.GetComponent<UnitSelectionComponent>();
                            if (
                                selectionComponent != null && selectionComponent.isSelected == true && baseBehaviorComponent.team == team && baseBehaviorComponent.live &&
                                baseBehaviorComponent.ownerId == userId)
                            {
                                if (tagsToSelect.Exists(x => (x.name == hit.transform.gameObject.tag)))
                                {
                                    if (sendToQueue)
                                        baseBehaviorComponent.AddCommandToQueue(hit.transform.gameObject);
                                    else
                                    {
                                        baseBehaviorComponent.GiveOrder(hit.transform.gameObject, true, true);
                                    }
                                }
                                else
                                {
                                    //baseBehaviorComponent.GiveOrder(hit.point);
                                    if (!formationList.ContainsKey(unit))
                                    {
                                        float distance = Vector3.Distance(unit.transform.position, hit.point);
                                        formationList.Add(unit, distance);
                                    }
                                }
                            }
                        }
                        SetOrderInFormation(formationList, hit.point, 1.3f, sendToQueue);
                    }
                    UnityEngine.Profiling.Profiler.EndSample(); // Profiler
                }
            }

            UnityEngine.Profiling.Profiler.BeginSample("p Post update"); // Profiler
            // If we press the left mouse button, save mouse location and begin selection
            if (UnityEngine.Input.GetMouseButtonDown(0) && !UnityEngine.Input.GetKey(KeyCode.LeftAlt) && !selectObject && !isClickGUI)
            {
                isSelecting = true;
                mousePosition1 = UnityEngine.Input.mousePosition;
            }
            // If we let go of the left mouse button, end selection
            if (UnityEngine.Input.GetMouseButtonUp(0) && isSelecting && !selectObject)
            {
                DeselectAllUnits();
                isSelecting = false;
                foreach (GameObject unit in GetSelectingObjects())
                {
                    BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
                    if (baseBehaviorComponent.IsVisible())
                    {
                        UnitSelectionComponent selection = unit.transform.gameObject.GetComponent<UnitSelectionComponent>();
                        selection.SetSelect(true);
                        selectedObjects.Add(unit.transform.gameObject);
                        selectedObjectsChanged = true;
                    }
                }
            }
            UnityEngine.Profiling.Profiler.EndSample(); // Profiler

            UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        }

        public bool PlaceBuildingOnTerrainUpdate(Dom.Element activeOver = null)
        {
            UnityEngine.Profiling.Profiler.BeginSample("p PlaceBuildingOnTerrainUpdate"); // Profiler
            if (buildedObject != null && !(activeOver != null && activeOver.className.Contains("clckable")))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition), out hit, 100, LayerMask.GetMask("Terrain")))
                {
                    if (hit.collider != null && hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                    {
                        BuildingBehavior buildedObjectBuildingBehavior = null;
                        if (selectedObject == null)
                        {
                            selectedObject = PhotonNetwork.Instantiate(buildedObject.name, hit.point, buildedObject.transform.rotation);
                            buildedObjectBuildingBehavior = selectedObject.GetComponent<BuildingBehavior>();
                            buildedObjectBuildingBehavior.uniqueId = uniqueIdIndex++;
                            buildedObjectBuildingBehavior.prefabName = buildedObject.name;

                            // Set owner
                            if (PhotonNetwork.InRoom)
                                selectedObject.GetComponent<PhotonView>().RPC("ChangeOwner", PhotonTargets.All, userId, team);
                            else
                                buildedObjectBuildingBehavior.ChangeOwner(userId, team);

                            // Set building as selected
                            buildedObjectBuildingBehavior.SetState(BuildingBehavior.BuildingState.Selected);
                        }
                        BuildingBehavior buildingBehavior = selectedObject.GetComponent<BuildingBehavior>();
                        selectedObject.transform.position = hit.point;

                        GameObject intersection = buildingBehavior.GetIntersection(null);

                        buildedObjectBuildingBehavior = selectedObject.GetComponent<BuildingBehavior>();
                        GameObject projector = selectedObject.GetComponent<UnitSelectionComponent>().projector;

                        // Block range conditions
                        bool blocked = false;
                        buildedObjectBuildingBehavior.GetBlockedBlockedBy();
                        if (buildedObjectBuildingBehavior.returnObjects.Count > 0)
                        {
                            blocked = true;
                            DrawRanges(buildedObjectBuildingBehavior.returnObjects, Color.red, storeDict: ref errorsProjectors);
                        }
                        else
                            DestyoyRanges(storeDict: ref errorsProjectors);

                        // In range conditions
                        bool notInRange = false;
                        string notEnough = "";
                        buildedObjectBuildingBehavior.IfHasConditionInRange();
                        if (buildedObjectBuildingBehavior.stringsConditionInRange.Count > 0)
                        {
                            buildedObjectBuildingBehavior.GetInRangeConditionError();
                            if (buildedObjectBuildingBehavior.returnObjects.Count > 0)
                                DrawRanges(buildedObjectBuildingBehavior.returnObjects, Color.green, storeDict: ref greenProjectors);
                            else
                            {
                                notInRange = true;
                                notEnough = string.Join(", ", buildedObjectBuildingBehavior.stringsConditionInRange.ToArray());
                                DestyoyRanges(storeDict: ref greenProjectors);
                            }
                        }

                        Color newColor = new Color(0.1f, 1f, 0.1f, 0.45f);
                        if (intersection != null || blocked || notInRange)
                            newColor = new Color(1f, 0.1f, 0.1f, 0.45f);

                        if (UnityEngine.Input.GetMouseButtonDown(0))
                        {
                            if (notInRange)
                            {
                                cameraUIBaseScript.DisplayMessage(new StringBuilder(30).AppendFormat("The building must be in the radius: {0}", notEnough).ToString(), 3000);
                            }
                            else if (blocked || intersection != null)
                            {
                                cameraUIBaseScript.DisplayMessage("Something is blocked", 3000);
                            }
                            else
                            {
                                DestyoyRanges(storeDict: ref greenProjectors);

                                // Create building in project state
                                buildedObjectBuildingBehavior.SetState(BuildingBehavior.BuildingState.Project);

                                foreach (GameObject unit in selectedObjects)
                                {
                                    BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
                                    PhotonView unitPhotonView = unit.GetComponent<PhotonView>();
                                    bool sendToQueue = UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift);
                                    if (sendToQueue)
                                    {
                                        baseBehaviorComponent.AddCommandToQueue(selectedObject);
                                    }
                                    else
                                    {
                                        baseBehaviorComponent.GiveOrder(selectedObject, true, true);
                                        buildedObject = null;
                                    }
                                }
                                selectedObject = null;
                                return true;
                            }
                        }
                        if (UnityEngine.Input.GetMouseButtonDown(1) || UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                        {
                            buildedObjectBuildingBehavior.StopAction();
                            buildedObject = null;
                            selectedObject = null;
                            DestyoyRanges(storeDict: ref errorsProjectors);
                            DestyoyRanges(storeDict: ref greenProjectors);
                            return true;
                        }

                        projector.active = true;
                        projector.GetComponent<Projector>().material.color = newColor;
                        var allNewRenders = selectedObject.GetComponents<Renderer>().Concat(selectedObject.GetComponentsInChildren<Renderer>()).ToArray();
                        foreach (var render in allNewRenders)
                            foreach (var material in render.materials)
                            {
                                material.SetFloat("_Mode", 2);
                                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                                material.SetInt("_ZWrite", 0);
                                material.DisableKeyword("_ALPHATEST_ON");
                                material.EnableKeyword("_ALPHABLEND_ON");
                                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                                material.renderQueue = 3000;
                                material.SetColor("_Color", newColor);
                            }
                        return false;
                    }
                }
            }
            UnityEngine.Profiling.Profiler.EndSample(); // Profiler
            return false;
        }

        Dictionary<GameObject, GameObject> greenProjectors = new Dictionary<GameObject, GameObject>();
        Dictionary<GameObject, GameObject> errorsProjectors = new Dictionary<GameObject, GameObject>();
        List<GameObject> foreachList = new List<GameObject>();
        public void DrawRanges(Dictionary<GameObject, float> blockedBuildings, Color projectorColor, ref Dictionary<GameObject, GameObject> storeDict)
        {
            if (rangeProjector == null)
            {
                Debug.LogWarning("Variable rangeProjector is not assinged in camera controller");
                return;
            }

            foreach (var blockedBuilding in blockedBuildings)
            {
                if (!storeDict.Keys.Any(item => item.GetHashCode() == blockedBuilding.Key.GetHashCode()))
                {
                    GameObject newProjector = Instantiate(rangeProjector, blockedBuilding.Key.transform.position + new Vector3(0, 2, 1), new Quaternion(0, 0, 0, 0));
                    Projector projector = newProjector.GetComponentInChildren<Projector>();

                    projector.material = new Material(projector.material);
                    projector.material.color = projectorColor;

                    projector.orthographicSize = blockedBuilding.Value;
                    storeDict.Add(blockedBuilding.Key, newProjector);
                }
            }
            foreachList.Clear();
            foreachList.AddRange(errorsProjectors.Keys);
            foreach (GameObject unit in foreachList)
            {
                if (!blockedBuildings.Keys.Any(item => item.GetHashCode() == unit.GetHashCode()))
                {
                    Destroy(errorsProjectors[unit]);
                    errorsProjectors.Remove(unit);
                }
            }
        }

        public void DestyoyRanges(ref Dictionary<GameObject, GameObject> storeDict)
        {
            foreachList.Clear();
            foreachList.AddRange(storeDict.Keys);
            foreach (GameObject unit in foreachList)
            {
                Destroy(storeDict[unit]);
                storeDict.Remove(unit);
            }
        }

        public void SelectBinds()
        {
            UnityEngine.Profiling.Profiler.BeginSample("p SelectBinds"); // Profiler
            if (keyTimer > 0)
                keyTimer -= Time.fixedDeltaTime;
            for (int number = 1; number <= 9; number++)
            {
                var key = KeyCode.Alpha0 + number;
                if (UnityEngine.Input.GetKeyDown(key))
                {
                    bool recreate = UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl);
                    bool addNew = UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift);
                    bool remove = UnityEngine.Input.GetKey(KeyCode.LeftAlt) || UnityEngine.Input.GetKey(KeyCode.RightAlt);

                    if (recreate || addNew || remove)
                    {
                        if (recreate)
                            unitsBinds[key].Clear();

                        foreach (GameObject selectedUnit in selectedObjects)
                        {
                            if ((addNew || recreate) && !unitsBinds[key].Contains(selectedUnit))
                                unitsBinds[key].Add(selectedUnit);
                            else if (remove)
                                unitsBinds[key].Remove(selectedUnit);

                            unitsBinds[key] = GetFilteredObjects(unitsBinds[key]);
                        }
                    }
                    else
                    {
                        var units = unitsBinds[key];
                        if (units.Count > 0)
                        {
                            DeselectAllUnits();
                            foreach (GameObject unit in units)
                            {
                                UnitSelectionComponent selection = unit.GetComponent<UnitSelectionComponent>();
                                selection.SetSelect(true);
                                selectedObjects.Add(unit);
                                selectedObjectsChanged = true;
                            }
                            if (keyTimer > 0 && keyPressed == number)
                                MapScript.MoveCameraToPoint(GetCenterOfObjects(units));
                            tempKey = key;
                            keyPressed = number;
                            keyTimer = 0.6f;
                        }
                    }
                }
            }
            UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        }

        public static float GetCameraOffset()
        {
            return (20.0f + Camera.main.transform.position.y - 16.0f) * -1.0f;
        }

        public void MoveCaeraToUnit(GameObject unit)
        {
            transform.position = new Vector3(unit.transform.position.x, transform.position.y, unit.transform.position.z);
            transform.position += new Vector3(transform.forward.normalized.x, 0, transform.forward.normalized.z) * GetCameraOffset();
        }

        private float mapCameraPositionX = 0.0f;
        private float mapCameraPositionZ = 0.0f;
        private Quaternion mapcameraRotation;

        private int cacheMapsCount = 0;
        [HideInInspector]
        public Dictionary<HtmlElement, HtmlElement> mapCache = new Dictionary<HtmlElement, HtmlElement>();
        public void UpdateMapsCamera(bool force = false)
        {
            UnityEngine.Profiling.Profiler.BeginSample("p UpdateMapsCamera"); // Profiler
            foreach (var mapInfo in mapCache)
            {
                bool cameraMoved = (cameraRotation != mapcameraRotation || cameraPositionX != mapCameraPositionX && cameraPositionZ != mapCameraPositionZ);
                if ((cameraMoved || mapCache.Count != cacheMapsCount))
                    MapScript.CreateOrUpdateCameraOnMap(mapInfo.Key, mapInfo.Value);
            }
            mapCameraPositionX = transform.position.x;
            mapCameraPositionZ = transform.position.z;
            mapcameraRotation = transform.rotation;
            cacheMapsCount = mapCache.Count;
            UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        }

        List<GameObject> units = new List<GameObject>();
        List<GameObject> freeWorkers = new List<GameObject>();
        UnitBehavior unitUnitBehavior;
        int[] cacheInfoButtons = new int[11];
        HtmlElement[] cacheInfoButtonElements = new HtmlElement[11];
        public void UpdateStatistic()
        {
            UnityEngine.Profiling.Profiler.BeginSample("p Update free workers"); // Profiler
            bool workersCalculated = false;
            freeWorkers.Clear();

            foreach (string typeName in Enum.GetNames(typeof(BaseBehavior.ResourceType)))
            {
                BaseBehavior.ResourceType resourceType = (BaseBehavior.ResourceType)Enum.Parse(typeof(BaseBehavior.ResourceType), typeName, true);
                workedOn[resourceType] = 0;
            }

            // Calculate statistic
            foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
            {
                if (!workersCalculated)
                {
                    unitUnitBehavior = unit.GetComponent<UnitBehavior>();
                    if (unitUnitBehavior.interactObject != null)
                    {
                        if (unitUnitBehavior.team == team && unitUnitBehavior.ownerId == userId)
                        {
                            BaseBehavior.ResourceType resourceType = unitUnitBehavior.interactObject.GetComponent<BaseBehavior>().resourceCapacityType;
                            workedOn[resourceType] += 1;
                        }
                    }
                    if (unitUnitBehavior.team == team && unitUnitBehavior.ownerId == userId && unitUnitBehavior.resourceGatherInfo.Count > 0 && unit.GetComponent<BaseBehavior>().IsIdle())
                        freeWorkers.Add(unit);
                }
            }
            workersCalculated = true;

            // Display free workers
            if (cacheInfoButtons[10] != freeWorkers.Count && cacheInfoButtonElements[10] != null)
            {
                cacheInfoButtonElements[10].remove();
                cacheInfoButtonElements[10] = null;
                cacheInfoButtons[10] = 0;
            }
            if (freeWorkers.Count > 0 && cacheInfoButtons[10] != freeWorkers.Count)
            {
                cacheInfoButtonElements[10] = UIBaseScript.CreateInfoButton("workers", freeWorkers.Count, "F1", "");
                cacheInfoButtons[10] = freeWorkers.Count;
            }

            UnityEngine.Profiling.Profiler.EndSample(); // Profiler

            UnityEngine.Profiling.Profiler.BeginSample("p Update binds display"); // Profiler

            // Display binds
            for (int i = 1; i <= 9; i++)
            {
                units.Clear();
                units.AddRange(unitsBinds[KeyCode.Alpha0 + i]);

                foreach (GameObject unit in units)
                    if (unit == null || !unit.GetComponent<BaseBehavior>().live)
                        unitsBinds[KeyCode.Alpha0 + i].Remove(unit);

                UnityEngine.Profiling.Profiler.BeginSample("p CreateInfoButton"); // Profiler
                if (cacheInfoButtons[i - 1] != units.Count && cacheInfoButtonElements[i - 1] != null)
                {
                    UI.document.removeChild(cacheInfoButtonElements[i - 1]);
                    cacheInfoButtonElements[i - 1] = null;
                    cacheInfoButtons[i - 1] = 0;
                }

                if (units.Count > 0 && cacheInfoButtons[i - 1] != units.Count)
                {
                    cacheInfoButtonElements[i - 1] = UIBaseScript.CreateInfoButton("solders", units.Count, i.ToString(), units[0].GetComponent<BaseBehavior>().skillInfo.imagePath);
                    cacheInfoButtons[i - 1] = units.Count;
                }
                UnityEngine.Profiling.Profiler.EndSample(); // Profiler
            }
            UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            SendChatMessage("You are dissconected from server");
        }
        public override void OnLeftRoom()
        {
            SendChatMessage("You are dissconected from room");
        }
        public override void OnPlayerEnteredRoom(Player player)
        {
            SendChatMessage(String.Format("<b>Player <b>{0}</b> connected", player.NickName));
        }
        public override void OnPlayerLeftRoom(Player player)
        {
            SendChatMessage(String.Format("<b>Player <b>{0}</b> left", player.NickName));
        }

        [PunRPC]
        public void SendChatMessage(string messageHTML)
        {
            UI.document.Run("SendChatMessage", messageHTML);
        }

        public List<GameObject> GetSelectUnitsOnScreen(GameObject selectUnit)
        {
            selectingObjects.Clear();
            BaseBehavior baseBehaviorComponent = selectUnit.GetComponent<BaseBehavior>();

            Vector3 centerCamera = transform.position;
            centerCamera += new Vector3(transform.forward.normalized.x, 0, transform.forward.normalized.z) * 20.0f;

            BaseBehavior.GetObjectsInRange(ref objects, centerCamera, 50.0f, live: true);
            foreach (GameObject unit in objects)
            {
                BaseBehavior baseUnitBehaviorComponent = unit.GetComponent<BaseBehavior>();
                if (baseUnitBehaviorComponent.skillInfo.uniqueName == baseBehaviorComponent.skillInfo.uniqueName && baseUnitBehaviorComponent.IsVisible())
                {
                    UnitSelectionComponent selection = unit.transform.gameObject.GetComponent<UnitSelectionComponent>();
                    if (selection != null && IsWithinSelectionBounds(unit, new Vector3(0, 0, 0), new Vector3(Screen.width, Screen.height, 0)))
                        selectingObjects.Add(unit);
                }
            }
            return GetFilteredObjects(selectingObjects);
        }

        public List<GameObject> GetFilteredObjects(List<GameObject> selectegObjects)
        {
            // At a time selected can be: the same team units, or buildings, or enemy team units
            var is_team_selected = false;
            var is_unit_selected = false;
            var is_live_selected = false;
            foreach (GameObject unit in selectegObjects)
            {
                BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
                if (baseBehaviorComponent.live)
                    is_live_selected = true;
                if (baseBehaviorComponent.team == team)
                    is_team_selected = true;
                if (baseBehaviorComponent.IsVisible() && baseBehaviorComponent.tag != "Building")
                    is_unit_selected = true;
            }

            List<GameObject> filteredObjects = new List<GameObject>();
            filteredObjects.AddRange(selectegObjects);
            foreach (GameObject unit in selectegObjects)
            {
                BaseBehavior unitBaseBehaviorComponent = unit.GetComponent<BaseBehavior>();
                if (unitBaseBehaviorComponent)
                {
                    if (!unitBaseBehaviorComponent.live && is_live_selected)
                        filteredObjects.Remove(unit);
                    if (unitBaseBehaviorComponent.canBeSelected == false)
                        filteredObjects.Remove(unit);
                    else if (unitBaseBehaviorComponent.team != team && is_team_selected)
                        filteredObjects.Remove(unit);
                }
                if (unit.tag == "Building" && is_unit_selected)
                    filteredObjects.Remove(unit);
            }
            return filteredObjects;
        }

        List<GameObject> selectingObjects = new List<GameObject>();
        public List<GameObject> GetSelectingObjects()
        {
            selectingObjects.Clear();
            Vector3 centerCamera = transform.position;
            centerCamera += new Vector3(transform.forward.normalized.x, 0, transform.forward.normalized.z) * 20.0f;

            BaseBehavior.GetObjectsInRange(ref objects, centerCamera, 50.0f, live: true);
            foreach (GameObject unit in objects)
            {
                UnitSelectionComponent selection = unit.transform.gameObject.GetComponent<UnitSelectionComponent>();
                if (selection != null && IsWithinSelectionBounds(unit, mousePosition1, UnityEngine.Input.mousePosition))
                    selectingObjects.Add(unit);
            }
            return GetFilteredObjects(selectingObjects);
        }

        public static float GetAngle(Vector2 vec1, Vector2 vec2, Vector2 vec3)
        {
            float lenghtA = Mathf.Sqrt(Mathf.Pow(vec2.x - vec1.x, 2) + Mathf.Pow(vec2.y - vec1.y, 2));
            float lenghtB = Mathf.Sqrt(Mathf.Pow(vec3.x - vec2.x, 2) + Mathf.Pow(vec3.y - vec2.y, 2));
            float lenghtC = Mathf.Sqrt(Mathf.Pow(vec3.x - vec1.x, 2) + Mathf.Pow(vec3.y - vec1.y, 2));
            float calc = ((lenghtA * lenghtA) + (lenghtB * lenghtB) - (lenghtC * lenghtC)) / (2 * lenghtA * lenghtB);
            return Mathf.Acos(calc) * Mathf.Rad2Deg;
        }

        public static Vector2 Rotate(Vector2 point, Vector2 pivot, double angleDegree)
        {
            double angle = angleDegree * Math.PI / 180;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            float dx = point.x - pivot.x;
            float dy = point.y - pivot.y;
            double x = cos * dx - sin * dy + pivot.x;
            double y = sin * dx + cos * dy + pivot.y;

            return new Vector2((float)x, (float)y);
        }

        public Vector3 GetCenterOfObjects(List<GameObject> units)
        {
            float totalX = 0, totalY = 0, totalZ = 0;
            foreach (GameObject unit in units)
            {
                totalX += unit.transform.position.x;
                totalY += unit.transform.position.y;
                totalZ += unit.transform.position.z;
            }
            return new Vector3(totalX / units.Count, totalY / units.Count, totalZ / units.Count);
        }

        public void SetOrderInFormation(Dictionary<GameObject, float> formationDict, Vector3 point, float distance, bool sendToQueue)
        {
            float count = formationDict.Count;
            if (count == 0)
                return;

            var formationList = formationDict.ToList();
            formationList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            var center = GetCenterOfObjects(new List<GameObject>(formationDict.Keys));
            // From where coorinates
            float centerX = center.x;
            float centerZ = center.z;

            float angle = GetAngle(
                new Vector2(point.x + 100, point.z),
                new Vector2(point.x, point.z), new Vector2(centerX, centerZ));
            if (point.z > centerZ)
                angle *= -1;

            int indexCount = 0;
            foreach (var unitInfo in formationList)
            {
                var unit = unitInfo.Key;
                BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();

                int formationSize = (int)Math.Ceiling(Math.Sqrt(count));
                int x = (int)(indexCount / formationSize);
                int z = (int)(indexCount % formationSize);
                var newPoint = new Vector3(
                    point.x + (x * distance) - (float)(formationSize - 1) / 2 * distance,
                    point.y,
                    point.z + (z * distance) - (float)(formationSize - 1) / 2 * distance
                    );
                var rotatedPoint = Rotate(new Vector2(newPoint.x, newPoint.z), new Vector2(point.x, point.z), angle);

                PhotonView unitPhotonView = unit.GetComponent<PhotonView>();
                if (sendToQueue)
                {
                    baseBehaviorComponent.AddCommandToQueue(new Vector3(rotatedPoint.x, newPoint.y, rotatedPoint.y));
                }
                else
                {
                    baseBehaviorComponent.GiveOrder(new Vector3(rotatedPoint.x, newPoint.y, rotatedPoint.y), true, true);
                }

                indexCount += 1;
            }
        }

        public void DeselectAllUnits()
        {
            selectedObjects.Clear();
            selectedObjectsChanged = true;
            foreach (TagToSelect tag in tagsToSelect)
            {
                var allUnits = GameObject.FindGameObjectsWithTag(tag.name);
                foreach (GameObject unit in allUnits)
                {
                    UnitSelectionComponent selection = unit.GetComponent<UnitSelectionComponent>();
                    if (selection != null)
                        selection.SetSelect(false);
                }
            }
        }

        public bool IsWithinSelectionBounds(GameObject gameObject)
        {
            return IsWithinSelectionBounds(gameObject, mousePosition1, UnityEngine.Input.mousePosition);
        }

        public bool IsWithinSelectionBounds(GameObject gameObject, Vector3 pos1, Vector3 pos2)
        {
            // At a time selected can be: the same team units, or buildings, or enemy team units
            var is_team_selected = false;
            var is_unit_selected = false;
            foreach (GameObject unit in selectedObjects)
            {
                BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
                if (baseBehaviorComponent.team == team)
                    is_team_selected = true;
                if (baseBehaviorComponent.IsVisible() && baseBehaviorComponent.tag != "Building")
                    is_unit_selected = true;
            }

            BaseBehavior unitBaseBehaviorComponent = gameObject.GetComponent<BaseBehavior>();
            if (unitBaseBehaviorComponent)
            {
                if (unitBaseBehaviorComponent.canBeSelected == false)
                    return false;
                if (unitBaseBehaviorComponent.team != team && is_team_selected)
                    return false;
            }
            if (gameObject.tag == "Building" && is_unit_selected)
                return false;

            var viewportBounds =
                GetViewportBounds(GetComponent<Camera>(), pos1, pos2);

            return viewportBounds.Contains(
                GetComponent<Camera>().WorldToViewportPoint(gameObject.transform.position));
        }

        void OnGUI()
        {
            if (isSelecting)
            {
                // Create a rect from both mouse positions
                var rect = GetScreenRect(mousePosition1, UnityEngine.Input.mousePosition);
                DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
                DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
            }
        }

        public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
        {
            // Move origin from bottom left to top left
            screenPosition1.y = Screen.height - screenPosition1.y;
            screenPosition2.y = Screen.height - screenPosition2.y;
            // Calculate corners
            var topLeft = Vector3.Min(screenPosition1, screenPosition2);
            var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
            // Create Rect
            return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
        }

        public static void DrawScreenRect(Rect rect, Color color)
        {
            GUI.color = color;
            GUI.DrawTexture(rect, WhiteTexture);
            GUI.color = Color.white;
        }

        public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
        {
            // Top
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
            // Left
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
            // Right
            DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
            // Bottom
            DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
        }

        public static Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
        {
            var v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
            var v2 = Camera.main.ScreenToViewportPoint(screenPosition2);
            var min = Vector3.Min(v1, v2);
            var max = Vector3.Max(v1, v2);
            min.z = camera.nearClipPlane;
            max.z = camera.farClipPlane;

            var bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }

        public static void SendCommandToAllSelected(object commandName)
        {
            foreach (GameObject unit in Camera.main.GetComponent<CameraController>().selectedObjects)
            {
                BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
                bool[] infoCommand = baseBehaviorComponent.UICommand((string)commandName);
                bool preformed = infoCommand[0];
                bool notEnoughResources = infoCommand[1];
                if (preformed)
                    break;
                if (notEnoughResources)
                    return;
            }
        }

        public void SelectOnly(string uniqueName)
        {
            List<GameObject> _selectedObjects = new List<GameObject>();
            _selectedObjects.AddRange(selectedObjects);
            foreach (GameObject unit in _selectedObjects)
            {
                BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
                if (baseBehaviorComponent.skillInfo.uniqueName != uniqueName)
                {
                    UnitSelectionComponent unitSelectionComponent = baseBehaviorComponent.GetComponent<UnitSelectionComponent>();
                    unitSelectionComponent.SetSelect(false);
                    selectedObjects.Remove(unit);
                    selectedObjectsChanged = true;
                }
            }
        }

        public static Dictionary<string, Dictionary<BaseSkillScript.UpgradeType, int>> playersUpgrades = new Dictionary<string, Dictionary<BaseSkillScript.UpgradeType, int>>();
        [PunRPC]
        public void _AddUpgrade(string userId, BaseSkillScript.UpgradeType upgradeType)
        {
            if (!playersUpgrades.ContainsKey(userId))
                playersUpgrades[userId] = new Dictionary<BaseSkillScript.UpgradeType, int>();

            if(!playersUpgrades[userId].ContainsKey(upgradeType))
                playersUpgrades[userId][upgradeType] = 1;
            else
                playersUpgrades[userId][upgradeType] += 1;
        }
        public void AddUpgrade(string userId, BaseSkillScript.UpgradeType upgradeType)
        {
            if (PhotonNetwork.InRoom)
                photonView.RPC("_AddUpgrade", PhotonTargets.All, userId, upgradeType);
            else
                _AddUpgrade(userId, upgradeType);
        }
        public static Dictionary<BaseSkillScript.UpgradeType, int> GetUpgrades(string userId)
        {
            if (!playersUpgrades.ContainsKey(userId))
                return new Dictionary<BaseSkillScript.UpgradeType, int>();

            return playersUpgrades[userId];
        }
        public static int GetUpgradeLevel(string userId, BaseSkillScript.UpgradeType upgradeType)
        {
            if (!playersUpgrades.ContainsKey(userId))
                return 0;
            if (!playersUpgrades[userId].ContainsKey(upgradeType))
                return 0;
            return playersUpgrades[userId][upgradeType];
        }

        public static bool GlobalUpgradeCheck(string unitName, int min, int max, BaseSkillScript.UpgradeType upgradeType, string ownerId)
        {
            int upgradeLevel = GetUpgradeLevel(ownerId, upgradeType);

            if (upgradeLevel >= min && (upgradeLevel <= max || max == -1))
                return true;
            return false;
        }
    }
}