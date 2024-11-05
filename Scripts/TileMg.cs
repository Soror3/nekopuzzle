using UnityEngine;

public enum TileType
{
    DEATH,
    ALIVE
}

public class TileMg : MonoBehaviour
{
    public TileType type;
    public Sprite deathSprite;
    public Sprite aliveSprite;

    SpriteRenderer spriteRenderer;
    Vector2Int intPosition;

    public delegate void Clicked(Vector2Int center);
    public Clicked clicked;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(TileType tileType, Vector2Int position)
    {
        intPosition = position;
        SetType(tileType);
    }

    void SetType(TileType tileType)
    {
        type = tileType;
        SetImage(tileType);
    }

    void SetImage(TileType tileType)
    {
        if (type == TileType.DEATH)
        {
            spriteRenderer.sprite = deathSprite;
        }
        else if (type == TileType.ALIVE)
        {
            spriteRenderer.sprite = aliveSprite;
        }
    }

    public void OnTile()
    {
        RecerseTile();
        clicked(intPosition);
    }

    public void RecerseTile()
    {
        if (type == TileType.DEATH)
        {
            SetType(TileType.ALIVE);
        }
        else if (type == TileType.ALIVE)
        {
            SetType(TileType.DEATH);
        }
    }
}
