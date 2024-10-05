using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CreatureState
{
    Idle,
    Grabbing,
    GrabbedMiss,
    GrabbedSuccess,
    GrabbedPop
}

public class Creature : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;

    public bool IsHovered { get; private set; } = false;
    public float maxSpeed = 2;
    public Color color;
    public float hungerValue = 10;

    public CreatureState State { get; private set; } = CreatureState.Idle;

    [Range(0, 1f)]
    public float sensitivityStart;

    [Range(0, 1f)]
    public float sensitivityAmount;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        rb.velocity = new Vector2(8.0f, 8.0f);
        spriteRenderer.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        // Visually unselect on mouse up
        if (Input.GetMouseButtonUp(0))
        {
            spriteRenderer.color = color;
        }

        // Keep max speed clamped
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        if (gameManager.IsGameOver)
        {
            Destroy(gameObject);
        }
    }

    public void SetState(CreatureState newState)
    {
        switch (newState)
        {
            case CreatureState.Grabbing:
                spriteRenderer.color = Color.yellow;
                break;
            case CreatureState.GrabbedMiss:
            case CreatureState.Idle:
                spriteRenderer.color = color;
                break;
            case CreatureState.GrabbedPop:
            case CreatureState.GrabbedSuccess:
                Destroy(gameObject);
                break;
        }
    }

    private void OnMouseEnter()
    {
        IsHovered = true;
        gameManager.SetCursor(true);
    }

    private void OnMouseExit()
    {
        IsHovered = false;
        gameManager.SetCursor(false);
    }

    private void OnMouseDown()
    {
        gameManager.SetTargetCreature(this);
    }


}
