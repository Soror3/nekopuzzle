using UnityEngine;

namespace MyGameNamespace
{
    public enum TilesType
    {
        DEATH,
        ALIVE
    }

    public class TileManager : MonoBehaviour
    {
        public TilesType type;
        public Sprite deathSprite;
        public Sprite aliveSprite;

        private SpriteRenderer spriteRenderer;
        private Vector2Int intPosition;

        // タイルがクリックされたときのデリゲート
        public delegate void Clicked(Vector2Int center);
        public Clicked clicked;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // タイルの初期化
        public void Init(TilesType tileType, Vector2Int position)
        {
            intPosition = position;
            SetType(tileType);
        }

        // タイルの状態を設定
        private void SetType(TilesType tileType)
        {
            type = tileType;
            SetImage(tileType);  // 画像を更新
        }

        // タイルの状態に応じて画像を設定
        private void SetImage(TilesType tileType)
        {
            if (tileType == TilesType.DEATH)
            {
                spriteRenderer.sprite = deathSprite;
            }
            else if (tileType == TilesType.ALIVE)
            {
                spriteRenderer.sprite = aliveSprite;
            }
        }

        // タイルがクリックされたときの処理
        public void OnTile()
        {
            ReverseTile();  // タイルの状態を反転
            if (clicked != null)  // clickedがnullでないか確認
            {
                clicked.Invoke(intPosition);  // イベント発火
            }
        }

        // タイルの状態を反転
        public void ReverseTile()
        {
            if (type == TilesType.DEATH)
            {
                SetType(TilesType.ALIVE);
            }
            else if (type == TilesType.ALIVE)
            {
                SetType(TilesType.DEATH);
            }
        }

        // タイルの状態を更新するメソッド
        public void UpdateTile(TilesType tileType)
        {
            SetType(tileType);  // タイルのタイプを設定し、画像を更新する
        }
    }
}
