using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public bool InGrabMode { get; private set; } = false;
    private VisualElement uiRoot;
    private VisualElement grabBar;

    // Start is called before the first frame update
    void Start()
    {
        uiRoot = GameObject.Find("Canvas").GetComponent<UIDocument>().rootVisualElement;
        grabBar = uiRoot.Q("GrabBar");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            SetGrabMode(false);
        }

        if (InGrabMode)
        {
            Debug.Log("I'm in grab mode!");
        }
    }

    public void SetGrabMode(bool newValue)
    {
        InGrabMode = newValue;
        if (newValue)
        {
            grabBar.style.visibility = Visibility.Visible;
        } else
        {
            grabBar.style.visibility = Visibility.Hidden;
        }

    }
}
