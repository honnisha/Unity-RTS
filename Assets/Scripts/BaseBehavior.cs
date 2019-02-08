using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerUI;
using System.Text;
using System;
using System.Linq;
using Photon.Pun;
using UnityEngine.AI;
using GangaGame;
using UISpace;
using CI.QuickSave;
using UnityEditor;

public class BaseBehavior : MonoBehaviourPunCallbacks, ISkillInterface
{
    static readonly string[] savedFields = new string[] { "health", "live", "uniqueId", "tear", "resourceCapacity", "resourceHold", "resourceType", "buildTimer" };

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
    public List<SkillCondition> _skillConditions;
    public List<SkillCondition> skillConditions { get { return _skillConditions; } set { _skillConditions = value; } }

    [HideInInspector]
    public Vector3 unitPosition = new Vector3();

    [HideInInspector]
    public int uniqueId = -1;
    [HideInInspector]
    public GameObject interactObject;
    [HideInInspector]
    public int interactObjectUniqueId = -1;

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
    public enum BehaviorType { Run, Hold, Counterattack, Aggressive, None };
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
    [HideInInspector]
    public GameObject target;
    [HideInInspector]
    public int targetUniqueId = -1;
    [HideInInspector]
    public Vector3 tempDestinationPoint = Vector3.zero;
    [HideInInspector]
    public Vector3 previousPosition;
    [HideInInspector]
    public float curSpeed;
    [HideInInspector]
    public CameraController cameraController;
    [HideInInspector]
    public UIBaseScript cameraUIBaseScript;

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
    public cakeslice.Outline[] allOutlines;
    new Collider collider;
    [HideInInspector]
    public string prefabName = "";

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
    [HideInInspector]
    public float resourceHold = 0.0f;
    [HideInInspector]
    public ResourceType resourceType = ResourceType.None;

    #endregion

    #region Sounds info

    public enum SoundEventType { None, Hit, Attack, Build, Die, TakeOrderToPoint, TakeOrderToTarget };
    [System.Serializable]
    public class SoundInfo
    {
        public SoundEventType type = SoundEventType.None;
        public List<AudioClip> soundList = new List<AudioClip>();
        [HideInInspector]
        public int lastSoundIndex = -1;
    }
    [Header("Sounds Info")]
    public SoundInfo[] soundsInfo;
    AudioSource audioSource;

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
        collider = GetComponent<Collider>();
        cameraUIBaseScript = Camera.main.GetComponent<UIBaseScript>();
        cameraController = Camera.main.GetComponent<CameraController>();
        unitSelectionComponent = GetComponent<UnitSelectionComponent>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        audioSource = GetComponent<AudioSource>();

        if (agent != null && obstacle != null)
            obstacle.enabled = false;

        unitPosition = transform.position;

        UpdateTearDisplay(allTurnOn: true);
        renders = gameObject.GetComponents<Renderer>().Concat(gameObject.GetComponentsInChildren<Renderer>()).ToArray();
        allOutlines = gameObject.GetComponents<cakeslice.Outline>().Concat(gameObject.GetComponentsInChildren<cakeslice.Outline>()).ToArray();
        UpdateTearDisplay();

        if (spawnPoint != null)
            spawnTarget = spawnPoint.transform.position;
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

    public void SendSoundEvent(SoundEventType soundEventType, bool dropCount = false)
    {
        if (audioSource == null)
            return;

        if (!IsVisible())
            return;

        float volume = PlayerPrefs.GetFloat("soundsVolume");
        foreach (var soundInfo in soundsInfo)
        {
            if (dropCount)
                soundInfo.lastSoundIndex = -1;

            if (soundInfo.type == soundEventType && soundInfo.soundList.Count > 0 && !audioSource.isPlaying)
            {
                if (soundInfo.lastSoundIndex == -1)
                    soundInfo.lastSoundIndex = UnityEngine.Random.Range(0, soundInfo.soundList.Count - 1);
                if (soundInfo.lastSoundIndex >= soundInfo.soundList.Count)
                    soundInfo.lastSoundIndex = 0;

                audioSource.PlayOneShot(soundInfo.soundList[soundInfo.lastSoundIndex], volume);
                soundInfo.lastSoundIndex++;
            }
        }
    }

