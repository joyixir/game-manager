using Joyixir.GameManager.Scripts.Level;
using UnityEngine;

namespace Joyixir.GameManager.Demo
{
    [CreateAssetMenu(fileName = "GameManagerDemoLevel", menuName = "Joyixir/GameManager/Demo/Levels", order = 0)]
    public class MyLevelConfig : BaseLevelConfig
    {
        public GameObject Dummy;
        public GameObject SecondDummy;
    }
}