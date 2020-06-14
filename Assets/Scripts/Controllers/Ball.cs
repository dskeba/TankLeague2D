
using UnityEngine;

namespace TankGame
{
    public class Ball : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D hitInfo)
        {
            if (hitInfo.tag.Equals("EnemyGoal"))
            {
                ScoreGoal(hitInfo, true);
            }
            else if (hitInfo.tag.Equals("PlayerGoal"))
            {
                ScoreGoal(hitInfo, false);
            }
        }

        private void ScoreGoal(Collider2D hitInfo, bool enemyGoal)
        {
            EventManager.Instance.ScoreGoal(enemyGoal);
        }
    }
}