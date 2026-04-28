using UnityEngine;
using UnityEngine.InputSystem;

public class LayerMovement : MonoBehaviour
{
    private float movementspeed = 5f;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); 

        // Force the character to face DOWN when the game first starts
        anim.SetFloat("LastInputX", 0f);
        anim.SetFloat("LastInputY", -1f);
    }

    void Update()
    {
        float x = 0f;
        float y = 0f;
        
        if (Time.timeScale == 0f)
        {
            return; 
        }

        if (Keyboard.current.aKey.isPressed) x = -1; 
        else if (Keyboard.current.dKey.isPressed) x = 1;

        if (Keyboard.current.wKey.isPressed) y = 1;
        else if (Keyboard.current.sKey.isPressed) y = -1;

        // Lock to one direction only
        if (x != 0) moveInput = new Vector2(x, 0); 
        else if (y != 0) moveInput = new Vector2(0, y); 
        else moveInput = Vector2.zero;

        // --- ANIMATOR LOGIC START ---
        
        // Check if we are moving (true or false)
        bool isMoving = moveInput != Vector2.zero;
        
        // Tell the Animator true or false!
        anim.SetBool("IsWalking", isMoving);

        if (isMoving)
        {
            anim.SetFloat("InputX", moveInput.x);
            anim.SetFloat("InputY", moveInput.y);

            anim.SetFloat("LastInputX", moveInput.x);
            anim.SetFloat("LastInputY", moveInput.y); 
        }
        // --- ANIMATOR LOGIC END ---
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * movementspeed;
    }
}