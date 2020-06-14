
using UnityEngine;

namespace TankGame
{
    public class TankGun : MonoBehaviour
    {
        public GameObject bullet;
        public Transform firePoint;
        public float timeBetweenShots = 1f;
        public bool canShoot = true;

        private float nextShot = 0f;

        private void Awake()
        {
            SubscribeToEvents();
            SetupTankGun();
        }

        private void SetupTankGun()
        {
            firePoint = transform.Find("FirePoint");
        }

        private void OnDestroy()
        {
            UnsubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            EventManager.Instance.OnRoundPrepare += OnRoundPrepare;
            EventManager.Instance.OnRoundBegin += OnRoundBegin;
            EventManager.Instance.OnScoreGoal += OnScoreGoal;
        }

        private void UnsubscribeToEvents()
        {
            EventManager.Instance.OnRoundPrepare -= OnRoundPrepare;
            EventManager.Instance.OnRoundBegin -= OnRoundBegin;
            EventManager.Instance.OnScoreGoal -= OnScoreGoal;
        }

        private void OnRoundPrepare()
        {
            canShoot = false;
        }

        private void OnRoundBegin()
        {
            canShoot = true;
        }

        private void OnScoreGoal(bool enemyGoal)
        {
            canShoot = false;
        }

        protected void LookAtPoint(Vector3 point)
        {
            Vector2 direction = new Vector2(
                point.x - transform.position.x,
                point.y - transform.position.y
            );
            transform.up = direction;
        }

        public void Shoot(Vector3 destination)
        {
            GameObject bulletObject = ObjectPoolManager.Instance.Get("Prefabs/Bullet");
            if (!bulletObject) { return; }
            bulletObject.transform.position = firePoint.position;
            bulletObject.transform.rotation = firePoint.rotation;
            Bullet bullet = bulletObject.GetComponent<Bullet>();
            bullet.SetDestinationGoal(destination);
            bullet.gameObject.SetActive(true);
            SoundManager.Instance.Play(MixerGroup.Sound, "Sounds/BulletFire");
        }

        public void TimedShoot(Vector3 destination)
        {
            if (!canShoot)
            {
                return;
            }

            if (Time.time > nextShot)
            {
                nextShot = Time.time + timeBetweenShots;
                Shoot(destination);
            }
        }
    } 
}
