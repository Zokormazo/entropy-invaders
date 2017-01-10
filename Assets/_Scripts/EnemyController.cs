using UnityEngine;
using System.Collections;

/*
 * EnemyController
 * Controls the enemies' behaviour
 */

public class EnemyController : MonoBehaviour {
    /*
     * Variables
     */

    // Private Variables
    private int type; // Enemy type
    private bool canFire; // Can the enemy fire?
    private float nextBullet; // Seconds to wait before firing next bullet
    private float lastBullet; // Timestamp of the last fire
    private int column; // The column where the enemy is
    private bool dead; // Is this enemy dead?
    private GameController gameController; // Game Controller Script reference
    private AudioSource enemyAudioSource; // Enemy's audio source reference
    private Rigidbody enemyRigidbody; // Enemy's Rigidbody Component
    private Renderer enemyRenderer; // Enemy's Renderer Component
    private Collider enemyCollider; // Enemy's Collider Component
    private ParticleSystem enemyParticleSystem; // Enemy's ParticleSystem Component

    // Public Variables
    [Header("Materials")]
    [Tooltip("Materials for health levels. WARNING: The array size must be 3 and must store valid materials")]
    public Material[] typeMaterials;

    [Header("Fire cadence")]
    [Tooltip("Min time between bullets")]
    public float minCadence;
    [Tooltip("Max time between bullets")]
    public float maxCadence;

    [Header("References")]
    [Tooltip("Prefab for fired bullets")]
    public GameObject bulletPrefab;

    [Header("SFX")]
    [Tooltip("Enemies Sound Effects")]
    public EntitySFX enemySFX;

    /*
     * Methods
     */

    // Initialization method
    private void Start () {
        enemyRigidbody = GetComponent<Rigidbody>(); // Store Rigidbody Component for later usage
        enemyRenderer = GetComponent<Renderer>(); // Store Renderer Component for later usage
        enemyCollider = GetComponent<Collider>(); // Store Collider Component for later usage
        enemyAudioSource = GetComponent<AudioSource>(); // Store AudioSource Component for later usage
        enemyParticleSystem = GetComponent<ParticleSystem>(); // Store ParticleSystem Component for later usage
	}

    // Collision callback: called when enemy enters on collision with another object.
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Bullet" || collision.gameObject.tag == "Defense") { // Hit by bullet or defense
            dead = true; // Mark enemy as dead
            enemyAudioSource.PlayOneShot(enemySFX.dead); // Play dead SFX
            enemyParticleSystem.Play(); // Play explosion particle system
            enemyRenderer.enabled = false; // Hide Enemy
            enemyCollider.enabled = false; // Disable enemies collider
            gameController.enemyKilled(type, column); // notify Game Controller
            Invoke("Disable", enemySFX.dead.length);
        } else if (collision.gameObject.tag == "Right Boundary") // Collission with right boundary
            transform.parent.gameObject.GetComponent<EnemiesGroupController>().collision(true);
        else if (collision.gameObject.tag == "Left Boundary") // Collission with left boundary
            transform.parent.gameObject.GetComponent<EnemiesGroupController>().collision(false);
        else if (collision.gameObject.tag == "Boundary") // Enemy reached down boundary, instant GameOver
            gameController.gameOver();
    }

    // Disable GameObject
   private void Disable() {
        gameObject.SetActive(false); // Disable GO
    }

    // Get enemy type
    public int getType() {
        return type;
    }

    // Set enemy type
    public void setType(int value) {
        type = value; // Store type value
        GetComponent<Renderer>().material = typeMaterials[value-1]; // Change material
    }

    // Get canFire value
    public bool getCanFire() {
        return canFire;
    }

    // Set canFire value
    public void setCanFire(bool value) {
        canFire = value;
        if (canFire) {
            nextBullet = Random.Range(minCadence, maxCadence);
            lastBullet = Time.time;
        }
    }

    // Get column value
    public int getColumn() {
        return column;
    }

    // Set column value
    public void setColumn(int value) {
        column = value;
    }

    // Get dead value
    public bool isDead() {
        return dead;
    }

    // Set GameController
    public void setGameController(GameController value) {
        gameController = value;
    }

    // Fire a bullet
    private void FireBullet() {
        Vector3 bulletPosition = new Vector3(enemyRigidbody.position.x, enemyRigidbody.position.y - 1, enemyRigidbody.position.z); // Calculate bullet spawning position
        Quaternion bulletRotation = Quaternion.Euler(180.0f, 0.0f, 0.0f); // Set bullet upside down
        Instantiate(bulletPrefab, bulletPosition, bulletRotation); // Instantiate bullet
        lastBullet = Time.time; // Store last bullet's firing time (now)
        nextBullet = Random.Range(2.0f, 6.0f); // Get next fire's time to wait
        enemyAudioSource.PlayOneShot(enemySFX.shoot); // Play fire SFX
    }

    // Main loop
    private void Update() {
        if (canFire && (Time.time - lastBullet) > nextBullet) // Can we fire?
            FireBullet(); // Yeah, we can Fire!
    }
}
