using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheEnd : MonoBehaviour
{
    [SerializeField] BoxCollider2D _piggiesHomeCollider;
    [SerializeField] PeacefulPiggy _piggyPrefab;


    // Start is called before the first frame update
    void Start()
    {
        CreatePeacefulPiggies();
    }

    void CreatePeacefulPiggies()
    {
        foreach (var piggyData in Game.Instance.PlayerPiggies())
        {
            // Debug.Log($"Creating piggy {piggyData.Name}");
            var piggy = CreateNewPiggy();
            piggy.SetData(piggyData);
            piggy.SetWalkArea(_piggiesHomeCollider);
            piggy.SetToEnd();
        }
    }

    PeacefulPiggy CreateNewPiggy()
    {
        Vector3 position = default;
        int i = 0;
        while (i < 100)
        {
        position = new Vector3(Random.Range(_piggiesHomeCollider.bounds.min.x, _piggiesHomeCollider.bounds.max.x),
                                Random.Range(_piggiesHomeCollider.bounds.min.y, _piggiesHomeCollider.bounds.max.y), 0);
            if (Physics2D.OverlapCircle(position, 0.5f, LayerMask.GetMask("Piggy")) == null) break;
            i++;
        }
        var piggy = Instantiate(_piggyPrefab, position, Quaternion.identity);
        return piggy;
    }
}
