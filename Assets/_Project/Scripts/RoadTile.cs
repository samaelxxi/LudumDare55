using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadTile : MonoBehaviour
{
    public enum RoadDirection { Up, Right, Down, Left }
    public static Dictionary<RoadDirection, Vector2Int> Directions = new()
    {
        { RoadDirection.Up, new Vector2Int(0, 1) },
        { RoadDirection.Right, new Vector2Int(1, 0) },
        { RoadDirection.Down, new Vector2Int(0, -1) },
        { RoadDirection.Left, new Vector2Int(-1, 0) },
    };


    [field: SerializeField] public  bool IsSwitchable { get; private set; }
    [field: SerializeField] public RoadDirection Direction { get; private set; }
    [field: SerializeField] public bool IsEscapeTile { get; private set; }

    public Vector2Int Position { get; private set; }
    public Vector3Int DirectionVector => Directions[Direction].ToVector3Int();

    [SerializeField] float _speedModifier = 1.0f;
    public float SpeedModifier => _speedModifier;

    int _visited = 0;

    public void SetPosition(Vector2Int position)
    {
        Position = position;
    }

    public void VisitTile()
    {
        _visited++;
    }

    public void SwitchDirection(RoadDirection newDirection)
    {
        Direction = newDirection;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = IsSwitchable ? Color.green : Color.red;
        GizmosExtensions.DrawArrow(transform.position, Directions[Direction].ToVector3Int(), 2.0f, 1);
    }
}
