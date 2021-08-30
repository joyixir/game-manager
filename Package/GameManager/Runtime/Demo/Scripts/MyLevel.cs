using Joyixir.GameManager.Scripts.Level;
using UnityEngine;

public class MyLevel : BaseLevel
{
    private MyLevelConfig _config;
    [SerializeField] private Transform dummyPlacement1;
    [SerializeField] private Transform dummyPlacement2;
    public override void InitializeLevel(BaseLevelConfig config) // Before starting the level. (Usually behind start panel)
    {
        base.InitializeLevel(config);
        _config = (MyLevelConfig)config;
        InstantiateSomeLevelThings();
    }

    private void InstantiateSomeLevelThings()
    {
        GameObject.Instantiate(_config.Dummy, dummyPlacement1);
        GameObject.Instantiate(_config.SecondDummy, dummyPlacement2);
    }

    protected internal override void StartLevel() // When click on start
    {
        base.StartLevel();
    }

    protected override void FillVariables()
    {
        base.FillVariables();
    }

    protected internal override void FinishLevel()
    {
        base.FinishLevel();
    }

    protected override void SubscribeToLevelRelatedEvents() // For example, listen to what player does
    {
    }

    protected override void UnSubscribeFromLevelRelatedEvents()
    {
    }

    protected internal override int CalculateEarnedMoney()
    {
        return 10; // Your estimate
    }

    protected internal override float CalculateSatisfaction()
    {
        return .5f; // Your estimate
    }

    protected internal override int CalculateScore()
    {
        return 100; // Your estimate
    }

    protected internal override bool IsWon()
    {
        return CalculateSatisfaction() > .6f; // Your rules
    }
}
