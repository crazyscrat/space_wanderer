using System;
using System.Collections;
using System.Threading.Tasks;
using Logic;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Data
{
  [Serializable]
  public class ModelData
  {
    public ReactiveProperty<int> PlayerLifes = new ReactiveProperty<int>(3);
    public ReactiveProperty<int> EnemiesDestroyed = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> EnemiesDestroyForWIn = new ReactiveProperty<int>(0);
    public int AsteroidTypes = 0;
    public int AsteroidMaxView = 0;
    public float AsteroidCooldown = 0f;
    public int Level = 0;
    public int Score = 0;
    public Ammo SelectedAmmo;
    
    private UserData _gameUserData;
    public UserData UserData => _gameUserData;
    
    private bool _clearData;

    public ModelData(bool clearData)
    {
      _clearData = clearData;

      LoadFromFile(); 
    }

    #region SAVE LOAD

    private void LoadFromFile()
    {
      if (_clearData) SaveLoader.Clear();
      _gameUserData = SaveLoader.Load();
      
      Score = _gameUserData.Score;
    }

    public bool Save()
    {
      return SaveLoader.Save(_gameUserData);
    }

    #endregion SAVE LOAD

    public IEnumerator LoadLevelData(int level, Factory factory)
    {
      LevelData levelData = factory.GetDataLevel(level);

      Level = level;
      PlayerLifes.Value = 3;
      EnemiesDestroyed.Value = 0;
      AsteroidCooldown = levelData.AsteroidCooldown;
      AsteroidMaxView = levelData.MaxAsteriodView;

      GameObject.FindWithTag("Background").GetComponent<SpriteRenderer>().sprite = levelData.Background;

      //data saved
      if (_gameUserData.LevelsState.ContainsKey(level))
      {
        AsteroidTypes = _gameUserData.LevelsState[level].AsteriodTypes;
        EnemiesDestroyForWIn.Value = _gameUserData.LevelsState[level].AsteriodVictory;
        yield break;
      }
      //new data
      else
      {
        EnemiesDestroyForWIn.Value = Random.Range(
          levelData.MinAsteriodVictory,
          levelData.MaxAsteriodVictory
        );

        int asteroidTypes = Random.Range(
          levelData.MinAsteriodTypes,
          levelData.MaxAsteriodTypes
        );

        asteroidTypes = Mathf.Clamp(asteroidTypes, 1, factory.AsteroidsMaxTypes);
        AsteroidTypes = asteroidTypes;

        _gameUserData.LevelsState[level] = new Level
        {
          AsteriodTypes = AsteroidTypes,
          AsteriodVictory = EnemiesDestroyForWIn.Value
        };
        
        Save();

        yield break;
      }
    }
  }
}