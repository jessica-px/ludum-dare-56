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
    public float hungerValue { get; private set; } = 100;
    private float hungerRate = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = gameObject.GetComponent<GameManager>();
        uiRoot = GameObject.Find("Canvas").GetComponent<UIDocument>().rootVisualElement;
        hungerBar = uiRoot.Q<ProgressBar>("HungerBar");
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
        hungerValue -= hungerRate;
        hungerBar.value = hungerValue;
        if (hungerValue <= 0)
        {
            gameManager.OnGameOver();
        }
    }

    public void ChangeHunger(float amount)
    {
        hungerValue += amount;
    }
}
