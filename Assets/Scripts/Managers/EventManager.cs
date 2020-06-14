using UnityEngine;

namespace TankGame
{
    public class EventManager : Singleton<EventManager>
    {
        public delegate void OnPickupBoostItemDelegate(Vector3 position);
        public event OnPickupBoostItemDelegate OnPickupBoostItem;
        public void PickupBoostItem(Vector3 position)
        {
            if (OnPickupBoostItem != null) { OnPickupBoostItem(position); }
        }

        public delegate void OnPickupHealthItemDelegate(Vector3 position);
        public event OnPickupHealthItemDelegate OnPickupHealthItem;
        public void PickupHealthItem(Vector3 position)
        {
            if (OnPickupHealthItem != null) { OnPickupHealthItem(position); }
        }

        public delegate void OnTankDeathDelegate(GameObject gameObject);
        public event OnTankDeathDelegate OnTankDeath;
        public void TankDeath(GameObject gameObject)
        {
            if (OnTankDeath != null) { OnTankDeath(gameObject); }
        }

        public delegate void OnScoreGoalDelegate(bool enemyGoal);
        public event OnScoreGoalDelegate OnScoreGoal;
        public void ScoreGoal(bool enemyGoal)
        {
            if (OnScoreGoal != null) { OnScoreGoal(enemyGoal); }
        }

        public delegate void OnUpdateScoreDelegate(string score);
        public event OnUpdateScoreDelegate OnUpdateScore;
        public void UpdateScore(string score)
        {
            if (OnUpdateScore != null) { OnUpdateScore(score); }
        }

        public delegate void OnRoundEndDelegate();
        public event OnRoundEndDelegate OnRoundEnd;
        public void RoundEnd()
        {
            if (OnRoundEnd != null) { OnRoundEnd(); }
        }

        public delegate void OnMatchEndDelegate();
        public event OnMatchEndDelegate OnMatchEnd;
        public void MatchEnd()
        {
            if (OnMatchEnd != null) { OnMatchEnd(); }
        }

        public delegate void OnRoundBeginDelegate();
        public event OnRoundEndDelegate OnRoundBegin;
        public void RoundBegin()
        {
            if (OnRoundBegin != null) { OnRoundBegin(); }
        }

        public delegate void OnRoundPrepareDelegate();
        public event OnRoundPrepareDelegate OnRoundPrepare;
        public void RoundPrepare()
        {
            if (OnRoundPrepare != null) { OnRoundPrepare(); }
        }

        public delegate void OnSurvivalSpawnAIDelegate(float secondsToWait);
        public event OnSurvivalSpawnAIDelegate OnSurvivalSpawnAI;
        public void SurvivalSpawnAI(float secondsToWait)
        {
            if (OnSurvivalSpawnAI != null) { OnSurvivalSpawnAI(secondsToWait); }
        }
    } 
}