using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PowerUI;
using System.Text;
using Photon.Pun;
using GangaGame;

namespace UISpace
{
    public class UIBaseScript : MonoBehaviour
    {
        CameraController cameraController;
        Dictionary<BaseBehavior.BehaviorType, SkillInfo> behaviorUIImages = new Dictionary<BaseBehavior.BehaviorType, SkillInfo>();
        List<SkillInfo> commandsBuildingsUIImages = new List<SkillInfo>();
        List<SkillInfo> commandsUnitsUIImages = new List<SkillInfo>();
        // Use this for initialization
        void Start()
        {
            cameraController = Camera.main.GetComponent<CameraController>();

            // Buildings
            commandsBuildingsUIImages.Add(new SkillInfo("stop", "Stop", "commands/stop-sign.png", "Stop command.", KeyCode.H));

            // Units
            commandsUnitsUIImages.Add(new SkillInfo("stop", "Stop", "commands/stop-sign.png", "Stop command.", KeyCode.H));
            commandsUnitsUIImages.Add(new SkillInfo("attack", "Go and attack", "commands/arrow-scope.png", "Move to target and attack enemies on the way.", KeyCode.A));

            // Behavior commands
            behaviorUIImages.Add(BaseBehavior.BehaviorType.Aggressive, new SkillInfo("behaviorType1", "Aggressive behavior", "commands/caveman.png", "The unit will attack nearest enemy targets.", KeyCode.T));
            behaviorUIImages.Add(BaseBehavior.BehaviorType.Counterattack, new SkillInfo("behaviorType2", "Counterattack behavior", "commands/wide-arrow-dunk.png", "The unit will counterattack nearest enemy targets.", KeyCode.T));
            behaviorUIImages.Add(BaseBehavior.BehaviorType.Hold, new SkillInfo("behaviorType3", "Hold behavior", "commands/static-guard.png", "Unit will hold the position, and not attack back.", KeyCode.T));
            behaviorUIImages.Add(BaseBehavior.BehaviorType.Run, new SkillInfo("behaviorType4", "Run behavior", "commands/run.png", "A unit will run away if attacked.", KeyCode.T));

            checkedUIImages.AddRange(commandsBuildingsUIImages);
            checkedUIImages.AddRange(commandsUnitsUIImages);
            checkedUIImages.AddRange(behaviorUIImages.Values);
        }

        private bool updateUI = false;
        public void UpdateUI()
        {
            UnityEngine.Profiling.Profiler.BeginSample("p UpdateUI"); // Profiler

            UI.document.Run("ClearInfo");

            bool userObjects = false;
            if (cameraController.selectedObjects.Count > 0)
            {
                unitBaseBehaviorComponent = cameraController.selectedObjects[0].GetComponent<BaseBehavior>();
                userObjects = unitBaseBehaviorComponent.team == cameraController.team && unitBaseBehaviorComponent.ownerId == cameraController.userId;
            }

            DisplayObjectsInfo(cameraController.selectedObjects);
            if (userObjects)
            {
                DisplayCommands(cameraController.selectedObjects);

                bool displaySkills = true;
                BuildingBehavior unitBuildingBehavior = cameraController.selectedObjects[0].GetComponent<BuildingBehavior>();
                if (unitBuildingBehavior != null && unitBuildingBehavior.GetState() != BuildingBehavior.BuildingState.Builded)
                    displaySkills = false;

                if (displaySkills)
                    DisplaySkillsInfo(cameraController.selectedObjects);
            }

            if (cameraController.selectedObjects.Count == 1)
            {
                DisplayDetailInfo(cameraController.selectedObjects[0]);
                if (userObjects)
                    UpdateQueue(cameraController.selectedObjects[0]);
            }
            else
                if(userObjects)
                    UpdateQueue();

            UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        }

        public void UpdateSkillsInfo()
        {
            UI.document.Run("ClearElementsBlock", "skills");
            UI.document.Run("ClearElementsBlock", "upgrades");
            DisplaySkillsInfo(cameraController.selectedObjects);
        }

        public void UpdateCommands()
        {
            UI.document.Run("ClearElementsBlock", "commands");
            DisplayCommands(cameraController.selectedObjects);
        }

