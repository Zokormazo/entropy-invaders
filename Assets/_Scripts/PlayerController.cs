using UnityEngine;

/*
 * PlayerController
 * Controls the player's behaviour
 */

public class PlayerController : MonoBehaviour {

    /*
     * Variables
     */

    // Private Variables
    private Rigidbody playerRigidBody; // Player's Rigidbody Component
    private AudioSource playerAudioSource; // Player's AudioSource Component
    private GameObject bullet; // For storing our fired bullet

    // Public Variables
    [Header("Configuration")]
    [Tooltip("Player's movement speed")]
    public float playerSpeed;

    [Header("References")]
    [Tooltip("Bullet Prefab")]
    public GameObject bulletPrefab;
    [Tooltip("Initial position GO")]
    public GameObject initialPosition;
    [Tooltip("Game Controller")]
    public GameController gameController;

    [Header("SFX")]
    [Tooltip("Player's Sound Effects")]
    public EntitySFX playerSFX;

    // Initialization method
    void Start() {
        playerRigidBody = GetComponent<Rigidbody>(); // Store Rigidbody component for later usage;
        playerAudioSource = GetComponent<AudioSource>(); // Store AudioSource component for later usage;
        bullet = null; // Set bullet reference to null
    }

    // Physics loop
    void FixedUpdate() {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0.0f, 0.0f); // Calculate movement Vector3
        playerRigidBody.velocity = movement * playerSpeed; // Set players velocity
        if (Input.GetButton("Run")) // If running
            playerRigidBody.velocity *= 2; // Double velocity
        playerRigidBody.rotation = Quaternion.Euler(0.0f, movement.x * -45, 0.0f); // Rotate the ship to movement side
    }

    // Main loop
    void Update() {
        if (bullet == null && Input.GetButton("Fire")) // Can we fire?
            FireBullet(); // Fire!
    }

    // Collision callback: called when player enters on collision with another object.
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Bullet") { // Collision with a bullet.
            playerAudioSource.PlayOneShot(playerSFX.dead); // Play player dead audio clip
            gameController.playerKilled(); // Exec player killed routine on game controller
        } else if (collision.gameObject.tag == "Enemy") // Enemy catches us, instant GameOver
            gameController.gameOver(); // Exec game over routine on game controller
    }

    // Fire a bullet!
    void FireBullet() {
        playerAudioSource.PlayOneShot(playerSFX.shoot); // Play shoot audio clip
        Vector3 bulletPosition = new Vector3(playerRigidBody.position.x, playerRigidBody.position.y + 1, playerRigidBody.position.z); // Calculate bullet's spawning position
        Quaternion bulletRotation = playerRigidBody.rotation; // Calculate bullet's rotation
        bullet = (GameObject)Instantiate(bulletPrefab, bulletPosition, bulletRotation); // Spawn bullet
    }

    // Reset player to initial state and stop it.
    public void resetPlayer() {
        playerRigidBody.velocity = Vector3.zero; // Stop player's velocity
        transform.rotation = Quaternion.identity; // Stop player's rotation
        transform.position = initialPosition.transform.position; // Set player at initial position
    }
}
