using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PowerUI;
using System.Text;
using System;
using System.IO;
using System.Text.RegularExpressions;

public class UIBaseScript : MonoBehaviour
{
    public class UIImage
    {
        public UIImage(string _name, int _count, string _imagePath, string _readName, string _readDescr, List<string> _tableStats, List<string> _costInfo, KeyCode _hotkey)
        {
            name = _name;
            count = _count;
            imagePath = _imagePath;
            readableName = _readName;
            readableDescription = _readDescr;
            tableStatistics = _tableStats;
            costInfo = _costInfo;
            hotkey = _hotkey;
        }
        public UIImage(string _name, string _readName, string _imagePath, string _readDescr, KeyCode _hotkey)
        {
            name = _name;
            readableName = _readName;
            imagePath = _imagePath;
            readableDescription = _readDescr;
            hotkey = _hotkey;
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
    }

    public List<UIImage> storageUIImages = new List<UIImage>();
    public List<UIImage> storageOrdersUIImages = new List<UIImage>();
    public List<UIImage> storageSkillUIImages = new List<UIImage>();

    private List<UIImage> commands = new List<UIImage>();

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        List<GameObject> selectegObjects = cameraController.GetSelectedObjects();

        List<UIImage> selectedUnitUIImage = GetSelectedObjectsToUIImage(selectegObjects);
        DisplayUIImageObjects("center", selectedUnitUIImage, ref storageUIImages);

        if (selectegObjects.Count == 1)
        {
            //Display query to build
            BaseBehavior unitBaseBehaviorComponent = selectegObjects[0].GetComponent<BaseBehavior>();
            BuildingBehavior buildingBehaviorComponent = selectegObjects[0].GetComponent<BuildingBehavior>();
            if (buildingBehaviorComponent != null && buildingBehaviorComponent.team == cameraController.team)
                updateQueue(buildingBehaviorComponent.unitsQuery, buildingBehaviorComponent.uqeryLimit, buildingBehaviorComponent.buildTimer);

            // Set health
            foreach (var element in UI.document.getElementsByClassName("unitHealth"))
                element.style.width = String.Format("{0:F0}%", unitBaseBehaviorComponent.health / unitBaseBehaviorComponent.maxHealth * 100);

            UnitBehavior unitBehaviorComponent = selectegObjects[0].GetComponent<UnitBehavior>();
            foreach (var element in UI.document.getElementsByClassName("dinamicInfo"))
            {
                element.innerHTML = "";

                if (unitBehaviorComponent != null && unitBehaviorComponent.resourceHold > 0)
                {
                    Dom.Element statusticDiv = UI.document.createElement("p");
                    if (unitBehaviorComponent.resourceType == BaseBehavior.ResourceType.Food)
                        statusticDiv.innerHTML = String.Format("Food: {0:F0}", unitBehaviorComponent.resourceHold);
                    if (unitBehaviorComponent.resourceType == BaseBehavior.ResourceType.Gold)
                        statusticDiv.innerHTML = String.Format("Gold: {0:F0}", unitBehaviorComponent.resourceHold);
                    if (unitBehaviorComponent.resourceType == BaseBehavior.ResourceType.Wood)
                        statusticDiv.innerHTML = String.Format("Wood: {0:F0}", unitBehaviorComponent.resourceHold);
                    element.appendChild(statusticDiv);
                }
                if (unitBaseBehaviorComponent.resourceCapacity > 0)
                {
                    Dom.Element statusticDiv = UI.document.createElement("p");
                    if (unitBaseBehaviorComponent.resourceCapacityType == BaseBehavior.ResourceType.Food)
                        statusticDiv.innerHTML = String.Format("Food: {0:F0}", unitBaseBehaviorComponent.resourceCapacity);
                    if (unitBaseBehaviorComponent.resourceCapacityType == BaseBehavior.ResourceType.Gold)
                        statusticDiv.innerHTML = String.Format("Gold: {0:F0}", unitBaseBehaviorComponent.resourceCapacity);
                    if (unitBaseBehaviorComponent.resourceCapacityType == BaseBehavior.ResourceType.Wood)
                        statusticDiv.innerHTML = String.Format("Wood: {0:F0}", unitBaseBehaviorComponent.resourceCapacity);
                    element.appendChild(statusticDiv);
                }
            }
        }

