/* Summary: Create corridors between rooms based on the L-system 
 *          algorithm.
 */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CorridorGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap dungeonTilemap;
    [SerializeField] private TileBase corridorTile;
    [SerializeField] private LSysAlgorithm LSysAlg;

    private Dictionary<Vector3Int, int> corridorTiles = new Dictionary<Vector3Int, int>();

    private Dictionary<RectInt, List<Vector3Int>> roomEntrances = new Dictionary<RectInt, List<Vector3Int>>();

    // To detect overlaps
    private int corrID = 0;

    // Creates a corridor between two rooms.
    public void GeneratorCorridor(RectInt startRoom, RectInt endRoom)
    {

        corrID++;

        // Generates the L-system instructions
        string ins = LSysAlg.GenerateStr("F", "F+F-F", 1);
      

        Vector2Int startPoint = GetRoomEdgePoint(startRoom, endRoom);
        Vector2Int endPoint = GetRoomEdgePoint(endRoom, startRoom);

        Debug.Log("Corridor from-  " +startPoint + "to- "+ endPoint);

        ExtractRoomEntance(endRoom, endPoint);

        // Use the L-system to create the corridor.
        DrawCorridor(ins, startPoint, endPoint);
    }
    void ExtractRoomEntance (RectInt room, Vector2Int endPoint)
    {
        if (!roomEntrances.ContainsKey(room))
        {
            roomEntrances[room] = new List<Vector3Int>();
        }

        if (endPoint.x == room.xMin || endPoint.x == room.xMax - 1)
        {
            roomEntrances[room].Add(
                new Vector3Int(endPoint.x, endPoint.y - 1, 0));

            roomEntrances[room].Add(
                new Vector3Int(endPoint.x, endPoint.y, 0));

            roomEntrances[room].Add(
                new Vector3Int(endPoint.x, endPoint.y + 1, 0));
        }
        else
        {
            roomEntrances[room].Add(
                new Vector3Int(endPoint.x -1 , endPoint.y, 0));

            roomEntrances[room].Add(
                new Vector3Int(endPoint.x, endPoint.y, 0));

            roomEntrances[room].Add(
                new Vector3Int(endPoint.x + 1, endPoint.y, 0));
        }
    }


    void DrawCorridor(string ins, Vector2Int start, Vector2Int end)
    {
        List<LSysAlgorithm.LSysStep> path = LSysAlg.GeneratePath(ins, start, end);
        foreach(LSysAlgorithm.LSysStep step in path)
        {
            DrawCorridorSection(step.pos, step.direction);

            if (step.directionChanged)
            {
                Vector2Int prevSideOffset;

                if(step.prevDirection == Vector2Int.up ||
                    step.prevDirection == Vector2Int.down)
                {
                    prevSideOffset = Vector2Int.right;
                }
                else
                {
                    prevSideOffset = Vector2Int.up;
                }
                Vector2Int cornerPos = step.pos + prevSideOffset;
                DrawTile(cornerPos);
            }

        }
    }
   
    void CheckForOverlap(Vector3Int tilePosition)
    {
        // Checks if the position is existing in the dictionary.
        if (corridorTiles.TryGetValue(tilePosition, out int prevCorridor))
        {
            // If so, a warning is logged to indicate so.
            if (prevCorridor != corrID)
            {
                Debug.LogWarning("Overlap: Corridor- " + corrID +
                    "overlaps " + prevCorridor + "on tile " + tilePosition);
            }
        }

        // Otherwise, the position gets added to the dictionary and is used.
        else
        {
            corridorTiles.Add(tilePosition, corrID);
        }
    }

    Vector2Int GetRoomEdgePoint (RectInt initialRoom, RectInt targetRoom)
    {
     
        Vector2Int initRoomCenter = new Vector2Int(
            Mathf.RoundToInt(initialRoom.center.x),
            Mathf.RoundToInt(initialRoom.center.y));

        Vector2Int targetRoomCenter = new Vector2Int(
            Mathf.RoundToInt(targetRoom.center.x),
            Mathf.RoundToInt(targetRoom.center.y));


        int diffX = targetRoomCenter.x - initRoomCenter.x;
        int diffY = targetRoomCenter.y - initRoomCenter.y;

        // If room is more towards the left/right
        if (Mathf.Abs(diffX) > Mathf.Abs(diffY))
        {

            int edgeX = diffX > 0
                ? initialRoom.xMax - 1
                : initialRoom.xMin;

           
            int edgeY = Mathf.RoundToInt(initialRoom.center.y);

            return new Vector2Int(edgeX, edgeY);
        }

        // If the room is more up/down
        else
        {
            // Use either the top or the bottom edge
            int edgeY = diffY > 0
                ? initialRoom.yMax - 1
                : edgeY = initialRoom.yMin;

            int edgeX = Mathf.RoundToInt(initialRoom.center.x);

            return new Vector2Int(edgeX, edgeY);
        }

    }

    void DrawTile(Vector2Int position)
    {
        Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
        CheckForOverlap(tilePosition);
        dungeonTilemap.SetTile(tilePosition, corridorTile);
    }

    void DrawCorridorSection(Vector2Int position, Vector2Int direction)
    {
        Vector2Int sideOffset;

        // Extra tiles are placed horizontally if the corridor is moving vertically and vice versa.
        if (direction == Vector2Int.up || direction == Vector2Int.down)
        {
            sideOffset = Vector2Int.right;
        }
        else
        {
            sideOffset = Vector2Int.up;
        }

        DrawTile(position- sideOffset);
        DrawTile(position);
        DrawTile(position + sideOffset);


    }

    // Extracts the tiles exactly where corridors enter a specific room.
    public List<Vector3Int> GetRoomEntrances(RectInt room)
    {
        if(roomEntrances.TryGetValue(room, out List <Vector3Int> entrances))
        {
            return entrances;
        }
        return new List<Vector3Int>();
    }

}
