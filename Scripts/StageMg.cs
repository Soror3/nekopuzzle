using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMg : MonoBehaviour
{
    public TextAsset[] tutorialStageFiles;  // チュートリアルステージファイル
    TileType[,] tileTable;
    TileMg[,] tileTableObj;

    public TileMg tilePrefab;
    public delegate void StageClear();
    public StageClear stageClear;

    private int currentTutorialStage;  // チュートリアルの現在のステージ番号

    // 現在のチュートリアルステージ番号を公開するプロパティ
    public int CurrentTutorialStage
    {
        get { return currentTutorialStage; }
        private set { currentTutorialStage = value; }
    }

    // 初期化: チュートリアルの進行状況に応じてステージを読み込み
    public void Initialize(int tutorialStage)
    {
        currentTutorialStage = tutorialStage;

        // 範囲チェックを追加
        if (currentTutorialStage < 0 || currentTutorialStage >= tutorialStageFiles.Length)
        {
            Debug.LogError("Tutorial stage index is out of bounds.");
            return;
        }

        LoadStageFromText(currentTutorialStage, tutorialStageFiles);
        CreateStage();
    }

    // ステージの生成
    public void CreateStage()
    {
        Debug.Log("CreateStage called.");

        // 既存のタイルオブジェクトがあればリセット
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

        tileTableObj = new TileMg[tileTable.GetLength(0), tileTable.GetLength(1)];

        for (int y = 0; y < tileTable.GetLength(1); y++)
        {
            for (int x = 0; x < tileTable.GetLength(0); x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                TileMg tile = Instantiate(tilePrefab);
                tile.Init(tileTable[x, y], position);  // タイルの初期化
                tile.clicked += ClickedTile;
                Vector2 setPosition = (Vector2)position * tileSize - halfSize;
                setPosition.y *= -1;
                tile.transform.position = setPosition;
                tileTableObj[x, y] = tile;
            }
        }
        Debug.Log("Stage created.");
    }

    // ステージのファイルからロード
    public void LoadStageFromText(int stageIndex, TextAsset[] stageFiles)
    {
        // stageIndexが範囲外の場合はエラーを表示して処理を中止
        if (stageIndex < 0 || stageIndex >= stageFiles.Length)
        {
            Debug.LogError($"Stage index {stageIndex} is out of bounds. The array has {stageFiles.Length} elements.");
            return;  // メソッドを終了してエラーを防ぐ
        }

        Debug.Log($"LoadStageFromText called for stage: {stageIndex}");
        string[] lines = stageFiles[stageIndex].text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        int columns = 5;
        int rows = 5;
        tileTable = new TileType[columns, rows];

        for (int y = 0; y < rows; y++)
        {
            string[] values = lines[y].Split(new[] { ',' });

            for (int x = 0; x < columns; x++)
            {
                if (values[x] == "0")
                {
                    tileTable[x, y] = TileType.DEATH;
                }
                else if (values[x] == "1")
                {
                    tileTable[x, y] = TileType.ALIVE;
                }
            }
        }
    }

    // タイルクリック時に呼び出し
    public void ClickedTile(Vector2Int center)
    {
        ReverseTiles(center);
        if (IsClear())
        {
            stageClear?.Invoke();  // ステージクリアイベント
        }
    }

    // 周囲のタイルを反転
    void ReverseTiles(Vector2Int center)
    {
        Vector2Int[] around =
        {
            center + Vector2Int.up,
            center + Vector2Int.down,
            center + Vector2Int.right,
            center + Vector2Int.left,
        };

        foreach (Vector2Int position in around)
        {
            if (position.x < 0 || tileTableObj.GetLength(0) <= position.x)
                continue;

            if (position.y < 0 || tileTableObj.GetLength(1) <= position.y)
                continue;

            tileTableObj[position.x, position.y].RecerseTile();
        }
    }

    // ステージがクリアされたかどうかの判定
    bool IsClear()
    {
        for (int y = 0; y < tileTable.GetLength(1); y++)
        {
            for (int x = 0; x < tileTable.GetLength(0); x++)
            {
                if (tileTableObj[x, y].type == TileType.ALIVE)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // 現在のステージを破壊
    public void DestroyStage()
    {
        for (int y = 0; y < tileTable.GetLength(1); y++)
        {
            for (int x = 0; x < tileTable.GetLength(0); x++)
            {
                if (tileTableObj[x, y] != null)
                {
                    Destroy(tileTableObj[x, y].gameObject);
                    tileTableObj[x, y] = null;
                }
            }
        }
    }

    // ゲーム進行状況を保存
    public void SaveProgress()
    {
        PlayerPrefs.SetInt("currentTutorialStage", currentTutorialStage);
        PlayerPrefs.Save();
        Debug.Log("Progress saved.");
    }
}
