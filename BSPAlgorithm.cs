/* Summary: This is the core (BSP-based) algorithm that designs 
 *          the dungeon levels geenerated throughout the game
 */

using System.Collections.Generic;
using UnityEngine;
public class BSPAlgorithm : MonoBehaviour
{
    private int dungeonWidth = 40;
    private int dungeonHeight = 25;

    private int currentRoomCount = 1;
    [SerializeField] private int roomPadding = 2;
    [SerializeField] private int minRoomSize = 5;

    private RectInt dungeonArea;

    public List<RectInt> generatedRooms = new List<RectInt>();

    private BSPNode rootNode;
    public List<(RectInt, RectInt)> roomConnections = new List<(RectInt, RectInt)>();


    private class BSPNode
    {
        public RectInt rect;
        public RectInt room;
        public Vector2Int center;

        public BSPNode left;
        public BSPNode right;

        public BSPNode(RectInt rect)
        {
            this.rect = rect;
        }
    }

    private int MinPartitionSize()
    {
        return minRoomSize + roomPadding * 2;
    }

    private bool ConnectionExisting(BSPNode roomA, BSPNode roomB)
    {
        foreach (var connection in roomConnections)
        {
            if ((connection.Item1 == roomA.room && connection.Item2 == roomB.room) ||
                (connection.Item1 == roomB.room && connection.Item2 == roomA.room))
            {
                return true;
            }
        }

        return false;
    }

    void CreateDungeonSpace()
    {
        dungeonArea = new RectInt(0, 0,
            dungeonWidth, dungeonHeight);
    }

    void CreateRootNode()
    {
        rootNode = new BSPNode(dungeonArea);
        Debug.Log("Root node is created");
    }

    void SplitNode(BSPNode node, int numOfRooms)
    {
        bool splitH;

        if (currentRoomCount >= numOfRooms)
        {
            return;
        }

        // Determining whether the node is split horizontal
        if (node.rect.width > node.rect.height)
        {
            splitH = false;
        }
        else if (node.rect.height > node.rect.width)
        {
            splitH = true;
        }
        else
        {
            splitH = Random.value > 0.5f;
        }

        if (splitH)
        {
            if (node.rect.height < MinPartitionSize() * 2)
            {
                //Debug.LogWarning("Horizontal split cant be done");
                splitH = false;
            }
        }


        if (!splitH)
        {
            if (node.rect.width < MinPartitionSize() * 2)
            {
                splitH = true;
            }
        }

        //if no directions are possible
        if ((splitH && node.rect.height < MinPartitionSize() * 2) ||
            (!splitH && node.rect.width < MinPartitionSize() * 2))
        {
            Debug.LogWarning("Node cannot split: " + node.rect.width
                + " x " + node.rect.height);
            return;
        }

        // horizontally

        if (splitH)
        {
            int splitPos = Random.Range
                (MinPartitionSize(), node.rect.height - MinPartitionSize());

            RectInt bottomSection = new RectInt(
               node.rect.x,
               node.rect.y,
               node.rect.width,
               splitPos);

            RectInt topSection = new RectInt(
                node.rect.x,
                node.rect.y + splitPos,
                node.rect.width,
                node.rect.height - splitPos);

            node.left = new BSPNode(bottomSection);
            node.right = new BSPNode(topSection);
            Debug.Log("Root node is split horizontally");
        }

        else
        {
            // vertically
            int splitPos = Random.Range
                    (MinPartitionSize(), node.rect.width - MinPartitionSize());

            RectInt leftSection = new RectInt(
                node.rect.x,
                node.rect.y,
                splitPos,
                node.rect.height);

            RectInt rightSection = new RectInt(
                node.rect.x + splitPos,
                node.rect.y,
                node.rect.width - splitPos,
                node.rect.height);

            node.left = new BSPNode(leftSection);
            node.right = new BSPNode(rightSection);

            Debug.Log("Root node is split vertically");
        }

        currentRoomCount++;

        SplitNode(node.left, numOfRooms);
        SplitNode(node.right, numOfRooms);
    }

