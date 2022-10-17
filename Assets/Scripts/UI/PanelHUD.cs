using System;
using Logic;
using SimpleInputNamespace;
using TMPro;
using UI;
using UniRx;
using UnityEngine;

public class PanelHUD : MonoBehaviour
{
  [SerializeField] private GameObject[] lifes;
  [SerializeField] private TouchButton buttonFire;
  [SerializeField] private Joystick joystick;
  [SerializeField] private TMP_Text _textScore;

  private ViewController _viewController;
  private LogicController _logic;

  private CompositeDisposable _disposable = new CompositeDisposable();

  public void Construct(ViewController viewController, LogicController logic)
  {
    _logic = logic;
    _viewController = viewController;
  }

  public void Subscribes()
  {
    _logic.CurrentLevelData.PlayerLifes
      .ObserveEveryValueChanged(value => _logic.CurrentLevelData.PlayerLifes.Value)
      .Subscribe(value =>
      {
        ChangeLifes();
      })
      .AddTo(_disposable);

    _logic.CurrentLevelData.EnemiesDestroyed
      .ObserveEveryValueChanged(value => _logic.CurrentLevelData.EnemiesDestroyed.Value)
      .Subscribe(value => { ChangeScore(); })
      .AddTo(_disposable);

    joystick
      .ObserveEveryValueChanged(v2 => joystick.Value)
      .Subscribe(
        value => _viewController.JoystickPosition.Value = joystick.Value
      ).AddTo(_disposable);

    buttonFire.isFire
      .ObserveEveryValueChanged(f => buttonFire.isFire.Value)
      .Subscribe(
        value => _viewController.isFire.Value = buttonFire.isFire.Value
      ).AddTo(_disposable);
  }

  private void ChangeScore()
  {
    _textScore.text = $"{_logic.CurrentLevelData.EnemiesDestroyed.Value}/{_logic.CurrentLevelData.EnemiesDestroyForWIn.Value}";
  }

  private void ChangeLifes()
  {
    int value = _logic.CurrentLevelData.PlayerLifes.Value;
    if (value >= 0)
    {
      for (int i = 0; i < lifes.Length; i++)
      {
        lifes[i].SetActive(i < value);
      }
    }

    if (value == 0)
    {
      _logic.CurrentGameState.Value = GameState.GameOver;
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