        // Cache
        BaseBehavior unitBaseBehaviorComponent = null;
        UnitBehavior unitBehaviorComponent = null;
        BuildingBehavior buildingBehaviorComponent = null;
        BaseSkillScript skillScript = null;

        //void Update()
        //{
        //}

        private void OnElementMouseDown(MouseEvent mouseEvent)
        {
            CameraController.SendCommandToAllSelected(mouseEvent.srcElement.id);
        }

        private void OnElementOnClick(MouseEvent mouseEvent)
        {
            if (mouseEvent.srcElement.className.Contains("unit"))
            {
                GameObject unit = PhotonNetwork.GetPhotonView(int.Parse(mouseEvent.srcElement.id)).gameObject;
                cameraController.DeselectAllUnits();

                UnitSelectionComponent selection = unit.GetComponent<UnitSelectionComponent>();
                selection.SetSelect(false);

                MapScript.MoveCameraToPoint(unit.transform.position);
            }
        }

        private void SelectObject(MouseEvent mouseEvent)
        {
            cameraController.MoveCaeraToUnit(cameraController.selectedObjects[0]);
        }

        private void SelectOnly(MouseEvent mouseEvent)
        {
            cameraController.SelectOnly(mouseEvent.srcElement.id);
        }

        private void OnElementOnMouseOver(MouseEvent mouseEvent)
        {
            DisplayDescription(mouseEvent.srcElement.id);
        }

        private void OnElementOnMouseOut(MouseEvent mouseEvent)
        {
            DestroyDescription();
        }

        List<string> displayedNames = new List<string>();

        public void DisplayObjectsInfo(List<GameObject> selectedObjects)
        {
            UnityEngine.Profiling.Profiler.BeginSample("p DisplayObjectsInfo"); // Profiler
            if (selectedObjects.Count == 1)
            {
                unitBaseBehaviorComponent = selectedObjects[0].GetComponent<BaseBehavior>();
                HtmlElement createdImage = DrawInfo(
                    "cblock", skillInfo: unitBaseBehaviorComponent.skillInfo, detailInfo: true, dinamicInfo: true, 
                    tableStatistics: unitBaseBehaviorComponent.GetStatistics().ToArray(), drawHP: true);
                createdImage.onclick = SelectObject;
            }
            else
            {
                displayedNames.Clear();
                foreach (GameObject selectedObject in selectedObjects)
                {
                    unitBaseBehaviorComponent = selectedObject.GetComponent<BaseBehavior>();
                    if (!displayedNames.Contains(unitBaseBehaviorComponent.skillInfo.uniqueName))
                    {
                        HtmlElement createdImage = DrawInfo(
                            "cblock", skillInfo: unitBaseBehaviorComponent.skillInfo, detailInfo: false, 
                            tableStatistics: unitBaseBehaviorComponent.GetStatistics().ToArray());
                        displayedNames.Add(unitBaseBehaviorComponent.skillInfo.uniqueName);
                        createdImage.onclick = SelectOnly;
                    }
                }
            }
            UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        }

