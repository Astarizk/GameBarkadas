using UnityEngine;
using UnityEngine.InputSystem;

public class LayerMovement : MonoBehaviour
{
    private float movementspeed = 5f;
    private Rigidbody2D rb;
    private int flip = 0;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float x = 0f;
        float y = 0f;
        if (Time.timeScale == 0f)
        {
            return; // stop all movement logic when paused
        }

        if (Keyboard.current.aKey.isPressed)
        {
            x = -1; 
            flip = 1;
        }
            
        else if (Keyboard.current.dKey.isPressed )
        {
            x = 1;
            flip = 0;
        }
        

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
        if (flip == 0)
        {
            transform.localScale = new Vector3(6, 6, 6);
        }
        else
        {
            transform.localScale = new Vector3(-6, 6, 6);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * movementspeed;
    }
}