using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public Tilemap kwiatkiTilemap;
    public List<Tile> groundTiles;
    public List<Tile> kwiatkiTiles;
    public Grid tileGrid;
    private GameManager gameManager;
    [Tooltip("Put value between 0 and 100")]
    public int kwiatekRespawnChance;
    private Vector3Int tilemapCenterPosition;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        if(gameManager.player != null)
            tilemapCenterPosition = tileGrid.WorldToCell(gameManager.player.transform.position);
        else tilemapCenterPosition = Vector3Int.zero;
        for (int i = tilemapCenterPosition.x - 20; i < tilemapCenterPosition.x + 20; i++)
        {
            for (int j = tilemapCenterPosition.y - 10; j < tilemapCenterPosition.y + 10; j++)
            {
                tilemap.SetTile(new Vector3Int(i, j, 0), groundTiles[Random.Range(0, groundTiles.Count)]);
                if(Random.Range(0, 100) > (100-kwiatekRespawnChance))
                    kwiatkiTilemap.SetTile(new Vector3Int(i, j, 0), kwiatkiTiles[Random.Range(0, kwiatkiTiles.Count)]);
            }
        }
    }
    
    void Update()
    {
        if (gameManager.gameState != GameManager.GameState.PLAYING)
            return;
        tilemapCenterPosition = tileGrid.WorldToCell(gameManager.player.transform.position);
        UpdateTilemap();
    }

    void UpdateTilemap()
    {
        for (int i = tilemapCenterPosition.x - 20; i < tilemapCenterPosition.x + 20; i++)
        {
            for (int j = tilemapCenterPosition.y - 10; j < tilemapCenterPosition.y + 10; j++)
            {
                if (tilemap.GetTile(new Vector3Int(i, j, 0)) == null)
                {
                    tilemap.SetTile(new Vector3Int(i, j, 0), groundTiles[Random.Range(0, groundTiles.Count)]);
                    if(Random.Range(0, 100) > (100-kwiatekRespawnChance))
                        kwiatkiTilemap.SetTile(new Vector3Int(i, j, 0), kwiatkiTiles[Random.Range(0, kwiatkiTiles.Count)]);
                }
            }
        }
    }
}
