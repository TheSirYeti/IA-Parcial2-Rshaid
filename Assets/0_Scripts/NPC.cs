using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public float speed;
    
    private Node startNode;
    private Node goalNode;

    [Header("Patrol")]
    public List<Node> patrollingNodes = new List<Node>();
    private int currentNode;
    private int patrollingDireciton;
    public List<Node> originalPatrollingNodes;
    private bool amPatroling = true;

    [Header("Field of View")]
    public float viewRadius;
    public float viewAngle;
    public LayerMask detectableAgentMask;
    public LayerMask obstacleMask;

    private void Start()
    {
        originalPatrollingNodes = patrollingNodes;
    }

    private void Update()
    {
        FieldOfView();
        Patrol();
    }

    void Patrol()
    {
        if (amPatroling)
        {
            Vector3 direction = patrollingNodes[currentNode].transform.position - transform.position;
            transform.forward = direction;
            transform.position += transform.forward * speed * Time.fixedDeltaTime;
        
            if (direction.magnitude <= 0.1f)
            {
                currentNode++;

                if (currentNode == patrollingNodes.Count)
                {
                    currentNode = 0;
                }
            }
        }
    }
    
    public List<Node> ConstructPathAStar(Node startingNode, Node goalNode)
    {
        if (startingNode == null && goalNode == null) return default;

        PriorityQueue frontier = new PriorityQueue();
        frontier.Put(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, int> costSoFar = new Dictionary<Node, int>();
        cameFrom.Add(startingNode, null);
        costSoFar.Add(startingNode, 0);

        while (frontier.Count() > 0)
        {
            Node current = frontier.Get();

            if (current == goalNode)
            {
                List<Node> path = new List<Node>();
                Node nodeToAdd = current;
                while (nodeToAdd != null)
                {
                    path.Add(nodeToAdd);
                    nodeToAdd = cameFrom[nodeToAdd];
                }
                //path.Reverse();
                return path;
            }

            foreach (var next in current.GetNeighbors())
            {
                int newCost = costSoFar[current] + next.cost;
                //Lo unico que cambia es la priority del frontier que le sumamos la heuristica
                float priority = newCost + Heuristic(next.transform.position, goalNode.transform.position);
                if (!costSoFar.ContainsKey(next))
                {
                    frontier.Put(next, priority);
                    costSoFar.Add(next, newCost);
                    cameFrom.Add(next, current);
                }
                else if (costSoFar.ContainsKey(next) && newCost < costSoFar[next])
                {
                    frontier.Put(next, priority);
                    costSoFar[next] = newCost;
                    cameFrom[next] = current;
                }
            }
        }
        return default;
    }

    float Heuristic(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }
    
    void FieldOfView()
    {
        //ClearDetectableEnemies();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, detectableAgentMask);

        foreach (var item in targetsInViewRadius)
        {
            Vector3 dirToTarget = (item.transform.position - transform.position);
            
            if (Vector3.Angle(transform.forward, dirToTarget.normalized) < viewAngle / 2)
            {
                if (InSight(transform.position, item.transform.position))
                {
                    patrollingNodes = ConstructPathAStar(patrollingNodes[currentNode],
                        NpcManager.instance.nodes[NpcManager.instance.GetClosestNode(this.transform)]);
                    currentNode = 0;
                    //item.GetComponent<DetectableEnemy>().Detect(true);
                    //_detectedAgents.Add(item.GetComponent<DetectableEnemy>());
                    Debug.DrawLine(transform.position, item.transform.position, Color.red);
                }
                else
                {
                    Debug.DrawLine(transform.position, item.transform.position, Color.green);
                }
            }
        }
    }

    bool InSight(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;
        if (!Physics.Raycast(start, dir, dir.magnitude, obstacleMask)) return true;
        else return false;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 lineA = DirFromAngle(viewAngle / 2 + transform.eulerAngles.y);
        Vector3 lineB = DirFromAngle(-viewAngle / 2 + transform.eulerAngles.y);

        Gizmos.DrawLine(transform.position, transform.position + lineA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + lineB * viewRadius);

    }
    
    Vector3 DirFromAngle(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
