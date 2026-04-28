using UnityEngine;
using UnityEngine.SceneManagement;

public class Retry : MonoBehaviour
{
    public int Level;

    public void LevelSelect()
    {
        Time.timeScale = 1f; // Resume game time
        SceneManager.LoadScene(Level);
    }
}