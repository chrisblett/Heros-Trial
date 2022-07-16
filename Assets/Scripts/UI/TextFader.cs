using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TextFader : MonoBehaviour
    {
        private struct TextFade
        {
            public Text text;
            public float startDelay;
            public float duration;
        }

        bool processing;
        string currentString;

        UIController uiController;

        void Start()
        {
            uiController = GetComponent<UIController>();
            currentString = "";
        }

        public void FadeText(Text text, float duration, float delay = 0.0f)
        {
            if (processing)
            {
                //Debug.Log("Interrupted while processing, starting new fade");

                StopAllCoroutines();
            }

            currentString = text.text;

            TextFade textFade = new TextFade();
            textFade.text = text;
            textFade.duration = duration;
            textFade.startDelay = delay;

            BeginFade(textFade);
        }

        public string GetCurrentText()
        {
            return currentString;
        }

        private void BeginFade(TextFade fade)
        {
            processing = true;
            StartCoroutine(DoFade(fade));
        }

        IEnumerator DoFade(TextFade textFade)
        {
            Text text = textFade.text;

            // Reset the text's alpha to full
            Color newColour = new Color(text.color.r, text.color.g, text.color.b, 1.0f);
            text.color = newColour;

            // Wait for the start delay
            yield return new WaitForSeconds(textFade.startDelay);

            float currentTime = 0.0f;
            while (currentTime < textFade.duration)
            {
                // Interpolate the alpha using the time passed and the total duration
                float newAlpha = Mathf.Lerp(1.0f, 0.0f, currentTime / textFade.duration);

                newColour = new Color(text.color.r, text.color.g, text.color.b, newAlpha);
                text.color = newColour;

                currentTime += Time.deltaTime;

                // Finish lerping the alpha value for this frame
                yield return null;
            }

            uiController.OnTextFadeEnded(text);

            Debug.Log("Fade done!");

            processing = false;
            currentString = "";

            yield break;
        }
    }
}