using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class HungerBarController : MonoBehaviour
{
    public GameManager gameManager;
    private VisualElement uiRoot;
    private ProgressBar hungerBar;
    private VisualElement hungerBarProgress;
    public float hungerValue { get; private set; } = 100;
    public float hungerRate;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = gameObject.GetComponent<GameManager>();
        uiRoot = GameObject.Find("Canvas").GetComponent<UIDocument>().rootVisualElement;
        hungerBar = uiRoot.Q<ProgressBar>("HungerBar");
        hungerBarProgress = uiRoot.Q(className: "unity-progress-bar__progress");
    }


    void Update()
    {
        if (!gameManager.IsGameOver)
        {
            UpdateHunger();
        }
    }

    void UpdateHunger()
    {
        hungerValue -= hungerRate * Time.deltaTime;
        hungerBar.value = hungerValue;
        if (hungerValue <= 0)
        {
            gameManager.OnGameOver();
        }
         else if (hungerValue <= 30)
        {
            hungerBarProgress.style.backgroundColor = Color.red;
        } else if (hungerValue <= 70)
        {
            hungerBarProgress.style.backgroundColor = Color.yellow;
        } else
        {
            hungerBarProgress.style.backgroundColor = Color.green;
        }

    }

    public void ChangeHunger(float amount)
    {
        hungerValue = Mathf.Min(hungerValue + amount, 100);
    }

    public void Reset()
    {
        hungerValue = 100;
    }
}
