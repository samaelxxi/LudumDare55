using System.Collections;
using System.Collections.Generic;
using DesignPatterns.Singleton;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Game : Singleton<Game>
{
    [field: SerializeField] public Grid Grid { get; private set; }
    [field: SerializeField] public Tilemap Roads { get; private set; }
    [field: SerializeField] public RoadManager RoadManager { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
