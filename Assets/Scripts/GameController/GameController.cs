using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Audio;
using Entity.Combat;
using Entity.Enemy;
using Entity.Item;
using Entity.Player;
using Entity.Stats;
using UI;
using System;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public GameObject Player { get; private set; }
    public UIController UIController { get; private set; }
    public SoundManager SoundManager { get; private set; }

    PlayerPotionManager playerPotionManager;

    public List<Transform> potionTransforms;

    // Keep track of how many enemies are currently hunting the player
    public int numAggroedEnemies;

    // How long to wait from player death until game over logic is processed
    public float gameOverWaitDelay;

    // Delegates
    public delegate void GameControllerEvent(GameObject gameObject);

    GameControllerEvent disableEnemy = new GameControllerEvent(DisableEnemy);
    GameControllerEvent disablePlayer = new GameControllerEvent(DisablePlayer);        
    
    [SerializeField] bool transitionInProgress;

    // Keeps track whether the player has successfully beat the game
    [field : SerializeField] public bool HasPlayerWon { get; private set; }

    private static Vector3 ForestPlayerSpawnPositon = new Vector3(12, 0, 15);
    private static Vector3 KeepPlayerSpawnPosition = new Vector3(5, 0, 45);

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);            
        }
    }

    void Start()
    {
        // Find the player        
        Player = GameObject.FindGameObjectWithTag("Player");
        Debug.Assert(Player != null, "GameController failed to find Player!");

        UIController = GameObject.Find("UIController").GetComponent<UIController>();

        UIController.UpdatePlayerHealth(Player.GetComponent<BaseCombat>().Health, PlayerStats.Instance.maximumHealth);
        SoundManager = FindObjectOfType<SoundManager>();

        playerPotionManager = transform.Find("PlayerPotionManager").GetComponent<PlayerPotionManager>();

        UpdatePotionUI(playerPotionManager.GetSelectedPotion());

        potionTransforms = new List<Transform>();

        FindPotionsInLevel();
        FindClosestToPlayer(potionTransforms);

        // Make the wait time 0 since we are coming from a scene change
        float waitTime = 3.0f;
        float fadeOutTime = 3.0f;
        StartTransition(0, waitTime, fadeOutTime);

        // Spawn the player
        SpawnPlayer();
    }

    private void Teleport_KeepStart()
    {
        TeleportPlayer(KeepPlayerSpawnPosition);
    }

    private void Teleport_ForestStart()
    {
        TeleportPlayer(ForestPlayerSpawnPositon);
    }

    void Update()
    {
        // Need to find a potential new closest potion since the current closest
        // may have been the recently obtained potion       
        if (potionTransforms.Count > 0)
        {
            Transform newClosest = FindClosestToPlayer(potionTransforms);

            // Set the potion audio source to the closest potion
            SoundManager.SetPotionAudioSourcePosition(newClosest.position);
        }
        
        // Debug command (Should be removed in player builds)
        /*
        if(Input.GetKeyDown(KeyCode.D))
        {
            Debug.LogWarning("Using debug command: KillPlayer");
            Player.GetComponent<PlayerCombat>().Die();
        }
        */
    }

    private void FindPotionsInLevel()
    {
        // Find all potion objects in the scene
        GameObject[] potionObjects = GameObject.FindGameObjectsWithTag("Potion");

        foreach (GameObject potion in potionObjects)
        {
            Transform t = potion.transform;
            potionTransforms.Add(t);
            
            //Debug.Log("Potion transform added, GameObject is: " + t.gameObject);
        }
    }

    private Transform FindClosestToPlayer(List<Transform> transforms)
    {
        if(transforms.Count == 0)
        {
            return null;
        }

        Vector3 playerPos = Player.transform.position;

        Transform closestTransform = null;
        float closestDistance = Mathf.Infinity;
                
        foreach(Transform t in transforms)
        {
            // Is this position closer?
            float currentDistance = Vector3.Distance(playerPos, t.position);
            if (currentDistance < closestDistance)
            {
                closestTransform = t;
                closestDistance = currentDistance; 
            }
        }
       
        return closestTransform;
    }

    public void OnEnemyTargetLost()
    {
        UIController.OnTargetLost();
    }

    public void OnEnemyTargetAcquired(BaseCombat combat, string identifier)
    {
        UIController.OnTargetAcquired(combat, identifier);
    }

    public void OnPlayerHealthChanged()
    {
        UIController.UpdatePlayerHealth(Player.GetComponent<BaseCombat>().Health, PlayerStats.Instance.maximumHealth);
    }

    public void OnEnemyLostPlayer(EnemyCombat enemyCombat)
    {        
        numAggroedEnemies--;
   
        // If the enemy that has left combat is not full health,
        // it must begin regenerating its health
        if(!enemyCombat.IsFullHealth())
        {
            enemyCombat.StartCoroutine(enemyCombat.RegenHealthCoroutine);
        }

        // Only allow if the player is alive
        if(!IsPlayerDead())
        {
            ProcessPlayerHealthRegeneration();
            AudioPlayer.Play2DSound(SoundManager.enemyLostPlayer);
        }        
    }

    public static void DisableEnemy(GameObject enemy)
    {
        Debug.Log("Disabling enemy");

        // Disable enemy components              
        foreach (Behaviour b in enemy.GetComponents<Behaviour>())
        {
            b.enabled = false;
        }
    }

    // Wait for a specified period of time and call a GameControllerEvent on a
    // GameObject param when done waiting
    private IEnumerator Wait(float time, GameControllerEvent gameEvent, GameObject obj)
    {
        yield return new WaitForSeconds(time);

        gameEvent(obj);
    }

    public void OnEnemyDeath(EnemyCombat combat, float enemyDeathSoundLength)
    {
        if(combat.isAggroed)
        {
            combat.isAggroed = false;
            numAggroedEnemies--;

            ProcessPlayerHealthRegeneration();
        }

        // Disable any collision on the enemy instantly
        combat.GetComponent<Collider>().enabled = false;

        // Wait until the enemy death sound has finished, then proceed
        // with calling the routine that disables the enemy components
        StartCoroutine(Wait(enemyDeathSoundLength, disableEnemy, combat.gameObject));
    }

    private void ProcessPlayerHealthRegeneration()
    {
        // The player has no enemies chasing after them
        if (numAggroedEnemies == 0)
        {
            PlayerCombat playerCombat = Player.GetComponent<PlayerCombat>();
            playerCombat.StartCoroutine(playerCombat.RegenHealthCoroutine);            
        }
    }

    public void OnEnemyAggroed()
    {
        numAggroedEnemies++;

        // Prevent the player from regenerating health, since they are no
        // longer 'safe'
        PlayerCombat combat = Player.GetComponent<PlayerCombat>();
        combat.StopCoroutine(combat.RegenHealthCoroutine);        
    }

    public void OnSelectedPotionChanged(PotionData newSelectedPotion)
    {
        UpdatePotionUI(newSelectedPotion);
        AudioPlayer.Play2DSound(SoundManager.potionCycleSound);
    }

    public void OnPotionConsumed(PotionData potion)
    {
        UpdatePotionUI(potion);
        AudioPlayer.PlaySoundFromPlayer(SoundManager.potionConsumedSound);
    }
   
    private Transform FindPotionTransform(Transform potionTransform)
    {
        Transform toRemove = null;
        foreach (Transform t in potionTransforms)
        {
            if (t == potionTransform)
            {
                //Debug.Log("Found transform match: t " + t.gameObject + " == " + potionTransform.gameObject);

                toRemove = potionTransform;
                break;
            }
        }

        Debug.Assert(potionTransform, "Failed to locate potion transform!");

        return toRemove;
    }

    public void OnPotionObtained(Transform potionTransform, PotionType type)
    {  
        // Remove the obtained potion transform from the list of potion transforms
        potionTransforms.Remove(FindPotionTransform(potionTransform));

        if (potionTransforms.Count == 0)
        {
            // No more potions exist, disable the GameObject
            SoundManager.DisablePotionAudioSource();
        }
 
        PotionData addedPotionData = playerPotionManager.AddPotion(type);
        
        Debug.Assert(addedPotionData != null, "Could not obtain potion properly!");
        
        bool obtainedIsSelected = playerPotionManager.SelectedPotion == type;
        if (obtainedIsSelected)
        {
            UpdatePotionUI(addedPotionData);
        }

        UIController.ShowPotionObtainedText(addedPotionData.Name);
        AudioPlayer.Play2DSound(SoundManager.potionPickupSound);        
    }

    public void UpdatePlayerInfoUIText(string text)
    {
        UIController.ShowText(text);
    }

    // Called when a time-limited potion is consumed
    public void OnTimeLimitedPotionConsumed()
    {
        playerPotionManager.SetTimeLimitedPotionActive(true);
        UIController.ShowPotionDurationBar();
    }

    // Called when a time-limited potion has expired
    public void OnTimeLimitedPotionExpired()
    {        
        playerPotionManager.SetTimeLimitedPotionActive(false);

        UIController.HidePotionDurationBar();

        // Reset the duration slider value
        UIController.SetPotionDurationSliderValue(1.0f);
    }

    public void GetTimeLimitedPotionCurrentTime(float currentTime, float duration)
    {
        UIController.SetPotionDurationSliderValue(1.0f - (currentTime / duration));
    }

    // Called on player death
    public void OnPlayerDeath()
    {
        // Disable objects that can be disabled straight away
        // such as all child GameObjects, except for the model 
        for (int i = 0; i < Player.transform.childCount; ++i)
        {
            GameObject childObject = Player.transform.GetChild(i).gameObject;
            if (childObject.name != "Graphics")
            {
                childObject.SetActive(false);
            }
        }

        // Update the UI now, since the player doesn't have a target since they are dead
        UIController.OnTargetLost();
        
        // Wait for the length of the player's death animation (to disable other components).
        StartCoroutine(Wait(Player.GetComponent<PlayerMovement>().DeathAnimationClipLength,
            disablePlayer, Player));

        StartCoroutine(WaitForGameOver());       
    }

    // Returns whether a potion is currently active
    public bool IsTimeLimitedPotionActive()
    {
        return playerPotionManager.IsTimeLimitedPotionActive;
    }

    public bool IsPotionTypeActive(PotionType type)
    {
        return playerPotionManager.IsTimeLimitedPotionActive &&
            playerPotionManager.activePotion == type;
    }
    
    public bool IsPlayerDead()
    {
        return Player.GetComponent<PlayerCombat>().IsDead;
    }

    private void UpdatePotionUI(PotionData potion)
    {
        UIController.SetPotionText(potion.Name, potion.Count);
    }

    private void ProcessGameOver()
    {
        //Debug.Log("Started Game Over");

        string textToDisplay;

        // Display different text on the game over screen depending on their fate
        if(HasPlayerWon)
        {
            textToDisplay = "You win!";
        }
        else
        {
            textToDisplay = "You are dead!";
        }

        UIController.OnGameOver(textToDisplay);
    }

    private static void DisablePlayer(GameObject player)
    {
        // Disable any collision
        player.GetComponent<Collider>().enabled = false;

        // Disable all components
        foreach (Behaviour b in player.GetComponents<Behaviour>())
        {
            b.enabled = false;
        }
    }

    private IEnumerator WaitForGameOver()
    {
        yield return new WaitForSeconds(gameOverWaitDelay);

        ProcessGameOver();
    }

    //EnemyManager should do this
    public enum Action
    { 
        ChasePlayer
    }

    public bool IsAllowed(Action action)
    {
        return !IsPlayerDead();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //OnLevelLoaded();
    }

    private void SpawnPlayer()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        switch (sceneIndex)
        {
            case 1:
                Teleport_ForestStart();
                break;
            case 2:
                Teleport_KeepStart();
                break;
        }
    }

    public void OnChangeLevel()
    {
        // Stop the player from moving
        Player.GetComponent<NavMeshAgent>().isStopped = true;

        float fadeInTime = 3;
        float fadeOutTime = fadeInTime;
        float waitTime = 2;

        StartTransition(fadeInTime, waitTime, fadeOutTime);

        StartCoroutine(DoLoadLevel(SceneManager.GetActiveScene().buildIndex + 1, fadeInTime));                
    }

    private IEnumerator DoLoadLevel(int buildIndex, float delay)
    {        
        yield return new WaitForSeconds(delay);
        
        GameSceneManager.LoadLevel(buildIndex);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnEnterKeep(Transform enterPos)
    {
        NavMeshAgent nav = Player.GetComponent<NavMeshAgent>();
        nav.isStopped = true;

        float fadeInTime = 3;
        float fadeOutTime = fadeInTime;
        float waitTime = 2;
        
        StartTransition(fadeInTime, waitTime, fadeOutTime);

        StartCoroutine(DoEnterKeep(fadeInTime, enterPos));
    }

    private IEnumerator DoEnterKeep(float waitTime, Transform keepEntry)
    {
        yield return new WaitForSeconds(waitTime);

        NavMeshAgent nav = Player.GetComponent<NavMeshAgent>();
        nav.enabled = false;

        Player.transform.position = keepEntry.position;        

        nav.enabled = true;
    }

    public void StartTransition(float inTime, float waitTime, float outTime)
    {
        float totalDuration = inTime + waitTime + outTime;

        //Debug.Log("This transition will take " + totalDuration + " seconds");
        
        StartCoroutine(WaitForTransition(totalDuration));

        UIController.Transition(inTime, waitTime, outTime);
    }

    IEnumerator WaitForTransition(float totalTime)
    {
        transitionInProgress = true;

        yield return new WaitForSeconds(totalTime);

        transitionInProgress = false;
    }
 
    public bool CanPlayerTarget()
    {        
        if(transitionInProgress)
        {
            Debug.Log("You cannot target: transition in progress");
            return false;
        }
        else if(IsPlayerDead())
        {
            Debug.Log("You cannot target: player is dead");
            return false;
        }
        else
        {
            return true;
        }
    }

    public void OnBossKilled(GameObject boss)
    {
        HasPlayerWon = true;
        StartCoroutine(WaitForGameOver());    
    }

    public void BeginRestartLevel(bool playerWon)
    {
        // If the player won, then when they restart start from the beginning,
        // otherwise restart the current level
        int sceneIndexToLoad = playerWon ? 1 : SceneManager.GetActiveScene().buildIndex;

        float fadeInTime = 2;
        float waitTime = 3;
        float fadeOutTime = 3;

        StartTransition(fadeInTime, waitTime, fadeOutTime);
        StartCoroutine(DoLoadLevel(sceneIndexToLoad, fadeInTime));
    }

    public void TeleportPlayer(Vector3 to)
    {
        NavMeshAgent nav = Player.GetComponent<NavMeshAgent>();

        nav.enabled = false;
        Player.transform.position = to;
        nav.enabled = true;
    }
}
