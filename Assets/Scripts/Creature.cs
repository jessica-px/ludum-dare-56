using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms;

public enum CreatureState
{
    Idle,
    Targeted,
    GrabbedMiss,
    GrabbedSuccess,
    GrabbedDeath
}

public class Creature : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameManager gameManager;
    private Animator animator;
    public bool HasLargeGrabBar;
    public GameObject Glow;

    public bool IsHovered { get; private set; } = false;
    public float maxSpeed = 2;
    public float startVeloctyRange;
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
        animator = transform.GetComponentInChildren<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        float sensitivityBuffer = .15f; // buffer to keep it from the very edge of the bar
        float maxSensitivityStart = 1 - GetGrabSensitivity() - sensitivityBuffer;
        sensitivityStart = Random.Range(sensitivityBuffer, maxSensitivityStart);

        gameManager.currCreatureCount++;

        // start by flinging it in a random direction
        Vector2 randomVelocity = new Vector2(Random.Range(-startVeloctyRange, startVeloctyRange), 3);
        rb.velocity = randomVelocity;
    }

    // Update is called once per frame
    void Update()
    {

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
            case CreatureState.Targeted:
                Glow.SetActive(true);
                animator.SetBool("IsTargeted", true);
                animator.SetBool("Targeted Layer.IsTargeted", true);
                break;
            case CreatureState.GrabbedMiss:
                SetState(CreatureState.Idle);
                break;
            case CreatureState.Idle:
                Glow.SetActive(false);
                break;
            case CreatureState.GrabbedDeath:
                DestroyCreature();
                break;
            case CreatureState.GrabbedSuccess:
                DestroyCreature();
                break;
        }
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision");
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Wall");
            PlayBumpAnim();
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




    public void PlayBumpAnim()
    {
        animator.Play("CreatureAnim_Bump");
        animator.Play("Bump Layer.CreatureAnim_Bump");
    }


}
