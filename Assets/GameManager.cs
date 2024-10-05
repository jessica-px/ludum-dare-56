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
    private VisualElement grabZone;
    private float GrabBarWidth = 300; // harcoded here bc I cannot figure out how to read this properly

    // Start is called before the first frame update
    void Start()
    {
        uiRoot = GameObject.Find("Canvas").GetComponent<UIDocument>().rootVisualElement;
        grabBar = uiRoot.Q("GrabBar");
        grabZone = uiRoot.Q("GrabZone");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            UnsetTargetCreature();
        }

        if (TargetCreature)
        {

        }
    }

    public void SetTargetCreature(Creature creature)
    {
        TargetCreature = creature;
        grabBar.style.visibility = Visibility.Visible;
        grabZone.style.marginLeft = GrabBarWidth * TargetCreature.sensitivityStart;
        grabZone.style.width = GrabBarWidth * TargetCreature.sensitivityAmount;
    }

    public void UnsetTargetCreature()
    {
        TargetCreature = null;
        grabBar.style.visibility = Visibility.Hidden;
    }
}
