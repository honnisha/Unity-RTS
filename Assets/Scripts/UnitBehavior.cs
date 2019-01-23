using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Photon.Pun;
using GangaGame;

public class UnitBehavior : BaseBehavior
{
    #region Building worker info

    [Header("Building worker info")]
    public float buildHpPerSecond = 2.5f;
    public ToolInfo buildingTool;
    [System.Serializable]
    public class ResourceGatherInfo
    {
        public ResourceType type = ResourceType.None;
        public float gatherPerSecond = 0.5f;
        public float maximumCapacity = 10.0f;
        public ToolInfo carryObject;
        public ToolInfo toolObject;
    }

    #endregion

    #region Gatering worker info

    [Header("Gatering worker info")]
    public List<ResourceGatherInfo> resourceGatherInfo = new List<ResourceGatherInfo>();
    [HideInInspector]
    public float resourceHold = 0.0f;
    public ResourceType resourceType = ResourceType.None;
    private string interactAnimation = "";

    public ResourceGatherInfo farmInfo;
    [HideInInspector]
    public GameObject interactObject;
    private float interactTimer = 0.0f;
    private float interactDistance = 0.0f;

    #endregion

    private float workingTimer = 0.0f;
    private bool blockedBuilding = false;
    private float toolInHandTimer = 0.0f;
    private float timetToStuck = 0.0f;

    public override void Awake()
    {
        base.Awake();

        health = maxHealth;
        resourceHold = 0.0f;
        interactType = InteractigType.None;

        tempSpeed = agent.speed;
        resourceType = ResourceType.None;
    }

    override public void Update()
    {
        UnityEngine.Profiling.Profiler.BeginSample("p UserUpdate"); // Profiler

        UpdateIsInCameraView(cameraController.IsInCameraView(transform.position));

        base.Update();
        if (!live)
            return;

        #region Animations
        Vector3 curMove = transform.position - previousPosition;
        base.curSpeed = curMove.magnitude / Time.deltaTime;
        base.previousPosition = transform.position;
        base.anim.SetFloat("Speed", curSpeed);
        base.anim.SetFloat("AttackSpeed", GetStatisticsInfo().attackTime);
        #endregion

        // Find target
        if (IsIdle())
        {
            if (!IsStopped())
                SetAgentStopped(true);

            if (behaviorType == BehaviorType.Aggressive)
                AttackNearEnemies(gameObject.transform.position, GetStatisticsInfo().agrRange, randomRange: 7.0f);
            SendOrderFromQueue();
        }

        // Wait for building if something blocked
        if (IsStopped())
            if (blockedBuilding && interactObject != null)
                StartInteract(interactObject);

        toolInHandTimer += Time.fixedDeltaTime;
        timerToCreateHolder += Time.fixedDeltaTime;
        if (toolInHandTimer > 0.25f)
        {
            // TODO: change to events instead check in update
            UpdateToolInHand();
            toolInHandTimer = 0.0f;
        }

        // Stuck
        if (!IsIdle() && interactType == InteractigType.None)
        {
            timetToStuck += Time.fixedDeltaTime;
            if (base.curSpeed > 0.05)
                timetToStuck = 0.0f;

            if (timetToStuck > 4.0f)
            {
                UnitStuck();
                timetToStuck = 0.0f;
            }
        }

        // Counterattack
        if (IsIdle() && attackedTeam != 0)
        {
            if (behaviorType == BehaviorType.Aggressive || behaviorType == BehaviorType.Counterattack)
            {
                bool targetFinded = AttackNearEnemies(transform.position, GetStatisticsInfo().agrRange, attackedTeam, randomRange: 8.0f);
                if (!targetFinded)
                {
                    attackedTeam = 0;
                    return;
                }
            }
        }

        // Interact with something
        bool interrupt = DoInteract();
        if (interrupt)
            return;


        MoveToTarget();

        DoAttack();

        if (isWalkAround && GameInfo.IsMasterClient() && target == null)
            WalkAround();

        if (pointMarkerPrefab != null)
            if (!unitSelectionComponent.isSelected || !live)
                DestroyPointMarker();

        if ((obstacle == null || !obstacle.enabled) && newAgentDestination != new Vector3())
        {
            if (!agent.enabled)
                agent.enabled = true;
            base.agent.isStopped = false;
            agent.destination = newAgentDestination;
            newAgentDestination = new Vector3();
        }
        if (isAgentStopped)
        {
            if (agent.enabled)
            {
                agent.ResetPath();
                base.agent.isStopped = true;
                agent.enabled = false;
                if (obstacle != null)
                    obstacle.enabled = true;
            }
        }
        if (!isAgentStopped)
        {
            if (!agent.enabled)
            {
                if (obstacle != null)
                    obstacle.enabled = false;
            }
        }
    }

