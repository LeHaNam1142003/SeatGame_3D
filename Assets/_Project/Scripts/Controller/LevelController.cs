using UnityEngine;
using Pancake;
using Debug = System.Diagnostics.Debug;

public class LevelController : SingletonDontDestroy<LevelController>
{
    [ReadOnly] public Level currentLevel;
    private GameConfig Game => ConfigController.Game;
    public void PrepareLevel(string address, int indexLevel)
    {
        GenerateLevel(address, indexLevel);
    }

    public void GenerateLevel(string address, int indexLevel)
    {
        if (currentLevel != null)
        {
            DestroyImmediate(currentLevel.gameObject);
        }

        if (indexLevel > ConfigController.Game.maxLevel)
        {
            indexLevel = (indexLevel - Game.startLoopLevel) % (Game.maxLevel - Game.startLoopLevel + 1) + Game.startLoopLevel;
        }
        else
        {
            if (Game.levelLoopType == LevelLoopType.NormalLoop)
            {
                indexLevel = (indexLevel - 1) % ConfigController.Game.maxLevel + 1;
            }
            else if (Game.levelLoopType == LevelLoopType.RandomLoop)
            {
                indexLevel = UnityEngine.Random.Range(Game.startLoopLevel, Game.maxLevel);
            }
        }

        Level level = GetLevelByIndex(address, indexLevel);
        currentLevel = Instantiate(level);
        currentLevel.gameObject.SetActive(false);
    }

    public Level GetLevelByIndex(string address, int indexLevel)
    {
        var levelGo = Resources.Load($"Levels/{address} {indexLevel}") as GameObject;
        Debug.Assert(levelGo != null, nameof(levelGo) + " != null");
        return levelGo.GetComponent<Level>();
    }
}
