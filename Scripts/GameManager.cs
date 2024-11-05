using UnityEngine;
using UnityEngine.SceneManagement;  // シーン管理を使用するために追加

public class GameManager : MonoBehaviour
{
    public static GameManager instance;  // Singletonインスタンス
    public int currentStage;  // 現在のステージ
    public GameObject clearPanel;  // クリアパネル

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // シーンが変わってもGameManagerを破棄しない
        }
        else
        {
            Destroy(gameObject);  // 既にインスタンスが存在する場合は削除
        }
    }

    void Start()
    {
        // ゲーム開始時に進行状況をロードする
        LoadProgress();
    }

    // ゲームの初期化やクリア時の処理などをここで行う
    public void UpdateStage(int stage)
    {
        currentStage = stage;
        SaveProgress();  // ステージを更新したら進行状況を保存
    }

    public void ShowClearPanel()
    {
        if (clearPanel != null)
        {
            clearPanel.SetActive(true);  // クリアパネルを表示
        }
    }

    public void ClearGame()
    {
        Debug.Log("全ステージクリア！");  // 全ステージクリア時の処理
        PlayerPrefs.SetInt("GameCleared", 1);  // クリア状態を保存
        PlayerPrefs.Save();
        Debug.Log("GameCleared フラグが保存されました: 1");  // ログで確認
        SceneManager.LoadScene("ShinClear");  // クリア後のシーンへ遷移
    }

    // 進行状況を保存
    public void SaveProgress()
    {
        PlayerPrefs.SetInt("currentStage", currentStage);
        PlayerPrefs.Save();
        Debug.Log("Progress saved. Current stage: " + currentStage);
    }

    // 進行状況をロード
    public void LoadProgress()
    {
        currentStage = PlayerPrefs.GetInt("currentStage", 1);  // デフォルトはステージ1
        Debug.Log("Progress loaded. Current stage: " + currentStage);
    }

    // タイトル画面クリック時の処理
    public void OnTitleClick()
    {
        int gameCleared = PlayerPrefs.GetInt("GameCleared", 0);  // ゲームクリア状態を取得
        Debug.Log("タイトルクリック時のGameClearedフラグ: " + gameCleared);  // 現在のフラグ値を確認

        if (gameCleared == 1)
        {
            // 全ステージクリア済みの場合
            SceneManager.LoadScene("ShinClear");
        }
        else
        {
            // クリアしていない場合は進行中のステージから再開
            SceneManager.LoadScene("Stage" + currentStage);  // 現在のステージに遷移
        }
    }

    // バックグラウンドへの移行やアプリ終了時に進行状況を保存
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveProgress();  // バックグラウンドに移行する時に進行状況を保存
        }
    }

    void OnApplicationQuit()
    {
        SaveProgress();  // アプリが終了する時に進行状況を保存
    }
}
