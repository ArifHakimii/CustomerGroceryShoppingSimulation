using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;


public class Agent : MonoBehaviour
{
    static string[] possibleTags = {"Group-1", "Group-2"};


    NavMeshAgent navagent;
    public QueueList queueList;

    System.Random rnd = new System.Random();

    DateTime motionT;
    Vector3 lastPosition;

    DateTime mouseT;
    const float mouseTime = 5;
    AgentState currState;
    int destinationStall;
    int destinationTarget;

    const float neighRadius = 10;
    GameObject[] stalls; 
    GameObject[] targets;

    Collider agentCollider;
    public Collider AgentCollider {get { return agentCollider; } }

    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<CapsuleCollider>();
        navagent = GetComponent<NavMeshAgent>();

        int tagIndex = UnityEngine.Random.Range(0, possibleTags.Length);
        this.tag = possibleTags[tagIndex];
        Debug.Log("Created a "+this.tag+" agent");
        if (tagIndex == 0) {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else {
            GetComponent<Renderer>().material.color = Color.blue;
        }

        // agent chooses to go to a random stall
        stalls = GameObject.FindGameObjectsWithTag("Stall");
        int stall = GetRandomStall();
        GotoStall(stall);
        currState = AgentState.Wandering;
    }

    // Update is called once per frame
    void Update()
    {
        // agent is in queue
        if (currState == AgentState.InQueue){
            return;
        }
           
        // agent is going to queue at the counter
        if (currState == AgentState.ToQueue) {
            
            // Once agent reaches the destination(counter), the agent state changes to InQueue
            if (ReachedDestination()) {
                currState = AgentState.InQueue;
            }

            return;
        }

        // agent is going to a target and will exit the grocery store
        if (currState == AgentState.ToTarget){
            // agent chooses a random target to go to which is located outside the store.
            // in this simulation, the target is the parked cars outside the store.
            // this state will happen after agent is done queueing at the counter and going through payment process.
            targets = GameObject.FindGameObjectsWithTag("Target");
            int target = GetRandomTarget();
            GotoTarget(target);
        }

        // agent will wander around the store area
        if (currState == AgentState.FollowingMouse) {
            if (ReachedDestination()) {
                currState = AgentState.Wandering;
            }

            DateTime mouseTNow = DateTime.Now;
            var diffTime = mouseTNow - mouseT;
            if (diffTime.Seconds > mouseTime)
                currState = AgentState.Wandering;
            return;
        }

        if (Input.GetMouseButtonDown(1)) {
                if (rnd.Next(2) == 0)
                    return;
                
                Ray movePosition = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(movePosition, out var hitInfo)) {
                    SetDestination(hitInfo.point);
                    currState = AgentState.FollowingMouse; 
                    mouseT = DateTime.Now;       
                }
            } 
        else {
            if (ReachedDestination(10)) {
                if (this.name == "Agent-19")
                    Debug.Log("Agent "+this.name +" is moving elsewhere");


                bool joinQ = UnityEngine.Random.value < 0.5;
                if (joinQ) {
                    joinQ = MoveToQueue(destinationStall);
                }

                if (!joinQ ) {
                    destinationStall = GetRandomStall();
                    GotoStall(destinationStall);
                    }
                }
            }

    }

    int GetStall() {
        return destinationStall;
    }
    int GetTarget() {
        return destinationTarget;
    }
    int GetRandomStall() {
        
        // sum up attractiveness for stalls
        // calculate the attractiveness ratio for each
        // get a random number 
        // use that number to choose a stall
        int index = UnityEngine.Random.Range(0, stalls.Length);        
        return index;
    }
    int GetRandomTarget() {
        
        // sum up attractiveness for target
        // calculate the attractiveness ratio for each
        // get a random number 
        // use that number to choose a target
        int index = UnityEngine.Random.Range(0, targets.Length);        
        return index;
    }

    // agent will go to stall
    void GotoStall(int index) {
        SetDestination(RandomStallPos(index));
    }

    // agent will go to target
    void GotoTarget(int index) {
        SetDestination(RandomTargetPos(index));
    }


    int CheckGangDestination() {
        List<Agent> agents = GetNearbyAgents(true);
        Vector3 center = Vector3.zero;
        if (agents.Count == 0) {
            return GetStall();
        }        
        
        int numStalls = stalls.Length;
        int[] vote = new int[numStalls];

        foreach (Agent agent in agents) {
            if (agent.IsInQueue() || agent.MovingToQueue() || agent.MovingToMouse())
                continue;
            vote[agent.GetStall()] ++;
         }

        int max = 0;
        int index = 0;
        for (int i=0; i<numStalls; i++) {
            if (vote[i] > max) {
                max = vote[i];
                index = i;
            }
        }

        return index;
    }


    List<Agent> GetNearbyAgents(bool aliketype) {
        Collider[] hitColliders = new Collider[20];
        float radius = neighRadius;
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders);

        List<Agent> context = new List<Agent>();
        for (int i=0; i<numColliders; i++)  {
            Collider c = hitColliders[i];
            Agent agent = c.GetComponent<Agent>();
            if (agent == null)
                continue;

            if (agent.CompareTag(this.tag) != aliketype)
                    continue;
                    
            if (agent.name == this.name) 
                continue;
            
            context.Add(agent);
        }

        return context;
    }

    // After wandering the store, agent will go to queue at a counter
    public bool MovingToQueue() {
        return currState == AgentState.ToQueue;
    }

    // Agent has arrived at a counter and state changes to InQueue
    public bool IsInQueue() {
        return currState == AgentState.InQueue;
    }

    // Agent will move to a target located outside the store, this happens after agent is done queuing
    public bool MoveToTarget() {
        return currState == AgentState.ToTarget;
    }

    // Agent state is following mouse
    public bool MovingToMouse() {
        return currState == AgentState.FollowingMouse;
    }

    // Agent choose which queue to enter
    public bool MoveToQueue(int qindex)
    {
        AgentQueue chosenQ = queueList.Get(qindex);
        bool success = chosenQ.Add(this);
        if (!success)
            return false;        
        currState = AgentState.ToQueue;
        return true;
    }
    
    // Agent is added to queue list of the counter
    public string MoveToQueue()
    {
        AgentQueue chosenQ = queueList.Get(rnd.Next(queueList.Count()));
        bool success = chosenQ.Add(this);
        if (!success)
            return "None";
        currState = AgentState.ToQueue;
        return chosenQ.name;
    }

    // What happens after agent is done queueing at counter
    public void MoveFromQueue()
    {
        //After done queueing, and doing payment at the counter. Agent will exit the store and to the target(Car)
        currState = AgentState.ToTarget;
        TurnOnNavMeshAgent();
    }

    public void SetDirection(Vector3 dir) {
        navagent.Move(dir);
    }

    public void SetDestination(Vector3 pos) {
        navagent.SetDestination(pos);
        motionT = DateTime.Now;
    }

    public float GetTimeInMotion() {
        return (DateTime.Now - motionT).Seconds;
    }

    public bool ReachedDestination() {
      return (navagent.remainingDistance <= navagent.stoppingDistance);
    }

    public bool ReachedDestination(float maxsec) {
      if (navagent.remainingDistance <= navagent.stoppingDistance) {
        return true;
      }

      if (GetTimeInMotion() > maxsec) {
        return true;
      }

      return false;
    }

    // Determining a random stall position for agent to go to
    public Vector3 RandomStallPos(int index) {
        GameObject stall = stalls[index];
        Vector3 stalldim = stall.transform.localScale;
        Vector3 stallpos = stall.transform.position;
        float y = transform.position.y;

        float sign = UnityEngine.Random.value < 0.5 ? -1 : 1;

        var x = stallpos.x + sign*UnityEngine.Random.Range(stalldim.x, 2*stalldim.x);
        var z = stallpos.z + sign*UnityEngine.Random.Range(stalldim.z, 2*stalldim.z);
        Vector3 randPos = new Vector3(x, y, z);

        return randPos;
    }


    // Determining a random target position for agent to go to
    public Vector3 RandomTargetPos(int index) {
        GameObject target = targets[index];
        Vector3 targetdim = target.transform.localScale;
        Vector3 targetpos = target.transform.position;
        float y = transform.position.y;

        float sign = UnityEngine.Random.value < 0.5 ? -1 : 1;

        var x = targetpos.x + sign*UnityEngine.Random.Range(targetdim.x, 2*targetdim.x);
        var z = targetpos.z + sign*UnityEngine.Random.Range(targetdim.z, 2*targetdim.z);
        Vector3 randPos = new Vector3(x, y, z);

        return randPos;
    }

    public Vector3 RandomNearPosition() {

        GameObject ground = GameObject.Find("Ground");
        Vector3 grounddim = ground.transform.localScale;
        Vector3 groundpos = ground.transform.position;
        float y = transform.position.y;

        var x = UnityEngine.Random.Range(groundpos.x-grounddim.x/2, groundpos.x+grounddim.x/2);
        var z = UnityEngine.Random.Range(groundpos.z-grounddim.z/2, groundpos.z+grounddim.z/2);
        Vector3 randPos = new Vector3(x, y, z);
        Vector3 rdir = randPos - transform.position;

        return rdir; //*5;
    }


    public void SetRandomDestination() {
        SetDestination(transform.position + RandomNearPosition());
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result) {
        for (int i = 0; i < 30; i++) {
            Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) {
                result = hit.position;
                return true;
                }
            }
        result = Vector3.zero;
        return false;
        }

    public Vector3 GetPos() {
        return transform.position;
    }

    public float GetRadius() {
        return ((CapsuleCollider)agentCollider).radius; 
    }

    public void TurnOffNavMeshAgent(Vector3 pos) {
        navagent.updatePosition = false;        
        transform.position = pos;
    }

    public void TurnOnNavMeshAgent() {
        navagent.updatePosition = true;   
    }
}
