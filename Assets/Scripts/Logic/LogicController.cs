using Data;
using UI;
using UnityEngine;
using UniRx;

namespace Logic
{
  public class LogicController : MonoBehaviour
  {
    #region FIELDS

    [Header("Delete save data file")] [SerializeField]
    private bool _clearData = false;

    [Space] 
    [SerializeField] private ViewController viewController;
    [SerializeField] private AsteroidSpawner _asteroidSpawner;

    [SerializeField] internal PlayerControl playerControl;

    private Factory _factory;
    private CompositeDisposable _disposable = new CompositeDisposable();

    private ModelData _modelData;
    public ModelData ModelData => _modelData;

    public ReactiveProperty<GameStateEnum> CurrentGameState { get; set; }

    public ReactiveCommand<int> SelectLevel = new ReactiveCommand<int>();

    public UserData SavedStateData => _modelData.UserData;

    [HideInInspector] public Vector2 leftBottomScreen;
    [HideInInspector] public Vector2 rightTopScreen;

    private IGameState _curGameState;
    private StartAppState _startAppState;
    private MenuState _menuState;
    private GameState _gameState;
    private WinGameState _winGameState;
    private LoadGameState _loadGameState;
    private GameOverState _gameOverState;
    
    #endregion

    private void Awake()
    {
      _startAppState = new StartAppState(this);
      SetGameState(_startAppState);
    }

    private void Start()
    {
      _modelData = new ModelData(_clearData);

      GetScreenCoordinates();

      _factory = new Factory(this);
      viewController.Construct(this, _factory);
      
      _menuState = new MenuState(this, _factory);
      _gameState = new GameState(this, viewController);
      _winGameState = new WinGameState(this, _factory);
      _loadGameState = new LoadGameState(this, _factory, _asteroidSpawner);
      _gameOverState = new GameOverState(this, _factory);
      
      Subscribes();

      CurrentGameState.Value = GameStateEnum.Menu;

      ChangeAmmo();
    }

    private void GetScreenCoordinates()
    {
      Camera main = Camera.main;
      if (main is not null)
      {
        leftBottomScreen = main.ViewportToWorldPoint(Vector3.zero);
        rightTopScreen = main.ViewportToWorldPoint(Vector3.one);
      }
    }

    private void SetGameState(IGameState newState)
    {
      if(_curGameState != null) _curGameState.Exit();
      _curGameState = newState;
      _curGameState.Enter();
    }
    
    private void ChangeState()
    {
      switch (CurrentGameState.Value)
      {
        case GameStateEnum.Menu:
          SetGameState(_menuState);
          break;
        case GameStateEnum.Game:
          SetGameState(_gameState);
          break;
        case GameStateEnum.Win:
          SetGameState(_winGameState);
          break;
        case GameStateEnum.GameOver:
          SetGameState(_gameOverState);
          break;
        case GameStateEnum.Pause:
          break;
        case GameStateEnum.StartApp:
          break;
      }
    }

    internal void StartLevel(int level)
    {
      _loadGameState.SetLevel(level);
      SetGameState(_loadGameState);
    }
    
    private void Subscribes()
    {
      CurrentGameState
        .ObserveEveryValueChanged(x => x.Value)
        .Skip(1)
        .Subscribe(x => { ChangeState(); })
        .AddTo(_disposable);
    }

    private void OnDestroy()
    {
      _disposable.Clear();
    }

    public void ChangeAmmo()
    {
      _modelData.SelectedAmmo = _factory.GetNextAmmo();
#if UNITY_EDITOR
      viewController.panelHUD.SetImageAmmo();
#endif
    }
  }
}