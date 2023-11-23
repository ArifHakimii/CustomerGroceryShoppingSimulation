using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentQueue : MonoBehaviour
{
    public QueueList queueList;

    List<Agent> agentqueue = new List<Agent>();

    //string counterId;
    Vector3 qdir;  // queue direction
    Vector3 qPos;
    Agent incomingAgent;
    float queueSpacing = 1;
  

    // Start is called before the first frame update
    void Start()
    {
        qdir = -transform.right; // the red vector
        qPos = transform.position + qdir*(agentqueue.Count+1)*queueSpacing;
        queueList.Add(this);

        Debug.Log("Queue created for "+gameObject.name+". With pos: "+qPos);

    }

    // Append a new agent coming into the queue and positioning the agent in line for the queue
    public bool Add(Agent agent) {
        if (incomingAgent != null) {
            return false;
        }
        incomingAgent = agent;
        qPos.y = agent.GetPos().y;
        agent.SetDestination(qPos);
        queueSpacing = agent.GetRadius()*2f;
        qPos += qdir*queueSpacing;
        return true;        
    }

    // Add space between agents in the queue
    public void Shift() {
        foreach (Agent agent in agentqueue) {
            agent.transform.position -= qdir*queueSpacing;
        }
        qPos -= qdir*queueSpacing; 
    }

    // Remove agent in position 0 from queue
    public Agent Pop() {
        Agent agent = agentqueue[0];
        agentqueue.RemoveAt(0); 
        agent.MoveFromQueue();

        agent.SetRandomDestination();
        Shift();
        return agent;  
    }

    // Size of queue
    public int Size() {
        return agentqueue.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if (incomingAgent == null)
            return;

        if (incomingAgent.IsInQueue()) {
            incomingAgent.TurnOffNavMeshAgent(qPos);
            agentqueue.Add(incomingAgent);
            incomingAgent = null;
            Debug.Log("Queue for "+gameObject.name+" now has size "+agentqueue.Count+". Next pos: "+qPos);
        }    
    }
}
