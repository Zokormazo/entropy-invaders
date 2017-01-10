using UnityEngine;
using System.Collections;

/*
 * EnemiesGroupController
 * Controls the enemies behaviour as group. Used for movement
 */

public class EnemiesGroupController : MonoBehaviour {
    /*
     * Variables
     */

    // Private variables
    private float currentSpeed; // Current enemy moving speed
    private bool right; // Are they moving to the right?

    /*
     * Methods
     */

    // Set all enemies velocity
    private void setChildrenVelocity(Vector3 velocity) {
        foreach(Transform child in transform) // Iterate over children
            child.gameObject.GetComponent<Rigidbody>().velocity = velocity; // Set velocity
    }

    // Start moving children
    public void StartMoving(float speed) {
        currentSpeed = speed; // Set current speed
        right = true; // Start moving right
        setChildrenVelocity(speed * transform.right); // Set enemies velocity
        GameController.OnChangeSpeed += GameController_OnChangeSpeed; // Register Event Callback
    }

    // Event callback: Called when GameController broadcast speed change.
    private void GameController_OnChangeSpeed(object sender, OnChangeSpeedEventArgs e) {
        currentSpeed += e.deltaSpeed; // Increment current speed
        int sign = (right) ? 1 : -1; // Calculate sign
        setChildrenVelocity(currentSpeed * sign * transform.right); // Set current speed to children
    }

    // Stop moving children
    public void stop() {
        setChildrenVelocity(Vector3.zero); // Stop childrens
        CancelInvoke("VelocityChange");
    }

    // Collision against side boundary
    public void collision(bool colRight) {
        if (right == colRight) {
            setChildrenVelocity(-currentSpeed * transform.up); // Go down
            right = !right; // Change direction
            //int sign = (right) ? 1 : -1; // Calculate sign
            float timeToWait = 1.0f / currentSpeed; // Since we have speed and no units, calculate time to wait.
            Invoke("VelocityChange", timeToWait);
        }
    }

    // Starts moving to the other side
    private void VelocityChange() {
        int sign = (right) ? 1 : -1;
        setChildrenVelocity(currentSpeed * sign * transform.right); // Start moving left/right
    }
}
