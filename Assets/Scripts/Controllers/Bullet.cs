
using UnityEngine;

namespace TankGame
{
    public class Bullet : MonoBehaviour
    {
        public int damage = 20;
        public float speed = 20f;
        public Rigidbody2D rb;
        public GameObject hitEffect;
        private bool hasDestination = false;
        public Vector3 destination;
        public Animator cameraAnimator;

        public void SetDestinationGoal(Vector3 destinationGoal)
        {
            hasDestination = true;
            destination = destinationGoal;
        }

        void OnEnable()
        {
            rb.velocity = transform.up * speed;
        }

        private void FixedUpdate()
        {
            if (hasDestination)
            {
                CheckForDestinationCollision();
            }
        }

        private void CheckForDestinationCollision()
        {
            if (transform.position.x > destination.x - 0.5 &&
                transform.position.x < destination.x + 0.5 &&
                transform.position.y > destination.y - 0.5 &&
                transform.position.y < destination.y + 0.5)
            {
                doCollide();
            }
        }

        void OnTriggerEnter2D(Collider2D hitInfo)
        {
            if (hitInfo.tag.Equals("Map") ||
                hitInfo.tag.Equals("Ball") ||
                hitInfo.tag.Equals("Tank"))
            {
                doCollide();
            }
        }

        void doCollide()
        {
            GameObject bulletExplosionObject = ObjectPoolManager.Instance.Get("Prefabs/BulletExplosion");
            if (!bulletExplosionObject) { return; }
            bulletExplosionObject.transform.position = transform.position;
            bulletExplosionObject.transform.rotation = transform.rotation;
            bulletExplosionObject.SetActive(true);
            ObjectPoolManager.Instance.Return("Prefabs/BulletExplosion", bulletExplosionObject, 0.25f);
            ObjectPoolManager.Instance.Return("Prefabs/Bullet", gameObject);
            SoundManager.Instance.Play(MixerGroup.Sound, "Sounds/Explosion");
        }
    }
}