using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Photon.Pun;
using GangaGame;
using UISpace;
using CI.QuickSave;

public class UnitBehavior : BaseBehavior, IPunObservable
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

        internal ResourceGatherInfo Clone()
        {
            return (ResourceGatherInfo)this.MemberwiseClone();
        }
    }

    #endregion

    #region Gatering worker info

    [Header("Gatering worker info")]
    public List<ResourceGatherInfo> resourceGatherInfo = new List<ResourceGatherInfo>();
    private string interactAnimation = "";

    public ResourceGatherInfo farmInfo;
    private float interactTimer = 0.0f;
    private float interactDistance = 0.0f;

    #endregion

    private float workingTimer = 0.0f;
    private bool blockedBuilding = false;
    private float toolInHandTimer = 0.0f;
    private float timetToStuck = 0.0f;

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.interactType);
            stream.SendNext(this.health);
            stream.SendNext(this.live);
        }
        else
        {
            this.interactType = (InteractigType)stream.ReceiveNext();
            this.health = (int)stream.ReceiveNext();
            this.live = (bool)stream.ReceiveNext();
        }
    }

    public override void Awake()
    {
        base.Awake();

        health = maxHealth;
        resourceHold = 0.0f;
        interactType = InteractigType.None;

        tempSpeed = agent.speed;
        resourceType = ResourceType.None;
    }

    float newIntersectionTimer = 0.0f;
    Vector3 curMove;
    public void Update()
    {
        UnityEngine.Profiling.Profiler.BeginSample("p Base Update"); // Profiler

        UpdateIsInCameraView();

        UpdateVision();

        UpdateHealth();

        UpdateDestroyBehavior();

        if (!live)
            return;
        
        UpdateProductionQuery();

        CheckTargetReached();

        UpdateAttack();

        if (unitSelectionComponent.isSelected && UnityEngine.Input.anyKeyDown)
            UICommand(null);

        UnityEngine.Profiling.Profiler.EndSample(); // Profiler

        UnityEngine.Profiling.Profiler.BeginSample("p UserUpdate"); // Profiler
        
        #region Animations
        curMove = transform.position - previousPosition;
        curSpeed = curMove.magnitude / Time.deltaTime;
        previousPosition = transform.position;
        anim.SetFloat("Speed", curSpeed);
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

        UnityEngine.Profiling.Profiler.BeginSample("p UpdateToolInHand"); // Profiler
        toolInHandTimer += Time.fixedDeltaTime;
        timerToCreateHolder += Time.fixedDeltaTime;
        if (toolInHandTimer > 0.25f)
        {
            // TODO: change to events instead check in update
            UpdateToolInHand();
            toolInHandTimer = 0.0f;
        }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler

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
        if (isAgentStopped && agent.enabled)
        {
            agent.ResetPath();
            base.agent.isStopped = true;
            agent.enabled = false;
            if (obstacle != null)
                obstacle.enabled = true;
        }
        if (!isAgentStopped && !agent.enabled)
        {
            if (obstacle != null)
                obstacle.enabled = false;
        }
        
        newIntersectionTimer += Time.fixedDeltaTime;
        if (toolInHandTimer > 1.0f)
        {
            UnityEngine.Profiling.Profiler.BeginSample("p Getting is outlineDraw"); // Profiler
            bool newIntersection = false;
            if (team > 0 && IsInCameraView() && IsVisible() && live && Physics.Linecast(transform.position + new Vector3(0, 1.0f, 0), Camera.main.transform.position))
                newIntersection = true;
            else
                newIntersection = false;

            if (newIntersection != unitSelectionComponent.intersection)
            {
                unitSelectionComponent.intersection = newIntersection;
                unitSelectionComponent.SetOutline(newIntersection);
            }
            newIntersectionTimer = 0.0f;
        }

        if (tempDestinationPoint != Vector3.zero && target == null)
        {
            SetAgentStopped(false);
            SetAgentDestination(tempDestinationPoint);
            tempDestinationPoint = Vector3.zero;
        }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
    }

    public override void UpdateIsInCameraView(bool newState = false)
    {
        isInCameraView = CameraController.IsInCameraView(transform.position);
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
        InteractigType interactTypeTemp = interactType;
        stucked = true;
        StopAction(false, SendRPC: true);
        ActionIsDone(stopActionType: interactTypeTemp);
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

    float minDistance = 0.0f;
    bool colliderCheck = true;
    bool attackTarget = false;
    RaycastHit hit;
    public void CheckTargetReached()
    {
        UnityEngine.Profiling.Profiler.BeginSample("p CheckTargetReached"); // Profiler
        if (interactType == InteractigType.None && target != null)
        {
            if (Time.frameCount % 15 == 0)
                if (target.GetComponent<UnitBehavior>() != null)
                    SetAgentDestination(GetClosestPositionToTarget(target.transform.position));

            bool reachDest = false;
            // If unit and target close enough
            Vector3 offset = new Vector3(0, 0.5f, 0);
            if (Physics.Raycast(transform.position + offset, (target.transform.position + offset - (transform.position + offset)).normalized, out hit, minDistance))
            {
                if (hit.transform.gameObject.GetHashCode() == target.GetHashCode())
                {
                    if (Vector3.Distance(transform.position, target.transform.position) < minDistance)
                        reachDest = true;
                }
            }
            // Or collisions touch each other
            if (colliderCheck && GetComponent<Collider>().bounds.Intersects(target.GetComponent<Collider>().bounds))
                reachDest = true;

            if (reachDest)
            {
                // Attack it target if target is enemy or stop
                if (attackTarget)
                    Attack(target);
                else
                    StartInteract(target);
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
                                resourceInfo = GetResourceFarmInfo(sourceType: SourceType.Farm);
                            else
                            {
                                resourceInfo = GetResourceFarmInfo(resourceType: objectResourceType);
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
        ResourceGatherInfo resourceInfo = resourceGatherInfo.Find(x => x.type == ResourceType.Food);
        if (resourceInfo == null)
            return false;

        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
        {
            UnitBehavior workerUnitBehavior = unit.GetComponent<UnitBehavior>();
            if (workerUnitBehavior.interactObject == farm && unit != gameObject)
                return false;
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
    public override void SetAgentStopped(bool newState)
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

        if (!IsVisible())
            return toolInfo;

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

    BaseBehavior objectToInteractBaseBehavior;
    BuildingBehavior oneObjectBuildingBehavior;
    GameObject newTarget = null;
    Dictionary<GameObject, float> objects = new Dictionary<GameObject, float>();
    public GameObject GetObjectToInteract(InteractigType type = InteractigType.None, float maxDistance = 15.0f)
    {
        allObjects.Clear();
        objects.Clear();
        UnityEngine.Profiling.Profiler.BeginSample("p GetObjectToInteract"); // Profiler
        GetObjectsInRange(ref allObjects, transform.position, maxDistance, team: -1);
        foreach (GameObject objectToInteract in allObjects.Concat(GameObject.FindGameObjectsWithTag("Ambient")))
        {
            objectToInteractBaseBehavior = objectToInteract.GetComponent<BaseBehavior>();
            float distance = Vector3.Distance(gameObject.transform.position, objectToInteract.transform.position);
            if (distance > 15.0f && (objectToInteractBaseBehavior.team == team))
                continue;

            if (type == InteractigType.Bulding)
            {
                oneObjectBuildingBehavior = objectToInteract.GetComponent<BuildingBehavior>();
                if (oneObjectBuildingBehavior != null &&
                    (oneObjectBuildingBehavior.GetState() == BuildingBehavior.BuildingState.Building ||
                        oneObjectBuildingBehavior.GetState() == BuildingBehavior.BuildingState.Project))
                    if(!objects.ContainsKey(objectToInteract))
                        objects.Add(objectToInteract, distance);
            }
            else if(type != InteractigType.None)
            {
                if (objectToInteractBaseBehavior.resourceCapacityType == resourceType && 
                    (objectToInteractBaseBehavior.resourceCapacity > 0 || objectToInteractBaseBehavior.resourceEndless))
                    if (!objects.ContainsKey(objectToInteract))
                    {
                        if(objectToInteractBaseBehavior.sourceType == SourceType.Farm)
                        {
                            if(IsCanWorkOnFarm(objectToInteract))
                                objects.Add(objectToInteract, distance);
                        }
                        else
                            objects.Add(objectToInteract, distance);
                    }
            }
        }
        if (objects.Count > 0)
        {
            newTarget = null;
            float minDist = maxDistance;
            foreach (var objectInfo in objects)
                if(objectInfo.Value < minDist)
                {
                    newTarget = objectInfo.Key;
                    minDist = objectInfo.Value;
                }
            
            return newTarget;
        }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        return null;
    }

    public override void ActionIsDone(InteractigType stopActionType = InteractigType.None)
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
        if (targetObject == null)
            return;

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
                    targetBuildingBehavior.SetState(BuildingBehavior.BuildingState.Building);

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
                    targetBuildingBehavior.GetState() == BuildingBehavior.BuildingState.Builded)
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
                if (targetBuildingBehavior != null && targetBuildingBehavior.GetState() != BuildingBehavior.BuildingState.Builded)
                {
                    target = null;
                    return;
                }

                CameraController cameraController = Camera.main.GetComponent<CameraController>();
                cameraController.resources[resourceType] += resourceHold;

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

    List<GameObject> allUnits = new List<GameObject>();
    public void GoToStoreResources()
    {
        Dictionary<GameObject, float> buildings = new Dictionary<GameObject, float>();
        allUnits.Clear();
        allUnits.AddRange(GameObject.FindGameObjectsWithTag("Building").Concat(GameObject.FindGameObjectsWithTag("Unit")));
        foreach (GameObject building in allUnits)
        {
            BaseBehavior buildingBaseBehavior = building.GetComponent<BaseBehavior>();
            ResourceType storedResource = buildingBaseBehavior.storedResources.Find(x => x == resourceType);
            if (buildingBaseBehavior.team == team && storedResource != ResourceType.None && buildingBaseBehavior.live)
            {
                BuildingBehavior buildingBuildingBehavior = building.GetComponent<BuildingBehavior>();
                if (buildingBuildingBehavior != null && buildingBuildingBehavior.GetState() != BuildingBehavior.BuildingState.Builded)
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

    public override void Attack(GameObject target)
    {
        if (!live)
            return;

        base.Attack(target);

        transform.LookAt(target.transform.position);
        SetAgentStopped(true);
        anim.Rebind();
        anim.SetFloat("AttackSpeed", GetStatisticsInfo().attackTime);
        anim.SetTrigger("Attack");
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

    public override void _SendToDestroy()
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

        TextBubble(new StringBuilder(30).AppendFormat("-{0:F0}", newDamage).ToString(), 1000);

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

            BaseBehavior attackerBehaviorComponent = attacker.GetComponent<BaseBehavior>();
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
        base.BecomeDead();

        SendSoundEvent(SoundEventType.Die);

        anim.Rebind();
        anim.SetTrigger("Die");
        live = false;
        // var collider = GetComponent<Collider>();
        // collider.enabled = false;
        agent.enabled = false;

        UpdateVisionTool();

        if (resourceCapacity <= 0)
            SendToDestroy();
    }

    [PunRPC]
    public override void _StopAction(bool deleteObject = false, bool agentStop = true)
    {
        interactDistance = 0.0f;
        DestroyPointMarker();

        if (agentStop)
            SetAgentStopped(true);

        base.tempDestinationPoint = Vector3.zero;
        base.target = null;

        if (agentStop && interactType == InteractigType.Attacking)
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
            CreateOrUpdatePointMarker(Color.green, (Vector3)newTarget, 1.0f, false, PointMarker.MarkerType.Point);
        else
        {
            PointMarker.MarkerType markerType = PointMarker.MarkerType.Point;
            if (((GameObject)newTarget).GetComponent<BuildingBehavior>() != null)
                markerType = PointMarker.MarkerType.Arrow;

            Color newColor = Color.green;
            BaseBehavior targetBaseBehavior = ((GameObject)newTarget).GetComponent<BaseBehavior>();
            if (targetBaseBehavior != null && IsTeamEnemy(targetBaseBehavior.team) && targetBaseBehavior.IsVisible())
                newColor = Color.red;

            CreateOrUpdatePointMarker(newColor, ((GameObject)newTarget).transform.position, 1.0f, false, markerType);
        }
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

        StopAction(true, agentStop: false);

        if (overrideQueueCommands)
            unitPosition = point;

        base.tempDestinationPoint = point;
        if (displayMarker)
            CreateOrUpdatePointMarker(Color.green, point, 1.5f, true, PointMarker.MarkerType.Point);
    }

    public override void _GiveOrder(GameObject targetObject, bool displayMarker, bool overrideQueueCommands, float speed = 0.0f)
    {
        if (targetObject == null)
            return;

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
        target = targetObject;

        Color colorMarker = Color.green;
        BaseBehavior targetBaseBehavior = targetObject.GetComponent<BaseBehavior>();
        if (targetBaseBehavior != null && IsTeamEnemy(targetBaseBehavior.team) && targetBaseBehavior.IsVisible())
            colorMarker = Color.red;

        PointMarker.MarkerType markerType = PointMarker.MarkerType.Point;
        if(targetObject.GetComponent<BuildingBehavior>() != null)
            markerType = PointMarker.MarkerType.Arrow;

        if (displayMarker)
            CreateOrUpdatePointMarker(colorMarker, target.transform.position, 1.5f, true, markerType);

        UnitStatistic statisic = GetStatisticsInfo();
        
        bool isBuilding = target.GetComponent<BuildingBehavior>() != null;
        BuildingBehavior targetBuildingBehavior = null;
        if (isBuilding)
            targetBuildingBehavior = target.GetComponent<BuildingBehavior>();

        if (isBuilding && targetBuildingBehavior.sourceType == SourceType.Farm)
        {
            if (targetBuildingBehavior.GetState() == BuildingBehavior.BuildingState.Builded && !IsCanWorkOnFarm(target))
            {
                UIBaseScript cameraUIBaseScript = Camera.main.GetComponent<UIBaseScript>();
                cameraUIBaseScript.DisplayMessage("Only one worker can work in farm", 1500, "farmError");

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

        minDistance = minDistanceToInteract;

        if (IsTeamEnemy(targetBaseBehavior.team) && targetBaseBehavior.live)
            minDistance = statisic.rangeAttack;

        if (isBuilding)
        {
            targetBuildingBehavior = target.GetComponent<BuildingBehavior>();
            if (targetBuildingBehavior.sourceType == SourceType.Farm)
                colliderCheck = false;
        }

        if (targetBaseBehavior != null && IsTeamEnemy(targetBaseBehavior.team) && targetBaseBehavior.live && statisic.attackType != AttackType.None)
            attackTarget = true;
        else
            attackTarget = false;
    }

    bool[] skillResult;
    bool[] result = { false, false };
    BaseBehavior unitBaseBehavior;
    public override bool[] UICommand(string commandName)
    {
        UnityEngine.Profiling.Profiler.BeginSample("p UICommand"); // Profiler

        result[0] = false;
        result[1] = false;
        
        if (!unitSelectionComponent.isSelected)
            return result;

        if (team != cameraController.team && ownerId != cameraController.userId || CameraController.isHotkeysBlocked)
            return result;
        
        skillResult = ActivateSkills(commandName);
        if (skillResult[0] || skillResult[1])
            return skillResult;

        if (commandName == "stop" || Input.GetKeyDown(KeyCode.H))
        {
            StopAction(true, SendRPC: true);
            return result;
        }
        if (commandName != null && commandName.Contains("behaviorType") || Input.GetKeyDown(KeyCode.T))
        {
            StopAction(true, SendRPC: true);
            var types = new BehaviorType[4] { BehaviorType.Aggressive, BehaviorType.Counterattack, BehaviorType.Hold, BehaviorType.Run };
            int pos = Array.FindIndex(types, w => w == behaviorType) + 1;
            if (pos > 3)
                pos = 0;
            foreach(GameObject unit in cameraController.selectedObjects)
            {
                unitBaseBehavior = unit.GetComponent<BaseBehavior>();
                unitBaseBehavior.behaviorType = types[pos];
            }
            cameraUIBaseScript.UpdateCommands();
            result[0] = true;
            return result;
        }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        return result;
    }

    public override bool IsHealthVisible()
    {
        if (!unitSelectionComponent.isSelected)
            foreach (var tagToSelect in cameraController.tagsToSelect)
                if (tagToSelect.healthVisibleOnlyWhenSelect && gameObject.CompareTag(tagToSelect.name))
                    return false;
        return true;
    }
    
    public override List<string> GetCostInformation()
    {
        statisticStrings.Clear();
        statisticStrings.Add(new StringBuilder(30).AppendFormat("Time to build: {0:F0} sec", skillInfo.timeToBuild).ToString());
        if (skillInfo.costFood > 0)
            statisticStrings.Add(new StringBuilder(30).AppendFormat("Food: {0:F0}", skillInfo.costFood).ToString());
        if (skillInfo.costGold > 0)
            statisticStrings.Add(new StringBuilder(30).AppendFormat("Gold: {0:F0}", skillInfo.costGold).ToString());
        if (skillInfo.costWood > 0)
            statisticStrings.Add(new StringBuilder(30).AppendFormat("Wood: {0:F0}", skillInfo.costWood).ToString());
        if (skillInfo.takesLimit > 0)
            statisticStrings.Add(new StringBuilder(30).AppendFormat("Limit cost: {0}", skillInfo.takesLimit).ToString());
        return statisticStrings;
    }

    private UnitStatistic _defaultStatistic;
    public override UnitStatistic GetStatisticsInfo()
    {
        _defaultStatistic = defaultStatistic;
        foreach (KeyValuePair<BaseSkillScript.UpgradeType, int> typesInfo in CameraController.GetUpgrades(ownerId))
        {
        }
        return _defaultStatistic;
    }

    public ResourceGatherInfo GetResourceFarmByType(InteractigType interactigType, ResourceType resourceType = ResourceType.None)
    {
        if (interactigType == InteractigType.Farming)
            return GetResourceFarmInfo(sourceType: SourceType.Farm);
        else
            return GetResourceFarmInfo(resourceType: resourceType);
    }

    public ResourceGatherInfo GetResourceFarmInfo(ResourceType resourceType = ResourceType.None, SourceType sourceType = SourceType.Default)
    {
        ResourceGatherInfo resourceInfo = new ResourceGatherInfo();
        if (sourceType == SourceType.Farm)
            resourceInfo = farmInfo.Clone();
        else if(resourceType != ResourceType.None)
            resourceInfo = resourceGatherInfo.Find(x => x.type == resourceType).Clone();

        foreach (KeyValuePair<BaseSkillScript.UpgradeType, int> typesInfo in CameraController.GetUpgrades(ownerId))
        {
            if (typesInfo.Key == BaseSkillScript.UpgradeType.Farm)
            {
                resourceInfo.gatherPerSecond += 0.05f * typesInfo.Value;
                resourceInfo.maximumCapacity += 2.5f * typesInfo.Value;
            }
            if (typesInfo.Key == BaseSkillScript.UpgradeType.Food)
            {
                resourceInfo.gatherPerSecond += 0.05f * typesInfo.Value;
                resourceInfo.maximumCapacity += 2.5f * typesInfo.Value;
            }
            if (typesInfo.Key == BaseSkillScript.UpgradeType.Gold)
            {
                resourceInfo.gatherPerSecond += 0.05f * typesInfo.Value;
                resourceInfo.maximumCapacity += 2.5f * typesInfo.Value;
            }
            if (typesInfo.Key == BaseSkillScript.UpgradeType.Wood)
            {
                resourceInfo.gatherPerSecond += 0.05f * typesInfo.Value;
                resourceInfo.maximumCapacity += 2.5f * typesInfo.Value;
            }
        }
        return resourceInfo;
    }
}
