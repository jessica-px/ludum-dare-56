using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CreatureState
{
    Idle,
    Grabbing,
    GrabbedMiss,
    GrabbedHit
}

public class Creature : MonoBehaviour
{
    private Rigidbody2D rigidbody;
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;

    private bool isSelected = false;
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
        rigidbody = gameObject.GetComponent<Rigidbody2D>();

        rigidbody.velocity = new Vector2(8.0f, 8.0f);
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
        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);
    }

    public void SetState(CreatureState newState)
    {
        switch (newState)
        {
            case CreatureState.Grabbing:
                spriteRenderer.color = new Color(230, 252, 228);
                break;
            case CreatureState.Idle:
                spriteRenderer.color = color;
                break;
            default:
                Destroy(gameObject);
                break;
        }
    }

    private void OnMouseEnter()
    {
        Debug.Log("on enter");
        gameManager.SetCursor(true);
    }

    private void OnMouseExit()
    {
        gameManager.SetCursor(false);
    }

    private void OnMouseDown()
    {
        gameManager.SetTargetCreature(this);
    }


}
