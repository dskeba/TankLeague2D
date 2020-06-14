using System.Collections;
using UnityEngine;

namespace TankGame
{
    public class SoccerSystem : MonoBehaviour
    {
        public string SceneName;

        private GameObject spawnPointBall;
        private GameObject spawnPointTankPlayer;
        private GameObject spawnPointTankAI;
        private GameObject ballGameObject;
        private GameObject tankPlayerObject;
        private GameObject tankAIObject;
        private GameObject goalExplosionGameObject;

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
            EventManager.Instance.UpdateScore("0 : 0");
        }

        public void OnDestroy()
        {
            UnsubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            EventManager.Instance.OnTankDeath += OnTankDeath;
            EventManager.Instance.OnRoundEnd += OnRoundEnd;
            EventManager.Instance.OnRoundPrepare += OnRoundPrepare;
            EventManager.Instance.OnMatchEnd += OnMatchEnd;
            EventManager.Instance.OnScoreGoal += OnScoreGoal;
            EventManager.Instance.OnPickupHealthItem += OnPickupHealthItem;
            EventManager.Instance.OnPickupBoostItem += OnPickupBoostItem;
        }

        private void UnsubscribeToEvents()
        {
            EventManager.Instance.OnTankDeath -= OnTankDeath;
            EventManager.Instance.OnRoundEnd -= OnRoundEnd;
            EventManager.Instance.OnRoundPrepare -= OnRoundPrepare;
            EventManager.Instance.OnMatchEnd -= OnMatchEnd;
            EventManager.Instance.OnScoreGoal -= OnScoreGoal;
            EventManager.Instance.OnPickupHealthItem -= OnPickupHealthItem;
            EventManager.Instance.OnPickupBoostItem -= OnPickupBoostItem;
        }

        private void SpawnGameObjects()
        {
            spawnPointBall = GameObject.FindGameObjectWithTag("SpawnPointBall");
            SpawnBall(spawnPointBall.transform.position);

            spawnPointTankPlayer = GameObject.FindGameObjectWithTag("SpawnPointTankPlayer");
            SpawnPlayerTank(spawnPointTankPlayer.transform.position);

            spawnPointTankAI = GameObject.FindGameObjectWithTag("SpawnPointTankAI");
            SpawnAITank(spawnPointTankAI.transform.position);

            var healthSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPointHealth");
            foreach(GameObject healthSpawnPoint in healthSpawnPoints)
            {
                SpawnHealthItem(healthSpawnPoint.transform.position);
            }

            var boostSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPointBoost");
            foreach (GameObject boostSpawnPoint in boostSpawnPoints)
            {
                SpawnBoostItem(boostSpawnPoint.transform.position);
            }
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

        private void SpawnBall(Vector3 position)
        {
            var ballGameObjectPrefab = Resources.Load<GameObject>("Prefabs/Ball");
            ballGameObject = Instantiate(ballGameObjectPrefab, position, Quaternion.identity);
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
            tankAI.target = ballGameObject;
        }

        private void SpawnGoalExplosion()
        {
            var goalExplosionGameObjectPrefab = Resources.Load<GameObject>("Prefabs/GoalExplosion");
            goalExplosionGameObject = Instantiate(goalExplosionGameObjectPrefab,
                ballGameObject.transform.position, Quaternion.identity);
            Destroy(goalExplosionGameObject, 1f);
            SoundManager.Instance.Play(MixerGroup.Sound, "Sounds/GoalExplosion");
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
                SpawnPlayerTank(spawnPointTankPlayer.transform.position);
            } else if (gameObject.GetComponent<TankAI>())
            {
                SpawnAITank(spawnPointTankAI.transform.position);
            }
        }

        private void SpawnTankExplosion(Vector3 position)
        {
            var tankExplosionGameObjectPrefab = Resources.Load<GameObject>("Prefabs/TankExplosion");
            var tankExplosionObject = Instantiate(tankExplosionGameObjectPrefab, position, Quaternion.identity);
            Destroy(tankExplosionObject, 1f);
        }

        private void OnScoreGoal(bool enemyGoal)
        {
            if (enemyGoal)
            {
                GameManager.Instance.playerScore += 1;
            }
            else
            {
                GameManager.Instance.enemyScore += 1;
            }
            EventManager.Instance.UpdateScore(GameManager.Instance.enemyScore + " : " + GameManager.Instance.playerScore);
            SpawnGoalExplosion();
            ballGameObject.SetActive(false);
            StartCoroutine(Coroutines.WaitForSecondsCoroutine(EventManager.Instance.RoundEnd, 3));
        }

        private void OnRoundEnd()
        {
            if (GameManager.Instance.playerScore > 1 || GameManager.Instance.enemyScore > 1)
            {
                Invoke("DelayedMatchEnd", 1f);
                return;
            }
            EventManager.Instance.RoundPrepare();
        }

        private void DelayedMatchEnd()
        {
            EventManager.Instance.MatchEnd();
        }

        private void OnRoundPrepare()
        {
            var tankPlayer = tankPlayerObject.GetComponent<Tank>();
            var tankAI = tankAIObject.GetComponent<Tank>();
            tankPlayer.ResetStats();
            tankAI.ResetStats();
            ballGameObject.transform.position = spawnPointBall.transform.position;
            ballGameObject.SetActive(true);
            tankPlayerObject.transform.position = spawnPointTankPlayer.transform.position;
            tankPlayerObject.transform.rotation = Quaternion.identity;
            tankAIObject.transform.position = spawnPointTankAI.transform.position;
            tankAIObject.transform.rotation = Quaternion.identity;
            StartCoroutine(Coroutines.WaitForSecondsCoroutine(EventManager.Instance.RoundBegin, 4));
        }

        private void OnMatchEnd()
        {
            GameManager.Instance.enemyScore = 0;
            GameManager.Instance.playerScore = 0;
            SceneLoadManager.Instance.LoadScene("Menu", UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
            {
                SpawnManager.Instance.UnloadPools();
            });
        }

    } 
}
