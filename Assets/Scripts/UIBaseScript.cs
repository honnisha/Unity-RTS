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
    public class UIImage
    {
        public UIImage(
            string _name, int _count, string _imagePath, string _readName, string _readDescr, List<string> _tableStats, List<string> _costInfo, 
            KeyCode _hotkey, SkillType _skillType = SkillType.None, string _errorMessage = "")
        {
            name = _name;
            count = _count;
            imagePath = _imagePath;
            readableName = _readName;
            readableDescription = _readDescr;
            tableStatistics = _tableStats;
            costInfo = _costInfo;
            hotkey = _hotkey;
            skillType = _skillType;
            errorMessage = _errorMessage;
        }
        public UIImage(
            string _name, string _readName, string _imagePath, string _readDescr, 
            KeyCode _hotkey, SkillType _skillType = SkillType.None, string _errorMessage = "")
        {
            name = _name;
            readableName = _readName;
            imagePath = _imagePath;
            readableDescription = _readDescr;
            hotkey = _hotkey;
            skillType = _skillType;
            errorMessage = _errorMessage;
        }

        public GameObject image;
        public string imagePath;
        public string name;
        public int count;
        public string readableName;
        public string readableDescription;
        public List<string> tableStatistics = new List<string>();
        public List<string> costInfo = new List<string>();
        public KeyCode hotkey;
        public SkillType skillType = SkillType.None;
        public string errorMessage;
    }

    public List<UIImage> storageUIImages = new List<UIImage>();
    public List<UIImage> storageOrdersUIImages = new List<UIImage>();
    public List<UIImage> storageSkillUIImages = new List<UIImage>();

    CameraController cameraController;
    Dictionary<BaseBehavior.BehaviorType, UIImage> behaviorUIImages = new Dictionary<BaseBehavior.BehaviorType, UIImage>();
    Dictionary<UIImage, int> commandsUIImages = new Dictionary<UIImage, int>();
    // Use this for initialization
    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();

        // Buildings
        commandsUIImages.Add(new UIImage("stop", "Stop", "commands/stop-sign.png", "Stop command.", KeyCode.H), 1);

        // Units
        commandsUIImages.Add(new UIImage("stop", "Stop", "commands/stop-sign.png", "Stop command.", KeyCode.H), 2);
        commandsUIImages.Add(new UIImage("attack", "Go and attack", "commands/arrow-scope.png", "Move to target and attack enemies on the way.", KeyCode.A), 2);

        // Behavior commands
        behaviorUIImages.Add(BaseBehavior.BehaviorType.Aggressive, new UIImage("behaviorType1", "Aggressive behavior", "commands/caveman.png", "The unit will attack nearest enemy targets.", KeyCode.T));
        behaviorUIImages.Add(BaseBehavior.BehaviorType.Counterattack, new UIImage("behaviorType2", "Counterattack behavior", "commands/wide-arrow-dunk.png", "The unit will counterattack nearest enemy targets.", KeyCode.T));
        behaviorUIImages.Add(BaseBehavior.BehaviorType.Hold, new UIImage("behaviorType3", "Hold behavior", "commands/static-guard.png", "Unit will hold the position, and not attack back.", KeyCode.T));
        behaviorUIImages.Add(BaseBehavior.BehaviorType.Run, new UIImage("behaviorType4", "Run behavior", "commands/run.png", "A unit will run away if attacked.", KeyCode.T));
    }

    private bool updateUI = false;
    public void UpdateUI()
    {
        updateUI = true;
    }
    
    private List<UIImage> skillUIImages = new List<UIImage>();
    // Update is called once per frame
    void Update()
    {
        UnityEngine.Profiling.Profiler.BeginSample("p Draw center block"); // Profiler

        DisplayUIImageObjects("center", GetSelectedObjectsToUIImage(cameraController.selectedObjects), ref storageUIImages);
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler

        UnityEngine.Profiling.Profiler.BeginSample("p Draw right block"); // Profiler
        if (cameraController.selectedObjects.Count == 1)
            DisplayDetailInfo(cameraController.selectedObjects[0], force: updateUI);

        skillUIImages.Clear();
        skillUIImages.AddRange(GetSelectedSkillsOfObjectsToUIImage(cameraController.selectedObjects));

        DisplayUIImageObjects("right", skillUIImages, ref storageSkillUIImages);
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler

        UnityEngine.Profiling.Profiler.BeginSample("p DisplayCommands"); // Profiler
        DisplayCommands(cameraController.selectedObjects);
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler

        UnityEngine.Profiling.Profiler.BeginSample("p HandleUIEvents"); // Profiler
        bool description = HandleUIEvents(cameraController.selectedObjects, skillUIImages);
        if (!description || updateUI)
            DestroyDescription();
        updateUI = false;
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
    }

    public bool HandleUIEvents(List<GameObject> selectegObjects, List<UIImage> skillUIImages)
    {
        bool description = false;
        Dom.Element activeElement = PowerUI.CameraPointer.All[0].ActiveOver;
        if (activeElement != null)
        {
            if (activeElement.className.Contains("discriptable"))
                description = DisplayDescription(activeElement.className, skillUIImages);
            else if (activeElement.className.Contains("descrCommand"))
                description = DisplayDescription(activeElement.className, commands: true);
            else if (UnityEngine.Input.GetMouseButtonUp(0) && activeElement.className.Contains("detailInfo"))
            {
                if (selectegObjects.Count > 0)
                    cameraController.MoveCaeraToUnit(selectegObjects[0]);
            }
            else if (activeElement.className.Contains("units") && !UnityEngine.Input.GetKey(KeyCode.LeftAlt))
            {
                var element = (HtmlDivElement)activeElement;
                var elementPos = new Vector2(element.getBoundingClientRect().X, element.getBoundingClientRect().Y);
                var mousePos = PowerUI.CameraPointer.All[0].Position;
                var mapPoint = (mousePos - elementPos) / new Vector2(element.getBoundingClientRect().Width, element.getBoundingClientRect().Height);
                if (UnityEngine.Input.GetMouseButton(0))
                {
                    cameraController.MoveCameraToPoint(cameraController.mapPointToPosition(mapPoint));
                }
                else if (UnityEngine.Input.GetMouseButtonDown(1))
                {
                    foreach (var unit in cameraController.GetSelectedObjects())
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
            else if (activeElement.className.Contains("unit") && UnityEngine.Input.GetMouseButton(0))
            {
                GameObject unit = PhotonNetwork.GetPhotonView(int.Parse(activeElement.id)).gameObject;
                cameraController.DeselectAllUnits();

                UnitSelectionComponent selection = unit.GetComponent<UnitSelectionComponent>();
                selection.SetSelect(false);

                cameraController.MoveCameraToPoint(unit.transform.position);
            }
        }
        return description;
    }

    BaseBehavior unitBaseBehaviorComponent = null;
    UnitBehavior unitBehaviorComponent = null;
    BuildingBehavior buildingBehaviorComponent = null;
    GameObject cachedDetailInfoObject = null;
    public void DisplayDetailInfo(GameObject unit, bool force = false)
    {
        if (unit == null)
            return;

        if (cachedDetailInfoObject == unit && !force)
            return;

        cachedDetailInfoObject = unit;

        //Display query to build
        unitBaseBehaviorComponent = unit.GetComponent<BaseBehavior>();

        if (buildingBehaviorComponent != null && buildingBehaviorComponent.team == cameraController.team && buildingBehaviorComponent.ownerId == cameraController.userId)
            updateQueue(buildingBehaviorComponent.productionQuery, buildingBehaviorComponent.uqeryLimit, buildingBehaviorComponent.buildTimer);

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

    public void DisplayMessage(string message, int timer, string uniqueMessage = "")
    {
        if (uniqueMessage != "" && UI.document.getElementsByClassName(uniqueMessage).length > 0)
            foreach (var element in UI.document.getElementsByClassName(uniqueMessage))
                element.remove();

        string[] args = new string[3] { message, timer.ToString(), uniqueMessage };
        UI.document.Run("CreateMessage", args);
    }

    List<UIImage> newUIImages = new List<UIImage>();
    public List<UIImage> GetSelectedSkillsOfObjectsToUIImage(List<GameObject> selectegObjects)
    {
        newUIImages.Clear();
        foreach (GameObject unit in selectegObjects)
        {
            if (unit == null)
                continue;
            
            unitBaseBehaviorComponent = unit.GetComponent<BaseBehavior>();
            if (unitBaseBehaviorComponent.team != cameraController.team || unitBaseBehaviorComponent.ownerId != cameraController.userId)
                continue;

            BuildingBehavior buildingBehaviorComponent = selectegObjects[0].GetComponent<BuildingBehavior>();
            if (buildingBehaviorComponent != null && buildingBehaviorComponent.state != BuildingBehavior.BuildingState.Builded)
                continue;

            //Skills
            if (unitBaseBehaviorComponent != null && unitBaseBehaviorComponent.live)
            {
                foreach (GameObject skillObject in unitBaseBehaviorComponent.skillList)
                {
                    BaseSkillScript skillComponent = skillObject.GetComponent<BaseSkillScript>();
                    BaseBehavior baseBehavior = skillObject.GetComponent<BaseBehavior>();
                    SkillErrorInfo skillErrorInfo = BaseSkillScript.GetSkillErrorInfo(unit, skillObject);
                    if (skillComponent != null && skillErrorInfo.isDisplayedAsSkill)
                    {
                        GetOrCreateUIImageToList(
                            ref newUIImages, skillComponent.skillInfo.uniqueName, skillComponent.skillInfo.imagePath,
                            skillComponent.skillInfo.readableName, skillComponent.skillInfo.readableDescription,
                            skillComponent.GetStatistics(), skillComponent.GetCostInformation(),
                            skillComponent.skillInfo.productionHotkey, skillComponent.skillInfo.skillType,
                            skillErrorInfo.errorMessage
                            );   
                    }

                    else if (baseBehavior != null && skillErrorInfo.isDisplayedAsSkill)
                    {
                        GetOrCreateUIImageToList(
                            ref newUIImages, baseBehavior.skillInfo.uniqueName, baseBehavior.skillInfo.imagePath,
                            baseBehavior.skillInfo.readableName, baseBehavior.skillInfo.readableDescription,
                            baseBehavior.GetStatistics(), baseBehavior.GetCostInformation(),
                            baseBehavior.skillInfo.productionHotkey, baseBehavior.skillInfo.skillType,
                            skillErrorInfo.errorMessage
                            );   
                    }
                }
            }

            //Created objects
            if (buildingBehaviorComponent != null && buildingBehaviorComponent.live)
                foreach (GameObject buildUnit in buildingBehaviorComponent.producedUnits)
                {
                    SkillErrorInfo skillErrorInfo = BaseSkillScript.GetSkillErrorInfo(unit, buildUnit);
                    unitBaseBehaviorComponent = buildUnit.GetComponent<BaseBehavior>();
                    if (unitBaseBehaviorComponent != null && skillErrorInfo.isDisplayedAsSkill)
                        GetOrCreateUIImageToList(
                            ref newUIImages, unitBaseBehaviorComponent.skillInfo.uniqueName, unitBaseBehaviorComponent.skillInfo.imagePath,
                            unitBaseBehaviorComponent.skillInfo.readableName, unitBaseBehaviorComponent.skillInfo.readableDescription,
                            unitBaseBehaviorComponent.GetStatistics(), unitBaseBehaviorComponent.GetCostInformation(),
                            unitBaseBehaviorComponent.skillInfo.productionHotkey, unitBaseBehaviorComponent.skillInfo.skillType, skillErrorInfo.errorMessage
                            );
                }
        }
        return newUIImages;
    }

    public List<UIImage> GetSelectedObjectsToUIImage(List<GameObject> selectegObjects)
    {
        List<UIImage> newUIImages = new List<UIImage>();
        foreach (GameObject unit in selectegObjects)
        {
            if (unit != null)
            {
                BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
                string name = baseBehaviorComponent.skillInfo.uniqueName;
                GetOrCreateUIImageToList(
                    ref newUIImages, name, baseBehaviorComponent.skillInfo.imagePath,
                    baseBehaviorComponent.skillInfo.readableName, baseBehaviorComponent.skillInfo.readableDescription, baseBehaviorComponent.GetStatistics(), new List<string>(), KeyCode.None,
                    baseBehaviorComponent.skillInfo.skillType, ""
                    );
            }
        }
        return newUIImages;
    }

    private void GetOrCreateUIImageToList(
        ref List<UIImage> list, string name, string imagePath,
        string _readName, string _readDescr, List<string> _tableStats, List<string> _costInfo, KeyCode _hotkey, SkillType _skillType = SkillType.Skill, string _errorMessage = ""
        )
    {
        if (!list.Exists(x => (x.name == name)))
            list.Add(new UIImage(name, 1, imagePath, _readName, _readDescr, _tableStats, _costInfo, _hotkey, _skillType, _errorMessage));
        else
            list.Find(x => (x.name == name)).count += 1;
    }
    
    BaseBehavior baseSelectedBehavior;
    UnitBehavior unitSelectedBehavior;
    int cacheCommandsType = 0;
    public void DisplayCommands(List<GameObject> selectegObjects)
    {
        if (UI.document.getElementsByClassName("commands").length <= 0)
            return;
        
        int newCommandsType = 0;
        BaseBehavior.BehaviorType unitBehaviorType = BaseBehavior.BehaviorType.None;

        if (selectegObjects.Count > 0)
        {
            baseSelectedBehavior = selectegObjects[0].GetComponent<BaseBehavior>();

            bool inProject = false;
            buildingBehaviorComponent = selectegObjects[0].GetComponent<BuildingBehavior>();
            if (buildingBehaviorComponent != null && buildingBehaviorComponent.state == BuildingBehavior.BuildingState.Project)
                inProject = true;

            if (baseSelectedBehavior.team == cameraController.team && baseSelectedBehavior.ownerId == cameraController.userId && (baseSelectedBehavior.live || inProject))
            {
                // Building commands
                if (buildingBehaviorComponent != null)
                    newCommandsType = 1;

                unitSelectedBehavior = selectegObjects[0].GetComponent<UnitBehavior>();
                // Units commands
                if (unitSelectedBehavior != null)
                {
                    newCommandsType = 2;
                    unitBehaviorType = unitSelectedBehavior.behaviorType;
                }
            }
        }
        if(newCommandsType != cacheCommandsType)
        {
            var commandsDiv = UI.document.getElementsByClassName("commands")[0];
            commandsDiv.innerHTML = "";
            if (newCommandsType > 0)
            {
                foreach (var commandsUIImage in commandsUIImages)
                {
                    if (commandsUIImage.Value != newCommandsType)
                        continue;

                    var createdImage = DrawInfo(parentElement: commandsDiv, unitImageInfo: commandsUIImage.Key, detailInfo: false, discriptable: true, drawImage: true, dinamicInfo: false);

                    createdImage.addEventListener("mousedown", delegate (MouseEvent e) {
                        cameraController.SendCommandToAllSelected(commandsUIImage.Key.name);
                    });
                }
                Debug.Log(unitBehaviorType);
                if(unitBehaviorType != BaseBehavior.BehaviorType.None)
                {
                    var createdImage = DrawInfo(parentElement: commandsDiv, unitImageInfo: behaviorUIImages[unitBehaviorType], detailInfo: false, discriptable: true, drawImage: true, dinamicInfo: false);

                    createdImage.addEventListener("mousedown", delegate (MouseEvent e) {
                        cameraController.SendCommandToAllSelected(behaviorUIImages[unitBehaviorType].name);
                    });
                }
            }
        }
        cacheCommandsType = newCommandsType;
    }

    private Dom.Element parentElement;
    public void DisplayUIImageObjects(
            string tableName,
            List<UIImage> newImages,
            ref List<UIImage> storageImages
        )
    {
        if (UI.document.getElementsByClassName(tableName).length <= 0)
            return;

        parentElement = UI.document.getElementsByClassName(tableName)[0];

        bool recreate = false;

        // Delete elements if description in table, but now in select more than one
        if (parentElement.getElementsByClassName("description").length > 0 && tableName == "center" && newImages.Count > 1)
            recreate = true;

        // Delete units which is selected no more
        if(!recreate)
        {
            foreach (UIImage unitImageInfo in newImages)
                if (!storageImages.Exists(x => (x.name == unitImageInfo.name)))
                {
                    recreate = true;
                    break;
                }

            if (!recreate)
                foreach (UIImage unitImageInfo in storageImages)
                    if (!newImages.Exists(x => (x.name == unitImageInfo.name)))
                    {
                        recreate = true;
                        break;
                    }
        }

        if (recreate || updateUI)
        {
            foreach (var element in parentElement.childNodes)
                if (element.className.Contains("proceduralContent"))
                {
                    element.remove();
                    break;
                }
            storageImages.Clear();
        }

        if (!recreate)
            return;

        var detailInfo = false;
        if (newImages.Count == 1 && tableName == "center")
            detailInfo = true;

        var discriptable = false;
        if (tableName == "right")
            discriptable = true;

        Dom.Element proceduralContent = null;
        var elementsPC = parentElement.getElementsByClassName("proceduralContent");
        if (elementsPC.length <= 0)
        {
            proceduralContent = UI.document.createElement("div");
            proceduralContent.className = "proceduralContent clckable";
            parentElement.appendChild(proceduralContent);
        }
        else
            proceduralContent = elementsPC[0];

        if (tableName == "right" && parentElement.getElementsByClassName("commands").length == 0)
        {
            Dom.Element commandsDiv = UI.document.createElement("div");
            commandsDiv.className = "clckable elementsBlock commands";
            proceduralContent.appendChild(commandsDiv);
        }

        // Create new selected units
        foreach (SkillType skillType in new SkillType[] { SkillType.None, SkillType.Skill, SkillType.Upgrade })
        {
            foreach (UIImage unitImageInfo in newImages.OrderBy(x => x.count).ToArray())
            {
                if (unitImageInfo.skillType != skillType)
                    continue;

                if (!storageImages.Exists(x => (x.name == unitImageInfo.name)) || recreate)
                {
                    if (detailInfo)
                    {
                        Dom.Element healthDiv = UI.document.createElement("div");
                        healthDiv.className = "healthback";
                        healthDiv.innerHTML = "<div class=\"health unitHealth\"></div>";
                        proceduralContent.appendChild(healthDiv);
                    }

                    string blockImagesName = "clckable elementsBlock elementsObject";
                    if (!detailInfo)
                    {
                        if (unitImageInfo.skillType == SkillType.Skill)
                            blockImagesName = "clckable elementsBlock elementsObject skills";
                        if (unitImageInfo.skillType == SkillType.Upgrade)
                            blockImagesName = "clckable elementsBlock elementsObject upgrades";
                    }

                    Dom.Element elementsBlock = null;
                    var allElementsBlock = parentElement.getElementsByClassName(blockImagesName);
                    if (allElementsBlock.length <= 0)
                    {
                        elementsBlock = UI.document.createElement("div");
                        elementsBlock.className = blockImagesName;
                        proceduralContent.appendChild(elementsBlock);
                    }
                    else
                        elementsBlock = allElementsBlock[0];

                    Dom.Element createdImage = DrawInfo(elementsBlock, unitImageInfo, detailInfo, discriptable, true, detailInfo);
                    storageImages.Add(unitImageInfo);
                    if (tableName == "center" && !detailInfo)
                    {
                        createdImage.addEventListener("mousedown", delegate (MouseEvent e) {
                            cameraController.SelectOnly(unitImageInfo.name);
                        });
                    }

                    if (tableName == "right")
                        createdImage.addEventListener("mousedown", delegate (MouseEvent e) {
                            cameraController.SendCommandToAllSelected(unitImageInfo.name);
                        });
                }
            }
        }
    }

    public Dom.Element DrawInfo(Dom.Element parentElement, UIImage unitImageInfo, bool detailInfo, bool discriptable, bool drawImage, bool dinamicInfo)
    {
        if (detailInfo)
        {
            Dom.Element statusticDiv = UI.document.createElement("div");
            statusticDiv.className = "title";
            statusticDiv.innerHTML = unitImageInfo.readableName;
            parentElement.appendChild(statusticDiv);
        }

        Dom.Element createdImage = null;
        if (drawImage)
        {   
            var elementContent = UI.document.createElement("div");
            elementContent.className = "elementsContent";
            parentElement.appendChild(elementContent);
            
            if (unitImageInfo.errorMessage != "")
                elementContent.className += " error";

            createdImage = UI.document.createElement("img");
            createdImage.className = "element clckable";
            createdImage.setAttribute("src", unitImageInfo.imagePath);
            elementContent.appendChild(createdImage);
            if(unitImageInfo.hotkey != KeyCode.None)
            {
                Dom.Element hotkeyDiv = UI.document.createElement("div");
                hotkeyDiv.className = "hotkey";
                hotkeyDiv.innerHTML = unitImageInfo.hotkey.ToString();
                elementContent.appendChild(hotkeyDiv);
            }
        }

        if (discriptable)
        {
            createdImage.className += " discriptable";
            createdImage.className += unitImageInfo.name;
        }

        if (detailInfo)
        {
            if (createdImage != null)
                createdImage.className += " detailInfo";

            Dom.Element infoDiv = UI.document.createElement("div");
            infoDiv.className = "info";
            parentElement.appendChild(infoDiv);

            Dom.Element statusticDiv = UI.document.createElement("div");
            statusticDiv.className = "description";
            statusticDiv.innerHTML = unitImageInfo.readableDescription;
            infoDiv.appendChild(statusticDiv);

            if (dinamicInfo)
            {
                Dom.Element dinamicInfoDiv = UI.document.createElement("div");
                dinamicInfoDiv.className = "dinamicInfo";
                parentElement.appendChild(dinamicInfoDiv);
            }

            if (unitImageInfo.costInfo.Count > 0)
            {
                Dom.Element detailCostsDiv = UI.document.createElement("div");
                detailCostsDiv.className = "stats costs";
                parentElement.appendChild(detailCostsDiv);
                foreach (string costInfo in unitImageInfo.costInfo)
                {
                    Dom.Element statsDiv = UI.document.createElement("p");
                    statsDiv.innerHTML = costInfo;
                    detailCostsDiv.appendChild(statsDiv);
                }
            }
            if (unitImageInfo.errorMessage != "")
            {
                Dom.Element errorMessageDiv = UI.document.createElement("div");
                errorMessageDiv.className = "errorMessage";
                errorMessageDiv.innerHTML = unitImageInfo.errorMessage;
                parentElement.appendChild(errorMessageDiv);
            }
            if (unitImageInfo.tableStatistics.Count > 0)
            {
                Dom.Element detailStatusticsDiv = UI.document.createElement("div");
                detailStatusticsDiv.className = "stats";
                parentElement.appendChild(detailStatusticsDiv);
                foreach (string statistic in unitImageInfo.tableStatistics)
                {
                    Dom.Element stats = UI.document.createElement("p");
                    stats.innerHTML = statistic;
                    detailStatusticsDiv.appendChild(stats);
                }
            }
        }
        return createdImage;
    }

    public void updateQueue(List<GameObject> unitsQuery, int qeryLimit, float buildTimer)
    {
        if (UI.document.getElementsByClassName("info").length == 0)
            return;

        var info = UI.document.getElementsByClassName("info")[0];

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
    public bool DisplayDescription(string className, List<UIImage> skillUIImages = null, bool commands = false)
    {
        checkedUIImages.Clear();
        if (skillUIImages != null)
            checkedUIImages.AddRange(skillUIImages);
        if (commands)
        {
            checkedUIImages.AddRange(commandsUIImages.Keys);
            checkedUIImages.AddRange(behaviorUIImages.Values);
        }

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
                    Dom.Element createdImage = DrawInfo(altInfo, skillUIImage, true, false, false, false);
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
