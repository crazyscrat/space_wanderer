using System.Collections;
using Data;
using Logic;
using UI;
using UniRx;

public class GameState : IGameState
{
  private CompositeDisposable _disposable = new CompositeDisposable();

  private ViewController _view;
  private LogicController _logic;

  public GameState(LogicController logic,
    ViewController view)
  {
    _logic = logic;
    _view = view;
  }

  public void Enter()
  {
    _view.isFire
      .ObserveEveryValueChanged(x => x.Value)
      .Subscribe(value => OnClickFire(_view.isFire.Value))
      .AddTo(_disposable);

    Observable.EveryUpdate().Subscribe(x => Update()).AddTo(_disposable);

    _logic.ModelData.EnemiesDestroyed
      .ObserveEveryValueChanged(x => x.Value)
      .Subscribe(async x =>
      {
        if (_logic.ModelData.EnemiesDestroyed.Value >=
            _logic.ModelData.EnemiesDestroyForWIn.Value)
        {
          _logic.CurrentGameState.Value = GameStateEnum.Win;
        }
      })
      .AddTo(_disposable);
  }

  public void Update()
  {
    MovePlayer();
  }

  public void Exit()
  {
    _disposable.Clear();
  }

  private void OnClickFire(bool fire)
  {
    if (fire)
    {
      _logic.playerControl.StartFire();
    }
    else
    {
      _logic.playerControl.StopFire();
    }
  }

  private void MovePlayer()
  {
    _logic.playerControl.Move(_view.JoystickPosition.Value);
  }
}