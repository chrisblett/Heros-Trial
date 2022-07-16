using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Entity.Combat;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [Header("Texts")]
        public Text targetNameText;
        public Text playerHealthText;
        public Text infoText;
        public Text selectedPotionText;
        public Text gameOverText;

        [Header("Sliders")]
        public Slider targetHealthBar;
        public Slider potionDurationBar;

        [Header("Buttons")]

        // A reference to restart button that appears on the game over screen
        public Button restartButton;

        // A reference to the exit button that appears on the game over screen
        public Button exitButton;

        TextFader textFader;
        BaseCombat combat;

        ScreenFader screenFader;

        Text[] texts;

        void Start()
        {
            GameObject fader = GameObject.Find("Fader");
            if (fader)
            {
                screenFader = fader.GetComponent<ScreenFader>();

                if (screenFader == null)
                {
                    Debug.LogError("Could not locate object Fader with expected ScreenFader, you must have a ScreenFader");
                }
            }
            else
            {
                Debug.LogError("Could not find Fader");
            }

            textFader = GetComponent<TextFader>();

            FindGameUITexts();

            HidePotionDurationBar();
            HideTargetUIComponents();

            // Used for the target health bar UI
            combat = null;

            // Hide the restart button
            if (restartButton)
            {
                restartButton.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Restart button Button reference is null");
            }

            if (exitButton)
            {
                exitButton.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Restart button Button reference is null");
            }
        }

        void Update()
        {
            if (combat)
            {
                // Update the health bar value with the target's current health
                targetHealthBar.value = combat.Health;
            }
        }

        public void SetPotionText(string name, int count)
        {
            selectedPotionText.text = name + " (" + count + ")";
        }

        public void UpdatePlayerHealth(float newHealth, float maxHealth)
        {
            playerHealthText.text = "Health: " + Mathf.RoundToInt(newHealth) + "/" + maxHealth;
        }

        public void OnTargetAcquired(BaseCombat combat, string identifier)
        {
            // Set the target name text
            targetNameText.text = identifier;

            this.combat = combat;

            ShowTargetUIComponents(identifier);

            targetHealthBar.minValue = 0;

            // Set the slider values according to the target's combat stats such as maximum health
            targetHealthBar.maxValue = combat.GetCombatStats().maximumHealth;
        }

        public void ShowTargetHealthBar()
        {
            targetHealthBar.gameObject.SetActive(true);
        }

        public void HideTargetUIComponents()
        {
            targetNameText.text = "";
            targetHealthBar.gameObject.SetActive(false);
        }

        public void ShowTargetUIComponents(string targetName)
        {
            targetNameText.text = targetName;
            targetHealthBar.gameObject.SetActive(true);
        }

        public void OnTargetLost()
        {
            this.combat = null;
            HideTargetUIComponents();
        }

        public void ShowText(string text)
        {
            // Only overwrite the text if it is different, otherwise wait for the current text to fade out
            if (textFader.GetCurrentText() != text)
            {
                infoText.text = text;
                textFader.FadeText(infoText, 2.0f, 2.0f);
            }
        }

        public void ShowPotionObtainedText(string potionName)
        {
            infoText.text = "Obtained a " + potionName;
            textFader.FadeText(infoText, 2.0f, 2.0f);
        }

        public void SetPotionDurationSliderValue(float value)
        {
            potionDurationBar.value = value;
        }

        public void ShowPotionDurationBar()
        {
            potionDurationBar.gameObject.SetActive(true);
        }

        public void HidePotionDurationBar()
        {
            potionDurationBar.gameObject.SetActive(false);
        }

        public void OnTextFadeEnded(Text t)
        {
            t.text = "";
            t.color = new Color(t.color.r, t.color.g, t.color.b, 1.0f);
        }

        public void OnGameOver(string text)
        {
            // Hide all other UI text apart from the game over text
            foreach (Text t in texts)
            {
                t.text = "";
            }
            gameOverText.text = text;

            // Show the restart button
            Transform restartButtonTransform = restartButton.transform.GetChild(0);
            if (restartButtonTransform)
            {
                Text restartButtonText = restartButtonTransform.gameObject.GetComponent<Text>();
                restartButtonText.text = "Restart";
                restartButton.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("Could not locate the restart button's child object");
            }

            // Show the exit button
            Transform exitButtonTransform = exitButton.transform.GetChild(0);
            if (exitButtonTransform)
            {
                Text exitButtonText = exitButtonTransform.gameObject.GetComponent<Text>();
                exitButtonText.text = "Exit";
                exitButton.gameObject.SetActive(true);
            }
        }

        public void Transition(float inTime, float waitTime, float outTime)
        {
            if (screenFader)
            {
                screenFader.Transition(inTime, waitTime, outTime);
            }
        }

        public void OnRestartRequested()
        {
            //Debug.Log("Player requested restart");

            GameController.Instance.BeginRestartLevel(GameController.Instance.HasPlayerWon);
        }

        public void OnExitRequested()
        {
            Debug.Log("Player requested quit");
            Application.Quit();
        }

        private void FindGameUITexts()
        {
            // Find the UI_Game Canvas
            GameObject uiGame = GameObject.Find("UI_Game");
            if (uiGame)
            {
                Transform uiGameCanvasTransform = uiGame.transform.Find("Canvas");
                if (uiGameCanvasTransform)
                {
                    GameObject uiGameCanvas = uiGameCanvasTransform.gameObject;

                    // Find all texts
                    texts = uiGameCanvas.GetComponentsInChildren<Text>();

                    // Set all to blank
                    foreach (Text t in texts)
                    {
                        // Set all the texts to blank apart from the button text
                        if (t.gameObject.transform.parent.gameObject.name != "Button_Restart")
                        {
                            t.text = "";
                        }
                    }
                }
                else
                {
                    Debug.LogError("Could not find Canvas child of " + uiGame.name);
                }
            }
            else
            {
                Debug.LogError("Could not find UI_Game! Did you rename it?");
            }
        }
    }
}