using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crowd : MonoBehaviour
{
    // Reference to the agent prefab
    public Agent agentPrefab;

    // List of agents in the crowd
    List<Agent> crowd = new List<Agent>();

    // Starting number of agents in the crowd
    public int startingCount = 3;

    // Factor that determines the influence of neighboring agents on an agent's movement
    [Range(1f, 100f)]
    public float driveFactor = 10f;

    // Maximum speed of an agent
    [Range(1f, 100f)]
    public float maxSpeed = 5f;

    // Radius around an agent to consider as neighbors
    [Range(1f, 10f)]
    public float neighRadius = 1.5f;

    // Start method is called before the first frame update
    void Start()
    {
        // Get the ground object and its dimensions
        GameObject ground = GameObject.Find("Ground");
        Vector3 grounddim = ground.transform.localScale;
        Vector3 groundpos = ground.transform.position;
        float y = groundpos.y + grounddim.y / 2;

        // Create the initial crowd of agents
        while (crowd.Count < startingCount)
        {
            // Generate random spawn coordinates within the ground boundaries
            var x = Random.Range(groundpos.x - grounddim.x / 2, groundpos.x + grounddim.x / 2);
            var z = Random.Range(groundpos.z - grounddim.z / 2, groundpos.z + grounddim.z / 2);
            Vector3 spawnPos = new Vector3(x, y, z);

            // Instantiate an agent and add it to the crowd
            Agent agent = Instantiate(agentPrefab, spawnPos, Quaternion.identity);
            agent.name = "Agent-" + crowd.Count;
            crowd.Add(agent);
        }
    }

    // Update method is called once per frame
    void Update()
    {
        // If the left mouse button is clicked, try to move an agent to a queue
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < 3; i++) // Try 3 times to find an agent not already in a queue or moving to a queue
            {
                // Select a random agent from the crowd
                Agent agent = crowd[Random.Range(0, crowd.Count)];

                // Check if the agent is not already in a queue or moving to a queue
                if (agent.IsInQueue() == false && agent.MovingToQueue() == false)
                {
                    // Get the name of the queue to move the agent to
                    string q = agent.MoveToQueue();

                    // Log the agent's movement to the queue
                    Debug.Log("Moving agent " + agent.name + " to queue " + q);

                    // Break the loop since an agent has been moved to a queue
                    break;
                }
            }
        }
    }
}
