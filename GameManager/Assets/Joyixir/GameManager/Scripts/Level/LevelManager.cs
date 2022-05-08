using System;
using System.Collections;
using System.Collections.Generic;
using Joyixir.GameManager.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Joyixir.GameManager.Level
{
    [AddComponentMenu("Joyixir/GameManagement/LevelManager")]
    public class LevelManager : MonoBehaviour
    {
        #region Static region

        internal static LevelManager Instance => _instance;
        private static LevelManager _instance;
        public static Action<LevelData> OnLevelFinish;
        public static Action OnLevelStart;
        public static Action OnLevelReady;
        public static Action<int> OnLevelUnlocked; // Starts from 0
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
        private bool levelStarted;
        private bool levelQueuedForStart;

        #endregion

        public static int LastLoadedLevelRealIndex { get; private set; }
        
        public static int PlayerLevel
        {
            get => GameManagementPlayerPrefs.PlayerLevel;
            set => GameManagementPlayerPrefs.PlayerLevel = value;
        }

        private void Awake()
        {
            if (!Instance) _instance = this;
            if (Instance != this) Destroy(gameObject);
        }

        internal void Initialize()
        {
            CreateNewLevel();
            HandleErrors();
        }

        private void HandleErrors()
        {
            if (levelsConfigs == null)
                throw new Exception("Assign some levels");
        }

        private void CreateNewLevel()
        {
            _pickedConfig = GetLevelConfigToLoad();
            if (string.IsNullOrEmpty(_pickedConfig.SceneName) || _pickedConfig.SceneName.ToString() == SceneManager.GetActiveScene().name)
            {
                CreateLevelWithPrefab();
                BroadcastLevelReady();
            }
            else
            {
                CreateLevelWithScene();
            }
        }

        private void BroadcastLevelReady()
        {
            _levelIsReady = true;
            OnLevelReady?.Invoke();
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

        internal static int GetLevelAttempts(int levelNumber)
        {
            return GameManagementPlayerPrefs.GetLevelAttempts(levelNumber);
        }

        private void CreateLevelWithScene()
        {
            if (CurrentLevel != null)
                Destroy(CurrentLevel.gameObject);

            StartCoroutine(SceneLoadingBehavior());
            AsyncOperationBasedOnCurrentLevelScene = SceneManager.LoadSceneAsync(_pickedConfig.SceneName.ToString());
            AsyncOperationBasedOnCurrentLevelScene.completed += InitializeLevelAfterSceneLoad;
        }

        private void InitializeLevelAfterSceneLoad(AsyncOperation obj)
        {
            CreateAndInitializeLevel();
            AsyncOperationBasedOnCurrentLevelScene.completed -= InitializeLevelAfterSceneLoad;
            BroadcastLevelReady();
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

        private BaseLevelConfig GetLevelConfigToLoad()
        {
            if (levelsConfigs.Count == 0)
                throw new Exception("No levels to load");
            var levelIndexToLoad = GetLevelIndexToLoad();

            var configToLoad = levelsConfigs[levelIndexToLoad];

            if (configToLoad == null)
                throw new Exception("You assigned a null element to configs");
            LastLoadedLevelRealIndex = levelIndexToLoad;
            return configToLoad;
        }

        private int GetLevelIndexToLoad()
        {
            var index = 0;
            if (minimumLevelToLoadAfterFirstFinish < levelsConfigs.Count)
            {
                var playerFinishedAllLevels = PlayerLevel > levelsConfigs.Count - 1;
                var levelIndex = playerFinishedAllLevels
                    ? Random.Range(minimumLevelToLoadAfterFirstFinish, levelsConfigs.Count)
                    : PlayerLevel;
                index = levelIndex;
            }
            else
                index = Random.Range(0, levelsConfigs.Count);
            return index;
        }

        private void UnSubscribeFromLevel()
        {
            CurrentLevel.OnStart -= Started;
            CurrentLevel.OnFinish -= FinishLevel;
        }

        internal void StartLevelWheneverReady()
        {
            if (levelStarted) return;
            if (!_levelIsReady && levelQueuedForStart)
                return;

            if (_levelIsReady)
            {
                StartLevelNow();
            }
            else
            {
                levelQueuedForStart = true;
                OnLevelReady += StartLevelWheneverReady;
                Initialize();
            }
        }

        private void StartLevelNow()
        {
            SubscribeToLevel();
            GameManagementPlayerPrefs.AttemptLevel(CurrentLevel.LevelNumber);
            CurrentLevel.StartLevel();
            if (levelQueuedForStart)
                OnLevelReady -= StartLevelWheneverReady;
            levelStarted = true;
            levelQueuedForStart = false;
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
            levelStarted = false;
        }

        private int CalculateMoneyFromScore(int score)
        {
            return score;
        }

        private static void IncreasePlayerLevel()
        {
            PlayerLevel++;
            OnLevelUnlocked?.Invoke(PlayerLevel);
        }
    }
}