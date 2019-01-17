﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerUI;
using System.Text;
using System;
using System.Linq;
using Photon.Pun;
using UnityEngine.AI;
using GangaGame;

public class BaseBehavior : MonoBehaviourPunCallbacks, IPunObservable, ISkillInterface
{
    private PhotonView photonView;

    #region Unit info

    [Header("Unit info")]
    public int team = 1;
    public int tear = 0;
    public string ownerId = "1";
    public float maxHealth = 100.0f;
    public float health;
    public bool live = true;
    public GameObject pointToInteract;

    public List<GameObject> tearDisplay;

    public bool debug = false;

    public bool isWalkAround = false;
    [HideInInspector]
    public float timeToWalk = 0.0f;
    [HideInInspector]
    public float timerToWalk = 0.0f;
    public float visionDistanve = 10.0f;

    public SkillInfo _skillInfo;
    public SkillInfo skillInfo { get { return _skillInfo; } set { _skillInfo = value; } }
    public SkillConditionType _skillConditions;
    public SkillConditionType skillConditions { get { return _skillConditions; } set { _skillConditions = value; } }

    [HideInInspector]
    public Vector3 unitPosition = new Vector3();

    #endregion

    #region Fight info

    public enum InteractigType { None, Attacking, Bulding, Gathering, CuttingTree, Farming };
    public InteractigType interactType = InteractigType.None;

    [HideInInspector]
    public float attackTimer;
    [HideInInspector]
    public float attackDelayTimer;
    [HideInInspector]
    public bool attacked;

    public enum AttackType { Stabbing, Cutting, Biting, Magic, None };
    public enum BehaviorType { Run, Hold, Counterattack, Aggressive };
    [System.Serializable]
    public class UnitStatistic
    {
        public float damage = 10.0f;
        public float rangeAttack = 2.0f;
        public float attackTime = 2.0f;
        public float attackDelay = 0.5f;
        public bool damageAfterTimer = true;

        public AttackType attackType = AttackType.None;
        public float stabbingResist = 0.0f;
        public float cuttingResist = 0.0f;
        public float bitingResist = 0.0f;
        public float magicResist = 0.0f;

        public float agrRange = 20.0f;
        public float alertRange = 10.0f;
    }
    public UnitStatistic defaultStatistic;
    public GameObject actionEffect;
    public BehaviorType behaviorType = BehaviorType.Counterattack;

    #endregion

    #region Effects info

    [Header("Effects info")]
    public GameObject bloodEffect;
    public List<GameObject> bloodOnBroundEffects = new List<GameObject>();
    public GameObject destroyEffectPrefab;
    public GameObject destroyEffectHandler;

    #endregion

    #region Destroy Info

    [Header("Destroy Info")]
    public float timeToDestroy = 30.0f;
    public float destroyAfter = 7.0f;
    public float toUndergroundSpeed = 1.0f;
    // [HideInInspector]
    public bool sendToDestroy = false;

    private bool sendToUnderground = false;
    public float delayToMoveUnderground = 3.0f;
    private float countToMoveUnderground = 0.0f;

    [System.Serializable]
    public class ToolInfo
    {
        public GameObject prefab;
        public GameObject holder;
    }
    public ToolInfo weapon;

    [HideInInspector]
    public NavMeshAgent agent;
    public NavMeshObstacle obstacle;
    [HideInInspector]
    public Animator anim;
    //[HideInInspector]
    public GameObject target;
    [HideInInspector]
    public Vector3 tempDestinationPoint = Vector3.zero;
    [HideInInspector]
    public Vector3 previousPosition;
    [HideInInspector]
    public float curSpeed;
    [HideInInspector]
    public CameraController cameraController;

    #endregion

    #region Skills info

    [Header("Skills")]
    public List<GameObject> skillList = new List<GameObject>();

    #endregion

    #region Units production

    [Header("Units production")]
    public List<GameObject> producedUnits = new List<GameObject>();
    public GameObject spawnPoint;
    [HideInInspector]
    public Vector3 spawnTarget = Vector3.zero;
    [HideInInspector]
    public GameObject spawnTargetObject;
    [HideInInspector]
    public float buildTimer = 0.0f;
    [HideInInspector]
    public List<GameObject> productionQuery = new List<GameObject>();

