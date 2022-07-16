using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameScene
{
    Menu,   // 0
    Forest, // 1
    Keep    // 2
}

// Handles switching scenes throughout the game
public class GameSceneManager : MonoBehaviour
{
    // Takes in a build index value and attempts to load the corresponding scene
    public static void LoadLevel(int buildIndex)
    {
        if (buildIndex > SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("Invalid build index");
        }
        else
        {
            Debug.LogWarning("Loading scene: " + SceneManager.GetSceneByBuildIndex(buildIndex).name);
            SceneManager.LoadScene(buildIndex);
        }
    }
}



