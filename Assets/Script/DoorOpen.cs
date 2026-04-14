using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    public GameManager Manager;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Manager.gameItem1Picked == true)
        {
            Destroy(gameObject);
        }
    }
}
