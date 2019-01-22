using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using PowerUI;
using System.Text.RegularExpressions;
using Photon.Pun;
using GangaGame;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class CameraController : MonoBehaviourPunCallbacks
{
    [Header("Resources")]
    public float food = 0;
    public float gold = 0;
    public float wood = 0;

    [Header("Select info")]
    public bool isSelecting = false;
    private Vector3 mousePosition1;

    public int team = 1;
    public string userId = "123";
    private int userNumber = 0;

    [HideInInspector]
    public bool chatInput = false;
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
    public string createSpawnBuildingName;
    public List<SpawnUnitInfo> startUnitsInfo = new List<SpawnUnitInfo>();

    private float clickTimer = 0.0f;

    public Texture mapTexture;

    private Dictionary<KeyCode, List<GameObject>> unitsBinds = new Dictionary<KeyCode, List<GameObject>>();
    private KeyCode tempKey;
    private float keyTimer = 0.0f;
    private int keyPressed;

    public enum WindowType { None, MainMenu, BigMap, Settings };
    public WindowType selectedWindowType = WindowType.None;
    public Dictionary<WindowType, List<object>> menuInfo = new Dictionary<WindowType, List<object>>();

    List<GameObject> selectedObjects = new List<GameObject>();

    Dictionary<string, List<Vector3>> spawnData;
    bool[,] chanksView = new bool[0, 0];
    float chunkSize = 11.0f;

    public bool debugVisionGrid = false;

    private int uniqueIdIndex = 0;

    // Use this for initialization
    void Start()
    {
        UI.document.Run("CreateLoadingScreen", "Retrieving data");

        menuInfo.Add(WindowType.MainMenu, new List<object>());
        menuInfo[WindowType.MainMenu].Add(KeyCode.F10);
        menuInfo[WindowType.MainMenu].Add("buttonMainMenu");

        menuInfo.Add(WindowType.BigMap, new List<object>());
        menuInfo[WindowType.BigMap].Add(KeyCode.M);
        menuInfo[WindowType.BigMap].Add("buttonBigMap");

        menuInfo.Add(WindowType.Settings, new List<object>());
        menuInfo[WindowType.Settings].Add("CreateSettings");

        for (int number = 1; number <= 9; number++)
            unitsBinds.Add(KeyCode.Alpha0 + number, new List<GameObject>());

        int mapSeed = UnityEngine.Random.Range(0, 1000);
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
                mapSeed = (int)mapSeedObd;

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
        GetComponent<RTS_Cam.RTS_Camera>().SetHeight();

        UI.document.Run("UpdateLoadingDescription", "Create terrain");
        TerrainGenerator terrainGenerator = Terrain.activeTerrain.GetComponent<TerrainGenerator>();
        if (terrainGenerator)
        {
            terrainGenerator.Generate(mapSeed, mapSize);
            UpdateViewChunks();
            spawnData = terrainGenerator.GetSpawnData(
                spawnCount: playerCount + GameInfo.GetNPCInfo().Count, maxTrees: 1000 * mapSize,
                goldCountOnRow: 4 * mapSize, goldRows: mapSize,
                animalsCountOnRow: 5 * mapSize, animalsRows: mapSize);

            InstantiateObjects(
                spawnData, maxTrees: 750 * mapSize + 100, 
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
                MoveCameraToPoint(spawnData["spawn"][0]);
                team = -10;
                GameObject DLight = GameObject.FindGameObjectWithTag("Light");
                DLight.GetComponent<Light>().intensity = 1.3f;
            }

            if (PhotonNetwork.InRoom)
            {
                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameInfo.PLAYER_LOADED_LEVEL, true} };
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
            StartGame();
        }
    }

    public void UpdateSettings()
    {
        var unitInfo = tagsToSelect.Find(x => x.name == "Unit");
        unitInfo.healthVisibleOnlyWhenSelect = PlayerPrefs.GetInt("isUnitHealthAlwaysSeen") == 1 ? false : true;
        var buildInfo = tagsToSelect.Find(x => x.name == "Building");
        buildInfo.healthVisibleOnlyWhenSelect = PlayerPrefs.GetInt("isBuildingHealthAlwaysSeen") == 1 ? false : true;
    }

    private int workedOnFood, workedOnGold, workedOnWood = 0;
    private float countWorkersTimer = 0.0f;
    // Update is called once per frame
    void Update()
    {
        UpdateVisionChunks();

        countWorkersTimer -= Time.deltaTime;
        if (countWorkersTimer <= 0)
        {
            UpdateMapsAndStatistic();
        }
        UpdateMapsCamera();

        UI.Variables["food"] = String.Format("{0:F0}", food);
        if (workedOnFood > 0) UI.Variables["food"] += String.Format(" ({0})", workedOnFood);
        UI.Variables["gold"] = String.Format("{0:F0}", gold);
        if (workedOnGold > 0) UI.Variables["gold"] += String.Format(" ({0})", workedOnGold);
        UI.Variables["wood"] = String.Format("{0:F0}", wood);
        if (workedOnWood > 0) UI.Variables["wood"] += String.Format(" ({0})", workedOnWood);

        var activeOver = PowerUI.CameraPointer.All[0].ActiveOver;

        // Chat
        if (PhotonNetwork.InRoom)
        {
            UpdateChat(activeOver);
        }

        // Select free workers
        UpdateSelectFreeWorkers();

        bool isClickGUI = false;
        if (activeOver != null && activeOver.className.Contains("clckable"))
            isClickGUI = true;
        
        SelectBinds();

        UpdateWindow(activeOver);

        bool objectPlaced = PlaceBuildingOnTerrainUpdate(activeOver);

        if (!objectPlaced && buildedObject == null)
            SelectUnitsUpdate(isClickGUI);
    }

    public void UpdateWindow(Dom.Element activeOver)
    {
        UnityEngine.Profiling.Profiler.BeginSample("p UpdateWindow"); // Profiler
        WindowType newWindowType = GetNewWindow(activeOver);
        if ((newWindowType != WindowType.None && selectedWindowType != WindowType.None) || UnityEngine.Input.GetKeyDown(KeyCode.Escape))
        {
            if (selectedWindowType == WindowType.MainMenu)
                UI.document.getElementsByClassName("menu")[0].innerHTML = "";
            else if (selectedWindowType == WindowType.BigMap || selectedWindowType == WindowType.Settings)
                UI.document.getElementsByClassName("window")[0].remove();

            if (newWindowType == selectedWindowType || UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                newWindowType = WindowType.None;
            selectedWindowType = WindowType.None;
        }

        if (newWindowType != WindowType.None)
        {
            if (newWindowType == WindowType.MainMenu)
                UI.document.Run("CreateMenu");
            else if (newWindowType == WindowType.BigMap)
            {
                UI.document.Run("DisplayBigMapWindow");
                UpdateMapsAndStatistic();
            }
            else if (newWindowType == WindowType.Settings)
            {
                UI.document.Run("DisplaySettingsWindow");
                MenuBehavior.CreateSettings("SettingsContent");
            }

            selectedWindowType = newWindowType;
        }

        if (activeOver != null && activeOver.className.Contains("CloseMenu") && UnityEngine.Input.GetMouseButtonDown(0))
        {
            UI.document.getElementsByClassName("menu")[0].innerHTML = "";
            selectedWindowType = WindowType.None;
        }
        else if (activeOver != null && activeOver.className.Contains("GoToMainMenu") && UnityEngine.Input.GetMouseButtonDown(0))
        {
            UI.document.Run("CreateLoadingScreen", "Leave");
            PhotonNetwork.Disconnect();
            PhotonNetwork.LoadLevel("Levels/menu");
        }
        else if (activeOver != null && activeOver.className.Contains("CloseTheGame") && UnityEngine.Input.GetMouseButtonDown(0))
        {
            Application.Quit();
            return;
        }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
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
        UnityEngine.Profiling.Profiler.BeginSample("p UpdateChat"); // Profiler
        if ((activeOver != null && activeOver.className.Contains("chatSend") && UnityEngine.Input.GetMouseButtonDown(0) ||
            UnityEngine.Input.GetKeyDown(KeyCode.Return) || (UnityEngine.Input.GetKeyDown(KeyCode.Escape) && chatInput)))
        {
            if (chatInput && !UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
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

    public void UpdateIsInCameraView(int x, int y, bool newState)
    {
        List<GameObject> objectsInChunk = BaseBehavior.GetObjectsInRange(GetPositionByChunk(x, y), chunkSize, units: false);
        foreach (GameObject objectinChunk in objectsInChunk)
        {
            BaseBehavior baseBehaviorComponent = objectinChunk.GetComponent<BaseBehavior>();
            baseBehaviorComponent.UpdateIsInCameraView(newState);
        }
    } 

    public void UpdateVisionChunks()
    {
        UnityEngine.Profiling.Profiler.BeginSample("p UpdateVisionChunks"); // Profiler
        Vector3 centerCamera = transform.position;
        centerCamera += new Vector3(transform.forward.normalized.x, 0, transform.forward.normalized.z) * GetCameraOffset() * -1.0f;
        Vector2 viewCameraPosition = GetChunkByPosition(centerCamera);

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

    public Vector2 GetChunkByPosition(Vector3 position)
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

    public bool IsInCameraView(Vector3 position)
    {
        Vector2 viewPosition = GetChunkByPosition(position);
        return chanksView[(int)viewPosition.x, (int)viewPosition.y];
    }

    void OnDrawGizmos()
    {
        if (spawnData != null)
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

    [PunRPC]
    public void InstantiateObjects(Dictionary<string, List<Vector3>> newSpawnData, List<GameObject> treePrefabs, List<GameObject> goldPrefabs, List<GameObject> animalPrefabs, int maxTrees = 0)
    {
        int index = 0;
        foreach(Vector3 objectPosition in newSpawnData["trees"])
        {
            if (maxTrees > 0 && index > maxTrees)
            {
                Debug.Log(String.Format("Trees limit reached: {0} ({1})", maxTrees, newSpawnData["trees"].Count));
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

        if (GameInfo.IsMasterClient())
        {
            foreach (Vector3 objectPosition in newSpawnData["animals"])
            {
                GameObject prefab = animalPrefabs[UnityEngine.Random.Range(0, animalPrefabs.Count)];
                for(int i = 0; i < 7; i++)
                {
                    GameObject newAnimal = PhotonNetwork.Instantiate(prefab.name, BaseBehavior.GetRandomPoint(objectPosition, 10.0f), prefab.transform.rotation);
                }
            }
        }
    }

    [PunRPC]
    public void InstantiateSpawn(Dictionary<string, List<Vector3>> newSpawnData, int newUserNumber, string newUserId, int NewTeam)
    {
        GameObject createdUnit = null;
        
        createdUnit = PhotonNetwork.Instantiate(createSpawnBuildingName, newSpawnData["spawn"][newUserNumber], new Quaternion(), 0);
        if (createdUnit == null)
        {
            UIBaseScript cameraUIBaseScript = Camera.main.GetComponent<UIBaseScript>();
            cameraUIBaseScript.DisplayMessage("Could not find free spawn place.", 3000);
        }
        else
        {
            BaseBehavior baseBehaviorComponent = createdUnit.GetComponent<BaseBehavior>();
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
        UpdateSettings();
        UI.document.Run("DeleteLoadingScreen");
    }

    public WindowType GetNewWindow(Dom.Element activeOver)
    {
        if (activeOver != null || UnityEngine.Input.anyKeyDown)
        {
            foreach (WindowType menuType in menuInfo.Keys)
            {
                foreach (object eventType in menuInfo[menuType])
                {
                    if (eventType is string)
                    {
                        if (activeOver != null && UnityEngine.Input.GetMouseButtonUp(0) && activeOver.className.Contains((string)eventType))
                            return menuType;
                    }
                    else
                        if (UnityEngine.Input.GetKeyDown((KeyCode)eventType))
                        return menuType;
                }
            }
        }
        return WindowType.None;
    }

    GameObject objectUnderMouse;
    public void SelectUnitsUpdate(bool isClickGUI)
    {
        UnityEngine.Profiling.Profiler.BeginSample("p SelectUnitsUpdate"); // Profiler
        clickTimer -= Time.deltaTime;
        bool selectObject = false;
        RaycastHit hit;
        bool raycast = Physics.Raycast(Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition), out hit, 100);
        if (raycast)
        {
            if(objectUnderMouse != null)
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
                                    }
                                }
                                else
                                {
                                    clickTimer = 0.4f;
                                    UnitSelectionComponent selectionComponent = hit.transform.gameObject.GetComponent<UnitSelectionComponent>();
                                    selectionComponent.SetSelect(true);
                                    selectedObjects.Add(hit.transform.gameObject);
                                }
                                selectObject = true;
                                isSelecting = false;
                            }
                        }

                if (UnityEngine.Input.GetMouseButtonDown(1))
                {
                    bool sendToQueue = UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift);
                    Dictionary<GameObject, float> formationList = new Dictionary<GameObject, float>();

                    foreach (GameObject unit in GetSelectedObjects())
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
                                    PhotonView unitPhotonView = unit.GetComponent<PhotonView>();
                                    if (PhotonNetwork.InRoom)
                                    {
                                        unitPhotonView.RPC("GiveOrderViewID", PhotonTargets.All, hit.transform.gameObject.GetComponent<PhotonView>().ViewID, true, true);
                                    }
                                    else
                                        baseBehaviorComponent.GiveOrder(hit.transform.gameObject, true, true);
                                }
                            }
                            else
                            {
                                //baseBehaviorComponent.GiveOrder(hit.point);
                                float distance = Vector3.Distance(unit.transform.position, hit.point);
                                formationList.Add(unit, distance);
                            }
                        }
                    }
                    SetOrderInFormation(formationList, hit.point, 1.3f, sendToQueue);
                }
            }
        }

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
                }
            }
        }
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

                        // Set owner
                        if (PhotonNetwork.InRoom)
                            selectedObject.GetComponent<PhotonView>().RPC("ChangeOwner", PhotonTargets.All, userId, team);
                        else
                            buildedObjectBuildingBehavior.ChangeOwner(userId, team);

                        // Set building as selected
                        if (PhotonNetwork.InRoom)
                            selectedObject.GetComponent<PhotonView>().RPC("SetAsSelected", PhotonTargets.All);
                        else
                            buildedObjectBuildingBehavior.SetAsSelected();
                    }
                    BuildingBehavior buildingBehavior = selectedObject.GetComponent<BuildingBehavior>();
                    selectedObject.transform.position = hit.point;

                    GameObject intersection = buildingBehavior.GetIntersection(null);

                    Color newColor = new Color(0.1f, 1f, 0.1f, 0.45f);
                    if (intersection != null)
                        newColor = new Color(1f, 0.1f, 0.1f, 0.45f);

                    buildedObjectBuildingBehavior = selectedObject.GetComponent<BuildingBehavior>();
                    GameObject projector = selectedObject.GetComponent<UnitSelectionComponent>().projector;
                    if (UnityEngine.Input.GetMouseButtonDown(0))
                    {
                        if (intersection == null)
                        {
                            // Create building in project state
                            buildedObjectBuildingBehavior.SetAsProject();

                            List<GameObject> selectedUnits = GetSelectedObjects();
                            foreach (GameObject unit in selectedUnits)
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
                                    if (PhotonNetwork.InRoom)
                                        unitPhotonView.RPC("GiveOrderViewID", PhotonTargets.All, selectedObject.GetComponent<PhotonView>().ViewID, true, true);
                                    else
                                        baseBehaviorComponent.GiveOrder(selectedObject, true, true);
                                    buildedObject = null;
                                }
                            }
                            selectedObject = null;
                            return true;
                        }
                        else
                        {
                            UIBaseScript cameraUIBaseScript = Camera.main.GetComponent<UIBaseScript>();
                            cameraUIBaseScript.DisplayMessage("Something is blocked", 3000);
                        }
                    }
                    if (UnityEngine.Input.GetMouseButtonDown(1) || UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                    {
                        buildedObjectBuildingBehavior.StopAction();
                        buildedObject = null;
                        selectedObject = null;
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

                var selectedUnits = GetSelectedObjects();
                if (recreate || addNew || remove)
                {
                    if (recreate)
                        unitsBinds[key].Clear();

                    foreach (GameObject selectedUnit in selectedUnits)
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
                        }
                        if (keyTimer > 0 && keyPressed == number)
                            MoveCameraToPoint(GetCenterOfObjects(units));
                        tempKey = key;
                        keyPressed = number;
                        keyTimer = 0.4f;
                    }
                }
            }
        }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
    }

    public float GetCameraOffset()
    {
        return (20.0f + transform.position.y - 16.0f) * -1.0f;
    }

    public void MoveCaeraToUnit(GameObject unit)
    {
        transform.position = new Vector3(unit.transform.position.x, transform.position.y, unit.transform.position.z);
        transform.position += new Vector3(transform.forward.normalized.x, 0, transform.forward.normalized.z) * GetCameraOffset();
    }

    public Vector3 mapPointToPosition(Vector2 mapPosition)
    {
        Terrain terrain = Terrain.activeTerrain;
        return new Vector3(terrain.terrainData.size.x * mapPosition.x, 0, terrain.terrainData.size.z - terrain.terrainData.size.z * mapPosition.y);
    }

    public void MoveCameraToPoint(Vector3 position)
    {
        transform.position = new Vector3(position.x, transform.position.y, position.z);
        transform.position += new Vector3(transform.forward.normalized.x, 0, transform.forward.normalized.z) * GetCameraOffset();
    }

    public Vector2 GetPositionOnMap(Vector3 position)
    {
        Terrain terrain = Terrain.activeTerrain;
        Vector3 positionOnTerrain = position - terrain.GetPosition();
        return new Vector2(positionOnTerrain.x / terrain.terrainData.size.x, positionOnTerrain.z / terrain.terrainData.size.z);
    }

    private static bool IsInTerrainRange(Vector3 target)
    {
        if(target.x > 0.0f && target.x <= Terrain.activeTerrain.terrainData.size.x)
            if (target.z > 0.0f && target.z <= Terrain.activeTerrain.terrainData.size.z)
                return true;

        return false;
    }

    public void CreateOrUpdateCameraOnMap(Dom.Element mapBlock, Dom.Element mapImage)
    {
        Vector3 vameraLookAt = transform.position - new Vector3(transform.forward.normalized.x, 0, transform.forward.normalized.z) * GetCameraOffset();
        bool isCameraInTerrain = IsInTerrainRange(vameraLookAt);
        float cameraScale = 20.0f / (Terrain.activeTerrain.terrainData.size.x / 50.0f / 4.0f);
        float cameraHeight = cameraScale / 1.2f;
        float cameraWidtht = cameraScale;
        Vector2 positionCameraOnMap = GetPositionOnMap(vameraLookAt);
        Dom.Element cameraDiv = null;
        if(isCameraInTerrain)
        {
            if (mapBlock.getElementsByClassName("mapCamera").length <= 0)
            {
                cameraDiv = UI.document.createElement("div");
                cameraDiv.className = "mapCamera clckable";
                cameraDiv.style.height = String.Format("{0}%", cameraHeight);
                cameraDiv.style.width = String.Format("{0}%", cameraWidtht);
                mapBlock.appendChild(cameraDiv);
            }
            else
                cameraDiv = mapBlock.getElementsByClassName("mapCamera")[0];

            if (cameraDiv != null)
            {
                cameraDiv.style.left = String.Format("{0}%", positionCameraOnMap.x * 100.0f);
                cameraDiv.style.bottom = String.Format("{0}%", positionCameraOnMap.y * 100.0f);
                cameraDiv.style.transform = String.Format("rotate({0}deg) translate(-50%,-50%)", transform.rotation.eulerAngles.y);
            }
        }
        else if (mapBlock.getElementsByClassName("mapCamera").length <= 0)
        {
            cameraDiv = mapBlock.getElementsByClassName("mapCamera")[0];
            cameraDiv.remove();
        }
    }

    public void UpdateMapsCamera()
    {
        foreach (var mapBlock in UI.document.getElementsByClassName("mapBlock"))
        {
            var mapImage = (HtmlElement)mapBlock.getElementsByClassName("map")[0];
            CreateOrUpdateCameraOnMap(mapBlock, mapImage);
        }
    }

    public void UpdateMapsAndStatistic()
    {
        UnityEngine.Profiling.Profiler.BeginSample("p UpdateMapsAndStatistic"); // Profiler
        bool workersCalculated = false;
        List<GameObject> freeWorkers = new List<GameObject>();
        foreach (var mapBlock in UI.document.getElementsByClassName("mapBlock"))
        {
            // Draw map
            TerrainGenerator terrainGenerator = Terrain.activeTerrain.GetComponent<TerrainGenerator>();
            if (terrainGenerator != null && terrainGenerator.mapTexture != null)
            {
                ((HtmlElement)mapBlock).image = terrainGenerator.mapTexture;
                mapBlock.style.height = "100%";
                mapBlock.style.width = "100%";
            }
            else
            {
                mapBlock.style.backgroundImage = String.Format("{0}.png", SceneManager.GetActiveScene().name);
            }
            var mapImage = (HtmlElement)mapBlock.getElementsByClassName("map")[0];

            Texture newTexture;

            if (mapTexture.height > 0)
            {
                mapImage.image = mapTexture;
                mapImage.style.height = "100%";
                mapImage.style.width = "100%";
            }

            CreateOrUpdateCameraOnMap(mapBlock, mapImage);

            var unitsBlock = (HtmlElement)mapBlock.getElementsByClassName("units")[0];
            unitsBlock.innerHTML = "";

            workedOnFood = 0; workedOnGold = 0; workedOnWood = 0;
            // Draw units + calculate statistic
            foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
            {
                BaseBehavior unitBaseBehavior = unit.GetComponent<BaseBehavior>();
                if (!workersCalculated)
                {
                    UnitBehavior unitUnitBehavior = unit.GetComponent<UnitBehavior>();
                    if (unitUnitBehavior.interactObject != null)
                    {
                        BaseBehavior interactObjectBehaviorComponent = unitUnitBehavior.interactObject.GetComponent<BaseBehavior>();
                        if (unitUnitBehavior.team == team && unitUnitBehavior.ownerId == userId)
                        {
                            if (interactObjectBehaviorComponent.resourceCapacityType == BaseBehavior.ResourceType.Food)
                                workedOnFood += 1;
                            if (interactObjectBehaviorComponent.resourceCapacityType == BaseBehavior.ResourceType.Gold)
                                workedOnGold += 1;
                            if (interactObjectBehaviorComponent.resourceCapacityType == BaseBehavior.ResourceType.Wood)
                                workedOnWood += 1;
                        }
                    }
                    if (unitUnitBehavior.team == team && unitUnitBehavior.ownerId == userId && unitUnitBehavior.resourceGatherInfo.Count > 0 && unitBaseBehavior.IsIdle())
                        freeWorkers.Add(unit);
                }
                if (unitBaseBehavior.IsDisplayOnMap() && IsInTerrainRange(unit.transform.position))
                {
                    Dom.Element unitDiv = UI.document.createElement("div");
                    unitDiv.className = "unit clckable";
                    unitDiv.id = unit.GetComponent<PhotonView>().ViewID.ToString();
                    Vector2 positionOnMap = GetPositionOnMap(unit.transform.position);
                    unitDiv.style.left = String.Format("{0}%", positionOnMap.x * 100.0f - 1.5);
                    unitDiv.style.bottom = String.Format("{0}%", positionOnMap.y * 100.0f - 1.5);
                    unitDiv.style.backgroundColor = unitBaseBehavior.GetDisplayColor();
                    unitsBlock.appendChild(unitDiv);
                }
            }
            workersCalculated = true;
        }

        var gameInfoBlock = (HtmlElement)UI.document.getElementsByClassName("gameInfo")[0];
        gameInfoBlock.innerHTML = "";

        // Display free workers
        if (freeWorkers.Count > 0)
            UI.document.Run("CreateInfoButton", "workers", freeWorkers.Count, "F1", "");

        // Display binds
        for (int number = 1; number <= 9; number++)
        {
            var units = unitsBinds[KeyCode.Alpha0 + number];

            foreach (GameObject unit in units)
            {
                if (unit == null)
                    units.Remove(unit);

                BaseBehavior unitBehaviorComponent = unit.GetComponent<BaseBehavior>();
                if (!unitBehaviorComponent.live)
                    units.Remove(unit);
            }
            if (units.Count > 0)
            {
                UI.document.Run("CreateInfoButton", "solders", units.Count, number, units[0].GetComponent<BaseBehavior>().skillInfo.imagePath);
            }
        }
        countWorkersTimer = 1.0f;
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
        BaseBehavior baseBehaviorComponent = selectUnit.GetComponent<BaseBehavior>();
        List<GameObject> objects = new List<GameObject>();

        Vector3 centerCamera = transform.position;
        centerCamera += new Vector3(transform.forward.normalized.x, 0, transform.forward.normalized.z) * 20.0f;

        var allUnits = BaseBehavior.GetObjectsInRange(centerCamera, 50.0f, live: true);
        foreach (GameObject unit in allUnits)
        {
            BaseBehavior baseUnitBehaviorComponent = unit.GetComponent<BaseBehavior>();
            if (baseUnitBehaviorComponent.skillInfo.uniqueName == baseBehaviorComponent.skillInfo.uniqueName && baseUnitBehaviorComponent.IsVisible())
            {
                UnitSelectionComponent selection = unit.transform.gameObject.GetComponent<UnitSelectionComponent>();
                if (selection != null && IsWithinSelectionBounds(unit, new Vector3(0, 0, 0), new Vector3(Screen.width, Screen.height, 0)))
                    objects.Add(unit);
            }
        }
        return GetFilteredObjects(objects);
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

    public List<GameObject> GetSelectedObjects()
    {
        return selectedObjects;
    }

    public List<GameObject> GetSelectingObjects()
    {
        List<GameObject> objects = new List<GameObject>();
        Vector3 centerCamera = transform.position;
        centerCamera += new Vector3(transform.forward.normalized.x, 0, transform.forward.normalized.z) * 20.0f;

        var allUnits = BaseBehavior.GetObjectsInRange(centerCamera, 50.0f, live: true);
        foreach (GameObject unit in allUnits)
        {
            UnitSelectionComponent selection = unit.transform.gameObject.GetComponent<UnitSelectionComponent>();
            if (selection != null && IsWithinSelectionBounds(unit, mousePosition1, UnityEngine.Input.mousePosition))
                objects.Add(unit);
        }
        return GetFilteredObjects(objects);
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
                if (PhotonNetwork.InRoom)
                    unitPhotonView.RPC("GiveOrder", PhotonTargets.All, new Vector3(rotatedPoint.x, newPoint.y, rotatedPoint.y), true, true);
                else
                    baseBehaviorComponent.GiveOrder(new Vector3(rotatedPoint.x, newPoint.y, rotatedPoint.y), true, true);
            }

            indexCount += 1;
        }
    }

    public void DeselectAllUnits()
    {
        selectedObjects.Clear();
        foreach (TagToSelect tag in tagsToSelect)
        {
            var allUnits = GameObject.FindGameObjectsWithTag(tag.name);
            foreach (GameObject unit in allUnits)
            {
                UnitSelectionComponent selection = unit.GetComponent<UnitSelectionComponent>();
                if(selection != null)
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
        List<GameObject> selectegObjects = GetSelectedObjects();
        foreach(GameObject unit in selectegObjects)
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

    public void SendCommandToAllSelected(string commandName)
    {
        foreach(GameObject unit in GetSelectedObjects())
        {
            BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
            bool[] infoCommand = baseBehaviorComponent.UICommand(commandName);
            bool preformed = infoCommand[0];
            bool notEnoughResources = infoCommand[1];
            if (preformed)
                break;
            if (notEnoughResources)
                return;
        }
    }

    public void RemoveQueueElementFromSelected()
    {
        string className = PowerUI.CameraPointer.All[0].ActiveOver.className;
        var r = new Regex(@" (\d)", RegexOptions.IgnoreCase);
        var match = r.Match(className);
        if (match.Success)
        {
            int index = int.Parse(match.Groups[1].Value);
            foreach (GameObject unit in GetSelectedObjects())
            {
                BuildingBehavior buildingBehavior = unit.GetComponent<BuildingBehavior>();
                buildingBehavior.DeleteFromProductionQuery(index);
            }
        }
    }

    public void SelectOnly(string uniqueName)
    {
        List<GameObject> _selectedObjects = new List<GameObject>();
        _selectedObjects.AddRange(GetSelectedObjects());
        foreach (GameObject unit in _selectedObjects)
        {
            BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
            if(baseBehaviorComponent.skillInfo.uniqueName != uniqueName)
            {
                UnitSelectionComponent unitSelectionComponent = baseBehaviorComponent.GetComponent<UnitSelectionComponent>();
                unitSelectionComponent.SetSelect(false);
                selectedObjects.Remove(unit);
            }
        }
    }
}
