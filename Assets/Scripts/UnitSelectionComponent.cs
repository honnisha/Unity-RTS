using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitSelectionComponent : MonoBehaviour
{
    public bool isSelected = false;
    public GameObject projector;

    // Use this for initialization
    void Start ()
    {
        Projector projectorComponent = projector.GetComponent<Projector>();
        projectorComponent.material = new Material(projectorComponent.material);
    }
	
	// Update is called once per frame
	void Update ()
    {
        var camera = Camera.main;
        CameraController cameraController = camera.GetComponent<CameraController>();
        BaseBehavior baseBehaviorComponent = gameObject.GetComponent<BaseBehavior>();
        bool isWithinSelectionBounds = false;
        if (cameraController.isSelecting)
            isWithinSelectionBounds = cameraController.IsWithinSelectionBounds(gameObject);

        RaycastHit hit;
        bool outlineDraw = false;
        if (baseBehaviorComponent.team > 0 &&
            baseBehaviorComponent.IsVisible() && baseBehaviorComponent.live && gameObject.GetComponent<BuildingBehavior>() == null &&
            Physics.Linecast(transform.position + new Vector3(0, 1.0f, 0), camera.transform.position))
            outlineDraw = true;
        else if (isWithinSelectionBounds && baseBehaviorComponent.IsVisible())
            outlineDraw = true;
        else if (baseBehaviorComponent.IsVisible() && Physics.Raycast(Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition), out hit))
            if (hit.transform == gameObject.transform)
                outlineDraw = true;

        Color newColor = new Color(1, 1, 1, 1f);
        int newColorId = 1;
        if (baseBehaviorComponent.team <= 0)
        {
            newColorId = 2;
            newColor = new Color(1, 1, 1, 1f);
        }
        else if (cameraController.team != baseBehaviorComponent.team)
        {
            newColorId = 0;
            newColor = new Color(1, 0, 0, 1f);
        }
        else
        {
            if (cameraController.userId == baseBehaviorComponent.ownerId)
            {
                newColorId = 1;
                newColor = new Color(0, 1, 0, 1f);
            }
            else
            {
                newColorId = 2;
                newColor = new Color(1, 1, 1, 1f);
            }
        }

        BuildingBehavior buildingBehavior = gameObject.GetComponent<BuildingBehavior>();
        if (buildingBehavior != null && buildingBehavior.state == BuildingBehavior.BuildingState.Selected)
            outlineDraw = false;


        cakeslice.Outline[] outlines = gameObject.GetComponents<cakeslice.Outline>();
        cakeslice.Outline[] childOutlines = gameObject.GetComponentsInChildren<cakeslice.Outline>();
        foreach (cakeslice.Outline outline in outlines.Concat(childOutlines).ToArray())
        {
            outline.enabled = outlineDraw;
            outline.color = newColorId;
        }

        if (baseBehaviorComponent.canBeSelected == false)
            return;

        if (baseBehaviorComponent.IsVisible() == false)
        {
            isSelected = false;
            projector.active = false;
            return;
        }
        
        projector.active = isSelected;

        if (projector.active)
        {
            Projector projectorComponent = projector.GetComponent<Projector>();
            projectorComponent.material.color = newColor;
        }
    }
}
