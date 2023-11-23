using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Normal {

    // The mean of the Gaussian distribution
    float mean;

    // The standard deviation of the Gaussian distribution
    float std;

    // Constructor for the Normal class
    public Normal(float mean, float std) {
        // Initialize the mean and standard deviation
        this.mean = mean;
        this.std = std;
    }

    // Generates a random number from a standard Gaussian distribution
    public static float NextGaussian() {
        // Generate two random numbers between 0 and 1
        float v1, v2, s;
        do {
            v1 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            v2 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            s = v1 * v1 + v2 * v2;
        } while (s >= 1.0f || s == 0f);

        // Calculate the standard Gaussian variable
        s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);

        // Return the standard Gaussian variable
        return v1 * s;
    }

    // Generates a random number from a Gaussian distribution with a specified mean and standard deviation
    public static float NextGaussian(float mean, float standard_deviation) {
        // Generate a standard Gaussian variable
        float gaussian = NextGaussian();

        // Scale the standard Gaussian variable to the specified mean and standard deviation
        return mean + gaussian * standard_deviation;
    }

    // Generates a random number from a Gaussian distribution with a specified mean, standard deviation, minimum, and maximum
    public static float NextGaussian(float mean, float standard_deviation, float min, float max) {
        // Generate a random number from the Gaussian distribution until it falls within the specified range
        float x;
        do {
            x = NextGaussian(mean, standard_deviation);
        } while (x < min || x > max);

        // Return the random number
        return x;
    }

    // Generates a random number from the Gaussian distribution associated with this object
    public float Sample() {
        // Generate a random number from the corresponding Gaussian distribution
        return NextGaussian(mean, std);
    }
}
