using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerUI;
using System.Text;
using System;
using System.Linq;
using Photon.Pun;
using UnityEngine.AI;

public class BaseBehavior : MonoBehaviourPunCallbacks, IPunObservable
{
    private PhotonView photonView;

    #region Unit info

    [Header("Unit info")]
    public int team = 1;
    public string ownerId = "1";
    public float maxHealth = 100.0f;
    public float health;
    public bool live = true;
    public GameObject pointToInteract;

    public float visionDistanve = 10.0f;

    [Header("UI")]
    [Tooltip("Icons in UI will be stuck if name the same")]
    public string uniqueName;
    public string readableName;
    public string readableDescription;
    public string imagePath;
    public float timeToBuild = 10.0f;
    public float costFood = 10.0f;
    public float costGold = 10.0f;
    public float costWood = 10.0f;
    public KeyCode productionHotkey;

    #endregion

    #region Fight info

    [Header("Attack Info")]
    public bool canAttack = true;
    public float damage = 50.0f;
    public float rangeAttack = 2.0f;

    public enum InteractigType { None, Attacking, Bulding, Gathering, CuttingTree, Farming };
    public InteractigType interactType = InteractigType.None;

    public float attackTime = 1.2f;
    public float attackDelay = 0.0f;
    public bool damageAfterTimer = true;
    [HideInInspector]
    public float attackTimer;
    [HideInInspector]
    public float attackDelayTimer;
    [HideInInspector]
    public bool attacked;

    public enum AttackType { Stabbing, Cutting, Biting, Magic, None };
    public AttackType attackType = AttackType.None;

    public float stabbingResist = 0.0f;
    public float cuttingResist = 0.0f;
    public float bitingResist = 0.0f;
    public float magicResist = 0.0f;

    public enum BehaviorType { Run, Hold, Counterattack, Aggressive };
    public BehaviorType behaviorType = BehaviorType.Counterattack;
    public float agrRange = 20.0f;
    public float alertRange = 10.0f;

    public GameObject actionEffect;

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
    public UnityEngine.AI.NavMeshAgent agent;
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

    #endregion

    #region Skills info

    [Header("Skills")]
    public List<BaseSkillScript> skillList = new List<BaseSkillScript>();

    #endregion

    #region Stuff

    [Header("Stuff")]
    [HideInInspector]
    public WorldUI objectUIInfo;
    public TextAsset HTMLHealthFile;
    public Vector3 InfoHTMLOffset = new Vector3();
    public GameObject pointMarkerPrefab;
    [HideInInspector]
    public GameObject pointMarker;
    public Vector2Int HTMLHealthSize = new Vector2Int(100, 100);
    private float tempHealth;
    public GameObject visionToolPrefab;
    [HideInInspector]
    public GameObject visionTool;
    [HideInInspector]
    public bool canBeSelected = true;
    public int visionCount = 0;
    [HideInInspector]
    public bool beenSeen = false;
    [HideInInspector]
    public GameObject holderObject;
    [HideInInspector]
    public ToolInfo holderToolInfo;

    #endregion

    #region Gatering hold info

    public enum ResourceType { None, Food, Gold, Wood };
    [Header("Gatering hold info")]
    public ResourceType resourceCapacityType = ResourceType.None;
    public float resourceCapacity = 0.0f;
    public List<ResourceType> storedResources = new List<ResourceType>();

    #endregion

