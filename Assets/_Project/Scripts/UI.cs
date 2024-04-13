using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Debug.Log(mousePos);
        // Debug.Log(Game.Instance.Grid.WorldToCell(mousePos));

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left mouse button clicked");
            RoadTile roadTile = Game.Instance.RoadManager.GetRoadTileAt(mousePos);
            if (roadTile != null)
            {
                Debug.Log($"Road tile at {mousePos} {roadTile.Position} is switchable: {roadTile.IsSwitchable} and direction: {roadTile.Direction}");
            }
            else
            {
                Debug.Log("No road tile at this position");
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right mouse button clicked");

            Game.Instance.RoadManager.SwitchRoadTileAt(mousePos);
        }




    }
}
