using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using UnityEngine.AI;
using Photon.Pun;
using System.Linq;

public class BuildingBehavior : BaseBehavior
{
    #region Units

    public enum BuildingState { Selected, Project, Building, Builded };
    [Header("Building info")]
    public BuildingState state = BuildingState.Builded;
    public float magnitDistance = 2.0f;

    [Header("Units production")]
    public List<GameObject> producedUnits = new List<GameObject>();
    public GameObject spawnPoint;
    private Vector3 spawnTarget = Vector3.zero;
    private GameObject spawnTargetObject;
    public List<GameObject> unitsQuery = new List<GameObject>();
    public int uqeryLimit = 5;
    public float buildTimer = 0.0f;

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
        if (spawnPoint != null)
            spawnTarget = spawnPoint.transform.position;
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();

        if(!terrainHasChanged && state == BuildingState.Builded && IsVisible())
        {
            terrainHasChanged = true;
            TerrainGenerator terrainGenerator = Terrain.activeTerrain.GetComponent<TerrainGenerator>();
            Vector2 newPosition = new Vector2(transform.transform.position.x + terrainChangeInfo.offset.y, transform.transform.position.z + terrainChangeInfo.offset.x);
            if (terrainChangeInfo.changeTexture)
                terrainGenerator.SetTextureOnTerrain(newPosition, terrainChangeInfo.size, terrainChangeInfo.layer, terrainChangeInfo.value);
            if(terrainChangeInfo.removeGrass)
                terrainGenerator.RemoveGrassOnTerrain(newPosition, terrainChangeInfo.size);
        }

        if (unitsQuery.Count > 0)
        {
            buildTimer -= Time.deltaTime;
            if (buildTimer <= 0.0f)
            {
                ProduceUnit(unitsQuery[0].name);
                unitsQuery.Remove(unitsQuery[0]);
                if (unitsQuery.Count > 0)
                {
                    BaseBehavior firstElementBehaviorComponent = unitsQuery[0].GetComponent<BaseBehavior>();
                    buildTimer = firstElementBehaviorComponent.timeToBuild;
                }
            }
        }
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        UnitSelectionComponent unitSelectionComponent = GetComponent<UnitSelectionComponent>();
        if (unitSelectionComponent.isSelected && team == cameraController.team)
        {
            if (spawnTargetObject != null)
                CreateOrUpdatePointMarker(Color.green, spawnTargetObject.transform.position, 0.0f, true);
            else
                CreateOrUpdatePointMarker(Color.green, spawnTarget, 0.0f, true);
        }
        if (!unitSelectionComponent.isSelected)
            DestroyPointMarker();
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
                    createdPhotonView.RPC("GiveOrder", PhotonTargets.All, createdObjectBehaviorComponent.GetRandomPoint(spawnTarget + dirToTarget * distance, 2.0f), true, false);
            }
            else
            {
                if (spawnTargetObject != null)
                    createdObjectBehaviorComponent.GiveOrder(spawnTargetObject, true, false);
                else
                    createdObjectBehaviorComponent.GiveOrder(createdObjectBehaviorComponent.GetRandomPoint(spawnTarget + dirToTarget * distance, 2.0f), true, false);
            }
            createdObjects.Add(createdObject);
        }
        return createdObjects;
    }

    public override void AlertAttacking(GameObject attacker)
    {
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
            sendToDestroy = true;
    }

    public override bool IsVisible()
    {
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
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
            return true;
        return false;
    }

    [PunRPC]
    public void SetAsSelected()
    {
        state = BuildingBehavior.BuildingState.Selected;
        canBeSelected = false;
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
        canBeSelected = true;
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
        canBeSelected = true;
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

    public bool DeleteFromProductionQuery(int index)
    {
        if (index >= unitsQuery.Count)
            return false;

        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        BaseBehavior baseBehavior = unitsQuery[index].GetComponent<BaseBehavior>();
        cameraController.food += baseBehavior.costFood;
        cameraController.gold += baseBehavior.costGold;
        cameraController.wood += baseBehavior.costWood;
        unitsQuery.RemoveAt(index);
        if (index == 0 && unitsQuery.Count > 0)
        {
            buildTimer = unitsQuery[0].GetComponent<BaseBehavior>().timeToBuild;
        }
        return true;
    }

    public bool StopAction()
    {
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (state == BuildingState.Building || state == BuildingState.Project || state == BuildingState.Selected)
        {
            cameraController.food += costFood;
            cameraController.gold += costGold;
            cameraController.wood += costWood;
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
        UnitSelectionComponent unitSelectionComponent = GetComponent<UnitSelectionComponent>();
        if (!unitSelectionComponent.isSelected)
            return result;

        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        UIBaseScript cameraUIBaseScript = Camera.main.GetComponent<UIBaseScript>();
        if (team != cameraController.team || cameraController.chatInput)
            return result;

        foreach (GameObject unit in producedUnits)
        {
            BaseBehavior baseBehaviorComponent = unit.GetComponent<BaseBehavior>();
            if (baseBehaviorComponent.uniqueName == commandName || Input.GetKeyDown(baseBehaviorComponent.productionHotkey))
            {
                if (unitsQuery.Count >= uqeryLimit)
                    return result;
                //Debug.Log(producedUnits.Count);

                // if not enough resources -> return second element true
                result[1] = !SpendResources(baseBehaviorComponent.costFood, baseBehaviorComponent.costGold, baseBehaviorComponent.costWood);
                if (result[1])
                    return result;

                if (buildTimer <= 0.0f)
                    buildTimer = baseBehaviorComponent.timeToBuild;
                unitsQuery.Add(unit);

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
        if (costFood > 0)
            statistics.Add(String.Format("Food: {0:F0}", costFood));
        if (costGold > 0)
            statistics.Add(String.Format("Gold: {0:F0}", costGold));
        if (costWood > 0)
            statistics.Add(String.Format("Wood: {0:F0}", costWood));
        return statistics;
    }
}
