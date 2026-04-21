using UnityEngine;
using UnityEngine.InputSystem;

public class Key : MonoBehaviour
{
    [Header("Check if item is ladder")]
    [SerializeField] public bool forladder;

    [Header("For Key")]
    [SerializeField] public bool key1 = false;

    [Header("For Ladder")]
    [SerializeField] public bool gotLadder;

    public GameManager manager; //  ADD THIS

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!forladder)
            {
                key1 = true;
                Debug.Log("Key picked up!");
            }
            else
            {
                gotLadder = true;
                Debug.Log("Ladder picked up");

                if (manager != null)
                {
                    manager.LadderItem = true; //  DIRECTLY UPDATE
                }
            }

            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = false;
    }
}