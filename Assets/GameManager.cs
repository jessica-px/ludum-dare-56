using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public bool IsGameOver { get; private set; } = false;
    public Creature? TargetCreature { get; private set; } = null;
    private VisualElement uiRoot;
    private VisualElement grabBar;
    private VisualElement grabPointer;
    private VisualElement grabZone;

    private GrabBarController grabBarController;

    // Start is called before the first frame update
    void Start()
    {
        grabBarController = gameObject.GetComponent<GrabBarController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGameOver)
        {
            return;
        }

        if(Input.GetMouseButtonUp(0) && TargetCreature)
        {
            OnReleaseMouse();
        }

        if (TargetCreature)
        {
            grabBarController.UpdateGrabBarPointer();
        }
    }



    public void SetTargetCreature(Creature creature)
    {
        if (!TargetCreature && !IsGameOver)
        {
            TargetCreature = creature;
            creature.SetState(CreatureState.Grabbing);
            grabBarController.ShowGrabBar();
        }
    }
 
    public void OnReleaseMouse()
    {
        grabBarController.HideGrabBar();
        if (grabBarController.IsGrabPointerInZone())
        {
            TargetCreature.SetState(CreatureState.GrabbedHit);
        } else
        {
            TargetCreature.SetState(CreatureState.GrabbedMiss);
        }
        UnsetTargetCreature();
    }

    private void UnsetTargetCreature()
    {
        if (TargetCreature)
        {
            TargetCreature.SetState(CreatureState.Idle);
        }
        TargetCreature = null;
    }

    public void OnGameOver()
    {
        IsGameOver = true;
        grabBarController.HideGrabBar();
        UnsetTargetCreature();
        Debug.Log("Game over!");
    }
}
