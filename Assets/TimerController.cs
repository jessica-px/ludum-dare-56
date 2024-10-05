using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TimerController : MonoBehaviour
{
    private GameManager gameManager;
    private VisualElement uiRoot;
    private Label timerLabel;
    float timeElapsed = 0;

    void Start()
    {
        gameManager = gameObject.GetComponent<GameManager>();
        uiRoot = GameObject.Find("Canvas").GetComponent<UIDocument>().rootVisualElement;
        timerLabel = uiRoot.Q<Label>("TimerLabel");
    }

    void Update()
    {
        if (!gameManager.IsGameOver)
        {
            timeElapsed += Time.deltaTime;
            int secondsElapsed = Mathf.RoundToInt(timeElapsed);
            var timespan = TimeSpan.FromSeconds(secondsElapsed);
            timerLabel.text = timespan.ToString((@"mm\:ss"));
        }
    }
}
