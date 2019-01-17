﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using UnityEngine.AI;
using Photon.Pun;
using System.Linq;
using GangaGame;

public class BuildingBehavior : BaseBehavior
{
    #region Units

    public enum BuildingState { Selected, Project, Building, Builded };
    [Header("Building info")]
    public BuildingState state = BuildingState.Builded;
    public float magnitDistance = 2.0f;

    public List<int> tempMaterialsMode = new List<int>();

    #endregion

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
                if (spawnTargetObject != null)
                    CreateOrUpdatePointMarker(Color.green, spawnTargetObject.transform.position, 0.0f, true);
                else
                    CreateOrUpdatePointMarker(Color.green, spawnTarget, 0.0f, true);
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
        TextBubble(String.Format("-{0:F0}", newDamage), 1000);
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

    [PunRPC]
    public void SetAsSelected()
    {
        state = BuildingBehavior.BuildingState.Selected;
        unitSelectionComponent.canBeSelected = false;
        health = 0;
        live = false;
        gameObject.layer = LayerMask.NameToLayer("Project");

        UnityEngine.AI.NavMeshObstacle navMesh = GetComponent<UnityEngine.AI.NavMeshObstacle>();
        if (navMesh != null)
            navMesh.enabled = false;

        var allNewRenders = GetComponents<Renderer>().Concat(GetComponentsInChildren<Renderer>()).ToArray();
        foreach (var render in allNewRenders)
            foreach (var material in render.materials)
                tempMaterialsMode.Add((int)material.GetFloat("_Mode"));
    }

    public void SetAsProject()
    {
        Color newColor = new Color(1, 1, 1, 0.45f);

        GameObject projector = GetComponent<UnitSelectionComponent>().projector;
        projector.active = false;
        projector.GetComponent<Projector>().material.color = newColor;

        var allRenders = GetComponents<Renderer>().Concat(GetComponentsInChildren<Renderer>()).ToArray();
        foreach (var render in allRenders)
            foreach (var material in render.materials)
                material.color = newColor;

        state = BuildingBehavior.BuildingState.Project;
        unitSelectionComponent.canBeSelected = true;
    }
    

    [PunRPC]
    public void SetAsBuilding()
    {
        gameObject.layer = LayerMask.NameToLayer("Building");
        live = true;
        UnityEngine.AI.NavMeshObstacle navMesh = gameObject.GetComponent<UnityEngine.AI.NavMeshObstacle>();
        if (navMesh != null)
            navMesh.enabled = true;

        state = BuildingState.Building;
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
    public override void GiveOrder(Vector3 point, bool displayMarker, bool overrideQueueCommands)
    {
        if (!live)
            return;

        spawnTargetObject = null;
        spawnTarget = point;
    }
    
    public override void GiveOrder(GameObject targetObject, bool displayMarker, bool overrideQueueCommands)
    {
        if (!live)
            return;

        spawnTarget = new Vector3();
        spawnTargetObject = targetObject;
    }

    public bool StopAction()
    {
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (state == BuildingState.Building || state == BuildingState.Project || state == BuildingState.Selected)
        {
            cameraController.food += skillInfo.costFood;
            cameraController.gold += skillInfo.costGold;
            cameraController.wood += skillInfo.costWood;
            if (objectUIInfo != null)
            {
                objectUIInfo.Destroy();
                objectUIInfo = null;
            }
            Destroy(gameObject);
            return true;
        }
        if(state == BuildingState.Builded)
            return DeleteFromProductionQuery(0);
        return false;
    }

    public override bool[] UICommand(string commandName)
    {
        bool[] result = { false, false };
        if (!unitSelectionComponent.isSelected)
            return result;
        
        if (team != cameraController.team || cameraController.chatInput)
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

                if (productionQuery.Count <= 1)
                    buildTimer = baseBehaviorComponent.skillInfo.timeToBuild;

                result[0] = true;
                return result;
            }
        }
        if (commandName == "stop" || Input.GetKeyDown(KeyCode.H))
        {
            result[0] = StopAction();
            return result;
        }
        return result;
    }

    public override List<string> GetCostInformation()
    {
        List<string> statistics = new List<string>();
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
