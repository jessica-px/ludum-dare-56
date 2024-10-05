using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public bool IsHovered { get; private set; } = false;
    public float maxSpeed = 2;
    public Color color;
    public float hungerValue = 10;

    public AudioClip successAudio;
    public AudioClip missAudio;
    public AudioClip deathAudio;

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
        audioSource = gameObject.GetComponent<AudioSource>();

        rb.velocity = new Vector2(8.0f, 8.0f);
        spriteRenderer.color = color;
        float maxSensitivityStart = 1 - sensitivityAmount;
        sensitivityStart = Random.Range(0, maxSensitivityStart);
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
                audioSource.PlayOneShot(missAudio);
                StartCoroutine(ResetToIdle());
                break;
            case CreatureState.Idle:
                spriteRenderer.color = color;
                break;
            case CreatureState.GrabbedDeath:
                spriteRenderer.color = Color.red;
                StartCoroutine(DestroyCreatureAfterDelay());
                audioSource.PlayOneShot(deathAudio);
                break;
            case CreatureState.GrabbedSuccess:
                spriteRenderer.color = Color.green;
                StartCoroutine(DestroyCreatureAfterDelay());
                audioSource.PlayOneShot(successAudio);
                break;
        }
    }

    IEnumerator ResetToIdle()
    {
        yield return new WaitForSeconds(.2f);
        SetState(CreatureState.Idle);
    }


    IEnumerator DestroyCreatureAfterDelay()
    {
        yield return new WaitForSeconds(.2f);
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


}
