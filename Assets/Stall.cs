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
