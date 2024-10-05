using UnityEngine;
using UnityEngine.UIElements;

public class GrabBarController : MonoBehaviour
{
    public GameManager gameManager;
    private VisualElement uiRoot;
    private VisualElement grabBar;
    private VisualElement grabPointer;
    private VisualElement grabZone;

    private float GrabBarWidth = 400; // harcoded widths here bc I cannot figure out how to read this properly
    private float GrabPointerWidth = 80;
    private float GrabPointerPercent = 0;
    private float GrabPointerSpeed = .008f;

    void Start()
    {
        gameManager = gameObject.GetComponent<GameManager>();
        uiRoot = GameObject.Find("Canvas").GetComponent<UIDocument>().rootVisualElement;
        grabBar = uiRoot.Q("GrabBar");
        grabZone = uiRoot.Q("GrabZone");
        grabPointer = uiRoot.Q("GrabPointer");
    }


    public void UpdateGrabBarPointer()
    {
        float newPercent = GrabPointerPercent + GrabPointerSpeed;
        if (newPercent > 1)
        {
            newPercent = 1;
            GrabPointerSpeed *= -1;
        }
        else if (newPercent < 0)
        {
            newPercent = 0;
            GrabPointerSpeed *= -1;
        }
        GrabPointerPercent = newPercent;

        float centerOffset = GrabPointerWidth / 2;
        grabPointer.style.left = GrabBarWidth * GrabPointerPercent - centerOffset;
    }

    public void HideGrabBar()
    {
        grabZone.style.visibility = Visibility.Hidden;
        grabPointer.style.visibility = Visibility.Hidden;
    }

    public void ShowGrabBar()
    {
        if (gameManager.TargetCreature)
        {
            grabZone.style.visibility = Visibility.Visible;
            grabPointer.style.visibility = Visibility.Visible;
            grabZone.style.marginLeft = GrabBarWidth * gameManager.TargetCreature.sensitivityStart;
            grabZone.style.width = GrabBarWidth * gameManager.TargetCreature.sensitivityAmount;
            GrabPointerPercent = 0;
        }
    }

    public bool IsGrabPointerInZone()
    {
        if (gameManager.TargetCreature)
        {
            bool grabPointerIsAboveMin = GrabPointerPercent >= gameManager.TargetCreature.sensitivityStart;
            bool grabPointerIsBelowMax = GrabPointerPercent <= gameManager.TargetCreature.sensitivityStart + gameManager.TargetCreature.sensitivityAmount;

            if (grabPointerIsAboveMin && grabPointerIsBelowMax)
            {
                Debug.Log("pointer in zone");
                return true;
            }
            else
            {
                Debug.Log("pointer not in zone");
                return false;
            }
        }
        return false;
    }

}
