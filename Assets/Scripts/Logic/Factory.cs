using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Logic
{
  public class Factory
  {
    private GameObject _playerPrefab;
    private GameObject _asteroidSpawnerPrefab;
    private GameObject _buttonLevelPrefab;
    private MissileControl[] _missilePrefabs;
    private LevelData[] _levels;
    private AsteroidControl[] _asteroidPrefabs;

    private List<MissileControl> _missiles = new List<MissileControl>(100);
    private List<AsteroidControl> _asteroids = new List<AsteroidControl>(100);
    private LogicController _logic;

    public Factory(LogicController logic)
    {
      _logic = logic;
      
      _playerPrefab = Resources.Load<GameObject>("Player");
      _asteroidSpawnerPrefab = Resources.Load<GameObject>("AsteroidSpawner");
      _buttonLevelPrefab = Resources.Load<GameObject>("Menu/Button.Level");
      _missilePrefabs = Resources.LoadAll<MissileControl>("Missiles");
      _levels = Resources.LoadAll<LevelData>("Levels");
      _asteroidPrefabs = Resources.LoadAll<AsteroidControl>("Asteroids");
    }

    public int AsteroidsMaxTypes => _asteroidPrefabs.Length;

    public AsteroidSpawner CreateAsteroidSpawner()
    {
      AsteroidSpawner spawner = Object.Instantiate(_asteroidSpawnerPrefab).GetComponent<AsteroidSpawner>();
      return spawner;
    }
    
    public PlayerControl CreatePlayer()
    {
      PlayerControl player = Object.Instantiate(_playerPrefab).GetComponent<PlayerControl>();
      return player;
    }

    #region MISSILES

    public MissileControl GetMissile(int index, Vector2 position)
    {
      MissileControl missile = _missiles.FirstOrDefault(m => !m.gameObject.activeInHierarchy);
      
      if (missile == null)
      {
        missile = CreateMissile(index);
      }

      missile.transform.position = position;
      missile.gameObject.SetActive(true);
      
      return missile;
    }

    private MissileControl CreateMissile(int index)
    {
      MissileControl missile = Object.Instantiate(_missilePrefabs[index]);
      missile.Construct(_logic);
      _missiles.Add(missile);

      return missile;
    }
    
    public void DestroyMissile(GameObject missile)
    {
      missile.SetActive(false);
    }
    
        
    public void DestroyAllMissiles()
    {
      for (int i = _missiles.Count-1; i >= 0; i--)
      {
        _missiles[i].Destroy();
      }
      _missiles.Clear();
    }

    #endregion MISSILES
    

    #region ASTEROIDS
    
    public AsteroidControl GetAsteroid(int index, Vector2 position)
    {
      AsteroidControl asteroid = _asteroids.FirstOrDefault(m => m.index == index && !m.gameObject.activeInHierarchy);
      
      if (asteroid == null)
      {
        asteroid = CreateAsteroid(index);
      }

      asteroid.transform.position = position;
      
      return asteroid;
    }
    
    private AsteroidControl CreateAsteroid(int index)
    {
      AsteroidControl asteroid = Object.Instantiate(_asteroidPrefabs[index]);
      asteroid.SetName(index);
      _asteroids.Add(asteroid);
      
      return asteroid;
    }

    public void DestroyAsteroid(GameObject asteroid)
    {
      asteroid.SetActive(false);
    }
    
    
    public void DestroyAllAsteroids()
    {
      for (int i = _asteroids.Count-1; i >= 0; i--)
      {
        _asteroids[i].Destroy();
      }
      _asteroids.Clear();
    }
    
    #endregion ASTEROIDS

    public LevelData GetDataLevel(int level)
    {
      LevelData levelData = _levels[level - 1];
      return levelData;
    }

    public LevelButton[] GenerateLevelButtons(Transform parentLevelButtons)
    {
      LevelButton[] buttons = new LevelButton[_levels.Length];
      for (int i = 0; i < _levels.Length; i++)
      {
        LevelButton button = Object.Instantiate(_buttonLevelPrefab, parentLevelButtons, true).GetComponent<LevelButton>();
        button.Construct(i, _logic);
        buttons[i] = button;
      }
      
      return buttons;
    }
  }
}