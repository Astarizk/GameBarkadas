using UnityEngine;
using UnityEngine.InputSystem;

public class LayerMovement : MonoBehaviour
{
    private float movementspeed = 5f;
    private Rigidbody2D rb;

    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float x = 0f;
        float y = 0f;

        if (Keyboard.current.aKey.isPressed)
            x = -1;
        else if (Keyboard.current.dKey.isPressed)
            x = 1;

        if (Keyboard.current.wKey.isPressed)
            y = 1;
        else if (Keyboard.current.sKey.isPressed)
            y = -1;

        //Lock to one direction only
        if (x != 0)
        {
            moveInput = new Vector2(x, 0); // horizontal only
        }
        else if (y != 0)
        {
            moveInput = new Vector2(0, y); // vertical only
        }
        else
        {
            moveInput = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * movementspeed;
    }
}