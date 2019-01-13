using GangaGame;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitSelectionComponent : MonoBehaviour
{
    [HideInInspector]
    public bool canBeSelected = true;

    public bool isSelected = false;
    public GameObject projector;
    BaseBehavior baseBehaviorComponent;
    CameraController cameraController;
    BuildingBehavior buildingBehavior;
    Color projectorColor = new Color(1, 1, 1, 1f);
    int projectorColorId = -1;
    cakeslice.Outline[] allOutlines;
    Projector projectorComponent;
    bool outlineState = true;
    bool intersection = false;

    // Use this for initialization
    void Start ()
    {
        projectorComponent = projector.GetComponent<Projector>();
        projectorComponent.material = new Material(projectorComponent.material);
        baseBehaviorComponent = gameObject.GetComponent<BaseBehavior>();
        cameraController = Camera.main.GetComponent<CameraController>();
        buildingBehavior = gameObject.GetComponent<BuildingBehavior>();
        canBeSelected = baseBehaviorComponent.canBeSelected;
        
        allOutlines = gameObject.GetComponents<cakeslice.Outline>().Concat(gameObject.GetComponentsInChildren<cakeslice.Outline>()).ToArray();
        SetOutline(false);

        if (buildingBehavior != null && buildingBehavior.DisableUpdate())
            enabled = false;
    }

    private void UpdateColor()
    {
        if (baseBehaviorComponent.team <= 0 || GameInfo.playerSpectate)
        {
            projectorColorId = 2;
            projectorColor = new Color(1, 1, 1, 1f);
        }
        else if (cameraController.team != baseBehaviorComponent.team)
        {
            projectorColorId = 0;
            projectorColor = new Color(1, 0, 0, 1f);
        }
        else
        {
            if (cameraController.userId == baseBehaviorComponent.ownerId)
            {
                projectorColorId = 1;
                projectorColor = new Color(0, 1, 0, 1f);
            }
            else
            {
                projectorColorId = 2;
                projectorColor = new Color(1, 1, 1, 1f);
            }
        }

        foreach (cakeslice.Outline outline in allOutlines)
            outline.color = projectorColorId;

        projectorComponent.material.color = projectorColor;
    }

    void Update()
    {
        UnityEngine.Profiling.Profiler.BeginSample("p Getting is outlineDraw"); // Profiler

        bool newIntersection = false;
        if (buildingBehavior == null)
        {
            if (baseBehaviorComponent.team > 0 && baseBehaviorComponent.IsInCameraView() &&
                baseBehaviorComponent.IsVisible() && baseBehaviorComponent.live &&
                Physics.Linecast(transform.position + new Vector3(0, 1.0f, 0), Camera.main.transform.position))
                newIntersection = true;
            else
                newIntersection = false;
        }
        if (newIntersection != intersection)
        {
            intersection = newIntersection;
            SetOutline(intersection);
        }

        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
    }

    public void SetSelect(bool newState)
    {
        if (!canBeSelected)
            return;

        if (projector.activeSelf != newState)
        {
            UpdateColor();

            projector.SetActive(newState);
            SetOutline(newState);
        }
        if (newState == false)
            intersection = false;
        isSelected = newState;
    }

    public void SetOutline(bool newState)
    {
        if (newState != outlineState)
        {
            UpdateColor();

            foreach (cakeslice.Outline outline in allOutlines)
                outline.enabled = newState;

            outlineState = newState;
        }
    }
}
