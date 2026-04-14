using UnityEngine;

public class BoxPressure : MonoBehaviour
{
    private GameObject box;
    public GameObject DoorBox;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        box = GameObject.FindGameObjectWithTag("Box");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Box"))
        {
            DoorBox.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Box"))
        {
            DoorBox.SetActive(true);
        }
    }
}