    void CreateRoom(BSPNode node)
    {
        int availWidth = node.rect.width - roomPadding * 2;
        int availHeight = node.rect.height - roomPadding * 2;

        if (availWidth < minRoomSize || availHeight < minRoomSize)
        {
            Debug.LogWarning("Partition too small for room");
            return;
        }

        int roomWidth = Random.Range(
            minRoomSize, availWidth + 1);

        int roomHeight = Random.Range(
            minRoomSize, availHeight + 1);

        int roomX = Random.Range(
            node.rect.x + roomPadding,
            node.rect.xMax - roomPadding - roomWidth + 1);

        int roomY = Random.Range(
            node.rect.y + roomPadding,
            node.rect.yMax - roomPadding - roomHeight + 1);

        node.room = new RectInt(
            roomX,
            roomY,
            roomWidth,
            roomHeight);

        generatedRooms.Add(node.room);

        node.center = new Vector2Int(
            Mathf.RoundToInt(node.room.center.x),
            Mathf.RoundToInt(node.room.center.y));

        Debug.Log("Room created at: " + node.room + "and the centre: " + node.center);
    }

    // Traverse through the tree and create rooms within each of.
    void GenRoomsFromTree(BSPNode node)
    {
        if (node.left == null && node.right == null)
        {
            CreateRoom(node);
            return;
        }

        if (node.left != null)
        {
            GenRoomsFromTree(node.left);
        }

        if (node.right != null)
        {
            GenRoomsFromTree(node.right);
        }
    }

    void ConnectRooms(BSPNode node)
    {
        if (node.left == null || node.right == null)
        {
            return;
        }

        ConnectRooms(node.left);
        ConnectRooms(node.right);
        FormConnection(node.left, node.right);
    }

    BSPNode GetRoomNode(BSPNode node)
    {
        if (node.left == null && node.right == null)
        {
            return node;
        }

        if (node.left != null && node.right != null)
        {
            if (Random.value < 0.5f)
            {
                return GetRoomNode(node.left);
            }
            else
            {
                return GetRoomNode(node.right);
            }
        }

        if (node.left != null)
        {
            return GetRoomNode(node.left);
        }

        return GetRoomNode(node.right);
    }

    // Connecting rooms with corridors
    void FormConnection(BSPNode leftNode, BSPNode rightNode)
    {
        BSPNode leftRoom = GetRoomwithLeastCorridors(leftNode);
        BSPNode rightRoom = GetRoomwithLeastCorridors(rightNode);

        if (leftRoom == null || rightRoom == null)
        {
            return;
        }

        Debug.Log(
            "Trying connection: " + leftRoom.center + "->" +
            rightRoom.center);


        if (ConnectionExisting(leftRoom, rightRoom))
        {
            Debug.Log(
                "Skipping dupl. connection: " + leftRoom.center + "->" +
                rightRoom.center);

            return;
        }

        Debug.Log(
            "Adding ok connection: " + leftRoom.center + "->" +
            rightRoom.center);

        roomConnections.Add((leftRoom.room, rightRoom.room));
    }

    private BSPNode GetRoomwithLeastCorridors(BSPNode node)
    {
        if (node.left == null && node.right == null)
        {
            return node;
        }

        BSPNode leftRoom = GetRoomNode(node.left);
        BSPNode rightRoom = GetRoomNode(node.right);

        int leftConns = GetRoomCorridorCount(leftRoom);
        int rightConns = GetRoomCorridorCount(rightRoom);

        return leftConns <= rightConns ? leftRoom : rightRoom;
    }

    private int GetRoomCorridorCount(BSPNode room)
    {
        int count = 0;

        foreach (var c in roomConnections)
        {
            if (c.Item1 == room.room || c.Item2 == room.room)
            {
                count++;
            }
        }
        return count;
    }

    public void GenerateLayout(int numofRooms, int width, int height)
    {
        dungeonWidth = width;
        dungeonHeight = height;

        generatedRooms.Clear();
        roomConnections.Clear();
        currentRoomCount = 1;

        CreateDungeonSpace();
        CreateRootNode();

        SplitNode(rootNode, numofRooms);
        GenRoomsFromTree(rootNode);
        ConnectRooms(rootNode);
    }

    //TODO; create a dungeon log to show the data 
}

