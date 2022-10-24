using System.Collections;
using System.Threading.Tasks;
using Data;
using UI;
using UnityEngine;
using UniRx;
using Random = UnityEngine.Random;

namespace Logic
{
  public class LogicController : MonoBehaviour
  {
    [Header("Delete save data file")] [SerializeField]
    private bool _clearData = false;

    [Space] [SerializeField] private ViewController viewController;
    [SerializeField] private AsteroidSpawner _asteroidSpawner;

    [SerializeField] private PlayerControl _playerControl;

    private Factory _factory;
    private CompositeDisposable _disposable = new CompositeDisposable();
    private CompositeDisposable _disposableMenu = new CompositeDisposable();

    [SerializeField] private ModelData _modelData;
    public ModelData ModelData => _modelData;


    public ReactiveProperty<GameState> CurrentGameState { get; set; }

    public ReactiveCommand<int> SelectLevel = new ReactiveCommand<int>();

    public UserData SavedStateData => _modelData.UserData;

    [HideInInspector] public Vector2 leftBottomScreen;
    [HideInInspector] public Vector2 rightTopScreen;

    private void Awake()
    {
      CurrentGameState = new ReactiveProperty<GameState>(GameState.StartApp);
    }

    private void Start()
    {
      _modelData = new ModelData(_clearData);

      GetScreenCoordinates();

      _factory = new Factory(this);
      viewController.Construct(this, _factory);

      SelectLevel.Subscribe(SelectLevelAtIndex)
        .AddTo(_disposableMenu);

      ShowMenu();

      ChangeAmmo();
    }

    private void GetScreenCoordinates()
    {
      Camera main = Camera.main;
      leftBottomScreen = main.ViewportToWorldPoint(Vector3.zero);
      rightTopScreen = main.ViewportToWorldPoint(Vector3.one);
    }

    private void ChangeState()
    {
      switch (CurrentGameState.Value)
      {
        case GameState.Menu:
          StopLevel();
          break;
        case GameState.Game:
          break;
        case GameState.Win:
          StopLevel();
          break;
        case GameState.GameOver:
          StopLevel();
          break;
        case GameState.Pause:
          break;
      }
    }

    private IEnumerator WinGame()
    {
      if (_modelData.Level == _modelData.UserData.LastOpenLevel)
      {
        _modelData.UserData.LastOpenLevel = _modelData.Level + 1;
      }

      _modelData.UserData.Score += _modelData.Score;

      _modelData.Save();
      yield return null;
    }

    void ShowMenu()
    {
      CurrentGameState.Value = GameState.Menu;
    }

    private void SelectLevelAtIndex(int level)
    {
      StartLevel(level);
    }

    void StartLevel(int level)
    {
      _modelData.LoadLevelData(level, _factory)
        .ToObservable()
        .Subscribe(_ =>
          {
            CreateObjects()
              .ToObservable()
              .Subscribe(_ =>
                {
                  Subscribes()
                    .ToObservable()
                    .Subscribe(_ => { CurrentGameState.Value = GameState.Game; });
                }
              );
          }
        );
    }

    private IEnumerator CreateObjects()
    {
      _asteroidSpawner = _factory.CreateAsteroidSpawner();
      _asteroidSpawner.Construct(this, _factory);

      _playerControl = _factory.CreatePlayer();
      _playerControl.Construct(this, _factory);

      yield break;
    }

    private IEnumerator Subscribes()
    {
      viewController.isFire
        .ObserveEveryValueChanged(x => x.Value)
        .Subscribe(value => OnClickFire(viewController.isFire.Value))
        .AddTo(_disposable);

      CurrentGameState
        .ObserveEveryValueChanged(x => x.Value)
        .Skip(1)
        .Subscribe(x => { ChangeState(); })
        .AddTo(_disposable);

      Observable.EveryUpdate().Subscribe(x => MovePlayer()).AddTo(_disposable);

      _modelData.EnemiesDestroyed
        .ObserveEveryValueChanged(x => x.Value)
        .Subscribe(async x =>
        {
          if (_modelData.EnemiesDestroyed.Value >= _modelData.EnemiesDestroyForWIn.Value)
          {
            Observable.FromCoroutine(WinGame)
              .Subscribe(_ => { CurrentGameState.Value = GameState.Win; });
          }
        })
        .AddTo(_disposable);

      yield break;
    }


    private void StopLevel()
    {
      _disposable.Clear();
      Destroy(_playerControl.gameObject);
      _factory.DestroyAllAsteroids();
      _factory.DestroyAllAmmo();
    }

    private void MovePlayer()
    {
      _playerControl.Move(viewController.JoystickPosition.Value);
    }

    private void OnClickFire(bool fire)
    {
      if (fire)
      {
        _playerControl.StartFire();
      }
      else
      {
        _playerControl.StopFire();
      }
    }

    private void OnDestroy()
    {
      _disposable.Clear();
      _disposableMenu.Clear();
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