        List<UIImage> skillUIImages = GetSelectedSkillsOfObjectsToUIImage(selectegObjects);
        DisplayUIImageObjects("right", skillUIImages, ref storageSkillUIImages);

        DisplayCommands(selectegObjects);

        bool description = false;
        Dom.Element activeElement = PowerUI.CameraPointer.All[0].ActiveOver;
        if (activeElement != null)
        {
            if (activeElement.className.Contains("discriptable"))
                description = DisplayDescription(activeElement.className, skillUIImages);
            else if (activeElement.className.Contains("descrCommand"))
                description = DisplayDescription(activeElement.className, commands);
            else if (UnityEngine.Input.GetMouseButtonUp(0) && activeElement.className.Contains("detailInfo"))
            {
                if (selectegObjects.Count > 0)
                    cameraController.MoveCaeraToUnit(selectegObjects[0]);
            }
        }
        if (!description)
            DestroyDescription();
    }

    public void DisplayMessage(string message, int timer)
    {
        string[] args = new string[2] { message, timer.ToString() };
        UI.document.Run("CreateMessage", args);
    }

    public List<UIImage> GetSelectedSkillsOfObjectsToUIImage(List<GameObject> selectegObjects)
    {
        List<UIImage> newUIImages = new List<UIImage>();
        foreach (GameObject unit in selectegObjects)
        {
            BaseBehavior unitBaseBehaviorComponent = unit.GetComponent<BaseBehavior>();
            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            if (unitBaseBehaviorComponent.team != cameraController.team)
                continue;

            BuildingBehavior buildingBehaviorComponent = selectegObjects[0].GetComponent<BuildingBehavior>();
            if (buildingBehaviorComponent != null && buildingBehaviorComponent.state != BuildingBehavior.BuildingState.Builded)
                continue;

            //Skills
            UnitBehavior unitBehaviorComponent = unit.GetComponent<UnitBehavior>();
            if (unitBehaviorComponent != null && unitBehaviorComponent.live)
                foreach (BaseSkillScript skill in unitBehaviorComponent.skillList)
                {
                    string name = skill.uniqueName;
                    if(skill.skillObject != null)
                    {
                        BuildingBehavior skillBuildingBehaviorComponent = skill.skillObject.GetComponent<BuildingBehavior>();
                        if (skillBuildingBehaviorComponent != null)
                        {
                            GetOrCreateUIImageToList(
                                ref newUIImages, name, skillBuildingBehaviorComponent.imagePath,
                                skillBuildingBehaviorComponent.readableName, skillBuildingBehaviorComponent.readableDescription,
                                skillBuildingBehaviorComponent.GetStatistics(), skillBuildingBehaviorComponent.GetCostInformation(),
                                skillBuildingBehaviorComponent.productionHotkey
                                );
                            continue;
                        }
                    }
                    else
                    {
                        GetOrCreateUIImageToList(
                            ref newUIImages, name, skill.imagePath,
                            skill.readableName, skill.readableDescription,
                            skill.GetStatistics(), skill.GetCostInformation(),
                            skill.hotkey
                            );
                    }
                }
            //Created objects
            if (buildingBehaviorComponent != null && buildingBehaviorComponent.live)
                foreach (GameObject buildUnit in buildingBehaviorComponent.producedUnits)
                {
                    BaseBehavior buildUnitBaseBehaviorComponent = buildUnit.GetComponent<BaseBehavior>();
                    string name = buildUnitBaseBehaviorComponent.uniqueName;
                    GetOrCreateUIImageToList(
                        ref newUIImages, name, buildUnitBaseBehaviorComponent.imagePath,
                        buildUnitBaseBehaviorComponent.readableName, buildUnitBaseBehaviorComponent.readableDescription,
                        buildUnitBaseBehaviorComponent.GetStatistics(), buildUnitBaseBehaviorComponent.GetCostInformation(),
                        buildUnitBaseBehaviorComponent.productionHotkey
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
            BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
            string name = baseBehaviorComponent.uniqueName;
            GetOrCreateUIImageToList(
                ref newUIImages, name, baseBehaviorComponent.imagePath,
                baseBehaviorComponent.readableName, baseBehaviorComponent.readableDescription, baseBehaviorComponent.GetStatistics(), new List<string>(), KeyCode.None
                );
        }
        return newUIImages;
    }

    private void GetOrCreateUIImageToList(
        ref List<UIImage> list, string name, string imagePath,
        string _readName, string _readDescr, List<string> _tableStats, List<string> _costInfo, KeyCode _hotkey
        )
    {
        if (!list.Exists(x => (x.name == name)))
            list.Add(new UIImage(name, 1, imagePath, _readName, _readDescr, _tableStats, _costInfo, _hotkey));
        else
            list.Find(x => (x.name == name)).count += 1;
    }

    public void DisplayCommands(List<GameObject> selectegObjects)
    {
        List<UIImage> newCommands = new List<UIImage>();

        var index = 0;
        bool recreate = false;
        if (commands.Count != newCommands.Count)
            recreate = true;
        else
            foreach (UIImage newCommand in newCommands)
            {
                if (commands[index].name != newCommand.name)
                    recreate = true;
                index++;
            }

        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (selectegObjects.Count > 0)
        {
            BaseBehavior baseBehavior = selectegObjects[0].GetComponent<BaseBehavior>();

            bool inProject = false;
            BuildingBehavior buildingBehaviorComponent = selectegObjects[0].GetComponent<BuildingBehavior>();
            if (buildingBehaviorComponent != null && buildingBehaviorComponent.state == BuildingBehavior.BuildingState.Project)
                inProject = true;

            if (baseBehavior.team == cameraController.team && (baseBehavior.live || inProject))
            {
                // Building commands
                if (buildingBehaviorComponent != null)
                {
                    newCommands.Add(new UIImage("stop", "Stop", "commands/stop-sign.png", "Stop command.", KeyCode.H));
                }

                UnitBehavior unitBehavior = selectegObjects[0].GetComponent<UnitBehavior>();
                // Units commands
                if (unitBehavior != null)
                {
                    newCommands.Add(new UIImage("stop", "Stop", "commands/stop-sign.png", "Stop command.", KeyCode.H));
                    newCommands.Add(new UIImage("attack", "Go and attack", "commands/arrow-scope.png", "Move to target and attack enemies on the way.", KeyCode.A));

                    if (unitBehavior.behaviorType == BaseBehavior.BehaviorType.Aggressive)
                        newCommands.Add(new UIImage("behaviorType1", "Aggressive behavior", "commands/caveman.png", "The unit will attack nearest enemy targets.", KeyCode.T));
                    if (unitBehavior.behaviorType == BaseBehavior.BehaviorType.Counterattack)
                        newCommands.Add(new UIImage("behaviorType2", "Counterattack behavior", "commands/wide-arrow-dunk.png", "The unit will counterattack nearest enemy targets.", KeyCode.T));
                    if (unitBehavior.behaviorType == BaseBehavior.BehaviorType.Hold)
                        newCommands.Add(new UIImage("behaviorType3", "Hold behavior", "commands/static-guard.png", "Unit will hold the position, and not attack back.", KeyCode.T));
                    if (unitBehavior.behaviorType == BaseBehavior.BehaviorType.Run)
                        newCommands.Add(new UIImage("behaviorType4", "Run behavior", "commands/run.png", "A unit will run away if attacked.", KeyCode.T));
                }
            }
        }

        if (UI.document.getElementsByClassName("commands").length <= 0)
            return;

        if (recreate || UI.document.getElementsByClassName("commands")[0].childCount < newCommands.Count)
        {

            var commandsDiv = UI.document.getElementsByClassName("commands")[0];
            commandsDiv.innerHTML = "";
            commands.Clear();
            foreach (var newCommand in newCommands)
            {
                var commandContent = UI.document.createElement("div");
                commandContent.className = "elementsContent";
                commandsDiv.appendChild(commandContent);

                commands.Add(newCommand);
                var createdImage = UI.document.createElement("img");
                createdImage.className = String.Format("element clckable descrCommand {0}", newCommand.name);
                createdImage.setAttribute("src", newCommand.imagePath);
                commandContent.appendChild(createdImage);

                if (newCommand.hotkey != KeyCode.None)
                {
                    Dom.Element hotkeyDiv = UI.document.createElement("div");
                    hotkeyDiv.className = "hotkey";
                    hotkeyDiv.innerHTML = newCommand.hotkey.ToString();
                    commandContent.appendChild(hotkeyDiv);
                }

                createdImage.addEventListener("mousedown", delegate (MouseEvent e) {
                    cameraController.SendCommandToAllSelected(newCommand.name);
                });
            }
        }
    }

    public void DisplayUIImageObjects(
            string tableName,
            List<UIImage> newImages,
            ref List<UIImage> storageImages
        )
    {
        if (UI.document.getElementsByClassName(tableName).length <= 0)
            return;

        var parentElement = UI.document.getElementsByClassName(tableName)[0];

        bool recreate = false;

        // Delete elements if description in table, but now in select more than one
        if (parentElement.getElementsByClassName("description").length > 0 && tableName == "center" && newImages.Count > 1)
            recreate = true;

        // Delete units which is selected no more
        foreach (UIImage unitImageInfo in storageImages.ToArray())
            if (!newImages.Exists(x => (x.name == unitImageInfo.name)))
                recreate = true;

        if (recreate)
        {
            foreach (var element in parentElement.childNodes)
                if (element.className.Contains("proceduralContent"))
                {
                    element.remove();
                    break;
                }
            storageImages.Clear();
        }
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
        foreach (UIImage unitImageInfo in newImages.OrderBy(x => x.count).ToArray())
        {
            if (!storageImages.Exists(x => (x.name == unitImageInfo.name)) || recreate)
            {
                if (detailInfo)
                {
                    Dom.Element healthDiv = UI.document.createElement("div");
                    healthDiv.className = "healthback";
                    healthDiv.innerHTML = "<div class=\"health unitHealth\"></div>";
                    proceduralContent.appendChild(healthDiv);
                }
                
                Dom.Element elementsBlock = null;
                var allElementsBlock = parentElement.getElementsByClassName("elementsObject");
                if (allElementsBlock.length <= 0)
                {
                    elementsBlock = UI.document.createElement("div");
                    elementsBlock.className = "elementsBlock elementsObject";
                    proceduralContent.appendChild(elementsBlock);
                }
                else
                    elementsBlock = allElementsBlock[0];

                CameraController cameraController = Camera.main.GetComponent<CameraController>();
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
                BaseBehavior unitBaseBehaviorComponent = unit.GetComponent<BaseBehavior>();
                Dom.Element progressBaseDiv = UI.document.createElement("div");
                progressBaseDiv.className = "production clckable";
                infoDiv.appendChild(progressBaseDiv);

                Dom.Element elementDiv = UI.document.createElement("img");
                elementDiv.setAttribute("src", unitBaseBehaviorComponent.imagePath);
                elementDiv.className = String.Format("clckable {0}", index);
                progressBaseDiv.appendChild(elementDiv);

                CameraController cameraController = Camera.main.GetComponent<CameraController>();
                if (index == 0)
                {
                    float time = unitBaseBehaviorComponent.timeToBuild;
                    Dom.Element progressDiv = UI.document.createElement("div");
                    progressDiv.className = String.Format("clckable {0}", index);
                    progressDiv.style.width = String.Format("{0:F0}%", buildTimer / unitBaseBehaviorComponent.timeToBuild * 100);
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

    public bool DisplayDescription(string className, List<UIImage> skillUIImages)
    {
        foreach (UIImage skillUIImage in skillUIImages)
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
                //altInfo.style.left = String.Format("{0}px", x);
                //altInfo.style.top = String.Format("{0}px", y);
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
