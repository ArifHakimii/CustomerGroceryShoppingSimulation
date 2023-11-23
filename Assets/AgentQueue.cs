using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stall : MonoBehaviour
{
    // Variable to store the attractiveness of the stall
    private float attractiveness;

    // Start method is called before the first frame update
    void Start()
    {
        // Generate a random attractiveness value between 0 and 100
        attractiveness = Random.Range(0, 100);
    }

    // Update method is called once per frame, currently empty
    void Update()
    {
        // This method is currently empty, but it can be used to implement behavior
        // related to the stall's attractiveness, such as attracting customers or
        // adjusting the attractiveness based on customer interactions.
    }
}

public class AgentQueue : MonoBehaviour
{
    // Reference to the queue list manager
    public QueueList queueList;

    // List of agents currently in the queue
    List<Agent> agentqueue = new List<Agent>();

    // Vector3 representing the direction of the queue
    Vector3 qdir;

    // Vector3 representing the position of the queue
    Vector3 qPos;

    // Reference to the agent currently entering the queue
    Agent incomingAgent;

    // Spacing between agents in the queue
    float queueSpacing = 1;

    // Start method is called before the first frame update
    void Start()
    {
        // Set the queue direction to the negative right vector (red vector)
        qdir = -transform.right;

        // Calculate the initial position of the queue based on the queue direction and spacing
        qPos = transform.position + qdir * (agentqueue.Count + 1) * queueSpacing;

        // Add this queue to the queue list manager
        queueList.Add(this);

        // Log the creation of the queue
        Debug.Log("Queue created for " + gameObject.name + ". With pos: " + qPos);
    }

    // Add a new agent to the queue and position it accordingly
    public bool Add(Agent agent)
    {
        // Check if an agent is already in the process of entering the queue
        if (incomingAgent != null)
        {
            return false; // Fail to add the agent
        }

        // Set the incoming agent to the specified agent
        incomingAgent = agent;

        // Set the y-coordinate of the queue position to match the agent's y-coordinate
        qPos.y = agent.GetPos().y;

        // Set the agent's destination to the queue position
        agent.SetDestination(qPos);

        // Adjust the queue spacing based on the agent's radius
        queueSpacing = agent.GetRadius() * 2f;

        // Update the queue position to accommodate the new agent
        qPos += qdir * queueSpacing;

        // Successfully added the agent to the queue
        return true;
    }

    // Shift the positions of all agents in the queue to maintain spacing
    public void Shift()
    {
        // Iterate through each agent in the queue
        foreach (Agent agent in agentqueue)
        {
            // Move the agent's position back by the queue spacing
            agent.transform.position -= qdir * queueSpacing;
        }

        // Update the queue position to reflect the shift
        qPos -= qdir * queueSpacing;
    }

    // Remove the first agent from the queue
    public Agent Pop()
    {
        // Get the first agent in the queue
        Agent agent = agentqueue[0];

        // Remove the agent from the list
        agentqueue.RemoveAt(0);

        // Notify the agent that it is leaving the queue
        agent.MoveFromQueue();

        // Set the agent's destination to a random location
        agent.SetRandomDestination();

        // Shift the positions of the remaining agents in the queue
        Shift();

        // Return the removed agent
        return agent;
    }

    // Get the size of the queue (number of agents in the queue)
    public int Size()
    {
        return agentqueue.Count;
    }

    // Update method is called once per frame
    void Update()
    {
        // Check if an agent is currently in the process of entering the queue
        if (incomingAgent == null)
        {
            return; // No action required
        }

        // Check if the incoming agent has reached its destination
        if (incomingAgent.IsInQueue())
        {
            // Disable the incoming agent's navigation mesh agent
            incomingAgent.TurnOffNavMeshAgent(qPos);

            // Add the incoming agent to the queue
            agentqueue.Add(incomingAgent);

            // Reset the reference to the incoming agent
            incomingAgent = null;

            // Log the updated queue size and the next queue position
            Debug.Log("Queue for " + gameObject.name + " now has size " + agentqueue.Count + ". Next pos: " + qPos);
        }
    }
}
