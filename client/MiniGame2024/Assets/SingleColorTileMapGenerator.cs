using UnityEngine;
using UnityEngine.Tilemaps;

public class SingleColorTileMapGenerator : MonoBehaviour
{
    public int width = 800;
    public int height = 600;
    public Sprite whiteSprite;

    private Tilemap tilemap;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        GenerateWhiteTileMap();
    }

    void GenerateWhiteTileMap()
    {
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = whiteSprite;
        tile.color = Color.white;

        // 填充整个 Tilemap
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
        Debug.Log(tilemap);
    }
}
