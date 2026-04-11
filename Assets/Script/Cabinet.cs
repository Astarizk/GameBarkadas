using UnityEngine;
using UnityEngine.InputSystem;

public class Cabinet : MonoBehaviour
{
    public Transform destination;
    public Transform exit;
    private GameObject player;
    private bool isHidden = false;

    private bool isPlayerInside = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (isPlayerInside && Keyboard.current.eKey.wasPressedThisFrame)
        {
            player.transform.position = destination.position;
        }       
        if (isPlayerInside && Keyboard.current.eKey.wasPressedThisFrame && isHidden == false)
        {
            player.transform.position = destination.position;
            isHidden = true;
        }
         if (isPlayerInside && Keyboard.current.eKey.wasPressedThisFrame && isHidden == true)
        {
            player.transform.position = exit.position;
            isHidden = false;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = false;
            isHidden = false;
        }
    }
}