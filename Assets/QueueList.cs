using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Behavior/QueueList")]
public class QueueList : ScriptableObject
{
    // A list to store all the active agent queues in the game
    [System.NonSerialized]
    List<AgentQueue> queues = new List<AgentQueue>();

    // Get the agent queue at the specified index from the list
    public AgentQueue Get(int i)
    {
        // Return the agent queue at the specified index
        return queues[i];
    }

    // Get the total number of agent queues in the list
    public int Count()
    {
        // Return the number of elements in the queue list
        return queues.Count;
    }

    // Add a new agent queue to the list
    public void Add(AgentQueue queue)
    {
        // Add the specified agent queue to the list
        queues.Add(queue);
    }
}