        BaseBehavior skillObjectBaseBehaviorComponent = null;
        SkillErrorInfo skillErrorInfo;
        Dictionary<GameObject, GameObject> skillsCache = new Dictionary<GameObject, GameObject>();
        string[] tableStatistics;
        string[] costInfo;
        public void DisplaySkillsInfo(List<GameObject> selectedObjects)
        {
            UnityEngine.Profiling.Profiler.BeginSample("p DisplayObjectsInfo"); // Profiler
            skillsCache.Clear();
            displayedNames.Clear();
            foreach (GameObject selectedObject in selectedObjects)
            {
                foreach (SkillType skillTypeFor in new SkillType[] { SkillType.Skill, SkillType.Upgrade } )
                {
                    unitBaseBehaviorComponent = selectedObject.GetComponent<BaseBehavior>();
                    foreach (GameObject skillObject in unitBaseBehaviorComponent.skillList.Concat(unitBaseBehaviorComponent.producedUnits))
                    {
                        skillErrorInfo = BaseSkillScript.GetSkillErrorInfo(selectedObject, skillObject);
                        if (!skillErrorInfo.isDisplayedAsSkill)
                            continue;

                        SkillInfo skillInfo = null;
                        string blockImagesName = "";
                        skillObjectBaseBehaviorComponent = skillObject.GetComponent<BaseBehavior>();
                        skillScript = skillObject.GetComponent<BaseSkillScript>();
                        if (skillObjectBaseBehaviorComponent != null && !displayedNames.Contains(skillObjectBaseBehaviorComponent.skillInfo.uniqueName))
                        {
                            skillInfo = skillObjectBaseBehaviorComponent.skillInfo;
                            costInfo = skillObjectBaseBehaviorComponent.GetCostInformation().ToArray();
                            tableStatistics = skillObjectBaseBehaviorComponent.GetStatistics().ToArray();
                        }
                        else if (skillScript != null && !displayedNames.Contains(skillScript.skillInfo.uniqueName))
                        {
                            skillInfo = skillScript.skillInfo;
                            costInfo = skillScript.GetCostInformation().ToArray();
                            tableStatistics = skillScript.GetStatistics().ToArray();
                        }
                        else
                            continue;

                        if (skillTypeFor != skillInfo.skillType)
                            continue;

                        if (skillInfo.skillType == SkillType.Skill)
                            blockImagesName = "skills";
                        else if (skillInfo.skillType == SkillType.Upgrade)
                            blockImagesName = "upgrades";

                        if (skillInfo != null)
                        {
                            HtmlElement createdImage = DrawInfo("rblock", blockImagesName: blockImagesName, skillInfo: skillInfo, detailInfo: false,
                                costInfo: costInfo, tableStatistics: tableStatistics, hotkey: skillInfo.productionHotkey.ToString(), errorMessage: skillErrorInfo.errorMessage);
                            displayedNames.Add(skillInfo.uniqueName);
                            skillsCache.Add(skillObject, selectedObject);
                            createdImage.onmousedown = OnElementMouseDown;

                            // Description
                            createdImage.onmouseover = OnElementOnMouseOver;
                            createdImage.onmouseout = OnElementOnMouseOut;

                            createdImage.onclick = OnElementOnClick;
                        }
                    }
                }
            }
            UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        }

        public void DisplayCommands(List<GameObject> selectedObjects)
        {
            UnityEngine.Profiling.Profiler.BeginSample("p UpdateCommands"); // Profiler
            if (selectedObjects.Count == 0)
                return;

            List<SkillInfo> infoCommands = null;
            unitBaseBehaviorComponent = selectedObjects[0].GetComponent<BaseBehavior>();

            if (selectedObjects[0].GetComponent<BuildingBehavior>() != null)
                infoCommands = commandsBuildingsUIImages;
            else
                infoCommands = commandsUnitsUIImages;

            foreach (var infoCommand in infoCommands)
            {
                HtmlElement createdImage = DrawInfo("rblock", blockImagesName: "commands", skillInfo: infoCommand, hotkey: infoCommand.productionHotkey.ToString());

                createdImage.onmouseover = OnElementOnMouseOver;
                createdImage.onmouseout = OnElementOnMouseOut;
                createdImage.onmousedown = OnElementMouseDown;
            }
            if (unitBaseBehaviorComponent.behaviorType != BaseBehavior.BehaviorType.None)
            {
                var skillInfo = behaviorUIImages[unitBaseBehaviorComponent.behaviorType];
                HtmlElement createdImage = DrawInfo("rblock", blockImagesName: "commands", skillInfo: skillInfo, hotkey: skillInfo.productionHotkey.ToString());

                createdImage.onmouseover = OnElementOnMouseOver;
                createdImage.onmouseout = OnElementOnMouseOut;
                createdImage.onmousedown = OnElementMouseDown;
            }
            UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        }

        public HtmlElement DrawInfo(
            string parentElementString, SkillInfo skillInfo, string blockImagesName = "", bool drawImage = true, string errorMessage = "",
            string hotkey = "", bool detailInfo = false, bool dinamicInfo = false, string[] costInfo = null, string[] tableStatistics = null, bool drawHP = false)
        {
            UnityEngine.Profiling.Profiler.BeginSample("p DrawInfo"); // Profiler
            object createdImageObject = UI.document.Run("DrawInfo", parentElementString, blockImagesName, detailInfo, drawImage, dinamicInfo, skillInfo.uniqueName, skillInfo.readableName, skillInfo.readableDescription,
            errorMessage, skillInfo.imagePath, hotkey, costInfo, tableStatistics, drawHP);

            UnityEngine.Profiling.Profiler.EndSample(); // Profiler
            return (HtmlElement)((Jint.Native.JsValue)createdImageObject).ToObject();
        }

