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

public class CameraController : MonoBehaviour
{
    [Header("Resources")]
    public float food = 0;
    public float gold = 0;
    public float wood = 0;

    [Header("Select info")]
    public bool isSelecting = false;
    private Vector3 mousePosition1;

    public int team = 1;
    public string userId = "1";
    public int userNumber = 1;

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

    public GameObject createSpawnBuilding;

    // Use this for initialization
    void Start()
    {
        GameObject createdUnit = null;

        // Multiplayer
        if (PhotonNetwork.InRoom)
        {
            // Getting userNumber
            int index = 1;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player == PhotonNetwork.LocalPlayer)
                    userNumber = index;
                index += 1;
            }

            object playerTeamObd;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(GameInfo.PLAYER_TEAM, out playerTeamObd))
            {
                team = (int)playerTeamObd;
            }
            userId = PhotonNetwork.LocalPlayer.UserId;
            Debug.Log("userNumber: " + userNumber + " team: " + team + " userId: " + userId);
        }
        // Singleplayer
        else
        {
            PhotonNetwork.OfflineMode = true;
        }

        foreach (GameObject spawnPoint in GameObject.FindGameObjectsWithTag("Spawn"))
        {
            SpawnBehavior spawnBehavior = spawnPoint.GetComponent<SpawnBehavior>();
            PhotonView spawnPhotonView = spawnPoint.GetComponent<PhotonView>();
            if (spawnBehavior.number == userNumber)
            {
                createdUnit = PhotonNetwork.Instantiate(createSpawnBuilding.name, spawnPoint.transform.position, spawnPoint.transform.rotation, 0);
                break;
            }
        }
        if (createdUnit == null)
        {
            UIBaseScript cameraUIBaseScript = Camera.main.GetComponent<UIBaseScript>();
            cameraUIBaseScript.DisplayMessage("Could not find free spawn place.", 3000);
        }
        else
        {
            BaseBehavior baseBehaviorComponent = createdUnit.GetComponent<BaseBehavior>();
            if (PhotonNetwork.InRoom)
                createdUnit.GetComponent<PhotonView>().RPC("ChangeOwner", PhotonTargets.All, userId, team);
            else
                baseBehaviorComponent.ChangeOwner(userId, team);

            transform.position = new Vector3(createdUnit.transform.position.x, transform.position.y, createdUnit.transform.position.z);
        }
    }

    public class ObjectWithDistance
    {
        public ObjectWithDistance(GameObject _unit, float _distance) {
            unit = _unit;
            distance = _distance;
        }
        public GameObject unit;
        public float distance;
    }
    // Update is called once per frame
    void Update()
    {
        UI.Variables["food"] = String.Format("{0:F0}", food);
        UI.Variables["gold"] = String.Format("{0:F0}", gold);
        UI.Variables["wood"] = String.Format("{0:F0}", wood);

        bool isClickGUI = false;
        if (PowerUI.CameraPointer.All[0].ActiveOver != null && PowerUI.CameraPointer.All[0].ActiveOver.className.Contains("clckable"))
            isClickGUI = true;

        // Place building on terrain
        if (buildedObject != null && !isClickGUI)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition), out hit, 100, LayerMask.GetMask("Terrain")))
            {
                if (hit.collider != null && hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                {
                    if (selectedObject == null)
                    {
                        selectedObject = PhotonNetwork.Instantiate(buildedObject.name, hit.point, buildedObject.transform.rotation);
                        BuildingBehavior buildedObjectBuildingBehavior = selectedObject.GetComponent<BuildingBehavior>();
                        buildedObjectBuildingBehavior.state = BuildingBehavior.BuildingState.Selected;
                        buildedObjectBuildingBehavior.canBeSelected = false;
                        buildedObjectBuildingBehavior.health = 0;
                        buildedObjectBuildingBehavior.team = team;
                        buildedObjectBuildingBehavior.ownerId = userId;
                        buildedObjectBuildingBehavior.live = false;
                        selectedObject.layer = LayerMask.NameToLayer("Project");

                        UnityEngine.AI.NavMeshObstacle navMesh = selectedObject.GetComponent<UnityEngine.AI.NavMeshObstacle>();
                        navMesh.enabled = false;
                    }
                    BuildingBehavior buildingBehavior = selectedObject.GetComponent<BuildingBehavior>();
                    selectedObject.transform.position = hit.point;

                    GameObject intersection = buildingBehavior.GetIntersection(null);

                    Color newColor = new Color(0.1f, 1f, 0.1f, 0.45f);
                    if (intersection != null)
                        newColor = new Color(1f, 0.1f, 0.1f, 0.45f);

                    GameObject projector = selectedObject.GetComponent<UnitSelectionComponent>().projector;
                    if (UnityEngine.Input.GetMouseButtonDown(0))
                    {
                        if (intersection == null)
                        {
                            // Create building in project state

                            buildingBehavior.canBeSelected = true;
                            newColor = new Color(1, 1, 1, 0.45f);
                            projector.active = false;
                            projector.GetComponent<Projector>().material.color = newColor;
                            foreach (var material in selectedObject.GetComponent<Renderer>().materials)
                                material.color = newColor;

                            BuildingBehavior buildedObjectBuildingBehavior = selectedObject.GetComponent<BuildingBehavior>();
                            buildedObjectBuildingBehavior.state = BuildingBehavior.BuildingState.Project;

                            List<GameObject> selectedUnits = GetSelectedObjects();
                            foreach (GameObject unit in selectedUnits)
                            {
                                BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
                                PhotonView unitPhotonView = unit.GetComponent<PhotonView>();
                                if (PhotonNetwork.InRoom)
                                    unitPhotonView.RPC("GiveOrder", PhotonTargets.All, selectedObject, true);
                                else
                                    baseBehaviorComponent.GiveOrder(selectedObject, true);
                            }
                            buildedObject = null;
                            selectedObject = null;
                            return;
                        }
                        else
                        {
                            UIBaseScript cameraUIBaseScript = Camera.main.GetComponent<UIBaseScript>();
                            cameraUIBaseScript.DisplayMessage("Something is blocked", 3000);
                        }
                    }
                    if (UnityEngine.Input.GetMouseButtonDown(1))
                    {
                        BuildingBehavior buildedObjectBuildingBehavior = selectedObject.GetComponent<BuildingBehavior>();
                        CameraController cameraController = Camera.main.GetComponent<CameraController>();
                        cameraController.gold += buildedObjectBuildingBehavior.costGold;
                        cameraController.wood += buildedObjectBuildingBehavior.costWood;
                        Destroy(selectedObject);
                        buildedObject = null;
                        selectedObject = null;
                        return;
                    }

                    projector.active = true;
                    projector.GetComponent<Projector>().material.color = newColor;
                    foreach (var material in selectedObject.GetComponent<Renderer>().materials)
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
                    return;
                }
            }
        }

        bool selectObject = false;
        if (UnityEngine.Input.GetMouseButtonUp(0) || UnityEngine.Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;

            if (!isClickGUI && Physics.Raycast(Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition), out hit, 100))
            {
                if (UnityEngine.Input.GetMouseButtonUp(0))
                    if (Vector3.Distance(mousePosition1, UnityEngine.Input.mousePosition) < 0.5)
                        if (hit.collider != null && tagsToSelect.Exists(x => (x.name == hit.transform.gameObject.tag)))
                        {
                            bool isCanBeSelected = true;
                            BaseBehavior baseBehaviorComponent = hit.transform.gameObject.GetComponent<BaseBehavior>();
                            if (baseBehaviorComponent != null && baseBehaviorComponent.canBeSelected == false)
                                isCanBeSelected = false;

                            if (isCanBeSelected)
                            {
                                DeselectAllUnits();

                                UnitSelectionComponent selectionComponent = hit.transform.gameObject.GetComponent<UnitSelectionComponent>();
                                selectionComponent.isSelected = true;
                                selectObject = true;
                                isSelecting = false;
                            }
                        }

                if (UnityEngine.Input.GetMouseButtonDown(1))
                {
                    foreach (TagToSelect tag in tagsToSelect)
                    {
                        List<ObjectWithDistance> formationList = new List<ObjectWithDistance>();
                        var allUnits = GameObject.FindGameObjectsWithTag(tag.name);
                        foreach (GameObject unit in allUnits)
                        {
                            BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
                            UnitSelectionComponent selectionComponent = unit.GetComponent<UnitSelectionComponent>();
                            if (selectionComponent.isSelected == true && baseBehaviorComponent.team == team && baseBehaviorComponent.live)
                            {
                                if (tagsToSelect.Exists(x => (x.name == hit.transform.gameObject.tag)))
                                {
                                    PhotonView unitPhotonView = unit.GetComponent<PhotonView>();
                                    if (PhotonNetwork.InRoom)
                                        unitPhotonView.RPC("GiveOrder", PhotonTargets.All, hit.transform.gameObject, true);
                                    else
                                        baseBehaviorComponent.GiveOrder(hit.transform.gameObject, true);
                                }
                                else
                                {
                                    //baseBehaviorComponent.GiveOrder(hit.point);
                                    float distance = Vector3.Distance(unit.transform.position, hit.point);
                                    formationList.Add(new ObjectWithDistance(unit, distance));
                                }
                            }
                        }
                        SetOrderInFormation(formationList, hit.point, 1.7f);
                    }
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
            foreach(GameObject unit in GetSelectingObjects())
            {
                UnitSelectionComponent selection = unit.transform.gameObject.GetComponent<UnitSelectionComponent>();
                selection.isSelected = true;
            }
        }
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
        List<GameObject> objects = new List<GameObject>();
        foreach (TagToSelect tag in tagsToSelect)
        {
            var allUnits = GameObject.FindGameObjectsWithTag(tag.name);
            foreach (GameObject unit in allUnits)
            {
                UnitSelectionComponent selection = unit.transform.gameObject.GetComponent<UnitSelectionComponent>();
                if (selection.isSelected == true)
                    objects.Add(unit);
            }
        }
        return GetFilteredObjects(objects);
    }

    public List<GameObject> GetSelectingObjects()
    {
        List<GameObject> objects = new List<GameObject>();
        foreach (TagToSelect tag in tagsToSelect)
        {
            var allUnits = GameObject.FindGameObjectsWithTag(tag.name);
            foreach (GameObject unit in allUnits)
            {
                UnitSelectionComponent selection = unit.transform.gameObject.GetComponent<UnitSelectionComponent>();
                if (selection != null && IsWithinSelectionBounds(unit))
                    objects.Add(unit);
            }
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

    public void SetOrderInFormation(List<ObjectWithDistance> formationList, Vector3 point, float distance)
    {
        float count = formationList.Count;
        if (count == 0)
            return;

        float totalX = 0, totalZ = 0;
        foreach (ObjectWithDistance unitWiithDistance in formationList.OrderBy(v => v.distance).ToArray())
        {
            var unit = unitWiithDistance.unit;
            totalX += unit.transform.position.x;
            totalZ += unit.transform.position.z;
        }
        // From where coorinates
        float centerX = totalX / formationList.Count;
        float centerZ = totalZ / formationList.Count;

        float angle = GetAngle(
            new Vector2(point.x + 100, point.z),
            new Vector2(point.x, point.z), new Vector2(centerX, centerZ));
        if (point.z > centerZ)
            angle *= -1;

        int indexCount = 0;
        foreach (ObjectWithDistance unitWiithDistance in formationList.OrderBy(v => v.distance).ToArray())
        {
            var unit = unitWiithDistance.unit;
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
            if (PhotonNetwork.InRoom)
                unitPhotonView.RPC("GiveOrder", PhotonTargets.All, new Vector3(rotatedPoint.x, newPoint.y, rotatedPoint.y), true);
            else
                baseBehaviorComponent.GiveOrder(new Vector3(rotatedPoint.x, newPoint.y, rotatedPoint.y), true);

            indexCount += 1;
        }
    }

    public void DeselectAllUnits()
    {
        foreach (TagToSelect tag in tagsToSelect)
        {
            var allUnits = GameObject.FindGameObjectsWithTag(tag.name);
            foreach (GameObject unit in allUnits)
            {
                UnitSelectionComponent selection = unit.transform.gameObject.GetComponent<UnitSelectionComponent>();
                selection.isSelected = false;
            }
        }
    }

    public bool IsWithinSelectionBounds(GameObject gameObject)
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
            GetViewportBounds(GetComponent<Camera>(), mousePosition1, UnityEngine.Input.mousePosition);

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
        foreach (GameObject unit in GetSelectedObjects())
        {
            BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
            if(baseBehaviorComponent.uniqueName != uniqueName)
            {
                UnitSelectionComponent unitSelectionComponent = baseBehaviorComponent.GetComponent<UnitSelectionComponent>();
                unitSelectionComponent.isSelected = false;
            }
        }
    }
}
