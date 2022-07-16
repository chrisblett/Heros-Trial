using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MenuController : MonoBehaviour
    {
        // How long the fade to black takes
        public float fadeInTime;

        // How long to wait once the screen becomes fully black
        public float waitTime;

        // How long the fade from black takes
        // Since a scene change happens here, this value can be any high enough value
        private float fadeOutTime = 99.9f;

        // A reference to a ScreenFader
        ScreenFader screenFader;

        void Start()
        {
            // Locate the GameObject with the ScreenFader component attached
            screenFader = GameObject.Find("Fader").GetComponent<ScreenFader>();
        }

        // Executed when the start button is clicked
        public void Button_StartGame()
        {
            Debug.LogWarning("Starting");
            StartGame();
        }

        // Executed when the exit button is clicked
        public void Button_ExitGame()
        {
            Debug.LogWarning("Exiting");
            Application.Quit();
        }

        private void StartGame()
        {
            // Begin the fade animation
            screenFader.Transition(fadeInTime, waitTime, fadeOutTime);

            // Start the game
            StartCoroutine(DoStartGame(fadeInTime));
        }

        private IEnumerator DoStartGame(float delay)
        {
            // Wait for a specified amount of time
            yield return new WaitForSeconds(delay);

            // Load the next scene specified in the build settings
            GameSceneManager.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}


