using UnityEngine;

/*
 * BulletController
 * Controls the bullet behaviour
 */

public class BulletController : MonoBehaviour {
    /*
     * Variables
     */
    
    // Private Variables
    private Rigidbody rigidBody; // Bullet's Rigidbody Component

    // Public Variables
    [Header("Configuration")]
    [Tooltip("The speed on units/sec of the bullet")]
    public float speed;

    /*
     * Methods
     */

	// Initialization method
	private void Start () {
        rigidBody = GetComponent<Rigidbody>(); // Store Rigidbody Component for later usage
        rigidBody.velocity = transform.up * speed; // Set bullet's velocity
	}

    // Collision callback: called when bullet enters on collision with another object.
    void OnCollisionEnter(Collision collision)
    {
        GameObject.Destroy(gameObject); // Destroy the bullet itself
    }
}
