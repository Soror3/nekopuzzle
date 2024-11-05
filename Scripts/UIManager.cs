using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject infoPanel;   // 説明パネル
    public Button infoButton;      // 説明ボタン
    public Button resetButton;     // リセットボタン

    public GameObject setumei;     //チュートリアルの文字

    void Start()
    {
        if (infoPanel == null)
        {
            Debug.LogError("infoPanelがアサインされていません。インスペクターでアサインしてください。");
            return;
        }

        if (resetButton == null)
        {
            Debug.LogError("resetButtonがアサインされていません。インスペクターでアサインしてください。");
            return;
        }

        // 説明パネルとリセットボタンの初期状態を設定
        infoPanel.SetActive(false);   // 説明パネルは初期状態で非表示
        resetButton.gameObject.SetActive(true);  // リセットボタンは表示
        setumei.gameObject.SetActive(true);  // 文字は表示

        // infoButtonにリスナーを追加
        if (infoButton != null)
        {
            infoButton.onClick.AddListener(ShowInfoPanel);
        }

        // 説明パネルにクリックイベントを追加して非表示にする
        infoPanel.AddComponent<Button>().onClick.AddListener(HideInfoPanel);
    }

    public void ShowInfoPanel()
    {
        infoPanel.SetActive(true);   // 説明パネルを表示
        resetButton.gameObject.SetActive(false);  // リセットボタンを非表示
        setumei.gameObject.SetActive(false);  // 文字も非表示
    }

    public void HideInfoPanel()
    {
        infoPanel.SetActive(false);  // 説明パネルを非表示
        resetButton.gameObject.SetActive(true);   // リセットボタンを再表示
        setumei.gameObject.SetActive(true);  // 文字も再表示        
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (infoPanel == null)
        {
            infoPanel = GameObject.Find("infoPanel");
        }
    }
}
