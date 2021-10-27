using Joyixir.GameManager.Scripts.Level;
using UnityEngine;

namespace Joyixir.GameManager.Demo.Scripts
{
    [CreateAssetMenu(fileName = "GameManagerSecondSceneBaseDemoLevel", menuName = "Joyixir/GameManager/Demo/GameManagerSecondSceneBaseDemoLevel", order = 0)]
    public class MySecondSceneBaseLevelConfig : BaseLevelConfig
    {
        public override string SceneName => "SecondSceneBasedLevelScene";
        public GameObject Dummy;
        public GameObject SecondDummy;
    }
}