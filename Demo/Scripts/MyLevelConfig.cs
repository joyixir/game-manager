using Joyixir.GameManager.Scripts.Level;
using UnityEngine;

namespace Joyixir.GameManager.Demo.Scripts
{
    [CreateAssetMenu(fileName = "GameManagerDemoLevel", menuName = "Joyixir/GameManager/Demo/GameManagerDemoLevel", order = 0)]
    public class MyLevelConfig : BaseLevelConfig
    {
        public GameObject Dummy;
        public GameObject SecondDummy;
        public override string SceneName => "";
    }
}