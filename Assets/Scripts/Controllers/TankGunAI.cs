
using UnityEngine;

namespace TankGame
{
    public class TankGunAI : TankGun
    {
        public GameObject target;

        void FixedUpdate()
        {
            LookAtBall();
        }

        void LookAtBall()
        {
            if (target == null) { return; }
            Vector3 targetPosition = target.transform.position;
            base.LookAtPoint(targetPosition);
        }
    }
}
 