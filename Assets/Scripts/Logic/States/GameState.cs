using Logic;
using UI;
using UniRx;
using UnityEngine;

public class GameState : IGameState
{
  #region FIELDS

  private CompositeDisposable _disposable = new CompositeDisposable();

  private ViewController _view;
  private LogicController _logic;

  #endregion

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

    _view.panelHUD.buttonAmmo.TouchButton.isAction
      .ObserveEveryValueChanged(f =>
        _view.panelHUD.buttonAmmo.TouchButton.isAction.Value)
      .Subscribe(
        value => ChangeAmmo()
      ).AddTo(_disposable);
    
#if UNITY_EDITOR
    EditorControl();
#endif
  }

  private void EditorControl()
  {
    Observable.EveryUpdate()
      .Subscribe(x =>
        _logic.PlayerControl.Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"))))
      .AddTo(_disposable);

    Observable.EveryUpdate()
      .Where(_ => Input.GetMouseButtonDown(0))
      .Subscribe(x => _logic.PlayerControl.StartFire())
      .AddTo(_disposable);

    Observable.EveryUpdate()
      .Where(_ => Input.GetMouseButtonUp(0))
      .Subscribe(x => _logic.PlayerControl.StopFire())
      .AddTo(_disposable);
    
    Observable.EveryUpdate()
      .Where(_ => Input.GetKeyDown(KeyCode.E))
      .Subscribe(x => ChangeAmmo())
      .AddTo(_disposable);
  }

  private void ChangeAmmo()
  {
    _logic.ModelData.SelectedAmmo = _logic.Factory.GetNextAmmo();
    
    _view.panelHUD.SetImageAmmo();
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
      _logic.PlayerControl.StartFire();
    }
    else
    {
      _logic.PlayerControl.StopFire();
    }
  }

  private void MovePlayer()
  {
    _logic.PlayerControl.Move(_view.JoystickPosition.Value);
  }
}