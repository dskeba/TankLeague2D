
using UnityEngine;

namespace TankGame
{
    public class BulletExplosion : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D hitInfo)
        {
            if (hitInfo.tag.Equals("Ball"))
            {
                Rigidbody2D rb = hitInfo.GetComponent<Rigidbody2D>();
                Vector2 myPosition = transform.position;
                Vector2 direction = rb.position - myPosition;
                rb.AddRelativeForce(direction);
                rb.velocity = direction.normalized * 28;
            }
            else if (hitInfo.tag.Equals("Tank"))
            {
                Tank tank = hitInfo.GetComponent<Tank>();
                tank.Damage(25);
                if (tank is TankPlayer)
                {
                    var virtualCameraObject = GameObject.FindWithTag("VirtualCamera");
                    var virtualCamera = virtualCameraObject.GetComponent<VirtualCamera>();
                    virtualCamera.ShakeCamera();
                }
            }
        }
    } 
}
