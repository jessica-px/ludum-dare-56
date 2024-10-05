using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    private Rigidbody2D rigidbody;
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    private bool isSelected = false;
    public float maxSpeed = 2;
    public Color color;

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

    private void OnMouseDown()
    {
        gameManager.SetGrabMode(true);
        spriteRenderer.color = Color.green;
    }
}
