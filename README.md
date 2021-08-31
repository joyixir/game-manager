# GameManager

 A simple unity game management system to handle loading a game, its levels and saving the levels data such as score, gained money, level number. Also it provides enough public Actions for other classes to do things based on game status changes like listening to level finish to send analytics, etc. 

## Features

- Starts, ends, restarts levels
- Handles level number, score, money, satisfaction
- Initialization is separated from level start
- Handles all required actions


## Installation
There are three ways to use it
**Keep in mind that this repo is dependent to Unity TMPro and [Joyixir Utility](https://github.com/joyixir/utility)**

### Unitypackage
- Go to the releases section and download the .unitypackage file


### PackageManager
##### Scoped Registry
TO add Joyixir scope to NPM scopedregistry, add the following to manifest.json
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

LevelConfig is the config that you need in order to initialize a level, like player speed, a list of obstacles or road to generate.

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
```
#####What is LevelData?
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
The implemented functions will help BaseLevel to make a LevelData for others to use.
```csharp
// BaseLevel.cs
public class LevelData
{
    public int EarnedMoney;
    public bool Forced;
    public float Satisfaction;
    public int Score;
    public bool WinStatus;
    public static LevelData GenerateFromLevel(BaseLevel level)
    {
        var levelData = new LevelData();
        levelData.EarnedMoney = level.CalculateEarnedMoney();
        levelData.Score = level.CalculateScore();
        levelData.WinStatus = level.IsWon();
        levelData.Satisfaction = level.CalculateSatisfaction();
        return levelData;
    }
}
```

## License

MIT