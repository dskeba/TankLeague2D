
using UnityEngine;

namespace TankGame
{
    public class Tank : MonoBehaviour
    {
        protected Transform healthBar;
        protected Transform boostBar;
        protected Transform tankBodyTransform;
        protected Transform tankGunTransform;
        protected GameObject trackLeft;
        protected GameObject trackRight;
        protected Rigidbody2D rigidBody;
        protected TankGun tankGun;
        protected Transform smokePoint;
        protected GameObject smoke;
        protected GameObject boostSmoke;
        protected SpriteRenderer hullSpriteRenderer;
        protected float timeBetweenSmoke = 0.1f;
        protected float timeBetweenBoostSmoke = 0.1f;
        protected float moveSpeed = 4f;
        protected float rotateSpeed = 100f;
        protected float rotationMovement = 0f;
        protected float forwardMovement = 0f;
        protected bool boostMovement = false;
        protected float maxHealth = 100f;
        protected float currentHealth = 100f;
        protected float maxBoost = 100f;
        protected float currentBoost = 100f;
        protected bool canMove = true;

        private float nextSmoke = 0f;
        private float nextBoostSmoke = 0f;
        private bool isDying = false;

        public float Health
        {
            set
            {
                currentHealth = value;
                healthBar.localScale = new Vector3(currentHealth / maxHealth, 1, 1);
            }
            get { return currentHealth; }
        }

        public float Boost
        {
            set
            {
                currentBoost = value;
                boostBar.localScale = new Vector3(currentBoost / maxBoost, 1, 1);
            }
            get { return currentBoost; }
        }

        private void Awake()
        {
            SubscribeToEvents();
        }

        protected void SetupBaseTank()
        {
            tankBodyTransform = transform.Find("TankBody");
            rigidBody = GetComponent<Rigidbody2D>();
            healthBar = transform.Find("HealthBar");
            boostBar = transform.Find("BoostBar");
            var tankHull = tankBodyTransform.Find("TankHull");
            var hull = tankHull.Find("Hull");
            hullSpriteRenderer = hull.GetComponent<SpriteRenderer>();
            tankGunTransform = tankBodyTransform.Find("TankGun");
            trackLeft = tankHull.Find("TrackLeft").gameObject;
            trackRight = tankHull.Find("TrackRight").gameObject;
            smokePoint = tankHull.Find("SmokePoint");
            smoke = Resources.Load<GameObject>("Prefabs/Smoke");
            boostSmoke = Resources.Load<GameObject>("Prefabs/BoostSmoke");
            ResetStats();
        }

        public void ResetStats()
        {
            tankBodyTransform.eulerAngles = new Vector3(0, 0, 0);
            isDying = false;
            Health = 100f;
            Boost = 100f;
        }

        private void OnDestroy()
        {
            UnsubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            EventManager.Instance.OnRoundPrepare += OnRoundPrepare;
            EventManager.Instance.OnRoundBegin += OnRoundBegin;
        }

        private void UnsubscribeToEvents()
        {
            EventManager.Instance.OnRoundPrepare -= OnRoundPrepare;
            EventManager.Instance.OnRoundBegin -= OnRoundBegin;
        }

        private void OnRoundPrepare()
        {
            canMove = false;
        }

        private void OnRoundBegin()
        {
            canMove = true;
        }

        public float Damage(float damageAmount)
        {
            if (isDying) { return this.Health; }
            Health -= damageAmount;
            if (Health <= 0)
            {
                isDying = true;
                EventManager.Instance.TankDeath(gameObject);
                Destroy(gameObject);
                SoundManager.Instance.Play(MixerGroup.Sound, "Sounds/TankDeath");
            }
            return this.Health;
        }

        public void FixedUpdate()
        {
            if (!canMove) { return; }
            if (forwardMovement != 0)
            {
                TimedSmoke();
                AnimateTracks();
            }
            if (boostMovement && Boost > 0)
            {
                TimedBoostSmoke();
                moveSpeed = 8f;
                Boost -= 1;
            } else
            {
                moveSpeed = 4f;
            }
            tankBodyTransform.Rotate(0, 0, Time.deltaTime * -rotationMovement * rotateSpeed, Space.Self);
            transform.Translate(tankBodyTransform.transform.up * Time.deltaTime * forwardMovement * moveSpeed, Space.World);
        }

        private void TimedSmoke()
        {
            if (Time.time > nextSmoke)
            {
                nextSmoke = Time.time + timeBetweenSmoke;
                EmitSmoke();
            }
        }

        private void EmitSmoke()
        {
            SoundManager.Instance.Play(MixerGroup.Sound, "Sounds/TankEngine", 0.45f);
            var newSmoke = Instantiate(smoke, smokePoint.position, smokePoint.rotation);
            Destroy(newSmoke, 1f);
        }

        private void TimedBoostSmoke()
        {
            if (Time.time > nextBoostSmoke)
            {
                nextBoostSmoke = Time.time + timeBetweenBoostSmoke;
                EmitBoostSmoke();
            }
        }

        private void EmitBoostSmoke()
        {
            var newBoostSmoke = Instantiate(boostSmoke, smokePoint.position, smokePoint.rotation);
            Destroy(newBoostSmoke, 1f);
        }

        public void AnimateTracks()
        {
            if (forwardMovement == 0)
            {
                trackLeft.GetComponent<Animator>().enabled = false;
                trackRight.GetComponent<Animator>().enabled = false;
            }
            else
            {
                trackLeft.GetComponent<Animator>().enabled = true;
                trackRight.GetComponent<Animator>().enabled = true;
            }
        }
    } 
}
