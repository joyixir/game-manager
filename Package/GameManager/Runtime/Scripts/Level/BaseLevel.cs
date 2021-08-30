using System;
using UnityEngine;

namespace Joyixir.GameManager.Scripts.Level
{
    public abstract class BaseLevel : MonoBehaviour
    {
        internal Action OnStart;
        internal Action<float> OnSatisfactionChanged;
        internal Action<LevelData> OnFinish;
        internal Action<bool> OnTapping;

        protected abstract void SubscribeToLevelRelatedEvents();
        protected abstract void UnSubscribeFromLevelRelatedEvents();

        protected bool forcedToFinish;
        internal bool ForcedToFinish => forcedToFinish;
        internal bool ForcedToWin;
        protected BaseLevelConfig _levelConfig;
        internal BaseLevelConfig LevelConfig => _levelConfig;


        public virtual void InitializeLevel(BaseLevelConfig config)
        {
            _levelConfig = config;
            FillVariables();
            PrepareUIRequirementsabstract();
        }

        protected virtual void FillVariables()
        {
        }

        protected internal virtual void StartLevel()
        {
            SubscribeToLevelRelatedEvents();
            OnStart?.Invoke();
        }

        protected internal virtual void FinishLevel()
        {
            UnSubscribeFromLevelRelatedEvents();
            OnFinish?.Invoke(LevelData.GenerateFromLevel(this));
        }

        protected void SatisfactionChanged()
        {
            OnSatisfactionChanged?.Invoke(Mathf.Clamp(CalculateSatisfaction(), 0, 1f));
        }

        protected internal abstract int CalculateEarnedMoney();

        protected internal abstract int CalculateScore();

        protected internal abstract float CalculateSatisfaction();

        protected internal abstract bool IsWon();

        protected internal abstract void PrepareUIRequirementsabstract();


        internal void ForceFinishLevel()
        {
            forcedToFinish = true;
            FinishLevel();
        }
    }

    public class LevelData
    {
        public int EarnedMoney;
        public bool Forced;
        public float Satisfaction;
        public int Score;
        public bool WinStatus;

        public static LevelData GenerateFromLevel(BaseLevel level)
        {
            var levelData = new LevelData();
            levelData.EarnedMoney = level.CalculateEarnedMoney();
            levelData.Score = level.CalculateScore();
            levelData.WinStatus = level.IsWon() || level.ForcedToWin;
            levelData.Satisfaction = level.ForcedToWin ? 1f : level.CalculateSatisfaction();
            levelData.Forced = level.ForcedToFinish;
            return levelData;
        }

        public override string ToString()
        {
            var export = $"EarnedMoney: {EarnedMoney}, Score: {Score}, WinStatus: {WinStatus}, Satisfaction: {Satisfaction}, Forced: {Forced}";
            return export;
        }
    }
}