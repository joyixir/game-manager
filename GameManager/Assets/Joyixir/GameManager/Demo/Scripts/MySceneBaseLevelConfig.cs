using Joyixir.GameManager.Scripts.Level;
using Joyixir.GameManager.Scripts.Types;
using UnityEngine;

namespace Joyixir.GameManager.Demo.Scripts
{
    [CreateAssetMenu(fileName = "GameManagerSceneBaseDemoLevel", menuName = "Joyixir/GameManager/Demo/GameManagerSceneBaseDemoLevel", order = 0)]
    public class MySceneBaseLevelConfig : BaseLevelConfig
    {
        public override SceneNames SceneName => SceneNames.LevelScene;
        public GameObject Dummy;
        public GameObject SecondDummy;
    }
}