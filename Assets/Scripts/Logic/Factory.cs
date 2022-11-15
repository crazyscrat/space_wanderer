using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Logic
{
  public class Factory
  {
    #region FIELDS

    private GameObject _playerPrefab;
    private GameObject _asteroidSpawnerPrefab;
    private GameObject _buttonLevelPrefab;
    private Ammo[] _ammoPrefabs;
    private LevelData[] _levels;
    private AsteroidControl[] _asteroidPrefabs;

    private List<Ammo> _ammo = new List<Ammo>(100);
    private List<AsteroidControl> _asteroids = new List<AsteroidControl>(100);
    private LogicController _logic;

    public int AsteroidsMaxTypes => _asteroidPrefabs.Length;

    #endregion
    
    public Factory(LogicController logic)
    {
      _logic = logic;

      _playerPrefab = Resources.Load<GameObject>("Player");
      _asteroidSpawnerPrefab = Resources.Load<GameObject>("AsteroidSpawner");
      _buttonLevelPrefab = Resources.Load<GameObject>("Menu/Button.Level");
      _ammoPrefabs = Resources.LoadAll<Ammo>("Ammo");
      for (var i = 0; i < _ammoPrefabs.Length; i++)
      {
        _ammoPrefabs[i].id = i;
      }

      _levels = Resources.LoadAll<LevelData>("Levels");
      _asteroidPrefabs = Resources.LoadAll<AsteroidControl>("Asteroids");
    }


    public AsteroidSpawner CreateAsteroidSpawner()
    {
      return InstantiateObject<AsteroidSpawner>(_asteroidSpawnerPrefab);
    }

    public PlayerControl CreatePlayer()
    {
      return InstantiateObject<PlayerControl>(_playerPrefab);
    }

    #region AMMO

    public Ammo GetMissile(Vector2 position)
    {
      Ammo ammo = _ammo.FirstOrDefault(m =>
        !m.gameObject.activeInHierarchy && m.AmmoType == _logic.ModelData.SelectedAmmo.AmmoType);

      if (ammo == null)
      {
        ammo = CreateAmmo();
      }

      SetPosition(ammo.gameObject, position);

      return ammo;
    }

    private Ammo CreateAmmo()
    {
      Ammo ammo = CreateObjectPool(_ammoPrefabs, _ammo, _logic.ModelData.SelectedAmmo.id);
      ammo.Construct(_logic);

      return ammo;
    }

    public void DestroyMissile(GameObject missile)
    {
      missile.SetActive(false);
    }

    public void DestroyAllAmmo()
    {
      for (int i = _ammo.Count - 1; i >= 0; i--)
      {
        _ammo[i].Destroy();
      }

      _ammo.Clear();
    }

    #endregion AMMO

    #region ASTEROIDS

    public AsteroidControl GetAsteroid(int index, Vector2 position)
    {
      AsteroidControl asteroid = _asteroids.FirstOrDefault(m => m.index == index && !m.gameObject.activeInHierarchy);

      if (asteroid == null)
      {
        asteroid = CreateAsteroid(index);
      }

      SetPosition(asteroid.gameObject, position);

      return asteroid;
    }

    private AsteroidControl CreateAsteroid(int index)
    {
      AsteroidControl asteroid = CreateObjectPool(_asteroidPrefabs, _asteroids, index);
      asteroid.SetName(index);

      return asteroid;
    }

    public void DestroyAsteroid(GameObject asteroid)
    {
      asteroid.SetActive(false);
    }


    public void DestroyAllAsteroids()
    {
      for (int i = _asteroids.Count - 1; i >= 0; i--)
      {
        _asteroids[i].Destroy();
      }

      _asteroids.Clear();
    }

    #endregion ASTEROIDS

    private T InstantiateObject<T>(GameObject obj) where T:MonoBehaviour
    {
      T newObject = Object.Instantiate(obj).GetComponent<T>();

      return newObject;
    }
    
    private T CreateObjectPool<T>(T[] array, List<T> list, int index) where T:MonoBehaviour
    {
      T newObject = Object.Instantiate(array[index]);
      list.Add(newObject);

      return newObject;
    }
    
    private void SetPosition(GameObject obj, Vector2 position)
    {
      obj.transform.position = position;
      obj.SetActive(true);
    }

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
        LevelButton button = Object.Instantiate(_buttonLevelPrefab, parentLevelButtons, true)
          .GetComponent<LevelButton>();
        button.Construct(i, _logic);
        buttons[i] = button;
      }

      return buttons;
    }

    public Ammo GetNextAmmo()
    {
      int idAmmo = 0;
      if (_logic.ModelData.SelectedAmmo != null)
      {
        idAmmo = _logic.ModelData.SelectedAmmo.id;
        idAmmo = ++idAmmo > _ammoPrefabs.Length - 1 ? 0 : idAmmo;
      }

      return _ammoPrefabs[idAmmo];
    }
  }
}