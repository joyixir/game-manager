using System;
using System.Collections.Generic;
using Joyixir.GameManager.Scripts.Utils;
using Joyixir.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Joyixir.GameManager.Scripts.Level
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;
        public static Action<LevelData> OnLevelFinish;
        public static Action OnLevelStart;
        public static Action OnLevelReady;
        public static Action<bool> OnTapping;

        [SerializeField] private int minimumLevelToLoadAfterFirstFinish = 2;
        [SerializeField] private List<BaseLevelConfig> levelsConfigs;
        [SerializeField] private BaseLevel Environment;

        private bool _levelIsReady;
        internal static BaseLevel CurrentLevel { get; set; }
        internal static BaseLevelConfig CurrentLevelConfig => CurrentLevel.LevelConfig;

        public static int PlayerLevel
        {
            get => GameManagementPlayerPrefs.PlayerLevel;
            set => GameManagementPlayerPrefs.PlayerLevel = value;
        }

        private void Awake()
        {
            if (!Instance) Instance = this;
            if (Instance != this) Destroy(gameObject);
        }

        internal void Initialize()
        {
            CreateNewLevel();
            HandleErrors();
            _levelIsReady = true;
            OnLevelReady?.Invoke();
        }

        private void HandleErrors()
        {
            if (levelsConfigs == null)
                throw new Exception("Assign some levels");
        }

        private void CreateNewLevel()
        {
            if (CurrentLevel != null)
                Destroy(CurrentLevel.gameObject);
            CurrentLevel = Instantiate(Environment, transform);
            CurrentLevel.InitializeLevel(GetLevelConfigToLoad());
        }

        internal void Skip()
        {
            ForceWin();
            Initialize();
        }

        internal void Retry()
        {
            ForceFinish();
            Initialize();
        }

        internal void ForceFinish()
        {
            CurrentLevel.ForceFinishLevel();
        }

        internal void ForceWin()
        {
            CurrentLevel.ForcedToWin = true;
            ForceFinish();
        }

        public BaseLevelConfig GetLevelConfigToLoad()
        {
            if (minimumLevelToLoadAfterFirstFinish >= levelsConfigs.Count)
                return levelsConfigs.PickRandom();

            var playerFinishedAllLevels = PlayerLevel > levelsConfigs.Count - 1;
            var levelIndex = playerFinishedAllLevels
                ? Random.Range(minimumLevelToLoadAfterFirstFinish, levelsConfigs.Count)
                : PlayerLevel;
            return levelsConfigs[levelIndex];
        }

        private void UnSubscribeFromLevel()
        {
            CurrentLevel.OnStart -= Started;
            CurrentLevel.OnFinish -= FinishLevel;
            CurrentLevel.OnTapping -= TapMode;
        }

        internal void StartLevel()
        {
            if (!_levelIsReady)
                Initialize();
            SubscribeToLevel();
            CurrentLevel.StartLevel();
        }

        private void SubscribeToLevel()
        {
            CurrentLevel.OnStart += Started;
            CurrentLevel.OnFinish += FinishLevel;
            CurrentLevel.OnTapping += TapMode;
        }

        private void TapMode(bool tappingEnabled)
        {
            OnTapping?.Invoke(tappingEnabled);
        }

        private void Started()
        {
            OnLevelStart?.Invoke();
        }

        private void FinishLevel(LevelData levelData)
        {
            UpdateScore(levelData.Score);
            FinishLevelBehaviour(levelData);
        }

        private static void UpdateScore(int score)
        {
            GameManagementPlayerPrefs.PlayerTotalScore += score;
        }

        private void FinishLevelBehaviour(LevelData levelData)
        {
            if (!_levelIsReady) return;
            if (levelData.WinStatus)
                IncreasePlayerLevel();
            UnSubscribeFromLevel();
            OnLevelFinish?.Invoke(levelData);
            _levelIsReady = false;
        }

        private int CalculateMoneyFromScore(int score)
        {
            return score;
        }

        private static void IncreasePlayerLevel()
        {
            PlayerLevel++;
        }
    }
}