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
    public float TimeElapsed { get; private set; } = 0;

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
            TimeElapsed += Time.deltaTime;
            int secondsElapsed = Mathf.RoundToInt(TimeElapsed);
            var timespan = TimeSpan.FromSeconds(secondsElapsed);
            timerLabel.text = timespan.ToString((@"mm\:ss"));
        }
    }

    public void Reset()
    {
        TimeElapsed = 0;
    }
}