        public void DisplayDetailInfo(GameObject unit)
        {
            UnityEngine.Profiling.Profiler.BeginSample("p DisplayDetailInfo"); // Profiler
            if (unit == null)
                return;

            unitBaseBehaviorComponent = unit.GetComponent<BaseBehavior>();

            // Set health
            foreach (var element in UI.document.getElementsByClassName("unitHealth"))
                element.style.width = new StringBuilder(16).AppendFormat("{0:F0}%", unitBaseBehaviorComponent.health / unitBaseBehaviorComponent.maxHealth * 100).ToString();

            foreach (var element in UI.document.getElementsByClassName("dinamicInfo"))
            {
                element.innerHTML = "";

                BaseBehavior.ResourceType resourseDisplayType = BaseBehavior.ResourceType.None;
                string gatgherInfo = "";
                float resources = 0.0f;
                if (unitBaseBehaviorComponent != null && unitBaseBehaviorComponent.resourceHold > 0)
                {
                    resources = unitBaseBehaviorComponent.resourceHold;
                    resourseDisplayType = unitBaseBehaviorComponent.resourceType;

                    UnitBehavior.ResourceGatherInfo resourceGatherInfo = unit.GetComponent<UnitBehavior>().GetResourceFarmByType(unitBaseBehaviorComponent.interactType, resourseDisplayType);
                    gatgherInfo = new StringBuilder(30).AppendFormat(" ({0:F2} per second. Maximum: {1:F1})", resourceGatherInfo.gatherPerSecond, resourceGatherInfo.maximumCapacity).ToString();
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
                        statusticDiv.innerHTML = new StringBuilder(50).AppendFormat("Food: {0:F0} {1}", resources, gatgherInfo).ToString();
                    else if (resourseDisplayType == BaseBehavior.ResourceType.Gold)
                        statusticDiv.innerHTML = new StringBuilder(50).AppendFormat("Gold: {0:F0} {1}", resources, gatgherInfo).ToString();
                    else if (resourseDisplayType == BaseBehavior.ResourceType.Wood)
                        statusticDiv.innerHTML = new StringBuilder(50).AppendFormat("Wood: {0:F0} {1}", resources, gatgherInfo).ToString();
                    element.appendChild(statusticDiv);
                }
            }
            UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        }

        public void DisplayMessage(string message, int timer, string uniqueMessage = "")
        {
            if (uniqueMessage != "" && UI.document.getElementsByClassName(uniqueMessage).length > 0)
                foreach (var element in UI.document.getElementsByClassName(uniqueMessage))
                    element.remove();

            string[] args = new string[3] { message, timer.ToString(), uniqueMessage };
            UI.document.Run("CreateMessage", args);
        }

        private void DeleteQueue()
        {
            UI.document.Run("DeleteQueue");
        }

        public void UpdateQueue(GameObject unit = null)
        {
            UnityEngine.Profiling.Profiler.BeginSample("p UpdateQueue"); // Profiler
            if (unit != null)
                unitBaseBehaviorComponent = unit.GetComponent<BaseBehavior>();

            DeleteQueue();

            if (unit != null && unitBaseBehaviorComponent.productionQuery.Count > 0)
            {
                var info = UI.document.getElementsByClassName("infoBlock")[0];

                Dom.Element infoDiv = UI.document.createElement("div");
                infoDiv.className = "query clckable";
                info.appendChild(infoDiv);

                var index = 0;
                foreach (var unitQueue in unitBaseBehaviorComponent.productionQuery)
                {
                    BaseBehavior skillBaseBehaviorComponent = unitQueue.GetComponent<BaseBehavior>();
                    BaseSkillScript skillScript = unitQueue.GetComponent<BaseSkillScript>();

                    string imagePath = "";
                    float timeToBuild = 0.0f;
                    if (skillBaseBehaviorComponent != null)
                    {
                        imagePath = skillBaseBehaviorComponent.skillInfo.imagePath;
                        timeToBuild = skillBaseBehaviorComponent.skillInfo.timeToBuild;
                    }
                    if (skillScript != null)
                    {
                        imagePath = skillScript.skillInfo.imagePath;
                        timeToBuild = skillScript.skillInfo.timeToBuild;
                    }

                    object createdObject = UI.document.Run("CreateQueueElement", imagePath, index, timeToBuild, unitBaseBehaviorComponent.buildTimer);
                    index += 1;
                    HtmlElement createdQueue = (HtmlElement)((Jint.Native.JsValue)createdObject).ToObject();
                    createdQueue.onmousedown = RemoveQueueElementFromSelected;
                }
            }
            UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        }

