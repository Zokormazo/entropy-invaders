using UnityEngine;

/*
 * DefenseController
 * Controls our defense's behaviour
 */

public class DefenseController : MonoBehaviour {
    /*
     * Variables
     */

    // Private Variables
    private Renderer defenseRenderer; // Defense's Renderer Component
    private AudioSource defenseAudioSource; // Defense's AudioSource Component
    private Collider defenseCollider; // Defense's Collider Component
    private int health; // Defense's health points

    // Public Variables
    [Header("Configuration")]
    [Tooltip("Initial health points of the defense. 0 is yet alive.")]
    public int initialHealth = 2; 
    [Tooltip("Materials for health levels. WARNING: The array size must be equal to the health variable and must store valid materials")]
    public Material[] DefenseMaterials;
    [Tooltip("SFX")]
    public DefenseSFX defenseSFX;

    /*
     * Methods
     */

    // Initialization method
    void Start() {
        defenseRenderer = GetComponent<Renderer>(); // Store Renderer Component for later usage
        defenseAudioSource = GetComponent<AudioSource>(); // Store AudioSource Component for later usage
        defenseCollider = GetComponent<Collider>(); // Store Collider Component for later usage
        health = initialHealth; // Set health to initial health;
    }

    // Collision callback: called when defense enters on collision with another object.
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Bullet") {// Hit by bullet
            health--; // Decrease defense's health
            if (health < 0) { // When health < 0, defense is dead. disable it & play dead clip
                defenseAudioSource.PlayOneShot(defenseSFX.dead);
                defenseRenderer.enabled = false;
                defenseCollider.enabled = false;
            }
            else { // Otherwise change material & play hit clip
                defenseRenderer.material = DefenseMaterials[health];
                defenseAudioSource.PlayOneShot(defenseSFX.hit);
            }
        } else if (collision.gameObject.tag == "Enemy") { // Hit by enemy. Instant kill
            defenseAudioSource.PlayOneShot(defenseSFX.dead);
            defenseRenderer.enabled = false;
            defenseCollider.enabled = false;
        }
    }

    // Reset Defense to initial status
    public void resetDefense() {
        health = initialHealth; // Reset health points
        defenseRenderer.material = DefenseMaterials[health]; // Reset material to full health material
        defenseRenderer.enabled = true;
        defenseCollider.enabled = true;
    }
}
