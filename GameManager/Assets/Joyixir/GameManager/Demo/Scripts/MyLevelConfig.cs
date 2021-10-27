using UnityEngine;
using Joyixir.GameManager.Level;

namespace Joyixir.GameManager.Demo
{
    [CreateAssetMenu(fileName = "GameManagerDemoLevel", menuName = "Joyixir/GameManager/Demo/GameManagerDemoLevel", order = 0)]
    public class MyLevelConfig : BaseLevelConfig
    {
        public GameObject Dummy;
        public GameObject SecondDummy;
        public override string SceneName => "";
    }
}