using Joyixir.GameManager.Scripts.Level;
using Joyixir.GameManager.Scripts.Types;
using UnityEngine;

namespace Joyixir.GameManager.Demo.Scripts
{
    [CreateAssetMenu(fileName = "GameManagerDemoLevel", menuName = "Joyixir/GameManager/Demo/Levels", order = 0)]
    public class MyLevelConfig : BaseLevelConfig
    {
        public GameObject Dummy;
        public GameObject SecondDummy;
        public override SceneNames SceneName => SceneNames.SimpleLevelScene;
    }
}