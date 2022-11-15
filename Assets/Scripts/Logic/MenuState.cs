using Logic;
using UniRx;
using UnityEngine;

public class MenuState : IGameState
{
  private CompositeDisposable _disposable = new CompositeDisposable();

  private LogicController _logic;

  public MenuState(LogicController logic)
  {
    _logic = logic;
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

    _logic.Factory.DestroyAllAsteroids();
    _logic.Factory.DestroyAllAmmo();
  }

  public void Update()
  {
  }

  public void Exit()
  {
    _disposable.Clear();
  }
}