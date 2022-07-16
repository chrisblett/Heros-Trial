using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UI
{
    public class ScreenFader : MonoBehaviour
    {
        Image img;

        void Start()
        {
            img = GetComponent<Image>();
            if (img)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0.0f);
            }
            else
            {
                Debug.LogError("Failed to locate Image component reference on ScreenFader");
            }

            SetToBlack();
        }

        public void Transition(float inDuration, float wait, float outDuration)
        {
            //Debug.Log("Doing transition");

            StartCoroutine(DoFade(inDuration, wait, outDuration));
        }

        public void SetToBlack()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0) return;

            img.color = new Color(img.color.r, img.color.g, img.color.b, 1.0f);

            //Debug.Log("Setting screen to black");
        }

        private IEnumerator DoFade(float inDuration, float wait, float outDuration)
        {
            float currentTime = 0;
            while (currentTime < inDuration)
            {
                float newAlpha = Mathf.Lerp(0.0f, 1.0f, currentTime / inDuration);
                img.color = new Color(img.color.r, img.color.g, img.color.b, newAlpha);

                currentTime += Time.deltaTime;

                yield return null;
            }

            yield return new WaitForSeconds(wait);

            currentTime -= inDuration;
            while (currentTime < outDuration)
            {
                float newAlpha = Mathf.Lerp(1.0f, 0.0f, currentTime / outDuration);
                img.color = new Color(img.color.r, img.color.g, img.color.b, newAlpha);

                currentTime += Time.deltaTime;

                yield return null;
            }
        }
    }
}


