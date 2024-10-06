using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum CreatureState
{
    Idle,
    Grabbing,
    GrabbedMiss,
    GrabbedSuccess,
    GrabbedDeath
}

public class Creature : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    public AudioSource audioSource;
    public bool HasLargeGrabBar;

    public bool IsHovered { get; private set; } = false;
    public float maxSpeed = 2;
    public float startVeloctyRange;
    public Color color;
    public float hungerValue = 10;

    private float LargeSensitivity = .5f;
    private float SmallSensitivity = .25f;

    public CreatureState State { get; private set; } = CreatureState.Idle;

    [Range(0, 1f)]
    public float sensitivityStart;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        audioSource = gameObject.GetComponent<AudioSource>();

        spriteRenderer.color = color;
        float sensitivityBuffer = .15f; // buffer to keep it from the very edge of the bar
        float maxSensitivityStart = 1 - GetGrabSensitivity() - sensitivityBuffer;
        sensitivityStart = Random.Range(sensitivityBuffer, maxSensitivityStart);

        // start by flinging it in a random direction
        Vector2 randomVelocity = new Vector2(Random.Range(-startVeloctyRange, startVeloctyRange), 3);
        rb.velocity = randomVelocity;
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
                spriteRenderer.color = color;
                StartCoroutine(ResetToIdle());
                break;
            case CreatureState.Idle:
                spriteRenderer.color = color;
                break;
            case CreatureState.GrabbedDeath:
                spriteRenderer.color = Color.red;
                DestroyCreature();
                break;
            case CreatureState.GrabbedSuccess:
                spriteRenderer.color = Color.green;
                DestroyCreature();
                break;
        }
    }

    IEnumerator ResetToIdle()
    {
        yield return new WaitForSeconds(.2f);
        SetState(CreatureState.Idle);
    }


    void DestroyCreature()
    {
        gameManager.currCreatureCount = Mathf.Max(0, gameManager.currCreatureCount - 1);
        Destroy(gameObject);
    }

    private void OnMouseEnter()
    {
        if (State == CreatureState.Idle)
        {
            IsHovered = true;
            gameManager.SetCursor(true);
        }
    }

    private void OnMouseExit()
    {
        IsHovered = false;
        gameManager.SetCursor(false);
    }

    private void OnMouseDown()
    {
        if (State == CreatureState.Idle)
        {
            gameManager.SetTargetCreature(this);
        }
        
    }

    public float GetGrabSensitivity()
    {
        if (HasLargeGrabBar)
        {
            return LargeSensitivity;
        }
        return SmallSensitivity;
    }
}
