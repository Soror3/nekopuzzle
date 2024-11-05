using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // クリックされた時
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        if (IsGameCleared())
        {
            string clearScene = PlayerPrefs.GetString("ClearScene", "ShinClear");  // デフォルトは"ShinClear"
            SceneManager.LoadScene(clearScene);  // 動的にシーン名を取得して遷移
            ResetFlag("GameCleared");
        }
        else if (IsUpdateAvailable())
        {
            string updateScene = PlayerPrefs.GetString("UpdateScene", "UpdateScene");  // デフォルトは"UpdateScene"
            SceneManager.LoadScene(updateScene);  // 動的にシーン名を取得して遷移
            ResetFlag("HasUpdate");
        }
        else
        {
            SceneManager.LoadScene("help");  // 説明画面に遷移
        }
    }

    bool IsGameCleared()
    {
        return PlayerPrefs.GetInt("GameCleared", 0) == 1;
    }

    bool IsUpdateAvailable()
    {
        return PlayerPrefs.GetInt("HasUpdate", 0) == 1;
    }

    void ResetFlag(string flagName)
    {
        PlayerPrefs.SetInt(flagName, 0);
        PlayerPrefs.Save();
    }
}
