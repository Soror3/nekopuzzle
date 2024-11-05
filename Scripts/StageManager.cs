using UnityEngine;
using UnityEngine.UI;  // UI関連の使用
using MyGameNamespace;  // TileManagerとTilesTypeの名前空間をインポート
using UnityEngine.SceneManagement;  // SceneManagerの使用
using System.Collections;

public class StageManager : MonoBehaviour
{
    public TextAsset[] stageFiles;  // ステージファイルの配列
    private TilesType[,] tileTable;  // タイルの状態を管理する配列
    private TileManager[,] tileTableObj;  // タイルのオブジェクト管理用配列

    public TileManager tilePrefab;  // タイルPrefab
    public GameObject clearPanel;  // クリアパネルの参照
    public Text currentStageText;  // 現在のステージ数を表示するテキスト

    private int currentStage;

    void Start()
    {
        Debug.Log("StageManager Start");  // デバッグ用ログ
        Debug.Log("Current Stage: " + GameManager.instance.currentStage);  // デバッグ用ログ

        currentStage = GameManager.instance.currentStage;  // GameManagerから現在のステージを取得
        Initialize(currentStage);  // ステージ初期化
    }

    public void Initialize(int stage)
    {
        Debug.Log("Initialize called with stage: " + stage);  // デバッグ用ログ

        if (stage < 0 || stage >= stageFiles.Length)
        {
            Debug.LogError("Stage index out of range: " + stage);
            return;
        }

        currentStage = stage;

        if (stageFiles[currentStage] == null)
        {
            Debug.LogError("Stage file is null for stage: " + currentStage);
            return;
        }

        LoadStageFromText(currentStage);  // ステージデータの読み込み
        CreateStage();  // ステージの生成
        UpdateStageText();  // ステージ番号のUI更新
    }

    public void CreateStage()
    {
        Debug.Log("CreateStage called.");  // デバッグ用ログ

        if (tileTable == null)
        {
            Debug.LogError("tileTable is null. Stage creation failed.");
            return;
        }

        if (tilePrefab == null)
        {
            Debug.LogError("tilePrefab is not assigned. Stage creation failed.");
            return;
        }

        // 既存のタイルオブジェクトをリセット
        if (tileTableObj != null)
        {
            foreach (var tile in tileTableObj)
            {
                if (tile != null)
                {
                    Destroy(tile.gameObject);
                }
            }
        }

        Vector2 halfSize;
        float tileSize = tilePrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        halfSize.x = tileSize * (tileTable.GetLength(0) / 2);
        halfSize.y = tileSize * (tileTable.GetLength(1) / 2);

        tileTableObj = new TileManager[tileTable.GetLength(0), tileTable.GetLength(1)];

        for (int y = 0; y < tileTable.GetLength(1); y++)
        {
            for (int x = 0; x < tileTable.GetLength(0); x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                TileManager tile = Instantiate(tilePrefab);
                tile.Init(tileTable[x, y], position);
                tile.clicked += ClickedTile;  // タイルクリック時の処理
                Vector2 setPosition = (Vector2)position * tileSize - halfSize;
                setPosition.y *= -1;
                tile.transform.position = setPosition;
                tileTableObj[x, y] = tile;
            }
        }
        Debug.Log("Stage created.");
    }

    public void LoadStageFromText(int loadStage)
    {
        Debug.Log($"LoadStageFromText called for stage: {loadStage}");  // デバッグ用ログ

        if (stageFiles[loadStage] == null)
        {
            Debug.LogError("Stage file is null");
            return;
        }

        string[] lines = stageFiles[loadStage].text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        int columns = 5;
        int rows = 5;

        tileTable = new TilesType[columns, rows];  // タイルデータの初期化
        tileTableObj = new TileManager[columns, rows];

        for (int y = 0; y < rows; y++)
        {
            string[] values = lines[y].Split(new[] { ',' });

            for (int x = 0; x < columns; x++)
            {
                if (x >= values.Length)
                {
                    Debug.LogWarning($"Column {x} is out of bounds for row {y}");
                    continue;
                }

                if (values[x] == "0")
                {
                    tileTable[x, y] = TilesType.DEATH;
                }
                else if (values[x] == "1")
                {
                    tileTable[x, y] = TilesType.ALIVE;
                }
                else
                {
                    Debug.LogWarning($"Unexpected value '{values[x]}' at ({x}, {y})");
                }
            }
        }
    }

    public void ClickedTile(Vector2Int center)
    {
        ReverseTiles(center);
        if (IsClear())
        {
            StartCoroutine(ClearStage());
        }
    }

    void ReverseTiles(Vector2Int center)
    {
        for (int x = center.x - 1; x <= center.x + 1; x++)
        {
            for (int y = center.y - 1; y <= center.y + 1; y++)
            {
                // >>> Mod. 2024/10/29 by Koutari
                // if (x < 0 || y < 0 || x >= tileTable.GetLength(0) || y >= tileTable.GetLength(1)) continue;
                if (x < 0 || y < 0 || x >= tileTable.GetLength(0) || y >= tileTable.GetLength(1) ||
                (x==center.x-1 && y==center.y-1) || 
                (x==center.x-1 && y==center.y+1) ||
                (x==center.x+1 && y==center.y-1) || 
                (x==center.x+1 && y==center.y+1) ) continue;
                // <<< Mod. 2024/10/29 by Koutari

                tileTable[x, y] = (tileTable[x, y] == TilesType.ALIVE) ? TilesType.DEATH : TilesType.ALIVE;
                tileTableObj[x, y].UpdateTile(tileTable[x, y]);
            }
        }
    }

    public bool IsClear()
    {
        foreach (TilesType tile in tileTable)
        {
            if (tile == TilesType.ALIVE)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator ClearStage()
    {
        Debug.Log("ClearStage called.");
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.ShowClearPanel();  // クリアパネルを表示
    }

    public void OnResetButton()
    {
        Initialize(currentStage);  // 現在のステージをリセット
    }

    public void LoadNextStage()
    {
        int nextStage = currentStage + 1;

        if (nextStage >= stageFiles.Length)
        {
            GameManager.instance.ClearGame();  // 全ステージクリア
        }
        else
        {
            GameManager.instance.UpdateStage(nextStage);  // 次のステージへ進む
            SceneManager.LoadScene("Stage" + nextStage);  // 次のステージシーンに移動
        }
    }

    // 現在のステージ数をUIに表示
    public void UpdateStageText()
    {
        if (currentStageText != null)
        {
            currentStageText.text = $"{currentStage}匹目";  // ステージ数を表示
        }
    }
}
