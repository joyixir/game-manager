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
    }
}