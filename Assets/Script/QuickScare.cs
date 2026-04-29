using UnityEngine;
using UnityEngine.Rendering.Universal;

public class QuickScare : MonoBehaviour
{
    private GameObject player;
    public GameObject Monster;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(Monster);
            Destroy(this);
        }
    }
}
