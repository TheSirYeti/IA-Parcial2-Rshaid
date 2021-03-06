using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Node : MonoBehaviour
{
    public float viewRadius;
    public List<Node> _neighbors = new List<Node>();

    public int cost = 1;
    public bool blocked;

    public void Initialize(Grid g, Vector2Int pos)
    {
        ChangeCost(1);
    }

    public List<Node> GetNeighbors()
    {
        return _neighbors;
    }

    /* private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameManager.instance.SetStartingNode(this);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            GameManager.instance.SetGoalNode(this);
        }
        else if (Input.GetMouseButtonDown(2)) SetBlocked(!blocked);

        if (Input.GetKey(KeyCode.UpArrow)) ChangeCost(cost + 1);
        if (Input.GetKey(KeyCode.DownArrow)) ChangeCost(cost - 1);
        if (Input.GetKey(KeyCode.R)) ChangeCost(1);
    }*/


    void ChangeCost(int c)
    {
        if (c < 1) c = 1;
        cost = c;
    }

    void SetBlocked(bool b)
    {
        blocked = b;
        Color color = b ? Color.black : Color.white;
        GameManager.instance.ChangeGameObjectColor(gameObject, color);
    }


}
