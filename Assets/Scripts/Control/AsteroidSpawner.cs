using System;
using Logic;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidSpawner : MonoBehaviour
{
  [Range(min: 1, max: 10)] [SerializeField]
  private float _rangeOneSide = 5f;

  private LogicController _logic;
  private Factory _factory;

  public int _asteroidsView = 0;

  private CompositeDisposable _disposable = new CompositeDisposable();

  public void Construct(LogicController logic, Factory factory)
  {
    _factory = factory;
    _logic = logic;

    _logic.CurrentGameState
      .ObserveEveryValueChanged(value => value.Value)
      .Subscribe(value =>
      {
        ChangeState(_logic.CurrentGameState.Value);
      })
      .AddTo(_disposable);
  }

  private void ChangeState(GameState state)
  {
    switch (state)
    {
      case GameState.Menu:
        StopSpawn();
        break;
      case GameState.Game:
        StartSpawn();
        break;
      case GameState.Win:
        StopSpawn();
        break;
      case GameState.GameOver:
        StopSpawn();
        break;
      case GameState.Pause:
        break;
    }
  }
  
  void StartSpawn()
  {
    Observable.Timer(TimeSpan.FromSeconds(_logic.CurrentLevelData.AsteroidCooldown))
      .Repeat()
      .Subscribe(_ =>
        {
          Debug.Log("SPAWN TIMER");
          if (_asteroidsView < _logic.CurrentLevelData.AsteroidMaxView) SpawnAsteroid();
        }
      ).AddTo(_disposable);
  }

  private void StopSpawn()
  {
    _disposable.Clear();
  }

  void SpawnAsteroid()
  {
    Vector2 position = transform.position + Vector3.right * Random.Range(-7f, 7f);

    int index = Random.Range(0, _logic.CurrentLevelData.AsteroidTypes);
    AsteroidControl asteroid = _factory.GetAsteroid(index, position);
    asteroid.gameObject.SetActive(true);
    asteroid.Construct(this);
    asteroid.Fly();

    _asteroidsView++;
  }

  private void OnDestroy()
  {
    _disposable.Clear();
  }

  public void DestroyAsteroid(AsteroidControl asteroid)
  {
    _asteroidsView--;
    _logic.CurrentLevelData.Score += asteroid.GetScore();
  }
}