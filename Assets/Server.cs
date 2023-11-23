
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Server class
class Server : MonoBehaviour
{
    // Mean and standard deviation for the service time distribution
    public float mean;
    public float std;

    // Reference to the queue of customers
    public AgentQueue customers;

    // Reference to the currently served customer
    Agent currCustomer = null!;

    // Flag indicating whether the server is free or busy
    bool serverFree = true;

    // Remaining time for the current service
    int serviceTime = 0;

    // Cumulative queue size over time
    int cumulativeQSize = 0;

    // Maximum queue size observed so far
    int maxQSize = 0;

    // Instance of the Normal class for generating service times
    Normal serviceGaussian;

    // Previous time for updating queue statistics
    DateTime prev;

    // Constant update rate for queue statistics
    const int updateRate = 1;

    // Current time
    long time;

    // Time since the last update
    float lastUpdate;

    // Start method is called before the first frame update
    void Start()
    {
        // Initialize the normal distribution for service times
        serviceGaussian = new Normal(mean, std);

        // Set the previous time for queue statistics update
        prev = DateTime.Now;
    }

    // Update method is called once per frame
    void Update()
    {
        // Increment the time counter
        time++;

        // Accumulate the time since the last update
        lastUpdate += Time.deltaTime;

        // Check if it's time to update queue statistics
        if (lastUpdate > updateRate)
        {
            // Update queue statistics
            update(customers, time);

            // Reset the time since the last update
            lastUpdate = 0;
        }
    }

    // Get the currently served customer
    public Agent getCurrCustomer()
    {
        return currCustomer;
    }

    // Get the cumulative queue size over time
    public int getCumulativeQSize()
    {
        return cumulativeQSize;
    }

    // Get the maximum queue size observed so far
    public int getMaxQSize()
    {
        return maxQSize;
    }

    // Update the server state and queue statistics
    public bool update(AgentQueue agentQueue, long Time)
    {
        // Get the current queue size
        int currQSize = agentQueue.Size();

        // Update the cumulative queue size
        cumulativeQSize += currQSize;

        // Update the maximum queue size if necessary
        if (currQSize > maxQSize)
        {
            maxQSize = currQSize;
        }

        // Check if the server is busy
        if (serverFree == false)
        {
            // Decrement the remaining service time
            serviceTime--;

            // Log the remaining service time
            Debug.Log(serviceTime + " second remaining for server " + name);

            // Check if the current service is finished
            if (serviceTime <= 0)
            {
                // Log the completion of the current service
                Debug.Log(name + " server done at time=" + Time);

                // Get the next customer from the queue
                currCustomer = agentQueue.Pop();

                // Mark the server as free
                serverFree = true;

                // Indicate that the service is finished
                return true;
            }
        }

        // Check if the server is free and there are customers in the queue
        if (serverFree == true && agentQueue.Size() > 0)
        {
            // Generate a service time from the normal distribution
            serviceTime = (int)serviceGaussian.Sample();

            // Log the start of the new service and its duration
            Debug.Log(name + " server starts. To finish in " + serviceTime);

            // Mark the server as busy
            serverFree = false;
        }

        // Indicate that the service is ongoing
        return false;
    }
}
