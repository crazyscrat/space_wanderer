using Logic;
using UniRx;
using UnityEngine;

public class MenuState : IGameState
{
  private CompositeDisposable _disposable = new CompositeDisposable();

  private Factory _factory;
  private LogicController _logic;

  public MenuState(LogicController logic,
    Factory
      factory)
  {
    _logic = logic;
    _factory = factory;
  }

  public void Enter()
  {
    StopGame();

    _logic.CurrentGameState.Value = GameStateEnum.Menu;

    _logic.SelectLevel.Subscribe(x => _logic.StartLevel(x))
      .AddTo(_disposable);
  }

  private void StopGame()
  {
    if (_logic.playerControl != null)
    {
      Object.Destroy(_logic.playerControl.gameObject);
    }

    _factory.DestroyAllAsteroids();
    _factory.DestroyAllAmmo();
  }

  public void Update()
  {
  }

  public void Exit()
  {
    _disposable.Clear();
  }
}