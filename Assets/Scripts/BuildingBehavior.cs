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
    public bool farm = false;

    [Header("Units production")]
    public List<GameObject> producedUnits = new List<GameObject>();
    public GameObject spawnPoint;
    private Vector3 spawnTarget = Vector3.zero;
    public List<GameObject> unitsQuery = new List<GameObject>();
    public int uqeryLimit = 5;
    public float buildTimer = 0.0f;

    #endregion

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

        if (unitsQuery.Count > 0)
        {
            buildTimer -= Time.deltaTime;
            if (buildTimer <= 0.0f)
            {
                ProduceUnit(unitsQuery[0]);
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
            CreateOrUpdatePointMarker(Color.green, spawnTarget, 0.0f);
        if (!unitSelectionComponent.isSelected)
            DestroyPointMarker();
    }

    public GameObject ProduceUnit(GameObject createdPrefab)
    {
        // Create object
        GameObject createdObject = PhotonNetwork.Instantiate(createdPrefab.name, spawnPoint.transform.position, spawnPoint.transform.rotation);

        BaseBehavior createdObjectBehaviorComponent = createdObject.GetComponent<BaseBehavior>();
        PhotonView createdPhotonView = createdObject.GetComponent<PhotonView>();
        // Set owner
        if (PhotonNetwork.InRoom)
            createdPhotonView.RPC("ChangeOwner", PhotonTargets.All, ownerId, team);
        else
            createdObjectBehaviorComponent.ChangeOwner(ownerId, team);

        // createdObjectBehaviorComponent.Awake();

        //Send command to created object to spawn target 
        if (PhotonNetwork.InRoom)
            createdPhotonView.RPC("GiveOrder", PhotonTargets.All, createdObjectBehaviorComponent.GetRandomPoint(spawnTarget, 2.0f), true);
        else
            createdObjectBehaviorComponent.GiveOrder(createdObjectBehaviorComponent.GetRandomPoint(spawnTarget, 2.0f), true);
        return createdObject;
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

    public bool UnitStartBuild(GameObject builder)
    {
        if (state == BuildingState.Project && !live)
        {
            GameObject intersection = GetIntersection(builder);
            if (intersection != null)
                return false;
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Building");
                live = true;
                UnityEngine.AI.NavMeshObstacle navMesh = gameObject.GetComponent<UnityEngine.AI.NavMeshObstacle>();
                if (navMesh != null)
                    navMesh.enabled = true;

                state = BuildingState.Building;
                canBeSelected = true;
                var allRenders = gameObject.GetComponents<Renderer>().Concat(gameObject.GetComponentsInChildren<Renderer>()).ToArray();
                foreach (var render in allRenders)
                    foreach (var material in render.materials)
                    {
                        material.SetFloat("_Mode", 1);
                        material.SetInt("_SrcBlend", 1);
                        material.SetInt("_DstBlend", 0);
                        material.SetInt("_ZWrite", 1);
                        material.EnableKeyword("_ALPHATEST_ON");
                        material.DisableKeyword("_ALPHABLEND_ON");
                        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = 3000;
                        material.color = new Color(1, 1, 1, 1.0f);
                    }
                if (spawnPoint != null)
                    spawnTarget = spawnPoint.transform.position;
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
    public override void GiveOrder(Vector3 point, bool displayMarker)
    {
        if (!live)
            return;

        spawnTarget = point;
    }

    [PunRPC]
    public override void GiveOrder(GameObject targetObject, bool displayMarker)
    {
        if (!live)
            return;

        // Order to target
        //target = targetObject;
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
        if (state == BuildingState.Building || state == BuildingState.Project)
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
        if (team != cameraController.team)
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
