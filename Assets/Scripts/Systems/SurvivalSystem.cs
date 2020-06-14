using System.Collections;
using UnityEngine;

namespace TankGame
{
    public class SurvivalSystem : MonoBehaviour
    {
        public string SceneName;

        private GameObject spawnPointTankPlayer;
        private GameObject[] spawnPointsTankAI;
        private GameObject tankPlayerObject;
        private GameObject tankAIObject;

        private void Start()
        {
            SceneLoadManager.Instance.SetActiveScene(SceneName);
            SceneLoadManager.Instance.LoadScene("UI", UnityEngine.SceneManagement.LoadSceneMode.Additive).completed += OnUISceneLoadCompleted;
            SubscribeToEvents();
            SpawnGameObjects();
        }

        private void OnUISceneLoadCompleted(AsyncOperation asyncOperation)
        {
            EventManager.Instance.RoundPrepare();
        }

        public void OnDestroy()
        {
            UnsubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            EventManager.Instance.OnTankDeath += OnTankDeath;
            EventManager.Instance.OnRoundPrepare += OnRoundPrepare;
            EventManager.Instance.OnRoundEnd += OnRoundEnd;
            EventManager.Instance.OnMatchEnd += OnMatchEnd;
            EventManager.Instance.OnPickupHealthItem += OnPickupHealthItem;
            EventManager.Instance.OnPickupBoostItem += OnPickupBoostItem;
            EventManager.Instance.OnSurvivalSpawnAI += OnSurvivalSpawnAI;
            EventManager.Instance.OnRoundBegin += OnRoundBegin;
        }

        private void UnsubscribeToEvents()
        {
            EventManager.Instance.OnTankDeath -= OnTankDeath;
            EventManager.Instance.OnRoundPrepare -= OnRoundPrepare;
            EventManager.Instance.OnRoundEnd -= OnRoundEnd;
            EventManager.Instance.OnMatchEnd -= OnMatchEnd;
            EventManager.Instance.OnPickupHealthItem -= OnPickupHealthItem;
            EventManager.Instance.OnPickupBoostItem -= OnPickupBoostItem;
            EventManager.Instance.OnSurvivalSpawnAI -= OnSurvivalSpawnAI;
            EventManager.Instance.OnRoundBegin -= OnRoundBegin;
        }

        private void SpawnGameObjects()
        {
            spawnPointTankPlayer = GameObject.FindGameObjectWithTag("SpawnPointTankPlayer");
            SpawnPlayerTank(spawnPointTankPlayer.transform.position);

            spawnPointsTankAI = GameObject.FindGameObjectsWithTag("SpawnPointTankAI");

            var healthSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPointHealth");
            foreach (GameObject healthSpawnPoint in healthSpawnPoints)
            {
                SpawnHealthItem(healthSpawnPoint.transform.position);
            }

            var boostSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPointBoost");
            foreach (GameObject boostSpawnPoint in boostSpawnPoints)
            {
                SpawnBoostItem(boostSpawnPoint.transform.position);
            }
        }

        private void OnRoundBegin()
        {
            EventManager.Instance.SurvivalSpawnAI(6f);
        }

        private void OnSurvivalSpawnAI(float secondsToWait)
        {
            int randomNum = Random.Range(0, spawnPointsTankAI.Length);
            SpawnAITank(spawnPointsTankAI[randomNum].transform.position);
            StartCoroutine(SpawnAITankAfterSeconds(secondsToWait));
        }

        private IEnumerator SpawnAITankAfterSeconds(float secondsToWait)
        {
            yield return new WaitForSeconds(secondsToWait);
            EventManager.Instance.SurvivalSpawnAI(secondsToWait);
        }

        private void SpawnHealthItem(Vector3 position)
        {
            var healthItemPrefab = Resources.Load<GameObject>("Prefabs/HealthItem");
            Instantiate(healthItemPrefab, position, Quaternion.identity);
        }

        private void SpawnBoostItem(Vector3 position)
        {
            var boostItemPrefab = Resources.Load<GameObject>("Prefabs/BoostItem");
            Instantiate(boostItemPrefab, position, Quaternion.identity);
        }

        private void SpawnPlayerTank(Vector3 position)
        {
            var tankPlayerPrefab = Resources.Load<GameObject>("Prefabs/Tank");
            tankPlayerObject = Instantiate(tankPlayerPrefab, position, Quaternion.identity);
            tankPlayerObject.AddComponent<TankPlayer>();
        }

        private void SpawnAITank(Vector3 position)
        {
            var tankAIPrefab = Resources.Load<GameObject>("Prefabs/Tank");
            tankAIObject = Instantiate(tankAIPrefab, position, Quaternion.identity);
            var tankAI = tankAIObject.AddComponent<TankAI>();
            tankAI.target = tankPlayerObject;
        }

        private void SpawnPickupEffect(Vector3 position)
        {
            var pickupEffectPrefab = Resources.Load<GameObject>("Prefabs/PickupEffect");
            var pickupEffectGameObject = Instantiate(pickupEffectPrefab, position, Quaternion.identity);
            Destroy(pickupEffectGameObject, 2f);
        }

        private void OnPickupBoostItem(Vector3 position)
        {
            SpawnPickupEffect(position);
            StartCoroutine(RespawnBoostItemAfterSeconds(position, 10.0f));
        }

        private IEnumerator RespawnBoostItemAfterSeconds(Vector3 position, float secondsToWait)
        {
            yield return new WaitForSeconds(secondsToWait);
            SpawnBoostItem(position);
        }

        private void OnPickupHealthItem(Vector3 position)
        {
            SpawnPickupEffect(position);
            StartCoroutine(RespawnHealthItemAfterSeconds(position, 10.0f));
        }

        private IEnumerator RespawnHealthItemAfterSeconds(Vector3 position, float secondsToWait)
        {
            yield return new WaitForSeconds(secondsToWait);
            SpawnHealthItem(position);
        }

        private void OnTankDeath(GameObject gameObject)
        {
            SpawnTankExplosion(gameObject.transform.position);
            if (gameObject.GetComponent<TankPlayer>())
            {
                EventManager.Instance.RoundEnd();
            } else if (gameObject.GetComponent<TankAI>())
            {
                GameManager.Instance.survivalScore++;
                EventManager.Instance.UpdateScore(GameManager.Instance.survivalScore.ToString());
            }
        }

        private void SpawnTankExplosion(Vector3 position)
        {
            var tankExplosionGameObjectPrefab = Resources.Load<GameObject>("Prefabs/TankExplosion");
            var tankExplosionObject = Instantiate(tankExplosionGameObjectPrefab, position, Quaternion.identity);
            Destroy(tankExplosionObject, 1f);
        }

        private void OnRoundEnd()
        {
            Invoke("DelayedMatchEnd", 1f);
        }

        private void DelayedMatchEnd()
        {
            EventManager.Instance.MatchEnd();
        }

        private void OnRoundPrepare()
        {
            EventManager.Instance.UpdateScore("0");
            var tankPlayer = tankPlayerObject.GetComponent<Tank>();
            tankPlayer.ResetStats();
            tankPlayerObject.transform.position = spawnPointTankPlayer.transform.position;
            tankPlayerObject.transform.rotation = Quaternion.identity;
            StartCoroutine(Coroutines.WaitForSecondsCoroutine(EventManager.Instance.RoundBegin, 4));
        }

        private void OnMatchEnd()
        {
            SceneLoadManager.Instance.LoadScene("Menu", UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
            {
                SpawnManager.Instance.UnloadPools();
            });
        }

    }
}
