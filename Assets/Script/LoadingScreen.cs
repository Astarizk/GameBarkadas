using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public GameObject loadingCanvas;
    public TextMeshProUGUI loadingText;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        loadingCanvas.SetActive(false);
        SceneManager.sceneLoaded += (s, m) => loadingCanvas.SetActive(false);
    }

    public void LoadScene(int index)   { StartCoroutine(Load(index, null)); }
    public void LoadScene(string name) { StartCoroutine(Load(-1, name)); }

    IEnumerator Load(int index, string name)
    {
        loadingCanvas.SetActive(true);
        if (loadingText != null) loadingText.text = "Loading...";

        yield return null;

        AsyncOperation op = name == null
            ? SceneManager.LoadSceneAsync(index)
            : SceneManager.LoadSceneAsync(name);

        op.allowSceneActivation = false;
        while (op.progress < 0.9f) yield return null;

        yield return new WaitForSecondsRealtime(0.5f);
        op.allowSceneActivation = true;
    }
}