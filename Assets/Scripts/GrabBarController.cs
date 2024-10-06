using UnityEngine;
using UnityEngine.UIElements;

public class GrabBarController : MonoBehaviour
{
    public GameManager gameManager;
    SpriteRenderer spriteRenderer;

    private float GrabPointerPercent = 0;
    private float GrabPointerSpeed = 1f;
    private float GrabBarYOffset = 0.8f;

    public GameObject pointer;
    public GameObject grabZoneShort;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }


    private void Update()
    {
        if (gameManager.TargetCreature)
        {
            TrackTargetCreature();
            UpdateGrabBarPointer();
        }
    }

    public void UpdateGrabBarPointer()
    {
        float newPercent = GrabPointerPercent + GrabPointerSpeed * Time.deltaTime;
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

        float rotation = newPercent * -180;
        pointer.transform.eulerAngles = new Vector3(0, 0, rotation);
    }

    public void TrackTargetCreature()
    {
        transform.position = gameManager.TargetCreature.transform.position + new Vector3(0, GrabBarYOffset, 0);
    }

    public void HideGrabBar()
    {
        spriteRenderer.enabled = false;
        grabZoneShort.SetActive(false);
        pointer.SetActive(false);
    }

    public void ShowGrabBar()
    {
        if (gameManager.TargetCreature)
        {
            GrabPointerPercent = 0;
            pointer.SetActive(true);
            spriteRenderer.enabled = true;
            float rotation = gameManager.TargetCreature.sensitivityStart * -180;

            if (!gameManager.TargetCreature.HasLargeGrabBar)
            {
                grabZoneShort.SetActive(true);
                grabZoneShort.transform.eulerAngles = new Vector3(0, 0, rotation);
            }
        }
    }

    public bool IsGrabPointerInZone()
    {
        if (gameManager.TargetCreature)
        {
            bool grabPointerIsAboveMin = GrabPointerPercent >= gameManager.TargetCreature.sensitivityStart;
            bool grabPointerIsBelowMax = GrabPointerPercent <= gameManager.TargetCreature.sensitivityStart + gameManager.TargetCreature.GetGrabSensitivity();

            if (grabPointerIsAboveMin && grabPointerIsBelowMax)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

}
