/* Summary: Deals with the generation of each of the three levels
 *          including the regular enemies being generated and the 
 *          type of boss assigned to a certain level.
 */

using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private DungeonGenerator dungeonGenerator;
    [SerializeField] private EnemySpawner enemySpawner;

    private GameManager gameManager;

    private int[] dungWidths = { 50, 60, 70 };
    private int[] dungHeights = { 30, 35, 40 };

    private int[] roomsPerLvl = { 8, 12, 16 };
    private int[] enemiesPerLvl = { 10, 13, 16 };
    public int CurrLevel => gameManager.currLevel;

    private void Start()
    {
        gameManager = GameManager.instance;
        GenerateLevel();
    }

    // TODO: Implement a specific ruletile per each level
    public void GenerateLevel()
    {
        int roomCount = roomsPerLvl[gameManager.currLevel - 1];
        int width = dungWidths[gameManager.currLevel - 1];
        int height = dungHeights[gameManager.currLevel - 1];
        int enemyCount = enemiesPerLvl[gameManager.currLevel - 1];

        enemySpawner.enemyCount = enemyCount;

        switch (gameManager.currLevel)
        {
            case 1:
                enemySpawner.levelBoss = enemySpawner.viperBoss;
                break;
            case 2:
                enemySpawner.levelBoss = enemySpawner.mutantRatBoss;
                break;
            case 3:
                enemySpawner.levelBoss = enemySpawner.wormBoss;
                break;
        }

        dungeonGenerator.GenerateDungeon(roomCount, width, height);

    }

   
}