    #endregion

    #region Stuff

    [Header("Stuff")]
    public TextAsset HTMLHealthFile;
    public Vector3 InfoHTMLOffset = new Vector3();
    public GameObject pointMarkerPrefab;
    public Vector2Int HTMLHealthSize = new Vector2Int(100, 100);
    private float tempHealth;
    public GameObject visionToolPrefab;
    public int visionCount = 0;
    public int uqeryLimit = 5;
    [HideInInspector]
    public bool beenSeen = false;
    [HideInInspector]
    public GameObject holderObject;
    [HideInInspector]
    public float timerToCreateHolder = 0.0f;
    [HideInInspector]
    public ToolInfo holderToolInfo;
    [HideInInspector]
    public float tempSpeed;
    [HideInInspector]
    public int attackedTeam = 0;
    [HideInInspector]
    public WorldUI objectUIInfo;
    [HideInInspector]
    public GameObject pointMarker;
    [HideInInspector]
    public bool canBeSelected = true;
    [HideInInspector]
    public GameObject visionTool;
    [HideInInspector]
    public UnitSelectionComponent unitSelectionComponent;

    [HideInInspector]
    public List<object> queueCommands = new List<object>();

    #endregion

    #region Gatering hold info

    public enum ResourceType { None, Food, Gold, Wood };
    [Header("Gatering hold info")]
    public ResourceType resourceCapacityType = ResourceType.None;
    public float resourceCapacity = 0.0f;
    public bool resourceEndless = false;
    public enum SourceType { Default, Farm, GoldMine, Wood };
    public SourceType sourceType = SourceType.Default;
    public List<ResourceType> storedResources = new List<ResourceType>();

    #endregion

