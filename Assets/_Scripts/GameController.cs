using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
 * GameController: Main Game Logic Manager
 */

public class GameController : MonoBehaviour {

    /*
     * Variables
     */ 

    // Private variables

    // Global game variables
    private int score; // Score
    private int lives; // Lives

    // Current level variables
    private int currentLevel; // current Level number
    private int enemiesAlive; // Current number of alive enemies
    private GameObject[,] enemiesArray = new GameObject[7, 7]; // array for storing current level enemies
    private IEnumerator runningCoroutine; // Var to store current running speed change coroutine IEnumerator

    // Events
    public static event System.EventHandler<OnChangeSpeedEventArgs> OnChangeSpeed;

    /*
     * Public variables
     */

    [Header("Prefabs")]
    [Tooltip("Prefab for enemies")]
    public GameObject enemy;
    [Header("Scene GameObjects")]
    [Tooltip("The player gameobject")]
    public PlayerController player;
    [Tooltip("Enemies Group gameobject")]
    public GameObject enemiesGroup;
    [Tooltip("Defense blocks")]
    public GameObject[] defenses;
    [Header("Positioning helpers")]
    [Tooltip("Upper left position where enemies are instantiated")]
    public Vector3 leftCorner;
    [Tooltip("Enemies offset between them at instantianting time")]
    public Vector3 offset;
    
    [Header("UI References")]
    [Tooltip("Text element for score")]
    public Text scoreText;
    [Tooltip("Text element for remaining lives")]
    public Text livesText;
    [Tooltip("Text element for current level")]
    public Text levelText;
    [Tooltip("Countdown panel")]
    public GameObject countdownPanel;
    [Tooltip("Countdown level text")]
    public Text countdownLevelText;
    [Tooltip("Countdown number text")]
    public Text countdownNumberText;
    [Tooltip("Gameover panel")]
    public GameObject gameOverPanel;
    [Tooltip("Gameend panel")]
    public GameObject gameEndPanel;

    [Header("Game Behaviour configuration")]
    [Tooltip("Player's initial lives")]
    public int initialLives;
    [Tooltip("Base speed for enemies")]
    public float baseSpeed;
    [Tooltip("Delta Speed for each step")]
    public float speedStep;
    [Tooltip("Delta Speed for each level")]
    public float levelSpeedStep;
    [Tooltip("Time between speed steps")]
    public float stepTime;

    [Header("Music & Sounds")]
    [Tooltip("Music Source")]
    public AudioSource musicSource;
    [Tooltip("SFX Source")]
    public AudioSource sfxSource;
    [Tooltip("Sound effect assets")]
    public GameSFX gameSFX;

	// Use this for initialization
	void Start () {
        initGame();
	}

    // Executed before destruction
    void OnDestroy() {
        OnChangeSpeed = null;
    }

    // Start a level
    void startLevel(int level) {
        currentLevel = level; // Set current level
        loadLevel(LevelsDefinition.levels[level -1]); // Load level enemies from definition array
        levelText.text = string.Format("Level: {0,2}", level.ToString("D2")); // Update UI
        resetDefenses(); // Reset defense states
        StartCoroutine(levelStartCountdown(level)); // Start countdown
    }

    // Countdown for level starting
    IEnumerator levelStartCountdown(int level) {
        countdownLevelText.text = string.Format("Level {0,2}", level.ToString("D2")); // Set level text for countdown
        countdownNumberText.text = "3"; // Show number
        countdownPanel.SetActive(true); // Show panel
        sfxSource.PlayOneShot(gameSFX.three); // Play 'three' audio clip
        yield return new WaitForSeconds(1.0f); // Wait 1 sec
        countdownNumberText.text = "2"; // Update number
        sfxSource.PlayOneShot(gameSFX.two); // Play 'two' audio clip
        yield return new WaitForSeconds(1.0f); // Wait 1 sec
        countdownNumberText.text = "1"; // Update number
        sfxSource.PlayOneShot(gameSFX.one); // Play 'one' audio clip
        yield return new WaitForSeconds(1.0f); // Wait 1 sec
        countdownNumberText.text = "GO"; // Update message
        sfxSource.PlayOneShot(gameSFX.go); // Play 'go' audio clip
        yield return new WaitForSeconds(0.5f); // Wait 0.5 secs
        countdownPanel.SetActive(false); // Hide panel
        player.enabled = true; // Enable players control
        enemiesGroup.GetComponent<EnemiesGroupController>().StartMoving(baseSpeed + level*levelSpeedStep); // Start moving enemies
        updateShooters(); // Set shooting enemies
        runningCoroutine = changeSpeed();
        StartCoroutine(runningCoroutine);
    }

    IEnumerator changeSpeed() {
        OnChangeSpeedEventArgs args = new OnChangeSpeedEventArgs();
        args.deltaSpeed = speedStep;
        while (true) {
            yield return new WaitForSeconds(stepTime);
            if (OnChangeSpeed != null)
                OnChangeSpeed(this, args);
        }
    }

    // Reset defense block's state
    void resetDefenses() {
        for (int i = 0; i < defenses.GetLength(0); i++) // Iterate over defenses
            defenses[i].GetComponent<DefenseController>().resetDefense(); // Reset it
    }

    // Destroy all enemy gameobjects
    void destroyEnemies() {
        // Iterate over enemies array
        for(int i = 0; i < enemiesArray.GetLength(0); i++)
            for (int j = 0; j < enemiesArray.GetLength(1); j++)
                if (enemiesArray[i, j] != null) // There is an enemy go
                    GameObject.Destroy(enemiesArray[i, j]); // Destroy it
    }

