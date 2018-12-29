using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Photon.Pun;

public class UnitBehavior : BaseBehavior
{
    [Header("Building worker info")]
    public float buildHpPerSecond = 2.5f;
    public ToolInfo buildingTool;
    private Vector3 targetOldPosition;
    [System.Serializable]
    public class ResourceGatherInfo
    {
        public ResourceType type = ResourceType.None;
        public float gatherPerSecond = 0.5f;
        public float maximumCapacity = 10.0f;
        public ToolInfo carryObject;
        public ToolInfo toolObject;
    }
     
    [Header("Gatering worker info")]
    public List<ResourceGatherInfo> resourceGatherInfo = new List<ResourceGatherInfo>();
    [HideInInspector]
    public float resourceHold = 0.0f;
    public ResourceType resourceType = ResourceType.None;
    private string interactAnimation = "";

    private GameObject interactObject;
    private float interactTimer = 0.0f;
    private float interactDistance = 0.0f;

    // Use this for initialization
    public override void Awake()
    {
        base.Awake();

        health = maxHealth;
        resourceHold = 0.0f;
        interactType = InteractigType.None;
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
        if (!live)
            return;

        #region Animations
        Vector3 curMove = transform.position - previousPosition;
        base.curSpeed = curMove.magnitude / Time.deltaTime;
        base.previousPosition = transform.position;
        base.anim.SetFloat("Speed", curSpeed);
        base.anim.SetFloat("AttackSpeed", attackTime);
        #endregion

        UnitSelectionComponent unitSelectionComponent = GetComponent<UnitSelectionComponent>();

        // Find target
        if (target == null && !base.agent.pathPending && !base.agent.hasPath)
        {
            if (behaviorType == BehaviorType.Aggressive)
                AttackNearEnemies(gameObject.transform.position, agrRange);
        }

        // Logic for tools in hand
        ToolInfo toolInfo = GetToolInfo();
        // Create tool in hand
        if (toolInfo != null && holderObject == null && toolInfo.prefab != null)
        {
            holderToolInfo = toolInfo;
            holderObject = (GameObject)Instantiate(toolInfo.prefab, toolInfo.holder.transform.position, toolInfo.holder.transform.rotation);
            holderObject.transform.SetParent(toolInfo.holder.transform);
        }
        // Destroy
        if ((toolInfo == null || holderToolInfo != toolInfo) && holderObject != null)
        {
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

        // Interact with something
        if (interactType == InteractigType.Bulding || interactType == InteractigType.Gathering || interactType == InteractigType.CuttingTree)
        {
            if (interactObject != null)
            {
                float distance = Vector3.Distance(interactObject.transform.position, transform.position);
                if (distance < interactDistance + 0.5f || interactDistance == 0.0f)
                {
                    if (interactType == InteractigType.Gathering || interactType == InteractigType.CuttingTree)
                    {
                        interactDistance = distance;
                        BaseBehavior baseObjectBehaviorComponent = interactObject.GetComponent<BaseBehavior>();
                        if (baseObjectBehaviorComponent != null)
                        {
                            ResourceGatherInfo resourceInfo = resourceGatherInfo.Find(x => x.type == baseObjectBehaviorComponent.resourceCapacityType);

                            interactTimer -= Time.deltaTime;
                            if (interactTimer <= 0)
                            {
                                interactTimer = 1.0f;
                                transform.LookAt(interactObject.transform.position);

                                if (interactAnimation != "")
                                    anim.SetBool(interactAnimation, true);

                                if (resourceType != baseObjectBehaviorComponent.resourceCapacityType)
                                {
                                    resourceHold = 0.0f;
                                    resourceType = baseObjectBehaviorComponent.resourceCapacityType;
                                }

                                if (baseObjectBehaviorComponent.resourceCapacity > resourceInfo.gatherPerSecond)
                                {
                                    if (resourceHold < resourceInfo.maximumCapacity)
                                    {
                                        baseObjectBehaviorComponent.resourceCapacity -= resourceInfo.gatherPerSecond;
                                        resourceHold += resourceInfo.gatherPerSecond;
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
                                    ActionIsDone();
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
                        interactDistance = distance;
                        interactTimer -= Time.deltaTime;
                        if (interactTimer <= 0)
                        {
                            interactTimer = 1.0f;
                            anim.SetBool("Builded", true);

                            BuildingBehavior targetBuildingBehavior = interactObject.GetComponent<BuildingBehavior>();
                            bool builded = targetBuildingBehavior.RepairOrBuild(buildHpPerSecond);
                            if (builded || !targetBuildingBehavior.live)
                            {
                                anim.SetBool("Builded", false);
                                interactObject = null;
                                base.agent.isStopped = true;
                                ActionIsDone();
                                interactType = InteractigType.None;
                            }
                        }
                    }
                }
                else
                {
                    GameObject tempTarget = interactObject;
                    StopAction(true);
                    GiveOrder(tempTarget, false);
                    interactObject = null;
                }
            }
            else
            {
                bool newCommand = ActionIsDone();
                if(!newCommand)
                    StopAction(true);
            }
        }

        // To avoid recalculate path
        if (target == null && targetOldPosition != null)
            targetOldPosition = new Vector3();

        // Move to target
        if (interactType == InteractigType.None && target != null)
        {
            BaseBehavior targetBaseBehavior = target.GetComponent<BaseBehavior>();
            if (Vector3.Distance(targetOldPosition, target.transform.position) > 0.4f)
            {
                BuildingBehavior targetBuildingBehavior = target.GetComponent<BuildingBehavior>();
                var targetPoint = target.transform.position;
                if (targetBaseBehavior.pointToInteract != null)
                    targetPoint = targetBaseBehavior.pointToInteract.transform.position;

                if (targetBuildingBehavior != null)
                {

                    NavMeshHit hitPosition;
                    // Finds the closest point on NavMesh within specified range.
                    if (NavMesh.SamplePosition(targetPoint, out hitPosition, 6.0f, NavMesh.AllAreas))
                        base.agent.destination = hitPosition.position;
                }
                else
                    base.agent.destination = targetPoint;

                targetOldPosition = targetPoint;
            }

            float minDistance = 0.4f;
            bool targetDest = false;
            bool isTargetEnemy = false;
            if (targetBaseBehavior != null)
                if (IsTeamEnemy(targetBaseBehavior.team))
                    isTargetEnemy = true;

            if (isTargetEnemy && targetBaseBehavior.live)
                minDistance = rangeAttack;

            float dist = Vector3.Distance(gameObject.transform.position, target.transform.position);
            RaycastHit hit;
            // If unit and target close enough
            if (dist <= minDistance && Physics.Linecast(transform.position, target.transform.position))
                targetDest = true;  
            // Or collisions touch each other
            else if (GetComponent<Collider>().bounds.Intersects(target.GetComponent<Collider>().bounds))
                targetDest = true;

            float magnitDistance = 3.0f;
            if (target.GetComponent<BuildingBehavior>() != null)
                magnitDistance = 6.0f;

            if (dist <= magnitDistance && Physics.Linecast(transform.position, target.transform.position + new Vector3(0, 1, 0)))
            {
                base.agent.isStopped = true;
                Vector3 targetPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, agent.speed * 0.9f * Time.deltaTime);
                transform.LookAt(targetPosition);
            }
            else
                base.agent.isStopped = false;

            if (targetDest)
            {
                // Attack it target if target is enemy or stop
                if (targetBaseBehavior != null && IsTeamEnemy(targetBaseBehavior.team) && targetBaseBehavior.live)
                {
                    //agent.ResetPath();
                    base.agent.isStopped = true;
                    if (canAttack)
                        Attack(target);
                }
                else
                {
                    ActionIsDone();
                    bool newOrder = StartInteract(target);
                    if (!newOrder)
                    {
                        target = null;
                        //agent.ResetPath();
                        base.agent.isStopped = true;
                    }
                }
            }
        }
        else if (target == null && !base.agent.isStopped && Vector3.Distance(gameObject.transform.position, base.agent.destination) <= 0.2f)
        {
            base.agent.isStopped = true;
            ActionIsDone();
        }
        if (tempDestinationPoint != Vector3.zero && target == null)
        {
            base.agent.isStopped = false;
            base.agent.destination = tempDestinationPoint;
            tempDestinationPoint = Vector3.zero;
        }

        // Attack
        if (interactType == InteractigType.Attacking && target != null)
        {
            BaseBehavior targetBaseBehavior = target.GetComponent<BaseBehavior>();
            if (IsTeamEnemy(targetBaseBehavior.team))
            {
                base.attackDelayTimer = base.attackDelayTimer - Time.deltaTime;
                if (!base.attacked && base.attackDelayTimer <= 0.0f && target != null)
                {
                    // Damage deal
                    base.attacked = true;
                    if (damageAfterTimer)
                        targetBaseBehavior.TakeDamage(damage, gameObject);
                    if (actionEffect != null)
                    {
                        ActionEffect unitActionEffect = actionEffect.transform.gameObject.GetComponent<ActionEffect>();
                        unitActionEffect.activate(gameObject, target, damage);
                    }
                }
                base.attackTimer = base.attackTimer - Time.deltaTime;
                if (base.attackTimer <= 0.0f)
                {
                    interactType = InteractigType.None;
                    base.agent.isStopped = false;

                    if (!targetBaseBehavior.live || tempDestinationPoint != Vector3.zero)
                    {
                        bool newOrder = StartInteract(target);
                        if (!newOrder)
                        {
                            base.agent.ResetPath();
                            target = null;
                            ActionIsDone();
                        }
                    }
                }
            }
        }

        if (pointMarkerPrefab != null)
            if (!unitSelectionComponent.isSelected || !live)
                DestroyPointMarker();
    }

    public ToolInfo GetToolInfo()
    {
        ToolInfo toolInfo = null;
        ResourceGatherInfo resourceInfo = resourceGatherInfo.Find(x => x.type == resourceType);
        if(resourceInfo != null)
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
        }
        if (toolInfo == null)
        {
            if (interactType == InteractigType.Bulding && buildingTool != null)
                toolInfo = buildingTool;
            if (interactType == InteractigType.Attacking && weapon != null)
                toolInfo = weapon;
        }
        return toolInfo;
    }

    public bool ActionIsDone()
    {
        if (interactType == InteractigType.Bulding || interactType == InteractigType.CuttingTree || interactType == InteractigType.Gathering)
        {
            List<CameraController.ObjectWithDistance> buildings = new List<CameraController.ObjectWithDistance>();
            var allUnits = GameObject.FindGameObjectsWithTag("Building").Concat(GameObject.FindGameObjectsWithTag("Ambient")).ToArray();
            foreach (GameObject building in allUnits)
            {
                float distance = Vector3.Distance(gameObject.transform.position, building.transform.position);
                if (distance > 10.0f)
                    continue;

                BuildingBehavior buildingBuildingBehavior = building.GetComponent<BuildingBehavior>();
                if (interactType == InteractigType.CuttingTree || interactType == InteractigType.Gathering)
                {
                    if (buildingBuildingBehavior.resourceCapacityType == resourceType && buildingBuildingBehavior.resourceCapacity > 0)
                        buildings.Add(new CameraController.ObjectWithDistance(building, distance));
                }
                if (interactType == InteractigType.Bulding)
                {
                    if (buildingBuildingBehavior.team == team && (
                            buildingBuildingBehavior.state == BuildingBehavior.BuildingState.Building || 
                            buildingBuildingBehavior.state == BuildingBehavior.BuildingState.Project
                            ))
                        buildings.Add(new CameraController.ObjectWithDistance(building, distance));
                }
            }
            if (buildings.Count > 0)
            {
                GameObject building = buildings.OrderBy(v => v.distance).ToArray()[0].unit;
                GiveOrder(building, false);
                Destroy(pointMarker);
                return true;
            }
        }
        Destroy(pointMarker);
        return false;
    }

    public override bool StartInteract(GameObject target)
    {
        // Start build object
        BuildingBehavior targetBuildingBehavior = target.GetComponent<BuildingBehavior>();
        if (targetBuildingBehavior != null && buildHpPerSecond > 0 && targetBuildingBehavior.team == team)
        {
            if (targetBuildingBehavior.health < targetBuildingBehavior.maxHealth)
            {
                bool canBuild = targetBuildingBehavior.UnitStartBuild(gameObject);
                if (canBuild)
                {
                    interactTimer = 1.0f;
                    transform.LookAt(target.transform.position);
                    anim.SetBool("Builded", true);
                    interactObject = target;
                    interactType = InteractigType.Bulding;
                    return false;
                }
            }
        }
        // Start gathering food
        BaseBehavior targetBaseBehavior = target.GetComponent<BaseBehavior>();
        UnitBehavior unitBehaviorComponent = target.GetComponent<UnitBehavior>();
        if (targetBaseBehavior != null && targetBaseBehavior.resourceCapacity > 0)
        {
            ResourceGatherInfo resourceInfo = resourceGatherInfo.Find(x => x.type == targetBaseBehavior.resourceCapacityType);
            if(resourceInfo != null)
            {
                interactType = InteractigType.None;
                if (targetBaseBehavior.resourceCapacityType == BaseBehavior.ResourceType.Food && !targetBaseBehavior.live)
                {
                    interactType = InteractigType.Gathering;
                    interactAnimation = "Gather";
                }
                if (targetBaseBehavior.resourceCapacityType == BaseBehavior.ResourceType.Wood)
                {
                    interactType = InteractigType.CuttingTree;
                    interactAnimation = "Cutting";
                }
                if (interactType != InteractigType.None)
                {
                    interactObject = target;
                    anim.SetBool(interactAnimation, true);
                    transform.LookAt(target.transform.position);
                    interactTimer = 1.0f;
                    return false;
                }
            }
        }
        // Store resources in building
        if (targetBuildingBehavior != null)
        {
            ResourceType storedResource = targetBuildingBehavior.storedResources.Find(x => x == resourceType);
            if (storedResource != ResourceType.None && resourceHold > 0 && targetBuildingBehavior.state == BuildingBehavior.BuildingState.Builded)
            {
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
                    GiveOrder(interactObject, false);
                    return true;
                }
            }
        }
        return false;
    }

    public void GoToStoreResources()
    {
        List<CameraController.ObjectWithDistance> buildings = new List<CameraController.ObjectWithDistance>();
        var allUnits = GameObject.FindGameObjectsWithTag("Building");
        foreach (GameObject building in allUnits)
        {
            BuildingBehavior buildingBuildingBehavior = building.GetComponent<BuildingBehavior>();
            ResourceType storedResource = buildingBuildingBehavior.storedResources.Find(x => x == resourceType);
            if (buildingBuildingBehavior.team == team && storedResource != ResourceType.None && buildingBuildingBehavior.state == BuildingBehavior.BuildingState.Builded)
            {
                float distance = Vector3.Distance(gameObject.transform.position, building.transform.position);
                buildings.Add(new CameraController.ObjectWithDistance(building, distance));
            }
        }
        if (buildings.Count > 0)
        {
            GameObject building = buildings.OrderBy(v => v.distance).ToArray()[0].unit;
            GiveOrder(building, false);
        }
    }

    public override bool IsVisible()
    {
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
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

        attacked = false;
        transform.LookAt(target.transform.position);
        anim.Rebind();
        anim.SetTrigger("Attack");
        attackTimer = attackTime;
        attackDelayTimer = attackDelay;
        interactType = InteractigType.Attacking;
    }

    public override void AlertAttacking(GameObject attacker)
    {
        if (behaviorType == BehaviorType.Run)
        {
            Vector3 dirToTarget = (transform.position - attacker.transform.position).normalized;
            GiveOrder(GetRandomPoint(transform.position + dirToTarget * 10, 3.0f), true);
        }
    }

    public override void ResourcesIsOut(GameObject worker)
    {
        timeToDestroy = 0.0f;
        sendToDestroy = true;
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
            anim.Rebind();
            anim.SetTrigger("Die");
            live = false;
            // var collider = GetComponent<Collider>();
            // collider.enabled = false;
            agent.enabled = false;

            UnitBehavior attackerBehaviorComponent = attacker.GetComponent<UnitBehavior>();
            attackerBehaviorComponent.ActionIsDone();

            if (resourceCapacity <= 0)
                sendToDestroy = true;
        }
        else
        {
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

    public void StopAction(bool deleteObject)
    {
        DestroyPointMarker();
        base.agent.isStopped = true;
        base.tempDestinationPoint = Vector3.zero;
        base.target = null;

        if (interactType == InteractigType.Attacking)
            anim.Rebind();

        if (interactType != InteractigType.None)
        {
            if(interactAnimation != "")
                anim.SetBool(interactAnimation, false);
            if (buildHpPerSecond > 0.0f)
                anim.SetBool("Builded", false);
            if (deleteObject)
                interactObject = null;
        }
        interactType = InteractigType.None;
    }

    [PunRPC]
    public override void GiveOrder(Vector3 point, bool displayMarker)
    {
        if (!live)
            return;

        StopAction(true);
 
        base.tempDestinationPoint = point;
        if (displayMarker)
            CreateOrUpdatePointMarker(Color.green, point, 1.5f);
    }

    public override void GiveOrder(GameObject targetObject, bool displayMarker)
    {
        if (!live)
            return;

        StopAction(false);

        // Order to target
        base.agent.isStopped = false;
        base.target = targetObject;
        
        Color colorMarker = Color.green;
        BaseBehavior targetBaseBehavior = targetObject.GetComponent<BaseBehavior>();
        if (targetBaseBehavior != null && IsTeamEnemy(targetBaseBehavior.team))
            colorMarker = Color.red;

        if (displayMarker)
            CreateOrUpdatePointMarker(colorMarker, target.transform.position, 1.5f);
    }

    public override bool[] UICommand(string commandName)
    {
        bool[] result = { false, false };
        UnitSelectionComponent unitSelectionComponent = GetComponent<UnitSelectionComponent>();
        if (!unitSelectionComponent.isSelected)
            return result;

        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        UIBaseScript cameraUIBaseScript = Camera.main.GetComponent<UIBaseScript>();
        if (team != cameraController.team)
            return result;
   
        foreach (BaseSkillScript skill in skillList)
        {
            KeyCode hotkey = skill.hotkey;
            if (skill.skillObject != null && skill.skillObject.GetComponent<BuildingBehavior>() != null)
                hotkey = skill.skillObject.GetComponent<BuildingBehavior>().productionHotkey;

            if (skill.uniqueName == commandName || Input.GetKeyDown(hotkey))
            {
                if(skill.skillObject != null)
                {
                    BuildingBehavior buildingBehavior = skill.skillObject.GetComponent<BuildingBehavior>();
                    if (buildingBehavior != null)
                    {
                        if (cameraController.buildedObject == null)
                        {
                            // if not enough resources -> return second element true
                            result[1] = !SpendResources(buildingBehavior.costFood, buildingBehavior.costGold, buildingBehavior.costWood);
                            if (result[1])
                                return result;

                            cameraUIBaseScript.DisplayMessage("Select place to build", 3000);
                            cameraController.buildedObject = skill.skillObject;
                        }
                    }
                    result[0] = true;
                }
                return result;
            }
        }
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

    public override List<string> GetStatistics()
    {
        List<string> statistics = base.GetStatistics();
        if (attackType != AttackType.None)
            statistics.Add(String.Format("Attack speed: {0:F1} sec", attackTime));
        return statistics;
    }

    public override List<string> GetCostInformation()
    {
        List<string> statistics = new List<string>();
        statistics.Add(String.Format("Time to build: {0:F0} sec", timeToBuild));
        if (costFood > 0)
            statistics.Add(String.Format("Food: {0:F0}", costFood));
        if (costGold > 0)
            statistics.Add(String.Format("Gold: {0:F0}", costGold));
        if (costWood > 0)
            statistics.Add(String.Format("Wood: {0:F0}", costWood));
        return statistics;
    }
}
