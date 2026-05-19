using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SequenceManager : MonoBehaviour
{
    public int[] correctSequence = { 0, 1, 2 };
    public int Which;
    private int currentIndex = 0;

    [Header("Cabinet")]
    public GameObject cabinetDoor;

    [Header("Indicator")]
    public TMP_Text indicatorText;

    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    public Color normalColor = Color.white;

    // BUTTONS
    public void PressRed()
    {
        CheckInput(0);
    }

    public void PressBlue()
    {
        CheckInput(1);
    }

    public void PressGreen()
    {
        CheckInput(2);
    }

    void CheckInput(int colorPressed)
    {
        if (colorPressed == correctSequence[currentIndex])
        {
            currentIndex++;

            StartCoroutine(ShowIndicator("Correct", correctColor));

            // Puzzle solved
            if (currentIndex >= correctSequence.Length)
            {
                UnlockCabinet();
            }
        }
        else
        {
            currentIndex = 0;

            StartCoroutine(ShowIndicator("Wrong Sequence", wrongColor));
        }
    }

    void UnlockCabinet()
    {
        StartCoroutine(ShowIndicator("Unlocked!", correctColor));

        cabinetDoor.SetActive(false);
    }

    IEnumerator ShowIndicator(string message, Color color)
    {
        indicatorText.text = message;
        indicatorText.color = color;

        indicatorText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        indicatorText.text = "";
        indicatorText.color = normalColor;

        indicatorText.gameObject.SetActive(false);
    }
}