    #region Photon

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.team);
            stream.SendNext(this.ownerId);
        }
        else
        {
            this.team = (int)stream.ReceiveNext();
            this.ownerId = (string)stream.ReceiveNext();
        }
    }

    #endregion

    [PunRPC]
    public void ChangeOwner(string newOwnerId, int newTeam)
    {
        ownerId = newOwnerId;
        team = newTeam;
    }

    virtual public void Awake()
    {
        photonView = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    virtual public void Update()
    {
        Renderer[] unitMeshRenders = gameObject.GetComponents<Renderer>();
        Renderer[] unitChildrenMeshRenders = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer unitMeshRender in unitMeshRenders.Concat(unitChildrenMeshRenders).ToArray())
            if (IsVisible() || beenSeen)
            {
                if (!unitMeshRender.enabled)
                    unitMeshRender.enabled = true;
            }
            else
                if (unitMeshRender.enabled)
                unitMeshRender.enabled = false;

        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (team == cameraController.team && ownerId == cameraController.userId && live)
        {
            if (visionTool == null && visionToolPrefab != null)
            {
                visionTool = (GameObject)Instantiate(visionToolPrefab, gameObject.transform.transform.position + new Vector3(0, 0.2f, 0), gameObject.transform.rotation);
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

        var tagsToSelect = cameraController.tagsToSelect.Find(x => (x.name == gameObject.tag));
        UnitSelectionComponent unitSelectionComponent = GetComponent<UnitSelectionComponent>();

        bool healthVisible = true;
        if (!live)
            healthVisible = false;
        if (tagsToSelect.healthVisibleOnlyWhenSelect && !unitSelectionComponent.isSelected)
            healthVisible = false;

        if (IsHealthVisible())
            healthVisible = true;

        if (healthVisible && live && IsVisible())
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

                if (team == cameraController.team)
                {
                    if (ownerId == cameraController.userId)
                        // If its your unit
                        objectUIInfo.document.getElementById("health").style.background = "green";
                    else
                        // If unit of your allies
                        objectUIInfo.document.getElementById("health").style.background = "yellow";
                }
                else
                    // Enemy unit
                    objectUIInfo.document.getElementById("health").style.background = "red";

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
        if (UnityEngine.Input.anyKeyDown)
            UICommand(null);

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
        if (sendToUnderground){
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
    }

    void OnDestroy()
    {
        if (objectUIInfo != null)
            objectUIInfo.Destroy();
        DestroyPointMarker();
    }

    public bool IsMasterClient()
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

    public void TextBubble(string message, int timer)
    {
        if (objectUIInfo != null)
            objectUIInfo.document.Run("CreateMessage", new string[2] { message, timer.ToString() });
    }

    public void StartVisible(BaseBehavior senderBaseBehaviorComponent)
    {
        // Debug.Log("StartVisible + " + gameObject.name + " " + visionCount);
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (senderBaseBehaviorComponent != null && senderBaseBehaviorComponent.team == cameraController.team)
            visionCount++;
    }

    public void StopVisible(BaseBehavior senderBaseBehaviorComponent)
    {
        // Debug.Log("StopVisible + " + gameObject.name + " " + visionCount);
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (senderBaseBehaviorComponent != null && senderBaseBehaviorComponent.team == cameraController.team && visionCount > 0)
            visionCount--;
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
                Collider unitCollider = unit.GetComponent<Collider>();
                if (gameObject != unit && exceptionUnit != unit && collider.bounds.Intersects(unitCollider.bounds))
                    return unit;
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

    public void CreateOrUpdatePointMarker(Color color, Vector3 target, float timer)
    {
        if (pointMarker != null)
            DestroyPointMarker();

        // Show point marker
        if (pointMarker == null && pointMarkerPrefab != null)
        {
            pointMarker = (GameObject)Instantiate(pointMarkerPrefab, target, pointMarkerPrefab.transform.rotation);
            PointMarker pointMarkerScript = pointMarker.GetComponent<PointMarker>();
            Projector projectorComponent = pointMarkerScript.projector.GetComponent<Projector>();
            projectorComponent.material.color = color;
            if (timer > 0)
                Destroy(pointMarker, timer);
        }
    }

    public float CalculateDamage(float damage, GameObject attacker)
    {
        float newDamage = damage;
        BaseBehavior attackerBaseBehavior = attacker.GetComponent<BaseBehavior>();
        if (attackerBaseBehavior.attackType == AttackType.Biting)
            newDamage -= (newDamage * bitingResist / 100);
        if (attackerBaseBehavior.attackType == AttackType.Cutting)
            newDamage -= (newDamage * cuttingResist / 100);
        if (attackerBaseBehavior.attackType == AttackType.Magic)
            newDamage -= (newDamage * magicResist / 100);
        if (attackerBaseBehavior.attackType == AttackType.Stabbing)
            newDamage -= (newDamage * stabbingResist / 100);
        return newDamage;
    }

    public void DestroyPointMarker()
    {
        if(pointMarker != null)
            Destroy(pointMarker);
        pointMarker = null;
    }

    public bool AttackNearEnemies(Vector3 centerOfSearch, float range, int attackTeam = 999)
    {
        if (!canAttack && interactType != InteractigType.Attacking)
            return false;

        var allUnits = GameObject.FindGameObjectsWithTag("Unit");
        GameObject targetUnit = null;
        float minDistance = range;
        foreach (GameObject unit in allUnits)
        {
            UnitBehavior unitBehaviorComponent = unit.GetComponent<UnitBehavior>();
            if (unitBehaviorComponent.team != team && unitBehaviorComponent.live && unitBehaviorComponent.team > 0)
            {
                if (attackTeam != 999 && attackTeam != unitBehaviorComponent.team)
                    continue;

                float dist = Vector3.Distance(centerOfSearch, unit.transform.position);
                if (dist <= minDistance && Physics.Linecast(transform.position, unit.transform.position))
                {
                    minDistance = dist;
                    targetUnit = unit;
                }
            }
        }
        if (targetUnit != null)
        {
            PhotonView createdPhotonView = GetComponent<PhotonView>();
            if (PhotonNetwork.InRoom)
                createdPhotonView.RPC("GiveOrder", PhotonTargets.All, targetUnit, true);
            else
                GiveOrder(targetUnit, true);
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
                if (unitBehaviorComponent.team == team && unitBehaviorComponent.live && dist <= alertRange)
                {
                    if (unitBehaviorComponent.interactType == InteractigType.None && unitBehaviorComponent.target == null
                        && !unitBehaviorComponent.agent.pathPending && !unitBehaviorComponent.agent.hasPath)
                    {
                        unitBehaviorComponent.AttackNearEnemies(unit.transform.position, agrRange, attackerBaseBehavior.team);
                    }
                }
            }
        }
    }

    public Vector3 GetRandomPoint(Vector3 point, float randomDistance)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(point + UnityEngine.Random.insideUnitSphere * randomDistance, out hit, randomDistance, NavMesh.AllAreas))
            return hit.position;

        return new Vector3();
    }

    public virtual void GiveOrder(Vector3 point, bool displayMarker) { }
    public virtual void GiveOrder(GameObject targetObject, bool displayMarker) { }
    public virtual void TakeDamage(float damage, GameObject attacker) { }
    public virtual void BecomeDead() { }
    public virtual bool StartInteract(GameObject target) { return false; }
    public virtual bool[] UICommand(string commandName) { return new bool[2] { false, false}; }
    public virtual bool IsHealthVisible() { return false; }
    public virtual List<string> GetCostInformation() { return new List<string>(); }
    public virtual bool IsVisible() { return false; }
    public virtual void AlertAttacking(GameObject attacker) { }
    public virtual void ResourcesIsOut(GameObject worker) { }

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
            cameraUIBaseScript.DisplayMessage(notEnoughMessage, 3000);
            return false;
        }
        cameraController.food -= food;
        cameraController.gold -= gold;
        cameraController.wood -= wood;
        return true;
    }

    public virtual List<string> GetStatistics()
    {
        List<string> statistics = new List<string>();
        string attackTypeName = "";
        if (attackType == AttackType.Biting)
            attackTypeName = "Biting";
        if (attackType == AttackType.Cutting)
            attackTypeName = "Cutting";
        if (attackType == AttackType.Magic)
            attackTypeName = "Magic";
        if (attackType == AttackType.Stabbing)
            attackTypeName = "Stabbing";

        if (attackType != AttackType.None && canAttack)
        {
            statistics.Add(String.Format("Att. type: {0}", attackTypeName));
            statistics.Add(String.Format("Damage: {0:F0}", damage));
            statistics.Add(String.Format("Attack speed: {0:F1} sec", attackTime));
        }

        statistics.Add(String.Format("Stabbing resist: {0:F0}%", stabbingResist));
        statistics.Add(String.Format("Cutting resist: {0:F0}%", cuttingResist));
        statistics.Add(String.Format("Biting resist: {0:F0}%", bitingResist));
        statistics.Add(String.Format("Magic resist: {0:F0}%", magicResist));
        return statistics;
    }
}
