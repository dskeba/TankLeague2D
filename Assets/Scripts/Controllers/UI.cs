using System.Collections;
using TMPro;
using UnityEngine;

namespace TankGame
{
    public class UI : MonoBehaviour
    {
        private TextMeshProUGUI scoreText;
        private TextMeshProUGUI countdownText;
        private int countdownTime = 3;

        private void Awake()
        {
            GetComponents();
            SubscribeToEvents();
        }

        private void GetComponents()
        {
            GameObject scoreGameObject = GameObject.FindGameObjectWithTag("Score");
            scoreText = scoreGameObject.GetComponent<TextMeshProUGUI>();

            GameObject countdownGameObject = GameObject.FindGameObjectWithTag("Countdown");
            countdownText = countdownGameObject.GetComponent<TextMeshProUGUI>();
            countdownText.gameObject.SetActive(false);
        }

        private void SubscribeToEvents()
        {
            EventManager.Instance.OnUpdateScore += OnUpdateScore;
            EventManager.Instance.OnMatchEnd += OnMatchEnd;
            EventManager.Instance.OnRoundPrepare += OnRoundPrepare;
        }

        private void UnsubscribeToEvents()
        {
            EventManager.Instance.OnUpdateScore -= OnUpdateScore;
            EventManager.Instance.OnMatchEnd -= OnMatchEnd;
            EventManager.Instance.OnRoundPrepare -= OnRoundPrepare;
        }

        private void OnUpdateScore(string score)
        {
            scoreText.SetText(score);
        }

        private void OnRoundPrepare()
        {
            StartCoroutine(CountdownToStart());
        }

        private void OnMatchEnd()
        {
            UnsubscribeToEvents();
        }

        IEnumerator CountdownToStart()
        {
            countdownText.gameObject.SetActive(true);
            while (countdownTime > 0)
            {
                countdownText.text = countdownTime.ToString();
                yield return new WaitForSeconds(1f);
                countdownTime--;
            }
            countdownText.text = "GO!";
            yield return new WaitForSeconds(1f);
            countdownText.gameObject.SetActive(false);
            countdownTime = 3;
        }
    } 
}
