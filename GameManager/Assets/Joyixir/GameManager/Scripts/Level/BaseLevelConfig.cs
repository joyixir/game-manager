using Joyixir.GameManager.Scripts.Types;
using UnityEngine;

namespace Joyixir.GameManager.Scripts.Level
{
    public abstract class BaseLevelConfig : ScriptableObject
    {
        public abstract SceneNames SceneName { get; }
        public BaseLevel yourLevelPrefab;

    }
}