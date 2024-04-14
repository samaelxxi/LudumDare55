using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


public class RoadManager : MonoBehaviour
{
    [SerializeField] Grid _grid;

    [SerializeField] List<RoadTile> _roadTilesRaw = new();

    Dictionary<Vector2Int, RoadTile> _roadTiles = new();

    void Awake()
    {
        Debug.Log("Awake");
        CollectArrows();
    }

    [NaughtyAttributes.Button]
    public void UpdateRoadTiles()
    {
        CollectArrows();
    }

    void OnValidate()
    {
        // CollectArrows();
    }


    void CollectArrows()
    {
        _roadTiles.Clear();
        _roadTilesRaw.Clear();
        _roadTilesRaw.AddRange(transform.GetComponentsInChildren<RoadTile>());
        // Debug.Log($"Found {_roadTilesRaw.Count} road tiles");
        foreach (var tile in transform.GetComponentsInChildren<RoadTile>())
        {
            if (tile.gameObject.activeInHierarchy == false)
                continue;
            var tilePos = tile.transform.position;
            var gridPos = _grid.WorldToCell(tilePos).ToVector2Int();
            // Debug.Log($"Tile at {gridPos} ");
            _roadTiles[gridPos] = tile;
            tile.SetPosition(gridPos);
            // var goodPos = _grid.CellToWorld(gridPos.ToVector3Int());
            // tile.transform.position = goodPos;  // snap to grid
        }

        foreach (var tile in _roadTilesRaw)
            tile.SetupArrow(tile.Direction);
    }

    public RoadTile GetRoadTileAt(Vector3 position)
    {
        var gridPos = _grid.WorldToCell(position).ToVector2Int();
        // Debug.Log($"Grid position is {gridPos} | {_roadTiles.ContainsKey(gridPos)}");
        if (_roadTiles.ContainsKey(gridPos))
            return _roadTiles[gridPos];

        return null;
    }

    public void TrySwitchRoadTileAt(Vector3 position)
    {
        var roadTile = GetRoadTileAt(position);
        if (roadTile != null && roadTile.IsSwitchable)
        {
            for (int i = 1; i < 4; i++)
            {
                var newDirection = (RoadTile.RoadDirection) (((int)roadTile.Direction + i) % 4);
                if (newDirection == RoadTile.RoadDirection.Left)
                    continue;
                var neighbourPos = roadTile.Position + RoadTile.Directions[newDirection];
                var nextTile = GetNeighbour(roadTile, newDirection);
                if (nextTile == null)
                    continue;
                if (GetNeighbour(nextTile, nextTile.Direction) == roadTile)
                    continue;
                if (_roadTiles.ContainsKey(neighbourPos))
                {
                    // Debug.Log($"Switching to {newDirection}");
                    roadTile.SwitchDirection(newDirection);
                    break;
                }
            }
        }
    }

    public RoadTile GetNeighbour(RoadTile roadTile, RoadTile.RoadDirection direction)
    {
        var neighbourPos = roadTile.Position + RoadTile.Directions[direction];
        if (_roadTiles.ContainsKey(neighbourPos))
            return _roadTiles[neighbourPos];

        return null;
    }

    public RoadTile GetNeighbour(Vector2 position, RoadTile.RoadDirection direction)
    {
        var intPos = _grid.WorldToCell(position).ToVector2Int();
        var neighbourPos = intPos + RoadTile.Directions[direction];
        if (_roadTiles.ContainsKey(neighbourPos))
            return _roadTiles[neighbourPos];

        return null;
    }

    public List<RoadTile> GetPathToEscape(RoadTile startTile)
    {
        List<RoadTile> path = new() { startTile };

        // BFS
        Queue<RoadTile> queue = new();
        Dictionary<RoadTile, bool> visited = new();
        Dictionary<RoadTile, RoadTile> previousTile = new();
        visited[startTile] = true;
        queue.Enqueue(startTile);

        while (queue.Count > 0)
        {
            var currentTile = queue.Dequeue();
            if (currentTile.IsEscapeTile)
            {
                // Trace back the path
                var pathTile = currentTile;
                while (pathTile != startTile)
                {
                    path.Add(pathTile);
                    pathTile = previousTile[pathTile];
                }
                path.Add(startTile); // Add the start tile to the path
                path.Reverse(); // Reverse the path to start from the start tile
                break;
            }

            foreach (var direction in RoadTile.Directions.Values)
            {
                var neighbourPos = currentTile.Position + direction;
                if (_roadTiles.ContainsKey(neighbourPos))
                {
                    var neighbourTile = _roadTiles[neighbourPos];
                    if (!visited.ContainsKey(neighbourTile))
                    {
                        visited[neighbourTile] = true;
                        queue.Enqueue(neighbourTile);
                        previousTile[neighbourTile] = currentTile;
                    }
                }
            }
        }

        return path;
    }
}

