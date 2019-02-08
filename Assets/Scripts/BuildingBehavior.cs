using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using UnityEngine.AI;
using Photon.Pun;
using System.Linq;
using GangaGame;

public class BuildingBehavior : BaseBehavior, IPunObservable
{
    #region Units

    public enum BuildingState { Selected, Project, Building, Builded };
    [Header("Building info")]
    public float magnitDistance = 2.0f;

    public List<int> tempMaterialsMode = new List<int>();

    #endregion

    public enum PlaceConditionType { InRange, BlockedRange };
    [System.Serializable]
    public class PlaceConditionInfo
    {
        public PlaceConditionType type = PlaceConditionType.BlockedRange;
        public string buildingName;
        public string readableName;
        public float range = 10.0f;
    }
    public List<PlaceConditionInfo> placeConditions = new List<PlaceConditionInfo>();

    [System.Serializable]
    public class TerrainChangeInfo
    {
        public Vector2 offset;
        public Vector2 size;
        public int layer;
        public int value;
        public bool changeTexture = false;
        public bool removeGrass = false;
    }
    [Header("Terrain modifications")]
    public TerrainChangeInfo terrainChangeInfo;
    private bool terrainHasChanged = false;

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.state);
            stream.SendNext(this.health);
            stream.SendNext(this.live);
        }
        else
        {
            this.state = (BuildingState)stream.ReceiveNext();
            this.health = (int)stream.ReceiveNext();
            this.live = (bool)stream.ReceiveNext();
        }
    }

    public override void Awake()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Ambient"))
            beenSeen = true;

        base.Awake();

        UpdateIsInCameraView(cameraController.IsInCameraView(transform.position));
        UpdateVision();

        if (DisableUpdate())
            enabled = false;
    }

    public bool DisableUpdate()
    {
        if (resourceCapacityType == ResourceType.Wood)
            return true;
        return false;
    }

    // Update is called once per frame
    override public void Update()
    {
        UpdateDestroyBehavior();
        
        if (resourceCapacityType == ResourceType.Wood)
            return;

        UpdateVisionTool();

        UpdateProductionQuery();

        UpdateHealth();

        if (unitSelectionComponent.isSelected && UnityEngine.Input.anyKeyDown)
            UICommand(null);
        
        UpdateTerrain();

        UpdatePointMarker();

        UpdateAttackBehavior();

        DoAttack();
    }

    void UpdateAttackBehavior()
    {
        if (actionEffect == null)
            return;

        UnitStatistic statisic = GetStatisticsInfo();

        if (Time.frameCount % 15 == 0)
        {
            if (behaviorType == BehaviorType.Aggressive)
                AttackNearEnemies(transform.position, statisic.agrRange, -1, randomRange: 8.0f);
        }

        if (target != null && interactType == InteractigType.None)
        {
            Vector3 offset = new Vector3(0, 0.5f, 0);
            RaycastHit hit;
            if (Physics.Raycast(actionEffect.transform.position + offset, (target.transform.position + offset - (actionEffect.transform.position + offset)).normalized, out hit, statisic.rangeAttack))
            {
                if (hit.transform.gameObject.GetHashCode() == target.GetHashCode())
                {
                    Attack(target);
                }
            }
            else
            {
                ActionIsDone();
            }
        }
    }

    public override void Attack(GameObject target)
    {
        if (!live)
            return;

        base.Attack(target);
    }

    public override void UpdateIsInCameraView(bool newState)
    {
        isInCameraView = newState;
        UpdateVision();
    }

    public override void StartVisible(BaseBehavior senderBaseBehaviorComponent)
    {
        // Debug.Log("StartVisible + " + gameObject.name + " " + visionCount);
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (senderBaseBehaviorComponent != null && senderBaseBehaviorComponent.team == cameraController.team)
            visionCount++;
        
        UpdateVision();
    }

    public override void StopVisible(BaseBehavior senderBaseBehaviorComponent)
    {
        // Debug.Log("StopVisible + " + gameObject.name + " " + visionCount);
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (senderBaseBehaviorComponent != null && senderBaseBehaviorComponent.team == cameraController.team && visionCount > 0)
            visionCount--;

        UpdateVision();
    }

    public override void SendToDestroy()
    {
        enabled = true;
        sendToDestroy = true;
    }

    public void UpdatePointMarker()
    {
        UnityEngine.Profiling.Profiler.BeginSample("p UpdatePointMarker"); // Profiler
        if (IsInCameraView())
        {
            if (unitSelectionComponent.isSelected && team == cameraController.team && ownerId == cameraController.userId)
            {
                if (target != null)
                {
                    Color color = Color.green;
                    BaseBehavior targetBehaviorComponent = target.GetComponent<BaseBehavior>();
                    if (IsTeamEnemy(targetBehaviorComponent.team))
                        color = Color.red;

                    CreateOrUpdatePointMarker(color, target.transform.position, 0.0f, true, PointMarker.MarkerType.Arrow);
                }
                else
                    CreateOrUpdatePointMarker(Color.green, spawnTarget, 0.0f, true, PointMarker.MarkerType.Flag);
            }
            if (!unitSelectionComponent.isSelected)
                DestroyPointMarker();
        }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
    }

    public void UpdateTerrain()
    {
        UnityEngine.Profiling.Profiler.BeginSample("p UpdateTerrain"); // Profiler
        if (!terrainHasChanged)
            if (state == BuildingState.Builded && IsVisible())
            {
                terrainHasChanged = true;
                TerrainGenerator terrainGenerator = Terrain.activeTerrain.GetComponent<TerrainGenerator>();
                if (terrainGenerator)
                {
                    Vector2 newPosition = new Vector2(transform.transform.position.x + terrainChangeInfo.offset.y, transform.transform.position.z + terrainChangeInfo.offset.x);
                    if (terrainChangeInfo.changeTexture)
                        terrainGenerator.SetTextureOnTerrain(newPosition, terrainChangeInfo.size, terrainChangeInfo.layer, terrainChangeInfo.value);
                    if (terrainChangeInfo.removeGrass)
                        terrainGenerator.RemoveGrassOnTerrain(newPosition, terrainChangeInfo.size);
                }
            }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
    }

    public override void AlertAttacking(GameObject attacker)
    {
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
        if (health <= 0)
        {
            BecomeDead();
        }
    }

    public override void BecomeDead()
    {
        health = 0.0f;
        live = false;

        if (resourceCapacity <= 0)
            SendToDestroy();
    }

    public override bool IsVisible()
    {
        if (GameInfo.playerSpectate)
            return true;

        if (state == BuildingState.Selected || state == BuildingState.Project)
        {
            if (cameraController.userId != ownerId)
                return false;
        }
        if (team == cameraController.team)
            return true;
        else
        {
            if (visionCount > 0)
            {
                beenSeen = true;
                return true;
            }
        }
        return false;
    }

    public override bool IsHealthVisible()
    {
        if (state == BuildingState.Building && live)
            return true;

        if (live && health < maxHealth)
        {
            return true;
        }
        if (cameraController.tagsToSelect.Find(x => x.name == tag).healthVisibleOnlyWhenSelect && !unitSelectionComponent.isSelected)
            return false;
        return true;
    }

    private BuildingState _state = BuildingState.Builded;
    public BuildingState state
    {
        set
        {
            if (value == BuildingState.Selected)
            {
                unitSelectionComponent.canBeSelected = false;
                health = 0;
                live = false;
                gameObject.layer = LayerMask.NameToLayer("Project");

                if (team == cameraController.team)
                {
                    NavMeshObstacle navMesh = GetComponent<NavMeshObstacle>();
                    if (navMesh != null)
                        navMesh.enabled = false;

                    var allNewRenders = GetComponents<Renderer>().Concat(GetComponentsInChildren<Renderer>()).ToArray();
                    foreach (var render in allNewRenders)
                        foreach (var material in render.materials)
                            tempMaterialsMode.Add((int)material.GetFloat("_Mode"));
                }
                UpdateVision();
            }
            if (value == BuildingState.Project)
            {
                Color newColor = new Color(1, 1, 1, 0.45f);

                GameObject projector = GetComponent<UnitSelectionComponent>().projector;
                projector.active = false;
                projector.GetComponent<Projector>().material.color = newColor;

                if (team == cameraController.team)
                {
                    var allRenders = GetComponents<Renderer>().Concat(GetComponentsInChildren<Renderer>()).ToArray();
                    foreach (var render in allRenders)
                        foreach (var material in render.materials)
                            material.color = newColor;
                }
                unitSelectionComponent.canBeSelected = true;
                UpdateVision();
            }
            if (value == BuildingState.Building)
            {
                gameObject.layer = LayerMask.NameToLayer("Building");
                live = true;
                UnityEngine.AI.NavMeshObstacle navMesh = gameObject.GetComponent<UnityEngine.AI.NavMeshObstacle>();
                if (navMesh != null)
                    navMesh.enabled = true;
                
                unitSelectionComponent.canBeSelected = true;
                var allRenders = gameObject.GetComponents<Renderer>().Concat(gameObject.GetComponentsInChildren<Renderer>()).ToArray();
                int index = 0;
                if (tempMaterialsMode.Count > 0)
                {
                    foreach (var render in allRenders)
                        foreach (var material in render.materials)
                        {
                            // https://github.com/jamesjlinden/unity-decompiled/blob/master/UnityEditor/UnityEditor/StandardShaderGUI.cs
                            material.SetFloat("_Mode", tempMaterialsMode[index]);
                            #region Standard shaders setters

                            if (tempMaterialsMode[index] == 0)
                            {
                                material.SetOverrideTag("RenderType", "");
                                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                                material.SetInt("_ZWrite", 1);
                                material.DisableKeyword("_ALPHATEST_ON");
                                material.DisableKeyword("_ALPHABLEND_ON");
                                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                                material.renderQueue = -1;
                            }
                            if (tempMaterialsMode[index] == 1)
                            {
                                material.SetOverrideTag("RenderType", "TransparentCutout");
                                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                                material.SetInt("_ZWrite", 1);
                                material.EnableKeyword("_ALPHATEST_ON");
                                material.DisableKeyword("_ALPHABLEND_ON");
                                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                            }
                            if (tempMaterialsMode[index] == 2)
                            {
                                material.SetOverrideTag("RenderType", "Transparent");
                                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                                material.SetInt("_ZWrite", 0);
                                material.DisableKeyword("_ALPHATEST_ON");
                                material.EnableKeyword("_ALPHABLEND_ON");
                                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                            }
                            if (tempMaterialsMode[index] == 2)
                            {
                                material.SetOverrideTag("RenderType", "Transparent");
                                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                                material.SetInt("_ZWrite", 0);
                                material.DisableKeyword("_ALPHATEST_ON");
                                material.DisableKeyword("_ALPHABLEND_ON");
                                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                            }

                            #endregion
                            index++;
                            material.color = new Color(1, 1, 1, 1.0f);
                        }
                }
                tempMaterialsMode.Clear();
                if (spawnPoint != null)
                    spawnTarget = spawnPoint.transform.position;
                UpdateVision();
            }
            _state = value;
        }
        get { return _state; }
    }

    public bool IsUnitCanBuild(GameObject builder)
    {
        if (state == BuildingState.Project && !live)
        {
            GameObject intersection = GetIntersection(builder);
            if (intersection != null)
                return false;
            else
            {
                return true;
            }
        }
        else if (live)
            return true;
        return false;
    }

    public Dictionary<GameObject, float> returnObjects = new Dictionary<GameObject, float>();
    public void GetBlockedBlockedBy()
    {
        returnObjects.Clear();
        foreach (PlaceConditionInfo placeCondition in placeConditions)
        {
            if (placeCondition.type == PlaceConditionType.BlockedRange)
            {
                GetObjectsInRange(ref allObjects, transform.position, placeCondition.range, team: -1, units: false);
                foreach (GameObject building in allObjects)
                {
                    BaseBehavior baseBehaviorComponent = building.GetComponent<BaseBehavior>();
                    if (baseBehaviorComponent.team == team)
                        if (baseBehaviorComponent.skillInfo.uniqueName == placeCondition.buildingName)
                        {
                            returnObjects.Add(building, placeCondition.range);
                        }
                }
            }
        }
    }

    public void GetInRangeConditionError()
    {
        returnObjects.Clear();
        foreach (PlaceConditionInfo placeCondition in placeConditions)
        {
            if (placeCondition.type == PlaceConditionType.InRange)
            {
                GetObjectsInRange(ref allObjects, transform.position, placeCondition.range, team: -1, units: false);
                foreach (GameObject building in allObjects)
                {
                    BaseBehavior baseBehaviorComponent = building.GetComponent<BaseBehavior>();
                    if (baseBehaviorComponent.team == team)
                        if (baseBehaviorComponent.skillInfo.uniqueName == placeCondition.buildingName)
                            returnObjects.Add(building, placeCondition.range);
                }
            }
        }
    }

    public List<string> stringsConditionInRange = new List<string>();
    public void IfHasConditionInRange()
    {
        stringsConditionInRange.Clear();
        foreach (PlaceConditionInfo placeCondition in placeConditions)
            if (placeCondition.type == PlaceConditionType.InRange)
                stringsConditionInRange.Add(placeCondition.readableName);
    }

    public bool RepairOrBuild(float giveHealth)
    {
        health += giveHealth;
        if(health >= maxHealth)
        {
            if(state == BuildingState.Building)
                state = BuildingState.Builded;

            health = maxHealth;
            return true;
        }
        return false;
    }

    [PunRPC]
    public override void _GiveOrder(Vector3 point, bool displayMarker, bool overrideQueueCommands, float speed = 0.0f)
    {
        if (!live)
            return;

        target = null;
        spawnTarget = point;
    }
    
    public override void _GiveOrder(GameObject targetObject, bool displayMarker, bool overrideQueueCommands, float speed = 0.0f)
    {
        if (!live)
            return;

        spawnTarget = new Vector3();
        target = targetObject;
    }

    [PunRPC]
    public override void _StopAction(bool deleteObject = false)
    {
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (state == BuildingState.Building || state == BuildingState.Project || state == BuildingState.Selected)
        {
            if (team == cameraController.team)
            {
                cameraController.food += skillInfo.costFood;
                cameraController.gold += skillInfo.costGold;
                cameraController.wood += skillInfo.costWood;
            }
            if (objectUIInfo != null)
            {
                objectUIInfo.Destroy();
                objectUIInfo = null;
            }
            PhotonView.Destroy(gameObject);
            cameraUIBaseScript.UpdateUI();
            return;
        }
        if(state == BuildingState.Builded)
            DeleteFromProductionQuery(0);
        return;
    }

    public override bool[] UICommand(string commandName)
    {
        bool[] result = { false, false };
        if (!unitSelectionComponent.isSelected)
            return result;
        
        if (team != cameraController.team || cameraController.IsHotkeysBlocked())
            return result;

        bool[] skillResult = ActivateSkills(commandName);
        if (skillResult[0] || skillResult[1])
            return skillResult;

        foreach (GameObject unit in producedUnits)
        {
            BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
            if (baseBehaviorComponent.skillInfo.uniqueName == commandName || Input.GetKeyDown(baseBehaviorComponent.skillInfo.productionHotkey))
            {
                if (productionQuery.Count >= uqeryLimit)
                    return result;
                //Debug.Log(producedUnits.Count);

                // if not enough resources -> return second element true
                result[1] = !SpendResources(baseBehaviorComponent.skillInfo.costFood, baseBehaviorComponent.skillInfo.costGold, baseBehaviorComponent.skillInfo.costWood);
                if (result[1])
                    return result;
                productionQuery.Add(unit);
                ProductionQueryUpdated();

                if (productionQuery.Count <= 1)
                    buildTimer = baseBehaviorComponent.skillInfo.timeToBuild;

                result[0] = true;
                return result;
            }
        }
        if (commandName == "stop" || Input.GetKeyDown(KeyCode.H))
        {
            StopAction(SendRPC: true);
            result[0] = true;
            return result;
        }
        return result;
    }

    List<string> statistics = new List<string>();
    public override List<string> GetCostInformation()
    {
        statistics.Clear();
        if (skillInfo.costFood > 0)
            statistics.Add(new StringBuilder(30).AppendFormat("Food: {0:F0}", skillInfo.costFood).ToString());
        if (skillInfo.costGold > 0)
            statistics.Add(new StringBuilder(30).AppendFormat("Gold: {0:F0}", skillInfo.costGold).ToString());
        if (skillInfo.costWood > 0)
            statistics.Add(new StringBuilder(30).AppendFormat("Wood: {0:F0}", skillInfo.costWood).ToString());
        return statistics;
    }

    public override UnitStatistic GetStatisticsInfo()
    {
        return defaultStatistic;
    }
}
