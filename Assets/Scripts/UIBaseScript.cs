using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PowerUI;
using System.Text;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Photon.Pun;
using GangaGame;

public class UIBaseScript : MonoBehaviour
{
    public struct UIImage
    {
        public string imagePath;
        public string name;
        public string readableName;
        public string readableDescription;
        public KeyCode hotkey;
        public string errorMessage;

        public UIImage( string _name, string _readName, string _imagePath, string _readDescr, KeyCode _hotkey, string _errorMessage = "")
        {
            imagePath = _imagePath;
            name = _name;
            readableName = _readName;
            readableDescription = _readDescr;
            hotkey = _hotkey;
            errorMessage = _errorMessage;
        }
    }

    CameraController cameraController;
    Dictionary<BaseBehavior.BehaviorType, UIImage> behaviorUIImages = new Dictionary<BaseBehavior.BehaviorType, UIImage>();
    Dictionary<UIImage, int> commandsUIImages = new Dictionary<UIImage, int>();
    // Use this for initialization
    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();

        // Buildings
        // new UIImage("stop", "Stop", "commands/stop-sign.png", "Stop command.", KeyCode.H), 1);

        // Units
        // commandsUIImages.Add(new UIImage("stop", "Stop", "commands/stop-sign.png", "Stop command.", KeyCode.H), 2);
        // commandsUIImages.Add(new UIImage("attack", "Go and attack", "commands/arrow-scope.png", "Move to target and attack enemies on the way.", KeyCode.A), 2);

