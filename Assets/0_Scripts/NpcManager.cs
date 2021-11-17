using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcManager : MonoBehaviour
{
    public List<NPC> npcs;
    public List<Node> nodes;

    public static NpcManager instance;

    private void Awake()
    {
        instance = this;
    }

    public int GetClosestNode(Transform t)
    {
        int id = -1;
        float minDistance = Mathf.Infinity;
        for(int i = 0; i < nodes.Count; i++)
        {
            float dist = Vector3.Distance(t.position, nodes[i].transform.position);
            if (dist < minDistance)
            {
                id = i;
                minDistance = dist;
            }
        }
        return id;
    }

    public void NotifyNPCs(Node node, string status)
    {
        foreach (NPC npc in npcs)
        {
            if (!npc.isFollowing)
            {
                npc.currentNode = 0;
                npc.patrollingNodes = new List<Node>();
                npc.patrollingNodes = npc.ConstructPathAStar(nodes[GetClosestNode(npc.transform)], node);
            }

            if (status == "Follow")
            {
                npc.isFollowing = true;
                //npc.isPatroling = false;
                npc.isReturning = false;
            }
        
            if (status == "Return")
            {
                npc.isFollowing = false;
                npc.isPatroling = false;
                npc.isReturning = true;
            }
        
            if (status == "Patrol")
            {
                npc.isFollowing = false;
                npc.isPatroling = true;
                npc.isReturning = false;
            }
        }
    }
}