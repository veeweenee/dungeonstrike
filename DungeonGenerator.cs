/* Summary: Creates the dungeon area using the BSP algorithm as a
 *          basis to create the rooms, corridors, enemies, etc.        
 */
using UnityEngine;
using UnityEngine.Tilemaps;
public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] public BSPAlgorithm BSPAlg;

    [SerializeField] private Tilemap dungeonTilemap;
    [SerializeField] private TileBase floorTile;

    [SerializeField] private CorridorGenerator corridorGenerator;
    [SerializeField] private Transform player;
    //public TileBase horzWallTile;
    //public TileBase vertWallTile;


    public void GenerateDungeon(int roomCount, int width, int height)
    {
        ClearEnemies();
        dungeonTilemap.ClearAllTiles();
        BSPAlg.GenerateLayout(roomCount, width, height);

        GenerateCorridors();
        DrawDungeon();

        EnemySpawner enemySpawner = FindFirstObjectByType<EnemySpawner>();
        enemySpawner.SpawnBoss();
        PlacePlayerInDungeon();
        enemySpawner.SpawnEnemy();
    }

    void DrawDungeon()
    {
        if (BSPAlg == null)
        {
            return;
        }

        foreach(RectInt room in BSPAlg.generatedRooms)
        {
            DrawFloor(room);
        }
        Debug.Log("Dungeon is drawn.");
    }

    void DrawFloor(RectInt rect)
    {
        for (int x = rect.xMin; x < rect.xMax; x++)
        {
            for (int y = rect.yMin; y < rect.yMax; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                dungeonTilemap.SetTile(position, floorTile);
            }
        }
    }

    void PlacePlayerInDungeon()
    {
        if (BSPAlg.generatedRooms.Count == 0)
        {
            return;
        }

        RectInt playerRoom = BSPAlg.generatedRooms[0];

        Vector3 spawnPos = new Vector3(playerRoom.center.x,
           playerRoom.center.y, 0f);
        player.position = spawnPos;

    }

    void GenerateCorridors()
    {
        foreach (var conn in BSPAlg.roomConnections)
        {
            corridorGenerator.GeneratorCorridor(conn.Item1, conn.Item2);
        }
    }

    void ClearEnemies()
    {
        EnemyController[] enemies = 
            FindObjectsByType<EnemyController>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach(EnemyController e in enemies)
        {
            Destroy(e.gameObject);
        }
    }
}
    
 