        // Behavior commands
        //behaviorUIImages.Add(BaseBehavior.BehaviorType.Aggressive, new UIImage("behaviorType1", "Aggressive behavior", "commands/caveman.png", "The unit will attack nearest enemy targets.", KeyCode.T));
        //behaviorUIImages.Add(BaseBehavior.BehaviorType.Counterattack, new UIImage("behaviorType2", "Counterattack behavior", "commands/wide-arrow-dunk.png", "The unit will counterattack nearest enemy targets.", KeyCode.T));
        //behaviorUIImages.Add(BaseBehavior.BehaviorType.Hold, new UIImage("behaviorType3", "Hold behavior", "commands/static-guard.png", "Unit will hold the position, and not attack back.", KeyCode.T));
        //behaviorUIImages.Add(BaseBehavior.BehaviorType.Run, new UIImage("behaviorType4", "Run behavior", "commands/run.png", "A unit will run away if attacked.", KeyCode.T));
    }

    private bool updateUI = false;
    public void UpdateUI()
    {
        UI.document.Run("ClearInfo");

        DisplayObjectsInfo(cameraController.selectedObjects);

        if (cameraController.selectedObjects.Count == 1)
            DisplayDetailInfo(cameraController.selectedObjects[0]);

        UpdateCommands(cameraController.selectedObjects);
    }

    // Cache
    BaseBehavior unitBaseBehaviorComponent = null;
    UnitBehavior unitBehaviorComponent = null;
    BuildingBehavior buildingBehaviorComponent = null;

    void Update()
    {
        bool description = false;
        if (cameraController.activeOver != null)
        {
            string className = cameraController.activeOver.className;

            if (className.Contains("discriptable"))
                description = DisplayDescription(className);

            else if (UnityEngine.Input.GetMouseButtonUp(0) && className.Contains("detailInfo"))
            {
                if (cameraController.selectedObjects.Count > 0)
                    cameraController.MoveCaeraToUnit(cameraController.selectedObjects[0]);
            }
            else if (className.Contains("units") && !UnityEngine.Input.GetKey(KeyCode.LeftAlt))
            {
                var element = (HtmlDivElement)cameraController.activeOver;
                var elementPos = new Vector2(element.getBoundingClientRect().X, element.getBoundingClientRect().Y);
                var mousePos = PowerUI.CameraPointer.All[0].Position;
                var mapPoint = (mousePos - elementPos) / new Vector2(element.getBoundingClientRect().Width, element.getBoundingClientRect().Height);
                if (UnityEngine.Input.GetMouseButton(0))
                {
                    cameraController.MoveCameraToPoint(cameraController.mapPointToPosition(mapPoint));
                }
                else if (UnityEngine.Input.GetMouseButtonDown(1))
                {
                    foreach (var unit in cameraController.selectedObjects)
                    {
                        unitBaseBehaviorComponent = unit.GetComponent<BaseBehavior>();
                        PhotonView unitPhotonView = unit.GetComponent<PhotonView>();
                        if (PhotonNetwork.InRoom)
                            unitPhotonView.RPC("GiveOrder", PhotonTargets.All, cameraController.mapPointToPosition(mapPoint), true);
                        else
                            unitBaseBehaviorComponent.GiveOrder(cameraController.mapPointToPosition(mapPoint), true, true);
                    }
                }
            }
            else if (className.Contains("unit") && UnityEngine.Input.GetMouseButton(0))
            {
                GameObject unit = PhotonNetwork.GetPhotonView(int.Parse(cameraController.activeOver.id)).gameObject;
                cameraController.DeselectAllUnits();

                UnitSelectionComponent selection = unit.GetComponent<UnitSelectionComponent>();
                selection.SetSelect(false);

                cameraController.MoveCameraToPoint(unit.transform.position);
            }
        }

        if (!description)
            DestroyDescription();
    }

    public void DisplayObjectsInfo(List<GameObject> selectedObjects)
    {
        if (selectedObjects.Count == 1)
            DisplayObjectInfo(unit: selectedObjects[0], detailInfo: true, dinamicInfo: true);

        displayedNames.Clear();
        foreach (GameObject selectedObject in selectedObjects)
        {
            unitBaseBehaviorComponent = selectedObject.GetComponent<BaseBehavior>();
            if (!displayedNames.Contains(unitBaseBehaviorComponent.skillInfo.uniqueName))
            {
                DisplayObjectInfo(unit: selectedObject, detailInfo: false);
                displayedNames.Add(unitBaseBehaviorComponent.skillInfo.uniqueName);
            }
        }
    }

    List<string> displayedNames = new List<string>();
    public void UpdateCommands(List<GameObject> selectedObjects)
    {
    }

    public void DisplayObjectInfo(GameObject unit, bool detailInfo = false, bool dinamicInfo = false)
    {
        unitBaseBehaviorComponent = unit.GetComponent<BaseBehavior>();
        DrawInfo(
            parentElement: "cblock",
            detailInfo: detailInfo,
            drawImage: true,
            discriptable: false,
            dinamicInfo: dinamicInfo,
            uniqueName: unitBaseBehaviorComponent.skillInfo.uniqueName,
            readableName: unitBaseBehaviorComponent.skillInfo.readableName,
            readableDescription: unitBaseBehaviorComponent.skillInfo.readableDescription,
            errorMessage: "",
            imagePath: unitBaseBehaviorComponent.skillInfo.imagePath,
            hotkey: unitBaseBehaviorComponent.skillInfo.productionHotkey.ToString(),
            costInfo: unitBaseBehaviorComponent.GetCostInformation().ToArray(),
            tableStatistics: null
            );
    }

    public void DrawInfo(
        string parentElement, bool detailInfo, bool drawImage, bool discriptable, bool dinamicInfo, string uniqueName, string readableName, string readableDescription,
        string errorMessage, string imagePath, string hotkey, string[] costInfo, string[] tableStatistics)
    {
        UI.document.Run("DrawInfo", parentElement, detailInfo, drawImage, discriptable, dinamicInfo, uniqueName, readableName, readableDescription,
        errorMessage, imagePath, hotkey, costInfo, tableStatistics);
    }
    
    public void DisplayDetailInfo(GameObject unit, bool force = false)
    {
        if (unit == null)
            return;

        unitBaseBehaviorComponent = unit.GetComponent<BaseBehavior>();
        buildingBehaviorComponent = unit.GetComponent<BuildingBehavior>();
        bool queueNotEmpty = false;
        if (buildingBehaviorComponent != null && buildingBehaviorComponent.productionQuery.Count > 0)
            queueNotEmpty = true;

        if (queueNotEmpty)
        {
            //Display query to build
            if (buildingBehaviorComponent != null && buildingBehaviorComponent.team == cameraController.team && buildingBehaviorComponent.ownerId == cameraController.userId)
                updateQueue(buildingBehaviorComponent.productionQuery, buildingBehaviorComponent.uqeryLimit, buildingBehaviorComponent.buildTimer);
        }

        if(true)
        {
            // Set health
            foreach (var element in UI.document.getElementsByClassName("unitHealth"))
                element.style.width = new StringBuilder(16).AppendFormat("{0:F0}%", unitBaseBehaviorComponent.health / unitBaseBehaviorComponent.maxHealth * 100).ToString();

            foreach (var element in UI.document.getElementsByClassName("dinamicInfo"))
            {
                element.innerHTML = "";

                BaseBehavior.ResourceType resourseDisplayType = BaseBehavior.ResourceType.None;
                float resources = 0.0f;
                if (unitBaseBehaviorComponent != null && unitBaseBehaviorComponent.resourceHold > 0)
                {
                    resources = unitBaseBehaviorComponent.resourceHold;
                    resourseDisplayType = unitBaseBehaviorComponent.resourceType;
                }
                else if (unitBaseBehaviorComponent.resourceCapacity > 0)
                {
                    resources = unitBaseBehaviorComponent.resourceCapacity;
                    resourseDisplayType = unitBaseBehaviorComponent.resourceCapacityType;
                }
                if (resourseDisplayType != BaseBehavior.ResourceType.None)
                {
                    Dom.Element statusticDiv = UI.document.createElement("p");
                    if (resourseDisplayType == BaseBehavior.ResourceType.Food)
                        statusticDiv.innerHTML = new StringBuilder(16).AppendFormat("Food: {0:F0}", resources).ToString();
                    if (resourseDisplayType == BaseBehavior.ResourceType.Gold)
                        statusticDiv.innerHTML = new StringBuilder(16).AppendFormat("Gold: {0:F0}", resources).ToString();
                    if (resourseDisplayType == BaseBehavior.ResourceType.Wood)
                        statusticDiv.innerHTML = new StringBuilder(16).AppendFormat("Wood: {0:F0}", resources).ToString();
                    element.appendChild(statusticDiv);
                }
            }
        }
    }

    public void DisplayMessage(string message, int timer, string uniqueMessage = "")
    {
        if (uniqueMessage != "" && UI.document.getElementsByClassName(uniqueMessage).length > 0)
            foreach (var element in UI.document.getElementsByClassName(uniqueMessage))
                element.remove();

        string[] args = new string[3] { message, timer.ToString(), uniqueMessage };
        UI.document.Run("CreateMessage", args);
    }

    public void updateQueue(List<GameObject> unitsQuery, int qeryLimit, float buildTimer)
    {
        if (UI.document.getElementsByClassName("infoBlock").length == 0)
            return;

        var info = UI.document.getElementsByClassName("infoBlock")[0];

        if (UI.document.getElementsByClassName("query").length > 0)
            UI.document.getElementsByClassName("query")[0].remove();

        if (unitsQuery.Count > 0)
        {
            Dom.Element infoDiv = UI.document.createElement("div");
            infoDiv.className = "query clckable";
            info.appendChild(infoDiv);

            var index = 0;
            foreach (var unit in unitsQuery)
            {
                unitBaseBehaviorComponent = unit.GetComponent<BaseBehavior>();
                BaseSkillScript skillScript = unit.GetComponent<BaseSkillScript>();

                string imagePath = "";
                float timeToBuild = 0.0f;
                if (unitBaseBehaviorComponent != null)
                {
                    imagePath = unitBaseBehaviorComponent.skillInfo.imagePath;
                    timeToBuild = unitBaseBehaviorComponent.skillInfo.timeToBuild;
                }
                if (skillScript != null)
                {
                    imagePath = skillScript.skillInfo.imagePath;
                    timeToBuild = skillScript.skillInfo.timeToBuild;
                }

                Dom.Element progressBaseDiv = UI.document.createElement("div");
                progressBaseDiv.className = "production clckable";
                infoDiv.appendChild(progressBaseDiv);

                Dom.Element elementDiv = UI.document.createElement("img");
                elementDiv.setAttribute("src", imagePath);

                string newClassName = new StringBuilder(20).AppendFormat("clckable {0}", index).ToString();
                elementDiv.className = newClassName;

                progressBaseDiv.appendChild(elementDiv);
                
                if (index == 0)
                {
                    Dom.Element progressDiv = UI.document.createElement("div");
                    progressDiv.className = newClassName;
                    progressDiv.style.width = new StringBuilder(20).AppendFormat("{0:F0}%", buildTimer / timeToBuild * 100).ToString();
                    progressBaseDiv.appendChild(progressDiv);
                    progressDiv.addEventListener("mousedown", delegate (MouseEvent e) {
                        cameraController.RemoveQueueElementFromSelected();
                    });
                }
                elementDiv.addEventListener("mousedown", delegate (MouseEvent e) {
                    cameraController.RemoveQueueElementFromSelected();
                });
                index += 1;
            }
        }
    }

    List<UIImage> checkedUIImages = new List<UIImage>();
    public bool DisplayDescription(string className, List<UIImage> skillUIImages = null)
    {
        checkedUIImages.Clear();
        if (skillUIImages != null)
            checkedUIImages.AddRange(skillUIImages);

        checkedUIImages.AddRange(commandsUIImages.Keys);
        checkedUIImages.AddRange(behaviorUIImages.Values);

        foreach (UIImage skillUIImage in checkedUIImages)
        {
            if (className.Contains(skillUIImage.name))
            {
                var containerDescription = UI.document.getElementsByClassName("containerDescription")[0];
                var altInfos = containerDescription.getElementsByClassName("altInfo");

                if (altInfos.length > 0 && !altInfos[0].className.Contains(skillUIImage.name))
                    DestroyDescription();

                Dom.Element altInfo = null;
                if (altInfos.length <= 0)
                {
                    altInfo = UI.document.createElement("div");
                    altInfo.className = "content altInfo ";
                    altInfo.className += skillUIImage.name;
                    containerDescription.appendChild(altInfo);
                    // Dom.Element createdImage = DrawInfo(altInfo, skillUIImage, true, false, false, false);
                }
                else
                    altInfo = altInfos[0];
                return true;
            }
        }
        return false;
    }
    public void DestroyDescription()
    {
        var elements = UI.document.getElementsByClassName("containerDescription");
        if (elements.length <= 0)
            return;

        var containerDescription = elements[0];
        var altInfos = containerDescription.getElementsByClassName("altInfo");
        if (altInfos.length > 0)
            altInfos[0].remove();
    }
}
