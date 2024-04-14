using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
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
    [SerializeField] SpriteRenderer _arrowSprite;
    [SerializeField] Animator _arrowAnimator;

    public Vector2Int Position { get; private set; }
    public Vector3Int DirectionVector => Directions[Direction].ToVector3Int();

    [SerializeField] float _speedModifier = 1.0f;
    public float SpeedModifier => _speedModifier;

    int _visited = 0;

    void Awake()
    {
        if (!IsSwitchable)
            _arrowSprite.enabled = false;
        else
            SetupArrow(Direction);
    }

    void OnValidate()
    {
        SetupArrow(Direction);
        _arrowSprite.enabled = true;
    }

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
        SetupArrow(newDirection);
    }

    void SetupArrow(RoadDirection newDirection)
    {
        string triggerName = "";
        RoadManager manager = Game.Instance.RoadManager;
        
        var leftNeigh = manager.GetNeighbour(this, RoadDirection.Left);
        // Debug.Log($"Left neighbour is {leftNeigh}");
        if (leftNeigh != null && leftNeigh.Direction == RoadDirection.Right)
            triggerName = "Left";
        if (triggerName == "")
        {
            var topNeigh = manager.GetNeighbour(this, RoadDirection.Up);
            // Debug.Log($"Top neighbour is {topNeigh}");
            if (topNeigh != null && topNeigh.Direction == RoadDirection.Down)
                triggerName = "Top";
        }
        if (triggerName == "")
        {
            var bottomNeigh = manager.GetNeighbour(this, RoadDirection.Down);
            // Debug.Log($"Bottom neighbour is {bottomNeigh}");
            if (bottomNeigh != null && bottomNeigh.Direction == RoadDirection.Up)
                triggerName = "Down";
        }
        if (triggerName == "")
            Debug.LogWarning("No trigger name found for arrow");
        string secondPart = newDirection.ToString();
        if (secondPart == "Up") secondPart = "Top";
        // Debug.Log($"Trigger name is {triggerName + secondPart}");
        _arrowAnimator.SetTrigger(triggerName + secondPart);

        if (!Application.isPlaying)
            SetAnimationFrame(triggerName + secondPart, 0, 0);
    }

    void OnDrawGizmos()
    {
        // check if playmoed
        if (Application.isPlaying)
        {
            Gizmos.color = IsSwitchable ? Color.green : Color.red;
            GizmosExtensions.DrawArrow(transform.position, Directions[Direction].ToVector3Int(), 2.0f, 1);
        }
    }

    private void SetAnimationFrame(string animationName, int layer, float normalizedAnimationTime)
    {
        if (_arrowAnimator != null) 
        {
            _arrowAnimator.speed = 0f;
            _arrowAnimator.Play(animationName, layer, 0);
            _arrowAnimator.Update(Time.deltaTime);
        }
    }
}