    public void UpdateTearDisplay(bool allTurnOn = false)
    {
        int index = 0;
        foreach (GameObject tearObject in tearDisplay)
        {
            if (allTurnOn)
                tearObject.SetActive(true);
            else
                tearObject.SetActive(tear == index);
            index++;
        }
    }

    public void ProductionQueryUpdated()
    {
        if (cameraController.selectedObjects.Count == 1 && cameraController.selectedObjects.Contains(gameObject))
        {
            cameraUIBaseScript.UpdateQueue(gameObject);
            cameraUIBaseScript.UpdateSkillsInfo();
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
                ProductionQueryUpdated();
            }
        }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
    }

    public virtual void Attack(GameObject target)
    {
        if (!live)
            return;

        SendSoundEvent(SoundEventType.Attack);

        UnitStatistic statisic = GetStatisticsInfo();

        attacked = false;
        attackTimer = statisic.attackTime;
        attackDelayTimer = statisic.attackDelay;
        interactType = InteractigType.Attacking;
    }

    public void DoAttack()
    {
        if (interactType == InteractigType.Attacking && target != null)
        {
            UnitStatistic statisic = GetStatisticsInfo();
            BaseBehavior targetBaseBehavior = null;
            if (target != null)
                targetBaseBehavior = target.GetComponent<BaseBehavior>();

            if (IsTeamEnemy(targetBaseBehavior.team))
            {
                attackDelayTimer -= Time.deltaTime;
                if (!attacked && attackDelayTimer <= 0.0f && target != null)
                {
                    // Damage deal
                    attacked = true;
                    if (statisic.damageAfterTimer)
                        targetBaseBehavior.TakeDamage(statisic.damage, gameObject);
                    if (actionEffect != null)
                    {
                        ActionEffect unitActionEffect = actionEffect.transform.gameObject.GetComponent<ActionEffect>();
                        unitActionEffect.activate(gameObject, target, statisic.damage);
                    }
                }
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0.0f)
                {
                    interactType = InteractigType.None;
                    if (!targetBaseBehavior.live || tempDestinationPoint != Vector3.zero)
                    {
                        ActionIsDone();

                        StartInteract(target);
                    }
                    else
                    {
                        ActionIsDone();
                        // SetAgentStopped(false);
                    }
                }
            }
        }
    }

    public virtual void ActionIsDone(InteractigType stopActionType = InteractigType.None) { return; }

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
            createdObjectBehaviorComponent.prefabName = createdPrefabName;

            createdObjectBehaviorComponent.uniqueId = cameraController.uniqueIdIndex++;

            PhotonView createdPhotonView = createdObject.GetComponent<PhotonView>();
            // Set owner
            if (PhotonNetwork.InRoom)
                createdPhotonView.RPC("ChangeOwner", PhotonTargets.All, ownerId, team);
            else
                createdObjectBehaviorComponent.ChangeOwner(ownerId, team);

            if (target != null)
                createdObjectBehaviorComponent.GiveOrder(target, true, false);
            else
                createdObjectBehaviorComponent.GiveOrder(GetRandomPoint(spawnTarget + dirToTarget * distance, 2.0f), true, false);
            createdObjects.Add(createdObject);
        }
        return createdObjects;
    }

    public void UpdateHealth()
    {
        UnityEngine.Profiling.Profiler.BeginSample("p UpdateHealth"); // Profiler
        if (IsInCameraView() && HTMLHealthFile != null)
        {
            if (IsHealthVisible() && live && IsVisible() && IsInCameraView())
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
                    objectUIInfo.document.getElementById("health").style.width = String.Format("{0:F0}%", health / maxHealth * 100);
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
        foreach (CameraController.TagToSelect tag in cameraController.tagsToSelect)
        {
            var allUnits = GameObject.FindGameObjectsWithTag(tag.name);
            foreach (GameObject unit in allUnits)
            {
                BaseBehavior unitBaseBehavior = unit.GetComponent<BaseBehavior>();
                if (unitBaseBehavior.IsVisible())
                {
                    if (gameObject != unit && exceptionUnit != unit && collider.bounds.Intersects(unit.GetComponent<Collider>().bounds))
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

    public void CreateOrUpdatePointMarker(Color color, Vector3 target, float timer, bool saveMarker, PointMarker.MarkerType markerType = PointMarker.MarkerType.Point)
    {
        if (pointMarker != null && saveMarker)
            DestroyPointMarker();

        // Show point marker
        if ((pointMarker == null || !saveMarker) && pointMarkerPrefab != null)
        {
            var createdMarker = (GameObject)Instantiate(pointMarkerPrefab, target, pointMarkerPrefab.transform.rotation);
            PointMarker pointMarkerScript = createdMarker.GetComponent<PointMarker>();
            pointMarkerScript.SetMarker(color, markerType, timer);
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

    public static void GetObjectsInRange(
        ref List<GameObject> objects, Vector3 position, float radius, bool live = true, int team = -1, 
        bool units = true, bool buildings = true, bool ambient = true, int teamException = -1, int teamGT = -100)
    {
        objects.Clear();
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
            BaseBehavior unitBaseBehavior = collider.gameObject.GetComponent<BaseBehavior>();
            if (team != -1 && unitBaseBehavior.team != team)
                continue;
            if (unitBaseBehavior.team <= teamGT)
                continue;
            if (unitBaseBehavior.live != live)
                continue;
            if (teamException != -1 && unitBaseBehavior.team == teamException)
                continue;

            objects.Add(collider.gameObject);
        }
    }

    [HideInInspector]
    public List<GameObject> allObjects = new List<GameObject>();
    public bool AttackNearEnemies(Vector3 centerOfSearch, float range, int attackTeam = -1, float randomRange = 0.0f, bool buildings = false)
    {
        UnitStatistic statisic = GetStatisticsInfo();
        if (statisic.attackType == AttackType.None && interactType != InteractigType.Attacking)
            return false;

        if (behaviorType == BehaviorType.Counterattack || behaviorType == BehaviorType.Aggressive)
        {
            GetObjectsInRange(ref allObjects, transform.position, range, team: attackTeam, buildings: buildings, teamException: team, teamGT: 0);
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
                GetObjectsInRange(ref allObjects, targetUnit.transform.position, randomRange, team: attackTeam, buildings: buildings, teamException: team, teamGT: 0);
                if (allObjects.Count > 0)
                    targetUnit = allObjects[UnityEngine.Random.Range(0, allObjects.Count - 1)];
            }
            if (targetUnit != null)
            {
                GiveOrder(targetUnit, true, false);
                return true;
            }
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
            if (unitBehaviorComponent != null)
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

    public bool[] ActivateSkills(string commandName)
    {
        bool[] result = { false, false };
        foreach (GameObject skillObject in skillList)
        {
            if (skillObject != null)
            {
                KeyCode hotkey = KeyCode.None;
                string uniqueName = "";
                BuildingBehavior buildingBehavior = skillObject.GetComponent<BuildingBehavior>();
                BaseSkillScript skillScript = skillObject.GetComponent<BaseSkillScript>();
                SkillErrorInfo skillErrorInfo = BaseSkillScript.GetSkillErrorInfo(gameObject, skillObject);
                if (buildingBehavior != null)
                {
                    hotkey = buildingBehavior.skillInfo.productionHotkey;
                    uniqueName = buildingBehavior.skillInfo.uniqueName;
                }
                else if (skillScript != null)
                {
                    hotkey = skillScript.skillInfo.productionHotkey;
                    uniqueName = skillScript.skillInfo.uniqueName;
                }

                if (uniqueName == commandName || UnityEngine.Input.GetKeyDown(hotkey))
                {
                    if (!skillErrorInfo.isCanBeUsedAsSkill)
                    {
                        if (skillErrorInfo.errorMessage != "")
                            cameraUIBaseScript.DisplayMessage(skillErrorInfo.errorMessage, 3000, "isCanBeUsedAsSkill");

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
                    else if (skillScript != null)
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
                            ProductionQueryUpdated();
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
        ProductionQueryUpdated();
        if (index == 0 && productionQuery.Count > 0)
        {
            if (productionQuery[0].GetComponent<BaseBehavior>() != null)
                buildTimer = productionQuery[0].GetComponent<BaseBehavior>().skillInfo.timeToBuild;
            if (productionQuery[0].GetComponent<BaseSkillScript>() != null)
                buildTimer = productionQuery[0].GetComponent<BaseSkillScript>().skillInfo.timeToBuild;
        }
        return true;
    }

    public virtual void StartVisible(BaseBehavior senderBaseBehaviorComponent) { }
    public virtual void StopVisible(BaseBehavior senderBaseBehaviorComponent) { }

    public virtual void GiveOrder(Vector3 point, bool displayMarker, bool overrideQueueCommands, float speed = 0.0f)
    {
        if (PhotonNetwork.InRoom)
            photonView.RPC("_GiveOrder", PhotonTargets.All, point, displayMarker, overrideQueueCommands, speed);
        else
            _GiveOrder(point, displayMarker, overrideQueueCommands, speed);
    }

    public void StartInteract(GameObject targetObject)
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonView targetObjectPhotonView = targetObject.GetComponent<PhotonView>();
            if (targetObjectPhotonView != null)
                photonView.RPC("StartInteractViewID", PhotonTargets.All, targetObject.GetComponent<PhotonView>().ViewID);
            else
            {
                BaseBehavior unitBaseBehavior = targetObject.GetComponent<BaseBehavior>();
                photonView.RPC("StartInteractUniqueID", PhotonTargets.All, unitBaseBehavior.uniqueId);
            }
        }
        else
            _StartInteract(targetObject);
    }
    [PunRPC]
    public virtual void StartInteractViewID(int targetViewId)
    {
        _StartInteract(PhotonNetwork.GetPhotonView(targetViewId).gameObject);
    }
    [PunRPC]
    public virtual void StartInteractUniqueID(int targetUniqueId)
    {
        GameObject unit = GetObjectByUniqueId(targetUniqueId);
        if (unit != null)
            _StartInteract(unit);
    }

    public virtual void GiveOrder(GameObject targetObject, bool displayMarker, bool overrideQueueCommands, float speed = 0.0f)
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonView targetObjectPhotonView = targetObject.GetComponent<PhotonView>();
            if (targetObjectPhotonView != null)
                photonView.RPC("GiveOrderViewID", PhotonTargets.All, targetObject.GetComponent<PhotonView>().ViewID, displayMarker, overrideQueueCommands, speed);
            else
            {
                BaseBehavior unitBaseBehavior = targetObject.GetComponent<BaseBehavior>();
                photonView.RPC("GiveOrderUniqueID", PhotonTargets.All, unitBaseBehavior.uniqueId, displayMarker, overrideQueueCommands, speed);
            }
        }
        else
            _GiveOrder(targetObject, displayMarker, overrideQueueCommands, speed);
    }
    [PunRPC]
    public virtual void GiveOrderViewID(int targetViewId, bool displayMarker, bool overrideQueueCommands, float speed = 0.0f)
    {
        _GiveOrder(PhotonNetwork.GetPhotonView(targetViewId).gameObject, displayMarker, overrideQueueCommands, speed);
    }
    [PunRPC]
    public virtual void GiveOrderUniqueID(int targetUniqueId, bool displayMarker = false, bool overrideQueueCommands = true, float speed = 0.0f)
    {
        GameObject unit = GetObjectByUniqueId(targetUniqueId);
        if (unit != null)
            _GiveOrder(unit, displayMarker, overrideQueueCommands, speed);
    }
    public static GameObject GetObjectByUniqueId(int targetUniqueId)
    {
        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Building").Concat(GameObject.FindGameObjectsWithTag("Ambient")).Concat(GameObject.FindGameObjectsWithTag("Unit")))
        {
            BaseBehavior unitBaseBehavior = unit.GetComponent<BaseBehavior>();
            if (unitBaseBehavior.uniqueId == targetUniqueId)
                return unit;
        }
        return null;
    }

    public void StopAction(bool deleteObject = false, bool SendRPC = false)
    {
        if (PhotonNetwork.InRoom && SendRPC)
            photonView.RPC("_StopAction", PhotonTargets.All, deleteObject);
        else
            _StopAction(deleteObject);
    }

    public virtual void _StopAction(bool deleteObject = false) { }
    public virtual void _GiveOrder(Vector3 point, bool displayMarker, bool overrideQueueCommands, float speed = 0.0f) { }
    public virtual void _GiveOrder(GameObject targetObject, bool displayMarker, bool overrideQueueCommands, float speed = 0.0f) { }
    public virtual bool IsIdle() { return true; }
    public virtual void TakeDamage(float damage, GameObject attacker) { }
    public virtual void BecomeDead() { }
    public virtual void _StartInteract(GameObject targetObject) { }
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

            cameraUIBaseScript.DisplayMessage(notEnoughMessage, 3000, "notEnoughResources");
            return false;
        }
        cameraController.food -= food;
        cameraController.gold -= gold;
        cameraController.wood -= wood;
        return true;
    }

    public static bool IsHasUnitWithTear(string unitName, int minTear, int maxTear, int TCTeam, GameObject skillSender = null)
    {
        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Building"))
        {
            BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
            if (baseBehaviorComponent.skillInfo.uniqueName == unitName && baseBehaviorComponent.team == TCTeam &&
                (baseBehaviorComponent.tear >= minTear && (baseBehaviorComponent.tear <= maxTear || maxTear == 0)))
                return true;
        }
        return false;
    }

    public bool IsQueueContain(string uniqueSkillName)
    {
        if (productionQuery.Count > 0)
            foreach (GameObject objectInQueue in productionQuery)
            {
                if (objectInQueue.GetComponent<BaseBehavior>() != null && objectInQueue.GetComponent<BaseBehavior>().skillInfo.uniqueName == uniqueSkillName)
                    return true;
                if (objectInQueue.GetComponent<BaseSkillScript>() != null && objectInQueue.GetComponent<BaseSkillScript>().skillInfo.uniqueName == uniqueSkillName)
                    return true;
            }
        return false;
    }

    [HideInInspector]
    public List<string> statisticStrings = new List<string>();
    public virtual List<string> GetStatistics()
    {
        UnitStatistic statisic = GetStatisticsInfo();

        statisticStrings.Clear();
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
            statisticStrings.Add(new StringBuilder(30).AppendFormat("Att. type: {0}", attackTypeName).ToString());
            statisticStrings.Add(new StringBuilder(30).AppendFormat("Damage: {0:F0}", statisic.damage).ToString());
            statisticStrings.Add(new StringBuilder(30).AppendFormat("Attack speed: {0:F1} sec", statisic.attackTime).ToString());
        }

        statisticStrings.Add(new StringBuilder(30).AppendFormat("Stabbing resist: {0:F0}%", statisic.stabbingResist).ToString());
        statisticStrings.Add(new StringBuilder(30).AppendFormat("Cutting resist: {0:F0}%", statisic.cuttingResist).ToString());
        statisticStrings.Add(new StringBuilder(30).AppendFormat("Biting resist: {0:F0}%", statisic.bitingResist).ToString());
        statisticStrings.Add(new StringBuilder(30).AppendFormat("Magic resist: {0:F0}%", statisic.magicResist).ToString());
        return statisticStrings;
    }

    public void Save(ref QuickSaveWriter saveWriter, int index)
    {
        if (prefabName == "")
            return;

        saveWriter.Write(new StringBuilder(15).AppendFormat("{0}_{1}", index, "path").ToString(), prefabName);
        saveWriter.Write(new StringBuilder(15).AppendFormat("{0}_{1}", index, "position").ToString(), transform.position);
        saveWriter.Write(new StringBuilder(15).AppendFormat("{0}_{1}", index, "rotation").ToString(), transform.rotation);
        saveWriter.Write(new StringBuilder(15).AppendFormat("{0}_{1}", index, "localScale").ToString(), transform.localScale);
        saveWriter.Write(new StringBuilder(15).AppendFormat("{0}_{1}", index, "team").ToString(), team);
        saveWriter.Write(new StringBuilder(15).AppendFormat("{0}_{1}", index, "ownerId").ToString(), ownerId);
        saveWriter.Write(new StringBuilder(15).AppendFormat("{0}_{1}", index, "resourceType").ToString(), (int)resourceType);

        if (agent != null && target == null && agent.destination != null && agent.hasPath)
            saveWriter.Write(new StringBuilder(15).AppendFormat("{0}_{1}", index, "targetDestination").ToString(), agent.destination);
        else if(spawnTarget != Vector3.zero)
            saveWriter.Write(new StringBuilder(15).AppendFormat("{0}_{1}", index, "targetDestination").ToString(), spawnTarget);
        else
            saveWriter.Write(new StringBuilder(15).AppendFormat("{0}_{1}", index, "targetDestination").ToString(), Vector3.zero);

        int productionCount = productionQuery.Count;
        saveWriter.Write(new StringBuilder(15).AppendFormat("{0}_{1}", index, "production").ToString(), productionCount);
        for (int i = 0; i < productionCount; i++)
        {
            BaseBehavior productionBehaviorComponent = productionQuery[i].GetComponent<BaseBehavior>();
            saveWriter.Write(new StringBuilder(15).AppendFormat("{0}_{1}_{2}", index, i, "production").ToString(), productionBehaviorComponent.skillInfo.uniqueName);
        }

        if (target != null)
        {
            BaseBehavior targetBehaviorComponent = target.GetComponent<BaseBehavior>();
            saveWriter.Write(new StringBuilder(15).AppendFormat("{0}_{1}", index, "targetUniqueId").ToString(), targetBehaviorComponent.uniqueId);
        }
        else
            saveWriter.Write(new StringBuilder(15).AppendFormat("{0}_{1}", index, "targetUniqueId").ToString(), -1);
        if (interactObject != null)
        {
            BaseBehavior targetBehaviorComponent = interactObject.GetComponent<BaseBehavior>();
            saveWriter.Write(new StringBuilder(15).AppendFormat("{0}_{1}", index, "interactObjectUniqueId").ToString(), targetBehaviorComponent.uniqueId);
        }
        else
            saveWriter.Write(new StringBuilder(15).AppendFormat("{0}_{1}", index, "interactObjectUniqueId").ToString(), -1);

        foreach (string valueName in savedFields)
        {
            object value = this.GetType().GetField(valueName).GetValue(this);
            saveWriter.Write(new StringBuilder(15).AppendFormat("{0}_{1}", index, valueName).ToString(), value);
        }
    }

    public static void Load(ref QuickSaveReader saveReader, int index)
    {
        try
        {
            string prefabName = saveReader.Read<string>(new StringBuilder(15).AppendFormat("{0}_{1}", index, "path").ToString());
            if (prefabName == "")
                return;

            Vector3 position = saveReader.Read<Vector3>(new StringBuilder(15).AppendFormat("{0}_{1}", index, "position").ToString());
            Vector3 localScale = saveReader.Read<Vector3>(new StringBuilder(15).AppendFormat("{0}_{1}", index, "localScale").ToString());
            Quaternion rotation = saveReader.Read<Quaternion>(new StringBuilder(15).AppendFormat("{0}_{1}", index, "rotation").ToString());
            GameObject newObject = PhotonNetwork.Instantiate(prefabName, position, rotation);
            newObject.transform.localScale = localScale;

            BaseBehavior baseBehaviorComponent = newObject.GetComponent<BaseBehavior>();
            baseBehaviorComponent.prefabName = prefabName;

            baseBehaviorComponent.targetUniqueId = saveReader.Read<int>(new StringBuilder(15).AppendFormat("{0}_{1}", index, "targetUniqueId").ToString());
            baseBehaviorComponent.interactObjectUniqueId = saveReader.Read<int>(new StringBuilder(15).AppendFormat("{0}_{1}", index, "interactObjectUniqueId").ToString());

            Vector3 targetDestination = saveReader.Read<Vector3>(new StringBuilder(15).AppendFormat("{0}_{1}", index, "targetDestination").ToString());
            if (targetDestination != Vector3.zero)
                baseBehaviorComponent.GiveOrder(targetDestination, false, false);

            int newTeam = saveReader.Read<int>(new StringBuilder(15).AppendFormat("{0}_{1}", index, "team").ToString());
            string newOwner = saveReader.Read<string>(new StringBuilder(15).AppendFormat("{0}_{1}", index, "ownerId").ToString());

            baseBehaviorComponent.ChangeOwner(newOwner, newTeam);

            try
            {
                int productionCount = saveReader.Read<int>(new StringBuilder(15).AppendFormat("{0}_{1}", index, "production").ToString());
                for (int i = 0; i < productionCount; i++)
                {
                    string productionUniqueName = saveReader.Read<string>(new StringBuilder(15).AppendFormat("{0}_{1}_{2}", index, i, "production").ToString());
                    foreach (GameObject producedUnit in baseBehaviorComponent.skillList.Concat(baseBehaviorComponent.producedUnits))
                    {
                        BaseBehavior producedUnitBehaviorComponent = producedUnit.GetComponent<BaseBehavior>();
                        if (producedUnitBehaviorComponent.skillInfo.uniqueName == productionUniqueName)
                            baseBehaviorComponent.productionQuery.Add(producedUnit);
                    }
                }
            }
            catch (Exception e)
            {
            }

            foreach (string valueName in savedFields)
            {
                try
                {
                    var info = baseBehaviorComponent.GetType().GetField(valueName).GetValue(baseBehaviorComponent);
                    Type fieldType = baseBehaviorComponent.GetType().GetField(valueName).FieldType;
                    object value = saveReader.Read<object>(new StringBuilder(15).AppendFormat("{0}_{1}", index, valueName).ToString());
                    if (fieldType.IsEnum)
                        baseBehaviorComponent.GetType().GetField(valueName).SetValue(baseBehaviorComponent, Enum.Parse(fieldType, (string)value));
                    else
                        baseBehaviorComponent.GetType().GetField(valueName).SetValue(baseBehaviorComponent, Convert.ChangeType(value, fieldType));
                }
                catch (Exception e)
                {
                    Debug.LogError(new StringBuilder().AppendFormat(
                        "Error for restore {0}_{1} value for {2}: {3}", index, valueName, baseBehaviorComponent.skillInfo.uniqueName, e.Message).ToString());
                }
            }

            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            if (baseBehaviorComponent.uniqueId > cameraController.uniqueIdIndex)
                cameraController.uniqueIdIndex = baseBehaviorComponent.uniqueId + 1;

            baseBehaviorComponent.UpdateTearDisplay();
        }
        catch (QuickSaveException e)
        {
            Debug.LogError(new StringBuilder().AppendFormat(
                "QuickSaveException {0}: {1}", index, e.Message).ToString());
        }
    }

    public void RestoreBehavior()
    {
        if (targetUniqueId > -1)
        {
            GiveOrderUniqueID(targetUniqueId);
            targetUniqueId = -1;
        }
        if (interactObjectUniqueId > -1)
        {
            GiveOrderUniqueID(interactObjectUniqueId);
            interactObjectUniqueId = -1;
        }
        if (!live && anim != null)
            anim.SetTrigger("Die");
    }
}