        public void RemoveQueueElementFromSelected(MouseEvent mouseEvent)
        {
            int index = int.Parse(mouseEvent.srcElement.id);
            BuildingBehavior buildingBehavior = cameraController.selectedObjects[0].GetComponent<BuildingBehavior>();
            buildingBehavior.DeleteFromProductionQuery(index);
        }

        string descriptionActiveClass = "";
        List<SkillInfo> checkedUIImages = new List<SkillInfo>();
        public bool DisplayDescription(string className, List<SkillInfo> skillUIImages = null)
        {
            UnityEngine.Profiling.Profiler.BeginSample("p DisplayDescription"); // Profiler
            foreach (SkillInfo skillUIImage in checkedUIImages)
            {
                if (className.Contains(skillUIImage.uniqueName))
                {
                    if (descriptionActiveClass == skillUIImage.uniqueName)
                        return true;

                    if (descriptionActiveClass != "" && descriptionActiveClass != skillUIImage.uniqueName)
                        DestroyDescription();

                    var containerDescription = UI.document.getElementsByClassName("containerDescription")[0];
                    Dom.Element altInfo = UI.document.createElement("div");
                    altInfo.className = new StringBuilder(30).AppendFormat("content altInfo {0}", skillUIImage.uniqueName).ToString();
                    containerDescription.appendChild(altInfo);
                    DrawInfo("altInfo", skillUIImage, drawImage: false, detailInfo: true);
                    descriptionActiveClass = skillUIImage.uniqueName;
                    return true;
                }
            }
            foreach (var skillElement in skillsCache)
            {
                unitBaseBehaviorComponent = skillElement.Key.GetComponent<BaseBehavior>();
                skillScript = skillElement.Key.GetComponent<BaseSkillScript>();

                SkillInfo skillInfo = null;
                if (unitBaseBehaviorComponent != null)
                {
                    skillInfo = unitBaseBehaviorComponent.skillInfo;
                    costInfo = unitBaseBehaviorComponent.GetCostInformation().ToArray();
                    tableStatistics = unitBaseBehaviorComponent.GetStatistics().ToArray();
                }
                if (skillScript != null)
                {
                    skillInfo = skillScript.skillInfo;
                    costInfo = skillScript.GetCostInformation().ToArray();
                    tableStatistics = skillScript.GetStatistics().ToArray();
                }

                if (className.Contains(skillInfo.uniqueName))
                {
                    if (descriptionActiveClass == skillInfo.uniqueName)
                        return true;

                    if (descriptionActiveClass != "" && descriptionActiveClass != skillInfo.uniqueName)
                        DestroyDescription();
                    
                    skillErrorInfo = BaseSkillScript.GetSkillErrorInfo(skillElement.Value, skillElement.Key);

                    var containerDescription = UI.document.getElementsByClassName("containerDescription")[0];
                    Dom.Element altInfo = UI.document.createElement("div");
                    altInfo.className = "content altInfo";
                    containerDescription.appendChild(altInfo);

                    DrawInfo("altInfo", skillInfo, drawImage: false, detailInfo: true,
                        errorMessage: skillErrorInfo.errorMessage,
                        tableStatistics: tableStatistics,
                        costInfo: costInfo);
                    
                    displayedNames.Add(skillInfo.uniqueName);
                    descriptionActiveClass = skillInfo.uniqueName;
                    return true;
                }
            }
            UnityEngine.Profiling.Profiler.EndSample(); // Profiler
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

            descriptionActiveClass = "";
        }

        public static HtmlElement CreateInfoButton(string className, int count, string hotkey, string img)
        {
            object createdObject = UI.document.Run("CreateInfoButton", className, count, hotkey, img);
            return (HtmlElement)((Jint.Native.JsValue)createdObject).ToObject();
        }
    }
}