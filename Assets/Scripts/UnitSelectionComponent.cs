using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitSelectionComponent : MonoBehaviour
{
    public bool isSelected = false;
    public GameObject projector;
    BaseBehavior baseBehaviorComponent;
    CameraController cameraController;
    BuildingBehavior buildingBehavior;
    Color projectorColor = new Color(1, 1, 1, 1f);
    int projectorColorId = -1;
    cakeslice.Outline[] allOutlines;

    // Use this for initialization
    void Start ()
    {
        Projector projectorComponent = projector.GetComponent<Projector>();
        projectorComponent.material = new Material(projectorComponent.material);
        baseBehaviorComponent = gameObject.GetComponent<BaseBehavior>();
        cameraController = Camera.main.GetComponent<CameraController>();
        buildingBehavior = gameObject.GetComponent<BuildingBehavior>();

        if (baseBehaviorComponent.team <= 0)
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
        
        allOutlines = gameObject.GetComponents<cakeslice.Outline>().Concat(gameObject.GetComponentsInChildren<cakeslice.Outline>()).ToArray();
    }

    Color oldColor;
    bool outlineDraw = false;

    void Update ()
    {
        if (!baseBehaviorComponent.IsInCameraView())
            return;

        UnityEngine.Profiling.Profiler.BeginSample("p Srart update"); // Profiler
        if (baseBehaviorComponent.canBeSelected == false)
            return;

        if (baseBehaviorComponent.IsVisible() == false)
            isSelected = false;
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler

        UnityEngine.Profiling.Profiler.BeginSample("p Getting is outlineDraw"); // Profiler
        bool newOutlineDraw = false;
        if (buildingBehavior != null && buildingBehavior.state == BuildingBehavior.BuildingState.Selected)
        {
            newOutlineDraw = false;
        }
        else if(buildingBehavior == null)
        {
            bool isWithinSelectionBounds = false;
            if (cameraController.isSelecting)
                isWithinSelectionBounds = cameraController.IsWithinSelectionBounds(gameObject);

            if (baseBehaviorComponent.team > 0 &&
                baseBehaviorComponent.IsVisible() && baseBehaviorComponent.live &&
                Physics.Linecast(transform.position + new Vector3(0, 1.0f, 0), Camera.main.transform.position))
                newOutlineDraw = true;
            else if (isWithinSelectionBounds && baseBehaviorComponent.IsVisible())
                newOutlineDraw = true;

            // RaycastHit hit;
            //else if (baseBehaviorComponent.IsVisible() && Physics.Raycast(Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition), out hit))
            //    if (hit.transform == gameObject.transform)
            //        newOutlineDraw = true;
        }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler

        UnityEngine.Profiling.Profiler.BeginSample("p Update allOutlines"); // Profiler
        if(outlineDraw != newOutlineDraw)
            foreach (cakeslice.Outline outline in allOutlines)
            {
                if (outline.enabled != newOutlineDraw)
                {
                    outline.enabled = newOutlineDraw;
                    outline.color = projectorColorId;
                    newOutlineDraw = outlineDraw;
                }
            }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler

        UnityEngine.Profiling.Profiler.BeginSample("p Update projector"); // Profiler
        projector.active = isSelected;
        if (projector.active && projectorColor != projectorColor)
        {
            Projector projectorComponent = projector.GetComponent<Projector>();
            projectorComponent.material.color = projectorColor;
            oldColor = projectorColor;
        }
        UnityEngine.Profiling.Profiler.EndSample(); // Profiler
    }
}
