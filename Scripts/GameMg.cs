using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMg : MonoBehaviour
{
    public StageMg stageManager;
    public GameObject clearPanel;

    void Start()
    {
        // セーブデータから現在のチュートリアルステージを読み込む
        int currentTutorialStage = PlayerPrefs.GetInt("currentTutorialStage", 0);

        // チュートリアルのステージを初期化
        stageManager.Initialize(currentTutorialStage);
        stageManager.stageClear += Cleared;

        // clearPanelとstageManagerの状態を確認
        if (clearPanel == null)
        {
            Debug.LogError("Clear panel is not assigned.");
        }
        if (stageManager == null)
        {
            Debug.LogError("Stage manager is not assigned.");
        }
    }

    public void Cleared()
    {
        Debug.Log("Stage cleared: " + stageManager.CurrentTutorialStage);
        StartCoroutine(ClearStage());
    }

    IEnumerator ClearStage()
    {
        clearPanel.SetActive(true);
        yield return new WaitForSeconds(1.0f);  // クリアパネル表示の待機時間

        stageManager.SaveProgress();  // 進行状況の保存

        // 次のチュートリアルステージをロード
        if (stageManager.CurrentTutorialStage >= stageManager.tutorialStageFiles.Length - 1)
        {
            SceneManager.LoadScene("Clear");  // すべてのステージをクリアしたらクリアシーンへ
        }
        else
        {
            stageManager.Initialize(stageManager.CurrentTutorialStage + 1);  // 次のチュートリアルステージをロード
        }

        clearPanel.SetActive(false);
    }

    public void OnTitleClick()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
