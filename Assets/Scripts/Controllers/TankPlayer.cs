
using UnityEngine;

namespace TankGame
{
    public class TankPlayer : Tank
    {
        public void Start()
        {
            SetupBaseTank();
            SetupTankPlayer();
        }

        private void SetupTankPlayer()
        {
            base.tankGun = base.tankGunTransform.gameObject.AddComponent<TankGunPlayer>();
            base.hullSpriteRenderer.color = Color.cyan;
        }

        void Update()
        {
            base.rotationMovement = Input.GetAxisRaw("Horizontal");
            base.forwardMovement = Input.GetAxisRaw("Vertical");
            base.boostMovement = Input.GetMouseButton(1);
        }

        new public void FixedUpdate()
        {
            base.FixedUpdate();
        }
    } 
}