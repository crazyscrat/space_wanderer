using System.Collections;
using Logic;
using UniRx;
using Object = UnityEngine.Object;

public class WinGameState : IGameState
{
  private CompositeDisposable _disposable = new CompositeDisposable();

  private Factory _factory;
  private LogicController _logic;

  public WinGameState(LogicController logic, Factory factory)
  {
    _logic = logic;
    _factory = factory;
  }

  public void Enter()
  {
    StopGame();

    Observable.FromCoroutine(WinGame).Subscribe(_ =>
    {
      _logic.CurrentGameState.Value = GameStateEnum.Win;
    }).AddTo(_disposable);
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

  private IEnumerator WinGame()
  {
    if (_logic.ModelData.Level ==
        _logic.ModelData.UserData.LastOpenLevel)
    {
      _logic.ModelData.UserData.LastOpenLevel =
        _logic.ModelData.Level + 1;
    }

    _logic.ModelData.UserData.Score +=
      _logic.ModelData.Score;

    _logic.ModelData.Save();
    yield return null;
  }
}