    // Loads level from array.
    void loadLevel(int[,] level) {
        // Reset alive enemies count
        enemiesAlive = 0;
        // Get enemies group transform for later usage
        Transform enemiesGroupTransform = enemiesGroup.GetComponent<Transform>();
        // Iterate over level array
        for (int i = 0; i < level.GetLength(0); i++)
            for (int j = 0; j < level.GetLength(1); j++)
                if (level[i, j] > 0) { // Enemy at position i,j. Instantiate and position it.
                    Vector3 enemyPosition = leftCorner + new Vector3(offset.x * j, -offset.y * i, offset.z); // Calculate spawning position
                    Quaternion enemyQuaternion = Quaternion.identity; // Set spawning rotation
                    enemiesArray[i, j] = (GameObject)Instantiate(enemy, enemyPosition, enemyQuaternion); // Spawn enemy
                    enemiesArray[i, j].transform.parent = enemiesGroupTransform; // Add enemy to enemies group
                    EnemyController currentEnemyScript = enemiesArray[i, j].GetComponent<EnemyController>(); // Get current enemy's script
                    currentEnemyScript.setType(level[i, j]); // Set enemy type
                    currentEnemyScript.setGameController(this); // Set GameController reference on the enemies script
                    currentEnemyScript.setColumn(j); // Set enemies column
                    enemiesAlive++; // Increment enemies Alive count
                }
    }

    // Starts Game
    void initGame() {
        // Set initial values
        score = 0;
        lives = initialLives;
        // Update UI
        scoreText.text = string.Format("Score: {0,4}", score.ToString("D4"));
        livesText.text = string.Format("Lives: {0}", lives);
        startLevel(1);
    }

    // Update shooter state of enemies on a given column
    void updateShooter(int column) {
        int last = enemiesArray.GetLength(0) - 1; // Start looking from down
        // Loop until we reach the first active gameobject on the column
        while (last >= 0 && (enemiesArray[last, column] == null || enemiesArray[last, column].GetComponent<EnemyController>().isDead()))
            last--;
        if (last >= 0) // There is at leat an enemy on the column
            enemiesArray[last, column].GetComponent<EnemyController>().setCanFire(true); // Enable shooting
    }

    // Update shooter state of enemies
    void updateShooters() {
        for (int i = 0; i < enemiesArray.GetLength(1); i++) // Iterate on all columns            
            updateShooter(i); // Update i column shooter
    }

    // Remove shooter state of enemies
    void removeShooters() { // Iterate on all columns
        for (int i = 0; i < enemiesArray.GetLength(1); i++) { // Iterate on all columns
            int last = enemiesArray.GetLength(0) - 1; // Start looking from down
            // Loop until we reach the first not dead gameobject on the column
            while (last >= 0 && (enemiesArray[last,i] == null || enemiesArray[last, i].GetComponent<EnemyController>().isDead()))
                last--;
            if (last >= 0) // There is a shooter
                enemiesArray[last, i].GetComponent<EnemyController>().setCanFire(false); // Disable shooting
        }
    }

    // Enemy of type value has been killed
    public void enemyKilled(int value, int column) {
        score += value * 10; // Increment score
        scoreText.text = string.Format("Score: {0,4}", score.ToString("D4")); // Update UI
        enemiesAlive--; // Decrement alive enemies count
        if (enemiesAlive > 0) // There is at least one more enemy
            updateShooter(column); // Update shooting state on enemies on same column
        else { // All enemies dead. Level Completed
            enemiesGroup.GetComponent<EnemiesGroupController>().stop(); // Stop enemies movement
            StopCoroutine(runningCoroutine);
            player.enabled = false; // Stop player
            player.resetPlayer(); // Reset player to initial position and stop it
            destroyEnemies(); // Destroy all enemy gameobjects
            if (currentLevel < LevelsDefinition.levels.GetLength(0)) // There are leves yeat
                startLevel(currentLevel + 1); // Load next level
        }
    }

    // Player has been killed
    public void playerKilled() {
        lives--; // Remove live
        livesText.text = string.Format("Lives: {0}", lives); // Update UI
        if (lives > 0) // Lives yet, continue game
            player.resetPlayer(); // Reset player to initial position
        else  // No more lives, GameOver
            gameOver();
    }

    // GameOver
    public void gameOver() {
        musicSource.Stop(); // Stop music
        sfxSource.PlayOneShot(gameSFX.gameOver); // Play gameover sfx clip
        enemiesGroup.GetComponent<EnemiesGroupController>().stop(); // Stop enemies movement
        StopCoroutine(runningCoroutine); // Stop change Speed coroutine
        removeShooters(); // Stop enemies shooting
        player.enabled = false; // Disable player controls
        player.resetPlayer(); // Reset player to initial position and stop it
        gameOverPanel.SetActive(true); // Show gameover panel
    }

    // Game End
    public void gameEnd() {
        musicSource.Stop(); // Stop music
        sfxSource.PlayOneShot(gameSFX.gameEnd); // Play game end sfx clip
        player.enabled = false; // Disable player controles
        player.resetPlayer(); // Reset player to initial position and stop it
        gameEndPanel.SetActive(true); // Show gameend panel
    }

    // Restart Game
    public void restart() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1); // Reload scene
    }

    // Quit Game
    public void quit() {
        Application.Quit(); // Quit
    }
}
