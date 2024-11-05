using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public void OnPanel(string loadScene)
    {
        SceneManager.LoadScene(loadScene);
    }
}
