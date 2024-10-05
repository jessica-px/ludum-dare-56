using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public Creature? TargetCreature { get; private set; } = null;
    private VisualElement uiRoot;
    private VisualElement grabBar;
    private VisualElement grabPointer;
    private VisualElement grabZone;

    private float GrabBarWidth = 300; // harcoded widths here bc I cannot figure out how to read this properly
    private float GrabPointerWidth = 10; 
    private float GrabPointerPercent = 0;
    private float GrabPointerSpeed = .001f;

    // Start is called before the first frame update
    void Start()
    {
        uiRoot = GameObject.Find("Canvas").GetComponent<UIDocument>().rootVisualElement;
        grabBar = uiRoot.Q("GrabBar");
        grabZone = uiRoot.Q("GrabZone");
        grabPointer = uiRoot.Q("GrabPointer");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0) && TargetCreature)
        {
            OnReleaseMouse();
        }

        if (TargetCreature)
        {
            UpdateGrabBarPointer();
        }
    }

    public void UpdateGrabBarPointer()
    {
        float newPercent = GrabPointerPercent + GrabPointerSpeed;
        if (newPercent > 1)
        {
            newPercent = 1;
            GrabPointerSpeed *= -1;
        } else if (newPercent < 0)
        {
            newPercent = 0;
            GrabPointerSpeed *= -1;
        }
        GrabPointerPercent = newPercent;

        float centerOffset = GrabPointerWidth / 2;
        grabPointer.style.left = GrabBarWidth * GrabPointerPercent - centerOffset;
    }

    public void SetTargetCreature(Creature creature)
    {
        if (!TargetCreature)
        {
            TargetCreature = creature;
            creature.SetState(CreatureState.Grabbing);
            grabBar.style.visibility = Visibility.Visible;
            grabZone.style.marginLeft = GrabBarWidth * TargetCreature.sensitivityStart;
            grabZone.style.width = GrabBarWidth * TargetCreature.sensitivityAmount;
        }
    }

    public bool IsGrabPointerInZone(Creature creature)
    {
        bool grabPointerIsAboveMin = GrabPointerPercent >= creature.sensitivityStart;
        bool grabPointerIsBelowMax = GrabPointerPercent <= creature.sensitivityStart + creature.sensitivityAmount;
  
        if (grabPointerIsAboveMin && grabPointerIsBelowMax)
        {
            Debug.Log("hit");
            return true;
        } else
        {
            Debug.Log("miss");
            return false;
        }
    }

    public void OnReleaseMouse()
    {
        grabBar.style.visibility = Visibility.Hidden;
        if (IsGrabPointerInZone(TargetCreature))
        {
            TargetCreature.SetState(CreatureState.GrabbedHit);
        } else
        {
            TargetCreature.SetState(CreatureState.GrabbedMiss);
        }
        TargetCreature = null;
    }
}
