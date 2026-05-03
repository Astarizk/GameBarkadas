using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChangerOnTimer : MonoBehaviour
{
    public float ChangeTimer;
    public int Scene;
    private void Update()
    {
        ChangeTimer -= Time.deltaTime;
        if (ChangeTimer <= 0)
        {
            SceneManager.LoadScene(Scene);
        }
    }
}
