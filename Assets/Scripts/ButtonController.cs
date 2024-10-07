using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum ButtonType
{
    Start,
    Tutorial,
    PlayAgain
}

public class StartMenuManager : MonoBehaviour
{
    public GameObject startScreen;
    public GameObject tutorialScreen;
    public GameObject gameScreen;
    public ButtonSFXController buttonSfxController;
    public Sprite hoverSprite;
    public Sprite unhoveredSprite;
    public ButtonType buttonType;

    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        buttonSfxController = GameObject.Find("ButtonSFX").GetComponent<ButtonSFXController>();
    }

    private void OnMouseDown()
    {
        buttonSfxController.PlayClick();

        switch (buttonType)
        {
            case ButtonType.Start:
                tutorialScreen.SetActive(true);
                startScreen.SetActive(false);
                break;
            case ButtonType.Tutorial:
                tutorialScreen.SetActive(false);
                gameScreen.SetActive(true);
                break;
            case ButtonType.PlayAgain:
                GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
                gameManager.StartNewGame(true);
                break;
        }
        
    }

    private void OnMouseEnter()
    {
        spriteRenderer.sprite = hoverSprite;
    }

    private void OnMouseExit()
    {
        spriteRenderer.sprite = unhoveredSprite;
    }
}
