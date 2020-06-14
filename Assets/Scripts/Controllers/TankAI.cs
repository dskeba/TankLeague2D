
using Pathfinding;
using UnityEngine;

namespace TankGame
{
    public class TankAI : Tank
    {
        public GameObject target;

        private float nextWaypointDistance = 1f;
        private Path path;
        private int currentWaypoint = 0;
        private Seeker seeker;

        public void Start()
        {
            SetupBaseTank();
            SetupTankAI();
        }

        private void SetupTankAI()
        {
            var tankGunAI = base.tankGunTransform.gameObject.AddComponent<TankGunAI>();
            base.hullSpriteRenderer.color = Color.red;
            tankGunAI.target = target;
            base.tankGun = tankGunAI;

            seeker = base.gameObject.AddComponent<Seeker>();
            InvokeRepeating("UpdatePath", 0f, 0.5f);
        }

        private void UpdatePath()
        {
            if (seeker.IsDone() && target != null)
            {
                seeker.StartPath(rigidBody.position, target.transform.position, OnPathComplete);
            }
        }

        private float CalculateDistanceToTarget()
        {
            return Vector2.Distance(tankBodyTransform.position, target.transform.position);
        }

        private void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                path = p;
                currentWaypoint = 0;
            }
        }

        new public void FixedUpdate()
        {
            CalculateMovement();
            base.FixedUpdate();
        }

        private void CalculateMovement()
        {
            if (path == null || target == null || currentWaypoint >= path.vectorPath.Count) {
                return;
            }

            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rigidBody.position).normalized;

            float deltaAngle = Vector2.SignedAngle(direction, tankBodyTransform.transform.up);

            float distance = Vector2.Distance(rigidBody.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            if (deltaAngle > 1)
            {
                base.rotationMovement = 1;
            }
            else if (deltaAngle < -1)
            {
                base.rotationMovement = -1;
            }
            else
            {
                base.rotationMovement = 0;
            }

            float distanceToTarget = CalculateDistanceToTarget();

            if (distanceToTarget > 10)
            {
                if (Mathf.Abs(deltaAngle) < 45)
                {
                    base.boostMovement = true;
                } 
                else
                {
                    base.boostMovement = false;
                }
                base.forwardMovement = 1;
            }
            else
            {
                if (HasShotOnTarget())
                {
                    base.tankGun.TimedShoot(target.transform.position);
                }
            }

        }

        private bool HasShotOnTarget()
        {
            Vector2 direction = (target.transform.position - base.tankGun.firePoint.position).normalized;
            Debug.DrawRay(base.tankGun.firePoint.position, direction * 10, Color.blue);
            RaycastHit2D hit = Physics2D.Raycast(base.tankGun.firePoint.position, direction);
            return hit.collider != null && hit.collider.tag == target.tag;
        }
    } 
}