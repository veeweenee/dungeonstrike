/* Summary: Handles the enemy AI behaviour when going towards the
 *          the player, trigggering attacks and the retreat movement
 *          of the worm boss.
 */

using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private DungeonGenerator dungeonGenerator; 
    [SerializeField] private Transform player;

    // Common and patroller based enemy prefabs. 
    [SerializeField] private GameObject ratEnemy;
    [SerializeField] private GameObject spiderEnemy;
    [SerializeField] private GameObject skeletonEnemy;

    // Level-boss prefabs 
    public GameObject levelBoss;
    [SerializeField] public GameObject viperBoss;
    [SerializeField] public GameObject mutantRatBoss;
    [SerializeField] public GameObject wormBoss;

    public int enemyCount = 10;
    public float safeSpawnDis = 4f;
    
    public RectInt bossRoom;
    public void SpawnEnemy()
    {
        List<RectInt> enemyRooms = new List<RectInt>();

        foreach(RectInt room in dungeonGenerator.BSPAlg.generatedRooms)
        {
            
            if (room != bossRoom)
            {
                enemyRooms.Add(room);
            }
        }
        // Include an enemy in each non-boss room
        foreach (RectInt room in enemyRooms)
        {
            Vector3 spawnPos = CreateValidEnemyPos(room);
            SpawnEnemyAtPos(spawnPos);
        }

        int enemiesRemaining = enemyCount - enemyRooms.Count;

        for(int i = 0; i < enemiesRemaining; i++)
        {
            int roomIndex = Random.Range(0, enemyRooms.Count);
            Vector3 spawnPos = CreateValidEnemyPos(enemyRooms[roomIndex]);
            SpawnEnemyAtPos(spawnPos);
        }

    }

    Vector3 CreateValidEnemyPos(RectInt room)
    {
        Vector3 spawnPos;
        int attempts = 0;
        int maxAttempts = 20;

        do
        {
            int x = Random.Range(room.xMin + 1, room.xMax - 1);
            int y = Random.Range(room.yMin + 1, room.yMax - 1);

            spawnPos = new Vector3(x, y, 0f);
            attempts++;

        } while (Vector3.Distance(spawnPos, player.position)<safeSpawnDis & attempts< maxAttempts);
        return spawnPos;
    }
    
   
    void SpawnEnemyAtPos(Vector3 spawnPos)
    {
        GameObject enemyChosen;
        float randomEnemy = Random.value;

        if (randomEnemy < 0.33f)
        {
            enemyChosen = ratEnemy;
        }
        else if (randomEnemy < 0.66f)
        {
            enemyChosen = spiderEnemy;
        }
        else
        {
            enemyChosen = skeletonEnemy;
        }

        GameObject spawnedEnemy = Instantiate(
            enemyChosen, spawnPos, Quaternion.identity);

        EnemyController enemy = spawnedEnemy.GetComponent<EnemyController>();
        enemy.player = player;

        Debug.Log("Contact attack to enemy: " + enemy.name);

    }

    public void SpawnBoss()
    {
        bossRoom = SetBossRoom();

        int x = bossRoom.xMin + bossRoom.width / 2;
        int y = bossRoom.yMin + bossRoom.height / 2;

        Vector3 bossSpawnPos = new Vector3(x, y, 0f);
        GameObject bossSpawned = Instantiate(levelBoss, bossSpawnPos, Quaternion.identity);

        EnemyController boss = bossSpawned.GetComponent<EnemyController>();

        if(boss != null)
        {
            boss.player = player;
        }

        Debug.Log("Boss room selected at: " + bossRoom);
        Debug.Log("Boss spawned at: " + bossSpawnPos);

    }
    // TODO: Move this implementation into DungeonGenerator
    private RectInt SetBossRoom()
    {
        int lastRoomIndex = dungeonGenerator.BSPAlg.generatedRooms.Count - 1;
        RectInt levelBossRoom = dungeonGenerator.BSPAlg.generatedRooms[lastRoomIndex];
        return levelBossRoom;

    }

}