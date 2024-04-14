using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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

    string _currentArrowDirection;
    int _visited = 0;

    void Awake()
    {
        if (!IsSwitchable && Application.isPlaying)
            _arrowSprite.enabled = false;
        // else
        //     SetupArrow(Direction);
    }

    void OnValidate()
    {
        if (Application.isPlaying)
            return;
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

    public void SetupArrow(RoadDirection newDirection)
    {
        // Debug.Log($"Setting up arrow for {Position} | {Direction} -> {newDirection}");
        string triggerName = "";

        RoadManager manager;
        if (Application.isPlaying)
            manager = Game.Instance.RoadManager;
        else
            manager = FindObjectOfType<RoadManager>();

        var leftNeigh = manager.GetNeighbour(this, RoadDirection.Left);
        if (leftNeigh != null && (leftNeigh.Direction == RoadDirection.Right || leftNeigh.IsSwitchable))
            triggerName = "Left";
        if (triggerName == "")
        {
            var topNeigh = manager.GetNeighbour(this, RoadDirection.Up);
            if (topNeigh != null && (topNeigh.Direction == RoadDirection.Down || topNeigh.IsSwitchable))
                triggerName = "Top";
        }
        if (triggerName == "")
        {
            var bottomNeigh = manager.GetNeighbour(this, RoadDirection.Down);
            if (bottomNeigh != null && (bottomNeigh.Direction == RoadDirection.Up || bottomNeigh.IsSwitchable))
                triggerName = "Down";
        }
        string secondPart = newDirection.ToString();
        if (secondPart == "Up") secondPart = "Top";
        if (triggerName == "" || secondPart == "")
            Debug.LogWarning("No trigger name found for arrow" + triggerName + "|" + secondPart);
        else
            _arrowAnimator.SetTrigger(triggerName + secondPart);
        _currentArrowDirection = triggerName + secondPart;

        if (!Application.isPlaying)
            SetAnimationFrame(triggerName + secondPart, 0, 0);
    }

    void OnDrawGizmos()
    {
        // check if playmod
        if (Application.isPlaying)
        {
            Gizmos.color = IsSwitchable ? Color.green : Color.red;
            GizmosExtensions.DrawArrow(transform.position, Directions[Direction].ToVector3Int(), 0.5f, 0.3f);
        }
        else
        {
            if (IsSwitchable)
                Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }

    private void SetAnimationFrame(string animationName, int layer, float normalizedAnimationTime)
    {
        // Debug.Log($"Setting animation frame for {animationName} | pos: {Position} | dir: {Direction}");
        if (_arrowAnimator != null) 
        {
            if (!_arrowAnimator.runtimeAnimatorController.animationClips.Any((a) => a.name == animationName))
            {
                Debug.LogError($"No animation clip found for {animationName} | pos: {Position} | dir: {Direction}");
                return;
            }
            var animationClip = _arrowAnimator.runtimeAnimatorController.animationClips.First(clip => clip.name == animationName);

            var bindings = AnimationUtility.GetObjectReferenceCurveBindings(animationClip).First();
            var keyframes = AnimationUtility.GetObjectReferenceCurve(animationClip, bindings);
            var spriteKey = keyframes.First().value as Sprite;
            _arrowSprite.sprite = spriteKey;
        }
    }
}
