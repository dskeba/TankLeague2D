
using UnityEngine;

namespace TankGame
{
    class BoostItem : MonoBehaviour
    {

        void OnTriggerEnter2D(Collider2D hitInfo)
        {
            if (hitInfo.tag == "Tank")
            {
                EventManager.Instance.PickupBoostItem(gameObject.transform.position);
                Tank tank = hitInfo.GetComponent<Tank>();
                tank.Boost = 100;
                SoundManager.Instance.Play(MixerGroup.Sound, "Sounds/PickUpHealth");
                Destroy(gameObject);
            }
        }

    }
}
