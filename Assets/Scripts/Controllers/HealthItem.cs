
using UnityEngine;

namespace TankGame
{
    class HealthItem : MonoBehaviour
    {

        void OnTriggerEnter2D(Collider2D hitInfo)
        {
            if (hitInfo.tag == "Tank")
            {
                EventManager.Instance.PickupHealthItem(gameObject.transform.position);
                Tank tank = hitInfo.GetComponent<Tank>();
                tank.Health = 100;
                SoundManager.Instance.Play(MixerGroup.Sound, "Sounds/PickUpHealth");
                Destroy(gameObject);
            }
        }

    }
}