    public override void UpdateIsInCameraView(bool newState)
    {
        isInCameraView = newState;
    }

    public override void StartVisible(BaseBehavior senderBaseBehaviorComponent)
    {
        // Debug.Log("StartVisible + " + gameObject.name + " " + visionCount);
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (senderBaseBehaviorComponent != null && senderBaseBehaviorComponent.team == cameraController.team)
            visionCount++;
    }

    public override void StopVisible(BaseBehavior senderBaseBehaviorComponent)
    {
        // Debug.Log("StopVisible + " + gameObject.name + " " + visionCount);
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (senderBaseBehaviorComponent != null && senderBaseBehaviorComponent.team == cameraController.team && visionCount > 0)
            visionCount--;
    }

    private bool stucked = true;
    float minDistanceToInteract = 0.2f;
    public Vector3 GetClosestPositionToTarget(Vector3 targetPosition)
    {
        Vector3 dirToUnit = (targetPosition - transform.position).normalized * 0.3f;
        NavMeshHit hitPosition;
        if (NavMesh.SamplePosition(targetPosition - dirToUnit, out hitPosition, 6.0f, NavMesh.AllAreas))
            return hitPosition.position;
        return targetPosition;
    }

    void OnDrawGizmos()
    {
        if (debug)
        {
            if (!IsStopped() && unitSelectionComponent.isSelected)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(agent.destination, 0.4f);

                Vector3 tempCorner = transform.position;
                foreach (Vector3 point in agent.path.corners)
                {
                    // Debug.DrawLine(tempCorner, point, Color.white);
                    Gizmos.DrawLine(tempCorner, point);
                    tempCorner = point;
                }
                Gizmos.color = Color.red;
                if (target != null)
                {
                    Gizmos.DrawSphere(GetClosestPositionToTarget(target.transform.position), 0.4f);
                }
            }
        }
    }

    private void UnitStuck()
    {
        stucked = true;
        StopAction(false);
        ActionIsDone(stopActionType: interactType);
        interactObject = null;
    }

    public void WalkAround()
    {
        if (timeToWalk <= 0.0f)
            timeToWalk = UnityEngine.Random.Range(5.0f, 20.0f);

        timerToWalk += Time.deltaTime;
        if (timerToWalk >= timeToWalk)
        {
            GiveOrder(GetRandomPoint(unitPosition, 10.0f), false, false, UnityEngine.Random.Range(0.9f, 1.3f));
            timerToWalk = 0.0f;
            timeToWalk = 0.0f;
        }
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
                base.attackDelayTimer -= Time.deltaTime;
                if (!base.attacked && base.attackDelayTimer <= 0.0f && target != null)
                {
                    // Damage deal
                    base.attacked = true;
                    if (statisic.damageAfterTimer)
                        targetBaseBehavior.TakeDamage(statisic.damage, gameObject);
                    if (actionEffect != null)
                    {
                        ActionEffect unitActionEffect = actionEffect.transform.gameObject.GetComponent<ActionEffect>();
                        unitActionEffect.activate(gameObject, target, statisic.damage);
                    }
                }
                base.attackTimer -= Time.deltaTime;
                if (base.attackTimer <= 0.0f)
                {
                    interactType = InteractigType.None;
                    if (!targetBaseBehavior.live || tempDestinationPoint != Vector3.zero)
                    {
                        ActionIsDone();

                        StartInteract(target);
                    }
                    else
                    {
                        SetAgentStopped(false);
                    }
                }
            }
        }
    }

    public void MoveToTarget()
    {
        UnityEngine.Profiling.Profiler.BeginSample("p MoveToTarget"); // Profiler
        if (interactType == InteractigType.None && target != null)
        {
            UnitStatistic statisic = GetStatisticsInfo();

            BaseBehavior targetBaseBehavior = target.GetComponent<BaseBehavior>();
            bool isBuilding = target.GetComponent<BuildingBehavior>() != null;
            BuildingBehavior targetBuildingBehavior = null;
            if (isBuilding)
                targetBuildingBehavior = target.GetComponent<BuildingBehavior>();

            if (isBuilding && targetBuildingBehavior.sourceType == SourceType.Farm)
            {
                if (targetBuildingBehavior.state == BuildingBehavior.BuildingState.Builded && !IsCanWorkOnFarm(target))
                {
                    StopAction(true);
                    ActionIsDone(InteractigType.Farming);
                    return;
                }
                interactObject = target;
                SetAgentDestination(GetRandomPoint(target.transform.position, 3.0f));
                target = null;
                return;
            }

            // Reset path if target moved
            var targetPoint = target.transform.position;
            if (targetBaseBehavior.pointToInteract != null)
                targetPoint = targetBaseBehavior.pointToInteract.transform.position;
            SetAgentDestination(GetClosestPositionToTarget(targetPoint));

            float minDistance = minDistanceToInteract;
            bool reachDest = false;
            bool isTargetEnemy = false;
            if (targetBaseBehavior != null)
                if (IsTeamEnemy(targetBaseBehavior.team) && statisic.attackType != AttackType.None)
                    isTargetEnemy = true;

            if (isTargetEnemy && targetBaseBehavior.live)
                minDistance = statisic.rangeAttack;

            bool targetIsPoint = false;
            bool colliderCheck = true;
            if (isBuilding)
            {
                targetBuildingBehavior = target.GetComponent<BuildingBehavior>();
                if (targetBuildingBehavior.sourceType == SourceType.Farm)
                {
                    targetIsPoint = true;
                    colliderCheck = false;
                }
            }

            float dist = Vector3.Distance(gameObject.transform.position, target.transform.position);
            RaycastHit hit;
            // If unit and target close enough
            if (targetIsPoint && Vector3.Distance(gameObject.transform.position, agent.destination) <= minDistance && Vector3.Distance(gameObject.transform.position, target.transform.position) < 5.0f)
                if (dist <= minDistance && Physics.Linecast(transform.position, target.transform.position))
                    reachDest = true;
            // Or collisions touch each other
            if (colliderCheck && GetComponent<Collider>().bounds.Intersects(target.GetComponent<Collider>().bounds))
                reachDest = true;

            float magnitDistance = 2.0f;
            if (targetBuildingBehavior != null)
                magnitDistance = targetBuildingBehavior.magnitDistance;

            if (false)
            {
                if (dist <= magnitDistance && Physics.Linecast(transform.position, target.transform.position + new Vector3(0, 1, 0)))
                {
                    SetAgentStopped(true);
                    Vector3 targetPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, agent.speed * 0.9f * Time.deltaTime);
                    transform.LookAt(targetPosition);
                }
                else
                    SetAgentStopped(false);
            }

            if (reachDest)
            {
                // Attack it target if target is enemy or stop
                if (targetBaseBehavior != null && IsTeamEnemy(targetBaseBehavior.team) && targetBaseBehavior.live)
                {
                    if (statisic.attackType != AttackType.None)
                        Attack(target);
                }
                else
                {
                    StartInteract(target);
                }
            }
        }
        else if (target == null && !IsStopped() && Vector3.Distance(gameObject.transform.position, base.agent.destination) <= 0.5f)
        {
            if (interactObject != null)
            {
                StartInteract(interactObject);
            }
            else
            {
                target = null;
                ActionIsDone();
            }
        }
        if (tempDestinationPoint != Vector3.zero && target == null)
        {
            SetAgentStopped(false);
            SetAgentDestination(tempDestinationPoint);
            tempDestinationPoint = Vector3.zero;
        }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
    }

    public bool DoInteract()
    {
        UnityEngine.Profiling.Profiler.BeginSample("p DoInteract"); // Profiler
        if (workingTimer >= 5.0f)
        {
            if (interactObject != null)
            {
                BuildingBehavior interactObjectBuildingBehavior = interactObject.GetComponent<BuildingBehavior>();
                if (interactObjectBuildingBehavior != null && interactObjectBuildingBehavior.sourceType == SourceType.Farm)
                {
                    GiveOrder(interactObject, false, false);
                    workingTimer = 0.0f;
                    return true;
                }
            }
        }
        // Interact with something
        if (
            interactType == InteractigType.Bulding || interactType == InteractigType.Gathering ||
            interactType == InteractigType.CuttingTree || interactType == InteractigType.Farming)
        {
            if (interactObject != null)
            {
                float distance = Vector3.Distance(interactObject.transform.position, transform.position);
                if (distance < interactDistance + 0.5f || interactDistance == 0.0f)
                {
                    if (interactType == InteractigType.Gathering || interactType == InteractigType.CuttingTree || interactType == InteractigType.Farming)
                    {
                        BaseBehavior baseObjectBehaviorComponent = interactObject.GetComponent<BaseBehavior>();
                        if (baseObjectBehaviorComponent != null)
                        {
                            ResourceGatherInfo resourceInfo = new ResourceGatherInfo();
                            var objectResourceType = baseObjectBehaviorComponent.resourceCapacityType;
                            if (baseObjectBehaviorComponent.sourceType == SourceType.Farm)
                                resourceInfo = farmInfo;
                            else
                            {
                                resourceInfo = resourceGatherInfo.Find(x => x.type == objectResourceType);
                                transform.LookAt(interactObject.transform.position);
                            }

                            interactTimer -= Time.deltaTime;
                            workingTimer += Time.deltaTime;
                            if (interactTimer <= 0)
                            {
                                interactTimer = 1.0f;

                                // if (interactAnimation != "")
                                //     anim.SetBool(interactAnimation, true);

                                if (resourceType != objectResourceType)
                                {
                                    resourceHold = 0.0f;
                                    resourceType = objectResourceType;
                                }

                                resourceHold += resourceInfo.gatherPerSecond;

                                if (!baseObjectBehaviorComponent.resourceEndless)
                                    if (baseObjectBehaviorComponent.resourceCapacity > resourceInfo.gatherPerSecond)
                                    {
                                        if (resourceHold < resourceInfo.maximumCapacity)
                                        {
                                            baseObjectBehaviorComponent.resourceCapacity -= resourceInfo.gatherPerSecond;
                                        }
                                    }
                                    else
                                    {
                                        // Resources is out
                                        baseObjectBehaviorComponent.ResourcesIsOut(gameObject);
                                        anim.SetBool(interactAnimation, false);
                                        interactAnimation = "";
                                        baseObjectBehaviorComponent.resourceCapacity = 0;
                                        resourceHold += resourceInfo.gatherPerSecond;
                                        interactObject = null;
                                        ActionIsDone(stopActionType: interactType);
                                        interactType = InteractigType.None;
                                    }

                                if (resourceHold >= resourceInfo.maximumCapacity)
                                {
                                    // Go to store resource
                                    anim.SetBool(interactAnimation, false);
                                    interactAnimation = "";
                                    anim.SetBool("Carry", true);
                                    GoToStoreResources();
                                    interactType = InteractigType.None;
                                }
                            }
                        }
                    }
                    if (interactType == InteractigType.Bulding)
                    {
                        interactTimer -= Time.deltaTime;
                        workingTimer += Time.deltaTime;
                        if (interactTimer <= 0)
                        {
                            interactTimer = 1.0f;
                            BuildingBehavior interactObjectBuildingBehavior = interactObject.GetComponent<BuildingBehavior>();
                            anim.SetBool(interactAnimation, true);

                            if (interactObjectBuildingBehavior.sourceType != SourceType.Farm)
                                SendSoundEvent(SoundEventType.Build);

                            bool builded = interactObjectBuildingBehavior.RepairOrBuild(buildHpPerSecond);
                            if (builded || !interactObjectBuildingBehavior.live)
                            {
                                if (interactObjectBuildingBehavior.sourceType == SourceType.Farm)
                                {
                                    GiveOrder(interactObject, false, false);
                                }
                                else
                                {
                                    StopAction(true);
                                    ActionIsDone(stopActionType: InteractigType.Bulding);
                                }
                            }
                        }
                    }
                }
                else
                {
                    GameObject tempTarget = interactObject;
                    StopAction(true);

                    GiveOrder(tempTarget, false, false);
                }
            }
            else
            {
                StopAction(true);
                ActionIsDone();
                if (target != null)
                {
                    StartInteract(target);
                }
                else
                {
                    Debug.Log(gameObject.name + ": " + interactType + "no target");
                }
            }
        }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        return false;
    }

    public void UpdateToolInHand()
    {
        UnityEngine.Profiling.Profiler.BeginSample("p UpdateToolInHand"); // Profiler
        toolInHandTimer = 0.0f;
        ToolInfo toolInfo = GetToolInfo();
        // Create tool in hand
        if (toolInfo != null && holderObject == null && toolInfo.prefab != null)
        {
            if (timerToCreateHolder >= 0.5f)
            {
                holderToolInfo = toolInfo;
                holderObject = (GameObject)Instantiate(toolInfo.prefab, toolInfo.holder.transform.position, toolInfo.holder.transform.rotation);
                holderObject.transform.SetParent(toolInfo.holder.transform);
            }
        }
        // Destroy
        if ((toolInfo == null || holderToolInfo != toolInfo) && holderObject != null)
        {
            timerToCreateHolder = 0.0f;
            holderToolInfo = null;
            Destroy(holderObject);
        }

        if (interactType == InteractigType.None && resourceGatherInfo.Count > 0)
            if (resourceHold <= 0)
            {
                anim.SetBool("Carry", false);
            }
            else
                anim.SetBool("Carry", true);
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
    }

    public bool IsCanWorkOnFarm(GameObject farm, bool displayErrorMessage = true)
    {
        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
        {
            UnitBehavior workerUnitBehavior = unit.GetComponent<UnitBehavior>();
            if (workerUnitBehavior.interactObject == farm && unit != gameObject)
            {
                UIBaseScript cameraUIBaseScript = Camera.main.GetComponent<UIBaseScript>();
                cameraUIBaseScript.DisplayMessage("Only one worker can work in farm", 1500, "farmError");
                return false;
            }
        }
        return true;
    }

    private Vector3 newAgentDestination = new Vector3();
    public void SetAgentDestination(Vector3 newDestination)
    {
        newAgentDestination = newDestination;

    }
    public bool IsStopped()
    {
        if (!agent.enabled)
            return true;
        if (!base.agent.pathPending && !base.agent.hasPath || base.agent.isStopped)
            return true;
        return false;
    }

    private bool isAgentStopped = false;
    public void SetAgentStopped(bool newState)
    {
        isAgentStopped = newState;
    }

    public override bool IsIdle()
    {
        if (target == null && IsStopped() && interactObject == null)
            return true;
        return false;
    }

    public ToolInfo GetToolInfo()
    {
        UnityEngine.Profiling.Profiler.BeginSample("p GetToolInfo"); // Profiler
        ToolInfo toolInfo = null;
        if(resourceType != ResourceType.None || interactType == InteractigType.Bulding)
        {
            ResourceGatherInfo resourceInfo = resourceGatherInfo.Find(x => x.type == resourceType);
            if (resourceInfo != null)
            {
                if (interactType == InteractigType.None && resourceHold > 0)
                {
                    toolInfo = resourceInfo.carryObject;
                    anim.SetBool("Carry", true);
                }
                if (interactType == InteractigType.CuttingTree || interactType == InteractigType.Gathering)
                {
                    toolInfo = resourceInfo.toolObject;
                }
                if (interactType == InteractigType.Farming)
                {
                    toolInfo = farmInfo.toolObject;
                }
            }
            if (toolInfo == null)
            {
                if (interactType == InteractigType.Bulding && buildingTool != null)
                {
                    if (interactObject != null && interactObject.GetComponent<BuildingBehavior>().sourceType == SourceType.Farm)
                        toolInfo = farmInfo.toolObject;
                    else
                        toolInfo = buildingTool;
                }
                if (interactType == InteractigType.Attacking && weapon != null)
                    toolInfo = weapon;
            }
        }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        return toolInfo;
    }

    public GameObject GetObjectToInteract(InteractigType type = InteractigType.None, ResourceType targetResource = ResourceType.None)
    {
        UnityEngine.Profiling.Profiler.BeginSample("p GetObjectToInteract"); // Profiler
        Dictionary<GameObject, float> objects = new Dictionary<GameObject, float>();
        var allObjects = GetObjectsInRange(transform.position, 15.0f, team: -1).Concat(GameObject.FindGameObjectsWithTag("Ambient")).ToArray();
        foreach (GameObject oneObject in allObjects)
        {
            BaseBehavior oneObjectBaseBehavior = oneObject.GetComponent<BaseBehavior>();
            float distance = Vector3.Distance(gameObject.transform.position, oneObject.transform.position);
            if (distance > 15.0f && (oneObjectBaseBehavior.team == team))
                continue;

            if (type == InteractigType.Bulding)
            {
                BuildingBehavior oneObjectBuildingBehavior = oneObject.GetComponent<BuildingBehavior>();
                if (oneObjectBuildingBehavior != null &&
                    (oneObjectBuildingBehavior.state == BuildingBehavior.BuildingState.Building ||
                        oneObjectBuildingBehavior.state == BuildingBehavior.BuildingState.Project))
                    objects.Add(oneObject, distance);
            }
            else
            {
                if (oneObjectBaseBehavior.resourceCapacityType == resourceType && 
                    (oneObjectBaseBehavior.resourceCapacity > 0 || oneObjectBaseBehavior.resourceEndless))
                    objects.Add(oneObject, distance);
            }
        }
        if (objects.Count > 0)
        {
            var buildingsList = objects.ToList();
            buildingsList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            return buildingsList.First().Key;
        }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        return null;
    }

    public void ActionIsDone(InteractigType stopActionType = InteractigType.None)
    {
        SetAgentStopped(true);
        Destroy(pointMarker);
        stucked = false;

        bool isQueueNewOrder = SendOrderFromQueue();
        if (isQueueNewOrder)
            return;
             
        // Find new target
        GameObject newTarget = GetObjectToInteract(type: stopActionType);
        if (newTarget != null)
        {
            GiveOrder(newTarget, false, false);

            Destroy(pointMarker);
            return;
        }

        SendOrderFromQueue();
        return;
    }

    public bool SendOrderFromQueue()
    {
        if (queueCommands.Count > 0)
        {
            if (queueCommands[0] is Vector3)
            {
                GiveOrder((Vector3)queueCommands[0], false, false);
            }
            else
            {
                GiveOrder((GameObject)queueCommands[0], false, false);
            }
            queueCommands.RemoveAt(0);
            return true;
        }
        return false;
    }

    public override void _StartInteract(GameObject targetObject)
    {
        UnityEngine.Profiling.Profiler.BeginSample("p _StartInteract"); // Profiler
        SetAgentStopped(true);

        stucked = false;
        interactDistance = Vector3.Distance(targetObject.transform.position, transform.position);
        // Start build object
        BuildingBehavior targetBuildingBehavior = targetObject.GetComponent<BuildingBehavior>();
        BaseBehavior targetBaseBehavior = targetObject.GetComponent<BaseBehavior>();
        UnitBehavior unitBehaviorComponent = targetObject.GetComponent<UnitBehavior>();
        if (targetBuildingBehavior != null && buildHpPerSecond > 0 && targetBuildingBehavior.team == team)
        {
            if (targetBuildingBehavior.health < targetBuildingBehavior.maxHealth)
            {
                bool canBuild = targetBuildingBehavior.IsUnitCanBuild(gameObject);
                if (canBuild)
                {
                    if (PhotonNetwork.InRoom)
                        targetObject.GetComponent<PhotonView>().RPC("SetAsBuilding", PhotonTargets.All, targetObject.transform.position);
                    else
                        targetBuildingBehavior.SetAsBuilding(newPosition: targetObject.transform.position);

                    interactTimer = 1.0f;
                    transform.LookAt(targetObject.transform.position);

                    if (targetBuildingBehavior.sourceType == SourceType.Farm)
                        interactAnimation = "Farming";
                    else
                        interactAnimation = "Builded";

                    anim.SetBool(interactAnimation, true);
                    interactObject = targetObject;
                    interactType = InteractigType.Bulding;
                    target = null;
                    blockedBuilding = false;
                    return;
                }
                else
                {
                    interactObject = targetObject;
                    target = null;
                    blockedBuilding = true;
                    return;
                }
            }
        }
        // Start gathering
        if ((targetBaseBehavior != null && targetBaseBehavior.resourceCapacity > 0) || (targetBuildingBehavior != null && targetBuildingBehavior.resourceEndless))
        {
            ResourceGatherInfo resourceInfo;
            if (targetBuildingBehavior != null && targetBuildingBehavior.sourceType == SourceType.Farm)
                resourceInfo = farmInfo;
            else
                resourceInfo = resourceGatherInfo.Find(x => x.type == targetBaseBehavior.resourceCapacityType);

            if(resourceInfo != null)
            {
                interactType = InteractigType.None;
                if (targetBuildingBehavior != null && targetBuildingBehavior.sourceType == SourceType.Farm && targetBaseBehavior.live &&
                    targetBuildingBehavior.state == BuildingBehavior.BuildingState.Builded)
                { 
                    interactType = InteractigType.Farming;
                    interactAnimation = "Farming";
                }
                else if (
                    targetBaseBehavior.sourceType == SourceType.Default &&
                    targetBaseBehavior.resourceCapacityType == BaseBehavior.ResourceType.Food && !targetBaseBehavior.live)
                {
                    interactType = InteractigType.Gathering;
                    interactAnimation = "Gather";
                }
                else if(targetBaseBehavior.resourceCapacityType == BaseBehavior.ResourceType.Gold)
                {
                    interactType = InteractigType.Gathering;
                    interactAnimation = "Mining";
                }
                else if (targetBuildingBehavior.sourceType == SourceType.Default && targetBaseBehavior.resourceCapacityType == BaseBehavior.ResourceType.Wood)
                {
                    interactType = InteractigType.CuttingTree;
                    interactAnimation = "Cutting";
                }
                if (interactType != InteractigType.None)
                {
                    if (resourceType != targetBaseBehavior.resourceCapacityType)
                    {
                        resourceHold = 0.0f;
                        resourceType = targetBaseBehavior.resourceCapacityType;
                    }

                    interactObject = targetObject;
                    anim.SetBool(interactAnimation, true);
                    transform.LookAt(targetObject.transform.position);
                    interactTimer = 1.0f;
                    target = null;
                    return;
                }
            }
        }
        // Store resources in building
        if (targetBaseBehavior != null)
        {
            ResourceType storedResource = targetBaseBehavior.storedResources.Find(x => x == resourceType);
            if (storedResource != ResourceType.None && resourceHold > 0 && targetBaseBehavior.live)
            {
                if (targetBuildingBehavior != null && targetBuildingBehavior.state != BuildingBehavior.BuildingState.Builded)
                {
                    target = null;
                    return;
                }

                CameraController cameraController = Camera.main.GetComponent<CameraController>();
                if (resourceType == ResourceType.Food)
                    cameraController.food += resourceHold;
                if (resourceType == ResourceType.Gold)
                    cameraController.gold += resourceHold;
                if (resourceType == ResourceType.Wood)
                    cameraController.wood += resourceHold;

                resourceHold = 0.0f;
                if (interactObject != null)
                {
                    GiveOrder(interactObject, false, false);
                    return;

                }
            }
        }
        target = null;
        ActionIsDone(interactType);
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
    }

    public void GoToStoreResources()
    {
        Dictionary<GameObject, float> buildings = new Dictionary<GameObject, float>();
        var allUnits = GameObject.FindGameObjectsWithTag("Building").Concat(GameObject.FindGameObjectsWithTag("Unit")).ToArray();
        foreach (GameObject building in allUnits)
        {
            BaseBehavior buildingBaseBehavior = building.GetComponent<BaseBehavior>();
            ResourceType storedResource = buildingBaseBehavior.storedResources.Find(x => x == resourceType);
            if (buildingBaseBehavior.team == team && storedResource != ResourceType.None && buildingBaseBehavior.live)
            {
                BuildingBehavior buildingBuildingBehavior = building.GetComponent<BuildingBehavior>();
                if (buildingBuildingBehavior != null && buildingBuildingBehavior.state != BuildingBehavior.BuildingState.Builded)
                    continue;

                float distance = Vector3.Distance(gameObject.transform.position, building.transform.position);
                buildings.Add(building, distance);
            }
        }
        if (buildings.Count > 0)
        {
            var buildingsList = buildings.ToList();
            buildingsList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            GameObject building = buildingsList.First().Key;

            GiveOrder(building, false, false);
        }
    }

    public override bool IsVisible()
    {
        if (GameInfo.playerSpectate)
            return true;

        if (team == cameraController.team && live)
            return true;
        else
        {
            if (visionCount > 0)
                return true;
        }
        return false;
    }

    public void Attack(GameObject target)
    {
        if (!live)
            return;

        SendSoundEvent(SoundEventType.Attack);

        SetAgentStopped(true);
        UnitStatistic statisic = GetStatisticsInfo();

        attacked = false;
        transform.LookAt(target.transform.position);
        anim.Rebind();
        anim.SetTrigger("Attack");
        attackTimer = statisic.attackTime;
        attackDelayTimer = statisic.attackDelay;
        interactType = InteractigType.Attacking;
    }

    public override void AlertAttacking(GameObject attacker)
    {
        BaseBehavior attackerBaseBehavior = attacker.GetComponent<BaseBehavior>();
        attackedTeam = attackerBaseBehavior.team;

        if (behaviorType == BehaviorType.Run)
        {
            if (target == null)
            {
                // Vector3 dirToTarget = (transform.position - attacker.transform.position).normalized;
                GiveOrder(GetRandomPoint(unitPosition, 10.0f), true, true);
            }
        }
    }

    public override void SendToDestroy()
    {
        sendToDestroy = true;
    }

    public override void ResourcesIsOut(GameObject worker)
    {
        timeToDestroy = 0.0f;
        SendToDestroy();
    }

    public override void TakeDamage(float damage, GameObject attacker)
    {
        if (!live)
            return;

        float newDamage = CalculateDamage(damage, attacker);

        SendAlertAttacking(attacker);
        AlertAttacking(attacker);
        health -= newDamage;
        TextBubble(String.Format("-{0:F0}", newDamage), 1000);

        if (bloodOnBroundEffects.Count > 0)
        {
            RaycastHit hitPoint;
            Physics.Linecast(transform.position, new Vector3(transform.position.x, transform.position.y - 20, transform.position.z), out hitPoint, LayerMask.GetMask("Terrain"));
            if (hitPoint.transform != null)
            {
                GameObject bloodPrefab = bloodOnBroundEffects[UnityEngine.Random.Range(0, bloodOnBroundEffects.Count)];
                var bloodObject = (GameObject)Instantiate(
                    bloodPrefab, 
                    hitPoint.point + new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), 0.1f, UnityEngine.Random.Range(-1.0f, 1.0f)), 
                    transform.rotation);
                // Destroy(bloodObject, 30.0f);
            }
        }

        if (health <= 0)
        {
            BecomeDead();

            UnitBehavior attackerBehaviorComponent = attacker.GetComponent<UnitBehavior>();
            attackerBehaviorComponent.ActionIsDone();
        }
        else
        {
            SendSoundEvent(SoundEventType.Hit);

            anim.Rebind();
            anim.SetTrigger("Damaged");
        }
        if (bloodEffect != null)
        {
            GameObject createdBloodEffect = Instantiate(bloodEffect, gameObject.transform.position, Quaternion.identity);
            createdBloodEffect.transform.parent = gameObject.transform;
            createdBloodEffect.transform.LookAt(attacker.transform.position);
            Destroy(createdBloodEffect, 1.5f);
        }
    }

    public override void BecomeDead()
    {
        SendSoundEvent(SoundEventType.Die);

        anim.Rebind();
        anim.SetTrigger("Die");
        live = false;
        // var collider = GetComponent<Collider>();
        // collider.enabled = false;
        agent.enabled = false;

        if (resourceCapacity <= 0)
            SendToDestroy();
    }

    public void StopAction(bool deleteObject)
    {
        interactDistance = 0.0f;
        DestroyPointMarker();
        SetAgentStopped(true);
        base.tempDestinationPoint = Vector3.zero;
        base.target = null;

        if (interactType == InteractigType.Attacking)
            anim.Rebind();

        if (interactType != InteractigType.None)
        {
            if(interactAnimation != "")
                anim.SetBool(interactAnimation, false);
        }
        if (deleteObject)
            interactObject = null;
        workingTimer = 0.0f;
        interactType = InteractigType.None;
    }

    public override void AddCommandToQueue(object newTarget)
    {
        queueCommands.Add(newTarget);
        if (newTarget is Vector3)
            CreateOrUpdatePointMarker(Color.green, (Vector3)newTarget, 1.0f, false);
        else
            CreateOrUpdatePointMarker(Color.green, ((GameObject)newTarget).transform.position, 1.0f, false);
    }

    [PunRPC]
    public override void _GiveOrder(Vector3 point, bool displayMarker, bool overrideQueueCommands, float speed = 0.0f)
    {
        if (speed != 0.0f)
            agent.speed = speed;
        else
            agent.speed = tempSpeed;

        if (overrideQueueCommands)
            SendSoundEvent(SoundEventType.TakeOrderToPoint);

        SendToPoint(point, displayMarker, overrideQueueCommands);
    }

    public void SendToPoint(Vector3 point, bool displayMarker, bool overrideQueueCommands)
    {
        if (overrideQueueCommands)
            queueCommands.Clear();

        if (!live)
            return;

        StopAction(true);

        if (overrideQueueCommands)
            unitPosition = point;

        base.tempDestinationPoint = point;
        if (displayMarker)
            CreateOrUpdatePointMarker(Color.green, point, 1.5f, true);
    }

    public override void _GiveOrder(GameObject targetObject, bool displayMarker, bool overrideQueueCommands, float speed = 0.0f)
    {
        if (overrideQueueCommands)
        {
            queueCommands.Clear();
            
            SendSoundEvent(SoundEventType.TakeOrderToTarget);
        }

        if (!live)
            return;

        StopAction(false);

        // Order to target
        SetAgentStopped(false);
        base.target = targetObject;

        Color colorMarker = Color.green;
        BaseBehavior targetBaseBehavior = targetObject.GetComponent<BaseBehavior>();
        if (targetBaseBehavior != null && IsTeamEnemy(targetBaseBehavior.team))
            colorMarker = Color.red;

        if (displayMarker)
            CreateOrUpdatePointMarker(colorMarker, target.transform.position, 1.5f, true);
    }

    public override bool[] UICommand(string commandName)
    {
        bool[] result = { false, false };
        if (!unitSelectionComponent.isSelected)
            return result;

        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        UIBaseScript cameraUIBaseScript = Camera.main.GetComponent<UIBaseScript>();
        if (team != cameraController.team || cameraController.chatInput)
            return result;

        bool[] skillResult = ActivateSkills(commandName);
        if (skillResult[0] || skillResult[1])
            return skillResult;

        if (commandName == "stop" || Input.GetKeyDown(KeyCode.H))
        {
            StopAction(true);
            return result;
        }
        if (commandName != null && commandName.Contains("behaviorType") || Input.GetKeyDown(KeyCode.T))
        {
            StopAction(true);
            var types = new BehaviorType[4] { BehaviorType.Aggressive, BehaviorType.Counterattack, BehaviorType.Hold, BehaviorType.Run };
            int pos = Array.FindIndex(types, w => w == behaviorType) + 1;
            if (pos > 3)
                pos = 0;
            foreach(GameObject unit in cameraController.GetSelectedObjects())
            {
                BaseBehavior unitBaseBehavior = unit.GetComponent<BaseBehavior>();
                unitBaseBehavior.behaviorType = types[pos];
            }
            result[0] = true;
            return result;
        }
        return result;
    }

    public override bool IsHealthVisible()
    {
        if (cameraController.tagsToSelect.Find(x => x.name == tag).healthVisibleOnlyWhenSelect && !unitSelectionComponent.isSelected)
            return false;
        return true;
    }

    public override List<string> GetStatistics()
    {
        List<string> statistics = base.GetStatistics();
        return statistics;
    }

    public override List<string> GetCostInformation()
    {
        List<string> statistics = new List<string>();
        statistics.Add(String.Format("Time to build: {0:F0} sec", skillInfo.timeToBuild));
        if (skillInfo.costFood > 0)
            statistics.Add(String.Format("Food: {0:F0}", skillInfo.costFood));
        if (skillInfo.costGold > 0)
            statistics.Add(String.Format("Gold: {0:F0}", skillInfo.costGold));
        if (skillInfo.costWood > 0)
            statistics.Add(String.Format("Wood: {0:F0}", skillInfo.costWood));
        return statistics;
    }

    public override UnitStatistic GetStatisticsInfo()
    {
        return defaultStatistic;
    }
}
