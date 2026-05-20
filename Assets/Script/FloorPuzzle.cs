using UnityEngine;

public class FloorPuzzle : MonoBehaviour
{
    public string groupID = "room1_puzzle";     // unique per room
    public GameObject door;
    public int totalPieces;                     // set this to how many pieces are in this group

    [HideInInspector] public int currentStep = 0;
    [HideInInspector] public bool isSolved = false;
    private PuzzlePiece[] pieces;
    void Awake()
    {
        PlayerPrefs.DeleteAll(); // REMOVE THIS AFTER ONE PLAY

        pieces = GetComponentsInChildren<PuzzlePiece>();
        totalPieces = pieces.Length;
    }
    void Start()
    {
        // Only find pieces that belong to THIS group
        pieces = GetComponentsInChildren<PuzzlePiece>();
        totalPieces = pieces.Length;

        if (PlayerPrefs.GetInt(groupID + "_solved", 0) == 1)
            ApplySolvedState();
    }

    public void ReportStep(PuzzlePiece piece)
    {
        if (isSolved) return;

        if (piece.order == currentStep)
        {
            piece.SetActivated(true);
            currentStep++;

            if (currentStep >= totalPieces)
                Solve();
        }
        else
        {
            ResetPuzzle();
        }
    }

    void Solve()
    {
        isSolved = true;
        PlayerPrefs.SetInt(groupID + "_solved", 1);
        PlayerPrefs.Save();
        ApplySolvedState();
    }

    void ApplySolvedState()
    {
        isSolved = true;
        currentStep = totalPieces;

        if (door != null) door.SetActive(false);

        if (pieces == null)
            pieces = GetComponentsInChildren<PuzzlePiece>();

        foreach (PuzzlePiece p in pieces)
            p.SetActivated(true);
    }

    public void ResetPuzzle()
    {
        if (isSolved) return;

        currentStep = 0;

        foreach (PuzzlePiece p in pieces)
            p.SetActivated(false);
    }

    [ContextMenu("Reset Puzzle (Test)")]
    public void ResetForTesting()
    {
        isSolved = false;
        currentStep = 0;
        PlayerPrefs.DeleteKey(groupID + "_solved");

        if (pieces == null)
            pieces = GetComponentsInChildren<PuzzlePiece>();

        foreach (PuzzlePiece p in pieces)
            p.SetActivated(false);

        if (door != null) door.SetActive(true);
    }
}