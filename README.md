# GameManager

 A simple unity game management system to handle loading a game, its levels and saving the levels data such as score, gained money, level number. Technically its a full game circle.
 Also it provides enough public Actions for other classes to do things based on game status changes like listening to level finish to send analytics, etc. 

## Features

- Starts, ends, restarts levels
- Handles level number, score, money, satisfaction
- Initialization is separated from level start
- Handles all required actions
- Tracks number of attempts


## Installation
There are three ways to use it
**Keep in mind that this repo is dependent to Unity TMPro**

### Unitypackage
- Go to the releases section and download the .unitypackage file


### PackageManager
##### Scoped Registry
**No longer supported(Latest version is 2.1.0)**

To add Joyixir scope to NPM scopedregistry, add the following to manifest.json
```json
{
    "scopedRegistries": [
      {
        "name": "npmjs",
        "url": "https://registry.npmjs.org/",
        "scopes": [
          "com.joyixir"
        ]
      }
    ]
}
```

##### Install the package
Open Window/PackageManager and head to My Registries. Install your desired version of Joyixir/GameManager

### Clone the repo
You can just clone the repo and do whatever you like with it. Even to make it better.
Keep in mind that in order to have the needed dependencies you have to clone using
```sh
git clone --recurse-submodules -j8 https://github.com/joyixir/game-manager.git
```

### Use as a submodule
You can use submodule branch. In this case you have to handle dependencies yourself.


# Use

Check the demo folder. 
Generally, you need to inherit from BaseLevelConfig and BaseLevel.

## LevelConfig
LevelConfig is the config that you need in order to initialize a level, like player speed, a list of obstacles or road to generate.

The package supports scene per config types, you just have to set the scene name in the build config when you override from BaseLevelConfig, like this:

```csharp
// MySceneBasedLevelConfig.cs
public override string SceneName => "SceneBasedLevelScene";
```
this way, the LevelManager loads this scene and then starts its level.
You can use an empty string in case you want to load your level in the same scene that you have the GameManager GameObject in it. **This method is not recommended specially when you have multiple level type**

And lastly, you have to assign the level prefab through its own config.

## Level and LevelManager

Level is your rules of what the player will do. 
Implementing a logic for calculating satisfaction and what “win” means is mandatory in what you inherited. 
You will use level config in order to initialize the level. Like instantiating player, then listening to what it does etc.(Check demo)

The safest way for you is to listen to static actions within LevelManager.
```csharp
// LevelManager.cs
public static LevelManager Instance;
public static Action<LevelData> OnLevelFinish;
public static Action OnLevelStart;
public static Action OnLevelReady;
public static Action<int> OnLevelUnlocked; // Starts from 0
```
## What is LevelData?
The short answer is the simplified data of ANY kind of levels. Doesn’t matter if a baseball or a shooter, simulation or a runner, every sort of level should be simplified to some general parameters.
You will have to implement the below functions within your Level in order to help GameManager, UI, Analytics, Currency .etc to work properly.
```csharp
// BaseLevel.cs
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
```

## LevelData
The implemented functions will help BaseLevel to make a LevelData for others to use.
```csharp
// BaseLevel.cs
public class LevelData
{
    public int EarnedMoney;
    public LevelFinishStatus ForcedStatus;
    public float Satisfaction;
    public int Score;
    public int LevelNumber;
    public int HumanReadableLevelNumber => LevelNumber + 1;
    public bool WinStatus;
    public int Attempt;

    public static LevelData GenerateFromLevel(BaseLevel level)
    {
        var levelData = new LevelData();
        levelData.EarnedMoney = level.CalculateEarnedMoney();
        levelData.Score = level.CalculateScore();
        levelData.WinStatus = level.IsWon();
        levelData.Satisfaction = level.CalculateSatisfaction();
        levelData.ForcedStatus = level.ForcedFinishStatus;
        levelData.LevelNumber = level.LevelNumber;
        return levelData;
    }
}
```

*\* We encourage you to use the provided ```public``` and ```public static``` variables and avoid using the ```internal``` ones.*
## License

MIT