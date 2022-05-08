using System;
using UnityEngine;

namespace Joyixir.GameManager.Utils
{
    internal static class GameManagementPlayerPrefs
    {
        public static int PlayerLevel
        {
            set => PlayerPrefs.SetInt("Joyixir_CurrentLevel", value);
            get => PlayerPrefs.GetInt("Joyixir_CurrentLevel", 0);
        }

        public static int PlayerTotalScore
        {
            set => PlayerPrefs.SetInt("Joyixir_TotalScore", value);
            get => PlayerPrefs.GetInt("Joyixir_TotalScore", 0);
        }

        public static int GetLevelAttempts(int levelNumber)
        {
            return PlayerPrefs.GetInt($"Joyixir_Level_{levelNumber}_Attempts", 0);
        }

        public static void AttemptLevel(int levelNumber)
        {
            var newAttempt = GetLevelAttempts(levelNumber) + 1;
            PlayerPrefs.SetInt($"Joyixir_Level_{levelNumber}_Attempts", newAttempt);
        }
    }
}