using UnityEngine;
using UnityEngine.SceneManagement;

public class Retry : MonoBehaviour
{
    public int Level;

    public void LevelSelect()
    {
        SceneManager.LoadScene(Level);
    }
}