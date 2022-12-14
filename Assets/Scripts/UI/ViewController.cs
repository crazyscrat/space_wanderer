using System.Threading.Tasks;
using Logic;
using UniRx;
using UnityEngine;

namespace UI
{
  public class ViewController : MonoBehaviour
  {

    #region FIELDS

    [SerializeField] private PanelLevelFinish panelLevelFinish;
    [SerializeField] private PanelLevelFinish _panelGameWin;
    [SerializeField] private PanelHUD _panelHUD;
    [SerializeField] private PanelMenu _panelMenu;
    
    public PanelHUD panelHUD => _panelHUD;

    private CompositeDisposable _disposable = new CompositeDisposable();
    private LogicController _logic;
    private Factory _factory;

    public ReactiveProperty<bool> isFire { get; private set; }
    public ReactiveProperty<Vector2> JoystickPosition { get; set; }

    #endregion
    
    public void Construct(LogicController logic, Factory factory)
    {
      _factory = factory;
      JoystickPosition = new ReactiveProperty<Vector2>(Vector2.zero);
      isFire = new ReactiveProperty<bool>(false);
      
      _logic = logic;
      panelLevelFinish.Construct(this);
      _panelGameWin.Construct(this);
      _panelHUD.Construct(this, _logic);
      _panelMenu.Construct(_factory);
      
      _logic.CurrentGameState
        .ObserveEveryValueChanged(value => _logic.CurrentGameState.Value)
        .Subscribe(value =>
        {
          ChangeState(_logic.CurrentGameState.Value);
        })
        .AddTo(_disposable);
    }

    private void OnDestroy()
    {
      _disposable.Clear();
    }

    private void ChangeState(GameStateEnum stateEnum)
    {
      switch (stateEnum)
      {
        case GameStateEnum.Menu:
          ActivateMenu(true);
          break;
        case GameStateEnum.Game:
          ActivateLevelUI(true);
          PanelGameOverActive(false);
          PanelGameWinActive(false);
          break;
        case GameStateEnum.Win:
          ActivateLevelUI(false);
          PanelGameWinActive(true);
          break;
        case GameStateEnum.GameOver:
          ActivateLevelUI(false);
          PanelGameOverActive(true);
          break;
        case GameStateEnum.Pause:
          break;
      }
    }

    private void ActivateLevelUI(bool activate)
    {
      PanelMenuActive(!activate);
      PanelHUDActive(activate);
    }

    private void ActivateMenu(bool activate)
    {
      PanelMenuActive(activate);
      PanelHUDActive(!activate);
      PanelGameOverActive(!activate);
      PanelGameWinActive(!activate);
    }
    
    private void PanelGameOverActive(bool active)
    {
      panelLevelFinish.gameObject.SetActive(active);
    }
    
    private void PanelGameWinActive(bool active)
    {
      _panelGameWin.gameObject.SetActive(active);
    }
    
    private void PanelHUDActive(bool active)
    {
      if (active)
      {
        _panelHUD.Subscribes();
      }
      else
      {
        _panelHUD.Dispose();
      }
      _panelHUD.gameObject.SetActive(active);
    }
    
    private void PanelMenuActive(bool active)
    {
      if(active)
      {
        _panelMenu.GenerateLevelButtons();
        _panelMenu.SetScore(_logic.SavedStateData.Score);
      }
      _panelMenu.gameObject.SetActive(active);
    }

    public void SetState(GameStateEnum stateEnum)
    {
      _logic.CurrentGameState.Value = stateEnum;
    }
  }
}