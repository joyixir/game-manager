using System;
using System.Collections;
using System.Collections.Generic;
using Joyixir.GameManager.Scripts.Utils;
using Joyixir.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Joyixir.GameManager.Scripts.Level
{
    [AddComponentMenu("Joyixir/GameManagement/LevelManager")]
    public class LevelManager : MonoBehaviour
    {
        #region Static region

        public static LevelManager Instance;
        public static Action<LevelData> OnLevelFinish;
        public static Action OnLevelStart;
        public static Action OnLevelReady;
        public static Action<float> OnSceneChangeProgressChanged;

        internal static BaseLevel CurrentLevel { get; set; }
        private static AsyncOperation AsyncOperationBasedOnCurrentLevelScene { get; set; }
        internal static BaseLevelConfig CurrentLevelConfig => CurrentLevel.LevelConfig;

        #endregion

        #region Serialized

        [SerializeField] private int minimumLevelToLoadAfterFirstFinish = 2;
        [SerializeField] private List<BaseLevelConfig> levelsConfigs;

        #endregion

        #region Private region

        private bool _levelIsReady;
        private BaseLevelConfig _pickedConfig;

        #endregion


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
        }

        private void HandleErrors()
        {
            if (levelsConfigs == null)
                throw new Exception("Assign some levels");
        }

        private void CreateNewLevel()
        {
            _pickedConfig = GetLevelConfigToLoad();
            if (_pickedConfig.SceneName.ToString() == SceneManager.GetActiveScene().name)
            {
                CreateLevelWithPrefab();
                OnLevelReady?.Invoke();
            }
            else
            {
                CreateLevelWithScene();
            }
        }

        private void CreateLevelWithPrefab()
        {
            if (CurrentLevel != null)
                Destroy(CurrentLevel.gameObject);
            CreateAndInitializeLevel();
        }

        private void CreateAndInitializeLevel()
        {
            CurrentLevel = Instantiate(_pickedConfig.yourLevelPrefab, transform);
            CurrentLevel.InitializeLevel(_pickedConfig);
        }

        private void CreateLevelWithScene()
        {
            if (CurrentLevel != null)
                Destroy(CurrentLevel.gameObject);


            AsyncOperationBasedOnCurrentLevelScene = SceneManager.LoadSceneAsync(_pickedConfig.SceneName.ToString());
            AsyncOperationBasedOnCurrentLevelScene.completed += InitializeLevelAfterSceneLoad;
        }

        private void InitializeLevelAfterSceneLoad(AsyncOperation obj)
        {
            CreateAndInitializeLevel();
            AsyncOperationBasedOnCurrentLevelScene.completed -= InitializeLevelAfterSceneLoad;
            OnLevelReady?.Invoke();
        }

        // This Function use for Loading view when scene changing.
        // UI view can listen to OnSceneChangeProgressChanged action.
        // This function do not tested just.
        private IEnumerator SceneLoadingBehavior()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                OnSceneChangeProgressChanged?.Invoke(AsyncOperationBasedOnCurrentLevelScene.progress);
                if (AsyncOperationBasedOnCurrentLevelScene.isDone)
                    break;
            }
        }

        internal void Skip()
        {
            ForceWin();
            Initialize();
        }

        internal void Retry()
        {
            CurrentLevel.ForceFinishLevel(LevelFinishStatus.Retry);
            Initialize();
        }

        internal void ForceFinish()
        {
            CurrentLevel.ForceFinishLevel();
        }

        internal void ForceWin()
        {
            CurrentLevel.ForceFinishLevel(LevelFinishStatus.Win);
        }

        internal void ForceLose()
        {
            CurrentLevel.ForceFinishLevel(LevelFinishStatus.Lose);
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