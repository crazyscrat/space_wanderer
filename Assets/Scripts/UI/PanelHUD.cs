using System;
using Logic;
using SimpleInputNamespace;
using TMPro;
using UI;
using UniRx;
using UnityEngine;

public class PanelHUD : MonoBehaviour
{
  #region FIELDS

  [SerializeField] private GameObject[] lifes;
  [SerializeField] private TouchButton buttonFire;
  [SerializeField] public AmmoButton buttonAmmo;
  [SerializeField] private Joystick joystick;
  [SerializeField] private TMP_Text _textScore;

  private ViewController _viewController;
  private LogicController _logic;

  private CompositeDisposable _disposable = new CompositeDisposable();

  #endregion

  public void Construct(ViewController viewController, LogicController logic)
  {
    _logic = logic;
    _viewController = viewController;
  }

  public void Subscribes()
  {
    _logic.ModelData.PlayerLifes
      .ObserveEveryValueChanged(value => 
        _logic.ModelData.PlayerLifes.Value)
      .Subscribe(value => { ChangeLifes(); })
      .AddTo(_disposable);

    _logic.ModelData.EnemiesDestroyed
      .ObserveEveryValueChanged(
        value => 
          _logic.ModelData.EnemiesDestroyed.Value)
      .Subscribe(value => { ChangeScore(); })
      .AddTo(_disposable);

    joystick
      .ObserveEveryValueChanged(v2 => joystick.Value)
      .Subscribe(
        value => 
          _viewController.JoystickPosition.Value = joystick.Value
      ).AddTo(_disposable);

    buttonFire.isAction
      .ObserveEveryValueChanged(f => buttonFire.isAction.Value)
      .Subscribe(
        value => 
          _viewController.isFire.Value = buttonFire.isAction.Value
      ).AddTo(_disposable);
  }

  public void SetImageAmmo()
  {
    buttonAmmo.SetImage(_logic.ModelData.SelectedAmmo.GetSprite());
  }

  private void ChangeScore()
  {
    _textScore.text =
      $"{_logic.ModelData.EnemiesDestroyed.Value}/{_logic.ModelData.EnemiesDestroyForWIn.Value}";
  }

  private void ChangeLifes()
  {
    int value = _logic.ModelData.PlayerLifes.Value;
    if (value >= 0)
    {
      for (int i = 0; i < lifes.Length; i++)
      {
        lifes[i].SetActive(i < value);
      }
    }

    if (value == 0)
    {
      _logic.CurrentGameState.Value = GameStateEnum.GameOver;
    }
  }

  private void OnDisable()
  {
    _disposable.Clear();
  }

  public void Dispose()
  {
    _disposable.Clear();
  }
}