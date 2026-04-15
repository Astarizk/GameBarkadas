using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public bool gameItem1Picked;
    public bool gameItem2Picked;
    public bool LadderItem;
    [Header("For Key")]
    [SerializeField]
    public GameObject ForItem1;
    public GameObject ForItem2;
    [Header("For Ladder")]
    [SerializeField]
    public GameObject ForLadder;

    private Key item1Script;
    private Key item2Script;
    private Key LadderScript;

    void Start()
    {
        item1Script = ForItem1.GetComponent<Key>();
        item2Script = ForItem2.GetComponent<Key>();
        LadderScript = ForLadder.GetComponent<Key>();
    }

    void Update()
    {
        // Item 1 check
        if (item1Script != null && item1Script.key1)
        {
            gameItem1Picked = true;
        }

        // Item 2 check
        if (item2Script != null && item2Script.key1)
        {
            gameItem2Picked = true;
        }        
        if (LadderScript != null && LadderScript.gotLadder)
        {
            LadderItem = true;
        }

    }
}