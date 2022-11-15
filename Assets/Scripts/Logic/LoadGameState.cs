using System.Collections;
using System.Collections.Generic;
using Data;
using Logic;
using UniRx;

public class LoadGameState : IGameState
{
  private CompositeDisposable _disposable = new CompositeDisposable();
  private LogicController _logic;
  private AsteroidSpawner _asteroidSpawner;
  private Factory _factory;
  private int _level;

  public LoadGameState(LogicController logic,
    Factory factory,
    AsteroidSpawner asteroidSpawner)
  {
    _factory = factory;
    _asteroidSpawner = asteroidSpawner;
    _logic = logic;
  }

  public void Enter()
  {
    _logic.ModelData.LoadLevelData(_level, _factory)
      .ToObservable()
      .Subscribe(_ =>
        {
          Observable.FromCoroutine(CreateObjects)
            .Subscribe(_ =>
            {
              _logic.CurrentGameState.Value = GameStateEnum.Game;
            })
            .AddTo(_disposable);
        }
      );
  }

  public void Update()
  {
  }

  public void Exit()
  {
    _disposable.Clear();
  }

  public void SetLevel(int level)
  {
    _level = level;
  }
  
  private IEnumerator CreateObjects()
  {
    _asteroidSpawner = _factory.CreateAsteroidSpawner();
    _asteroidSpawner.Construct(_logic, _factory);

    _logic.playerControl = _factory.CreatePlayer();
    _logic.playerControl.Construct(_logic, _factory);

    yield break;
  }

}