    #region Photon

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.interactType);
        }
        else
        {
            this.interactType = (InteractigType)stream.ReceiveNext();
        }
    }

    #endregion

    [PunRPC]
    public void ChangeOwner(string newOwnerId, int newTeam)
    {
        ownerId = newOwnerId;
        team = newTeam;
    }

    Renderer[] renders;
    virtual public void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        unitSelectionComponent = GetComponent<UnitSelectionComponent>();
        photonView = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        if (agent != null && obstacle != null)
            obstacle.enabled = false;

        unitPosition = transform.position;
        renders = gameObject.GetComponents<Renderer>().Concat(gameObject.GetComponentsInChildren<Renderer>()).ToArray();

        if (spawnPoint != null)
            spawnTarget = spawnPoint.transform.position;

        UpdateTearDisplay();
    }


    [HideInInspector]
    public bool isInCameraView = false;
    public bool IsInCameraView() { return isInCameraView; }
    public virtual void UpdateIsInCameraView(bool newState) { }

    private bool isRender = true;
    virtual public void Update()
    {
        UpdateVision();
        
        UpdateDestroyBehavior();
        
        UpdateVisionTool();

        UpdateHealth();

        UpdateProductionQuery();

        if (unitSelectionComponent.isSelected && UnityEngine.Input.anyKeyDown)
            UICommand(null);
    }

    public void UpdateTearDisplay()
    {
        int index = 0;
        foreach (GameObject tearObject in tearDisplay)
        {
            tearObject.SetActive(tear == index);
            index++;
        }
    }

    public void UpdateProductionQuery()
    {
        UnityEngine.Profiling.Profiler.BeginSample("p UpdateProductionQuery"); // Profiler
        if (productionQuery.Count > 0)
        {
            buildTimer -= Time.deltaTime;
            if (buildTimer <= 0.0f)
            {
                if (productionQuery[0].GetComponent<BaseBehavior>() != null)
                    ProduceUnit(productionQuery[0].name);
                if (productionQuery[0].GetComponent<BaseSkillScript>() != null)
                    productionQuery[0].GetComponent<BaseSkillScript>().Activate(gameObject);

                productionQuery.Remove(productionQuery[0]);
                if (productionQuery.Count > 0)
                {
                    BaseBehavior firstElementBehaviorComponent = productionQuery[0].GetComponent<BaseBehavior>();
                    buildTimer = firstElementBehaviorComponent.skillInfo.timeToBuild;
                }
            }
        }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
    }

    public List<GameObject> ProduceUnit(string createdPrefabName)
    {
        return ProduceUnit(createdPrefabName, 1, 0.0f);
    }

    public List<GameObject> ProduceUnit(string createdPrefabName, int number, float distance)
    {
        List<GameObject> createdObjects = new List<GameObject>();
        Vector3 dirToTarget = (spawnPoint.transform.position - transform.position).normalized;

        for (int i = 1; i <= number; i++)
        {
            // Create object
            GameObject createdObject = PhotonNetwork.Instantiate(createdPrefabName, spawnPoint.transform.position, spawnPoint.transform.rotation);

            BaseBehavior createdObjectBehaviorComponent = createdObject.GetComponent<BaseBehavior>();
            PhotonView createdPhotonView = createdObject.GetComponent<PhotonView>();
            // Set owner
            if (PhotonNetwork.InRoom)
                createdPhotonView.RPC("ChangeOwner", PhotonTargets.All, ownerId, team);
            else
                createdObjectBehaviorComponent.ChangeOwner(ownerId, team);

            //Send command to created object to spawn target
            if (PhotonNetwork.InRoom)
            {
                if (spawnTargetObject != null)
                    createdPhotonView.RPC("GiveOrderViewID", PhotonTargets.All, spawnTargetObject.GetComponent<PhotonView>().ViewID, true, false);
                else
                    createdPhotonView.RPC("GiveOrder", PhotonTargets.All, BaseBehavior.GetRandomPoint(spawnTarget + dirToTarget * distance, 2.0f), true, false);
            }
            else
            {
                if (spawnTargetObject != null)
                    createdObjectBehaviorComponent.GiveOrder(spawnTargetObject, true, false);
                else
                    createdObjectBehaviorComponent.GiveOrder(BaseBehavior.GetRandomPoint(spawnTarget + dirToTarget * distance, 2.0f), true, false);
            }
            createdObjects.Add(createdObject);
        }
        return createdObjects;
    }

    public void UpdateHealth()
    {
        UnityEngine.Profiling.Profiler.BeginSample("p UpdateHealth"); // Profiler
        if (IsInCameraView() && HTMLHealthFile != null)
        {
            bool healthVisible = false;
            if (IsHealthVisible())
                healthVisible = true;

            if (healthVisible && live && IsVisible() && IsInCameraView())
            {
                if (objectUIInfo == null && HTMLHealthFile != null)
                {
                    objectUIInfo = new WorldUI(HTMLHealthSize.x, HTMLHealthSize.y);
                    objectUIInfo.FaceCamera(Camera.main);
                    // objectUIInfo.ParentToOrigin(gameObject);
                    objectUIInfo.PixelPerfect = true;
                    objectUIInfo.transform.position = gameObject.transform.position;
                    objectUIInfo.transform.position += InfoHTMLOffset;
                    objectUIInfo.document.innerHTML = HTMLHealthFile.text;
                    objectUIInfo.Layer = LayerMask.NameToLayer("HP");
                    
                    objectUIInfo.document.getElementById("health").style.background = GetDisplayColor(false);
                    tempHealth = 0.0f;
                }
            }
            else
            {
                if (objectUIInfo != null)
                {
                    objectUIInfo.Destroy();
                    objectUIInfo = null;
                }
            }
            if (objectUIInfo != null)
            {
                objectUIInfo.transform.position = gameObject.transform.position;
                objectUIInfo.transform.position += InfoHTMLOffset;
                if (tempHealth != health)
                {
                    Dom.Element healthElement = objectUIInfo.document.getElementById("health");
                    healthElement.style.width = String.Format("{0:F0}%", health / maxHealth * 100);
                    tempHealth = health;
                }
            }
        }
        else if (objectUIInfo != null)
        {
            objectUIInfo.Destroy();
            objectUIInfo = null;
        }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
    }

    public void UpdateVisionTool()
    {
        UnityEngine.Profiling.Profiler.BeginSample("p UpdateVisionTool"); // Profiler
        if (team == cameraController.team && live)
        {
            if (visionTool == null && visionToolPrefab != null)
            {
                visionTool = (GameObject)Instantiate(visionToolPrefab, gameObject.transform.transform.position + new Vector3(0, 1.5f, 0), gameObject.transform.rotation);
                visionTool.transform.SetParent(gameObject.transform);
                FieldOfView fieldOfView = visionTool.GetComponent<FieldOfView>();
                fieldOfView.viewRadius = visionDistanve;
            }
        }
        else
        {
            if (visionTool != null)
            {
                FieldOfView fieldOfView = visionTool.GetComponent<FieldOfView>();
                fieldOfView.FindVisibleTargets();
                Destroy(visionTool);
            }
        }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
    }

    public void UpdateVision()
    {
        UnityEngine.Profiling.Profiler.BeginSample("p UpdateVision"); // Profiler
        bool newIsRender = IsInCameraView() && (IsVisible() || beenSeen);

        if (newIsRender != isRender)
        {
            isRender = newIsRender;
            foreach (Renderer render in renders)
                render.enabled = isRender;

            // foreach (Collider collider in gameObject.GetComponents<Collider>().Concat(gameObject.GetComponentsInChildren<Collider>()).ToArray())
            //     collider.enabled = IsVisible() || beenSeen;
        }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
    }

    public virtual void SendToDestroy() { }

    public void UpdateDestroyBehavior()
    {
        UnityEngine.Profiling.Profiler.BeginSample("p UpdateDestroyBehavior"); // Profiler
        if (sendToDestroy == true && !sendToUnderground)
        {
            timeToDestroy -= Time.deltaTime;
            if (timeToDestroy <= 0)
            {
                live = false;
                if (destroyEffectPrefab != null && destroyEffectHandler != null)
                {
                    var effectObject = (GameObject)Instantiate(destroyEffectPrefab, destroyEffectHandler.transform.position, destroyEffectHandler.transform.rotation);
                    effectObject.transform.SetParent(destroyEffectHandler.transform);
                }
                sendToUnderground = true;

                // UnityEngine.AI.NavMeshObstacle navMesh = GetComponent<UnityEngine.AI.NavMeshObstacle>();
                // if(navMesh != null)
                //      navMesh.enabled = false;

                // UnityEngine.AI.NavMeshAgent navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
                // if (navMeshAgent != null)
                //     navMeshAgent.enabled = false;

                Collider collider = GetComponent<Collider>();
                collider.enabled = false;
                Destroy(gameObject, destroyAfter);
            }
        }
        if (sendToUnderground)
        {
            delayToMoveUnderground -= Time.deltaTime;
            if (delayToMoveUnderground <= 0)
            {
                countToMoveUnderground -= Time.deltaTime;
                if (countToMoveUnderground <= 0)
                {
                    countToMoveUnderground = 0.001f;
                    transform.position = transform.position + new Vector3(0, (toUndergroundSpeed / 100) * -1, 0);
                }
            }
        }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
    }

    public bool IsDisplayOnMap()
    {
        return IsVisible();
    }

    public string GetDisplayColor(bool selected = true)
    {
        if (GameInfo.playerSpectate)
            return "#FFF";

        if (team <= 0)
            return "#FFF";
        else if (cameraController.team != team)
            return "#F00";
        else
        {
            if (cameraController.userId == ownerId)
            {
                if (selected && unitSelectionComponent.isSelected)
                    return "#A7FFA9";
                else
                    return "#00A500";
            }
            else
                return "#FFF";
        }
    }

    void OnDestroy()
    {
        if (objectUIInfo != null)
            objectUIInfo.Destroy();
        DestroyPointMarker();
    }

    public void TextBubble(string message, int timer)
    {
        if (objectUIInfo != null)
            objectUIInfo.document.Run("CreateMessage", new string[2] { message, timer.ToString() });
    }

    public GameObject GetIntersection(GameObject exceptionUnit)
    {
        Collider collider = GetComponent<Collider>();
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        foreach (CameraController.TagToSelect tag in cameraController.tagsToSelect)
        {
            var allUnits = GameObject.FindGameObjectsWithTag(tag.name);
            foreach (GameObject unit in allUnits)
            {
                BaseBehavior unitBaseBehavior = unit.GetComponent<BaseBehavior>();
                if (unitBaseBehavior.IsVisible())
                {
                    Collider unitCollider = unit.GetComponent<Collider>();
                    if (gameObject != unit && exceptionUnit != unit && collider.bounds.Intersects(unitCollider.bounds))
                        return unit;
                }
            }
        }
        return null;
    }

    public bool IsTeamEnemy(int unitTeam)
    {
        if (unitTeam == -1)
            return false;
        if (unitTeam == team)
            return false;
        return true;
    }

    public void CreateOrUpdatePointMarker(Color color, Vector3 target, float timer, bool saveMarker)
    {
        if (pointMarker != null && saveMarker)
            DestroyPointMarker();

        // Show point marker
        if ((pointMarker == null || !saveMarker) && pointMarkerPrefab != null)
        {
            var createdMarker = (GameObject)Instantiate(pointMarkerPrefab, target, pointMarkerPrefab.transform.rotation);
            PointMarker pointMarkerScript = createdMarker.GetComponent<PointMarker>();
            Projector projectorComponent = pointMarkerScript.projector.GetComponent<Projector>();
            projectorComponent.material.color = color;
            if (timer > 0)
                Destroy(createdMarker, timer);
            if (saveMarker)
                pointMarker = createdMarker;
        }
    }

    public float CalculateDamage(float damage, GameObject attacker)
    {
        BaseBehavior attackerBaseBehavior = attacker.GetComponent<BaseBehavior>();
        UnitStatistic attackerStatisic = attackerBaseBehavior.GetStatisticsInfo();

        UnitStatistic statisic = GetStatisticsInfo();

        float newDamage = damage;
        if (attackerStatisic.attackType == AttackType.Biting)
            newDamage -= (newDamage * statisic.bitingResist / 100);
        if (attackerStatisic.attackType == AttackType.Cutting)
            newDamage -= (newDamage * statisic.cuttingResist / 100);
        if (attackerStatisic.attackType == AttackType.Magic)
            newDamage -= (newDamage * statisic.magicResist / 100);
        if (attackerStatisic.attackType == AttackType.Stabbing)
            newDamage -= (newDamage * statisic.stabbingResist / 100);
        return newDamage;
    }

    public void DestroyPointMarker()
    {
        if (pointMarker != null)
            Destroy(pointMarker);
        pointMarker = null;
    }

    public static List<GameObject> GetObjectsInRange(Vector3 position, float radius, bool live = true, int team = -1, bool units = true, bool buildings = true, bool ambient = true)
    {
        List<GameObject> objects = new List<GameObject>();
        int unitsMask = 1 << LayerMask.NameToLayer("Unit");
        int buildingMask = 1 << LayerMask.NameToLayer("Building");
        int ambientMask = 1 << LayerMask.NameToLayer("Ambient");
        int mask = 1;
        if (units && !buildings)
            mask = unitsMask;
        else if (!units && buildings)
            mask = buildingMask;
        else
            mask = unitsMask | buildingMask;
        if (ambient)
            mask |= ambientMask;

        foreach (Collider collider in Physics.OverlapSphere(position, radius, mask))
        {
            GameObject unit = collider.gameObject;
            BaseBehavior unitBaseBehavior = unit.GetComponent<BaseBehavior>();
            if (unitBaseBehavior.team != team && team != -1)
                continue;
            if (unitBaseBehavior.live != live)
                continue;
            objects.Add(unit);
        }
        return objects;
    }

    public bool AttackNearEnemies(Vector3 centerOfSearch, float range, int attackTeam = -1, float randomRange = 0.0f)
    {
        UnitStatistic statisic = GetStatisticsInfo();
        if (statisic.attackType == AttackType.None && interactType != InteractigType.Attacking)
            return false;

        var allObjects = GetObjectsInRange(transform.position, range, team: attackTeam);
        if (allObjects.Count <= 0)
            return false;

        GameObject targetUnit = null;
        // Find colsest target
        float distance = range;
        foreach (GameObject unit in allObjects)
        {
            float distanceToUnit = Vector3.Distance(centerOfSearch, unit.transform.position);
            if (distanceToUnit < distance)
            {
                targetUnit = unit;
                distance = distanceToUnit;
            }
        }
        if (randomRange != 0.0f)
        {
            // Get random target
            var targetObjects = GetObjectsInRange(targetUnit.transform.position, randomRange, team: attackTeam);
            if (targetObjects.Count > 0)
                targetUnit = targetObjects[UnityEngine.Random.Range(0, targetObjects.Count - 1)];
        }
        if (targetUnit != null)
        {
            PhotonView createdPhotonView = GetComponent<PhotonView>();
            if (PhotonNetwork.InRoom)
                createdPhotonView.RPC("GiveOrder", PhotonTargets.All, targetUnit, true);
            else
                GiveOrder(targetUnit, true, false);
            return true;
        }
        return false;
    }

    public void SendAlertAttacking(GameObject attacker)
    {
        BaseBehavior attackerBaseBehavior = attacker.GetComponent<BaseBehavior>();
        var allUnits = GameObject.FindGameObjectsWithTag("Unit");
        foreach (GameObject unit in allUnits)
        {
            UnitBehavior unitBehaviorComponent = unit.GetComponent<UnitBehavior>();
            if (unitBehaviorComponent != null && unitBehaviorComponent.behaviorType == BehaviorType.Counterattack
                || unitBehaviorComponent.behaviorType == BehaviorType.Aggressive)
            {
                float dist = Vector3.Distance(gameObject.transform.position, unit.transform.position);
                UnitStatistic statisic = GetStatisticsInfo();
                if (unitBehaviorComponent.team == team && unitBehaviorComponent.live && dist <= statisic.alertRange)
                {
                    if (unitBehaviorComponent.interactType == InteractigType.None && unitBehaviorComponent.target == null
                        && !unitBehaviorComponent.agent.pathPending && !unitBehaviorComponent.agent.hasPath)
                    {
                        unitBehaviorComponent.AttackNearEnemies(unit.transform.position, statisic.agrRange, attackerBaseBehavior.team);
                    }
                }
            }
        }
    }

    public static Vector3 GetRandomPoint(Vector3 point, float randomDistance)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(point + UnityEngine.Random.insideUnitSphere * randomDistance, out hit, randomDistance, NavMesh.AllAreas))
            return hit.position;

        return new Vector3();
    }

    public virtual void AddCommandToQueue(object newTarget)
    {
        queueCommands.Add(newTarget);
    }

    [PunRPC]
    public virtual void StartInteractViewID(int targetViewId)
    {
        StartInteract(PhotonNetwork.GetPhotonView(targetViewId).gameObject);
    }
    [PunRPC]
    public virtual void GiveOrderViewID(int targetViewId, bool displayMarker, bool overrideQueueCommands)
    {
        GiveOrder(PhotonNetwork.GetPhotonView(targetViewId).gameObject, displayMarker, overrideQueueCommands);
    }

    public bool[] ActivateSkills(string commandName)
    {
        bool[] result = { false, false };
        foreach (GameObject skillObject in skillList)
        {
            if (skillObject != null)
            {
                UIBaseScript cameraUIBaseScript = Camera.main.GetComponent<UIBaseScript>();
                KeyCode hotkey = KeyCode.None;
                string uniqueName = "";
                bool isCanBeUsedAsSkill = true;
                string error = "";
                BuildingBehavior buildingBehavior = skillObject.GetComponent<BuildingBehavior>();
                BaseSkillScript skillScript = skillObject.GetComponent<BaseSkillScript>();
                if (buildingBehavior != null)
                {
                    hotkey = buildingBehavior.skillInfo.productionHotkey;
                    uniqueName = buildingBehavior.skillInfo.uniqueName;

                    isCanBeUsedAsSkill = buildingBehavior.IsCanBeUsedAsSkill(gameObject);
                    error = buildingBehavior.ErrorMessage(gameObject);
                }
                else if (skillScript != null)
                {
                    hotkey = skillScript.skillInfo.productionHotkey;
                    uniqueName = skillScript.skillInfo.uniqueName;

                    isCanBeUsedAsSkill = skillScript.IsCanBeUsedAsSkill(gameObject);
                    error = skillScript.ErrorMessage(gameObject);
                }

                if (uniqueName == commandName || UnityEngine.Input.GetKeyDown(hotkey))
                {
                    if (!isCanBeUsedAsSkill)
                    {
                        if (error != "")
                            cameraUIBaseScript.DisplayMessage(error, 3000, "isCanBeUsedAsSkill");

                        result[1] = true;
                        return result;
                    }

                    if (buildingBehavior != null)
                    {
                        if (cameraController.buildedObject == null)
                        {
                            // if not enough resources -> return second element true
                            result[1] = !SpendResources(buildingBehavior.skillInfo.costFood, buildingBehavior.skillInfo.costGold, buildingBehavior.skillInfo.costWood);
                            if (result[1])
                                return result;

                            cameraUIBaseScript.DisplayMessage("Select place to build", 3000, "selectPlace");
                            cameraController.buildedObject = skillObject;
                        }
                    }
                    else if(skillScript != null)
                    {
                        result[1] = !SpendResources(skillScript.skillInfo.costFood, skillScript.skillInfo.costGold, skillScript.skillInfo.costWood);
                        if (result[1])
                            return result;

                        if (skillScript.skillInfo.skillType == SkillType.Skill)
                            skillScript.Activate(gameObject);
                        if (skillScript.skillInfo.skillType == SkillType.Upgrade)
                        {
                            productionQuery.Add(skillObject);
                            if (productionQuery.Count <= 1)
                                buildTimer = skillScript.skillInfo.timeToBuild;
                        }
                    }
                    result[0] = true;
                    return result;
                }
            }
        }
        return result;
    }

    public bool DeleteFromProductionQuery(int index)
    {
        if (index >= productionQuery.Count)
            return false;
        
        BaseBehavior baseBehavior = productionQuery[index].GetComponent<BaseBehavior>();
        BaseSkillScript skillScript = productionQuery[index].GetComponent<BaseSkillScript>();

        if (baseBehavior != null)
        {
            cameraController.food += baseBehavior.skillInfo.costFood;
            cameraController.gold += baseBehavior.skillInfo.costGold;
            cameraController.wood += baseBehavior.skillInfo.costWood;
        }
        else if (skillScript)
        {
            cameraController.food += skillScript.skillInfo.costFood;
            cameraController.gold += skillScript.skillInfo.costGold;
            cameraController.wood += skillScript.skillInfo.costWood;
        }
        productionQuery.RemoveAt(index);
        if (index == 0 && productionQuery.Count > 0)
        {
            if (productionQuery[0].GetComponent<BaseBehavior>() != null)
                buildTimer = productionQuery[0].GetComponent<BaseBehavior>().skillInfo.timeToBuild;
            if (productionQuery[0].GetComponent<BaseSkillScript>() != null)
                buildTimer = productionQuery[0].GetComponent<BaseSkillScript>().skillInfo.timeToBuild;
        }
        return true;
    }

    public virtual void StartVisible(BaseBehavior senderBaseBehaviorComponent){ }
    public virtual void StopVisible(BaseBehavior senderBaseBehaviorComponent) { }

    public virtual void GiveOrder(Vector3 point, bool displayMarker, bool overrideQueueCommands) { }
    public virtual void GiveOrder(GameObject targetObject, bool displayMarker, bool overrideQueueCommands) { }
    public virtual bool IsIdle() { return true; }
    public virtual void TakeDamage(float damage, GameObject attacker) { }
    public virtual void BecomeDead() { }
    public virtual void StartInteract(GameObject targetObject) { }
    public virtual bool[] UICommand(string commandName) { return new bool[2] { false, false }; }
    public virtual bool IsHealthVisible() { return true; }
    public virtual List<string> GetCostInformation() { return new List<string>(); }
    public virtual bool IsVisible() { return false; }
    public virtual void AlertAttacking(GameObject attacker) { }
    public virtual void ResourcesIsOut(GameObject worker) { }
    public virtual UnitStatistic GetStatisticsInfo() { return null; }

    public bool SpendResources(float food, float gold, float wood)
    {
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (cameraController.food < food || cameraController.gold < gold || cameraController.wood < wood)
        {
            string notEnoughMessage = "Not enough resources!";

            if (cameraController.food < food)
                notEnoughMessage = String.Format("{0} food: {1:F0}", notEnoughMessage, food - cameraController.food);
            if (cameraController.gold < gold)
                notEnoughMessage = String.Format("{0} gold: {1:F0}", notEnoughMessage, gold - cameraController.gold);
            if (cameraController.wood < wood)
                notEnoughMessage = String.Format("{0} wood: {1:F0}", notEnoughMessage, wood - cameraController.wood);

            UIBaseScript cameraUIBaseScript = Camera.main.GetComponent<UIBaseScript>();
            cameraUIBaseScript.DisplayMessage(notEnoughMessage, 3000, "notEnoughResources");
            return false;
        }
        cameraController.food -= food;
        cameraController.gold -= gold;
        cameraController.wood -= wood;
        return true;
    }

    public static bool IsHasUnitWithTear(string unitName, int TCTear, int TCTeam)
    {
        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Building"))
        {
            BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
            if (unit.name.Contains(unitName) && baseBehaviorComponent.team == TCTeam && baseBehaviorComponent.tear == TCTear)
                return true;
        }
        return false;
    }

    public string ErrorMessage(GameObject sender)
    {
        if(skillConditions == SkillConditionType.TownCenterTear1 && !IsHasUnitWithTear("townCenter", 1, team))
            return "You need to have at least one town center with first upgrade";
        if (skillConditions == SkillConditionType.TownCenterTear2 && !IsHasUnitWithTear("townCenter", 2, team))
            return "You need to have at least one town center with second upgrade";
        return "";
    }

    public bool IsDisplayedAsSkill(GameObject sender)
    {
        return true;
    }

    public bool IsCanBeUsedAsSkill(GameObject sender)
    {
        if (skillConditions == SkillConditionType.TownCenterTear1 && !IsHasUnitWithTear("townCenter", 1, team))
            return false;
        if (skillConditions == SkillConditionType.TownCenterTear2 && !IsHasUnitWithTear("townCenter", 2, team))
            return false;
        return true;
    }

    public virtual List<string> GetStatistics()
    {
        UnitStatistic statisic = GetStatisticsInfo();

        List<string> statistics = new List<string>();
        string attackTypeName = "";
        if (statisic.attackType == AttackType.Biting)
            attackTypeName = "Biting";
        if (statisic.attackType == AttackType.Cutting)
            attackTypeName = "Cutting";
        if (statisic.attackType == AttackType.Magic)
            attackTypeName = "Magic";
        if (statisic.attackType == AttackType.Stabbing)
            attackTypeName = "Stabbing";

        if (statisic.attackType != AttackType.None)
        {
            statistics.Add(String.Format("Att. type: {0}", attackTypeName));
            statistics.Add(String.Format("Damage: {0:F0}", statisic.damage));
            statistics.Add(String.Format("Attack speed: {0:F1} sec", statisic.attackTime));
        }

        statistics.Add(String.Format("Stabbing resist: {0:F0}%", statisic.stabbingResist));
        statistics.Add(String.Format("Cutting resist: {0:F0}%", statisic.cuttingResist));
        statistics.Add(String.Format("Biting resist: {0:F0}%", statisic.bitingResist));
        statistics.Add(String.Format("Magic resist: {0:F0}%", statisic.magicResist));
        return statistics;
    }
}
