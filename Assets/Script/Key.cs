using UnityEngine;
using UnityEngine.InputSystem;

public class Key : MonoBehaviour
{
    [Header("Check if item is ladder")]
    [SerializeField]
    public bool forladder;
    [Header("For Key")]
    [SerializeField]
    public bool key1 = false;

    private bool playerInRange = false;
    [Header("For Ladder")]
    [SerializeField]
    public bool gotLadder;
    void Update()
    {
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame && forladder == false)
        {
            key1 = true;
            Debug.Log("Key picked up!");

            gameObject.SetActive(false);
        }
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame && forladder == true)
        {
            gotLadder = true;
            Debug.Log("Ladder picked up");

            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}