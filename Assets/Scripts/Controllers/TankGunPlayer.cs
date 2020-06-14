
using UnityEngine;

namespace TankGame
{
    public class TankGunPlayer : TankGun
    {
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                base.TimedShoot(GetCurrentCursorPosition());
            }
        }

        void FixedUpdate()
        {
            LookAtMouse();
        }

        void LookAtMouse()
        {
            Vector3 mousePosition = GetCurrentCursorPosition();
            base.LookAtPoint(mousePosition);
        }

        Vector3 GetCurrentCursorPosition()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    } 
}
