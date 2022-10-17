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
    [SerializeField] private bool _clearData = false;
    [SerializeField] private ViewController viewController;
    [SerializeField] private AsteroidSpawner _asteroidSpawner;

    [SerializeField] private PlayerControl _playerControl;

    private Factory _factory;
    private CompositeDisposable _disposable = new CompositeDisposable();
    private CompositeDisposable _disposableMenu = new CompositeDisposable();

    [SerializeField] private CurrentData _currentLevelData = new CurrentData();

    public CurrentData CurrentLevelData => _currentLevelData;
    public ReactiveProperty<GameState> CurrentGameState { get; set; }

    public ReactiveCommand<int> SelectLevel = new ReactiveCommand<int>();

    private GameStateData _gameStateData;
    public GameStateData SavedStateData => _gameStateData;

    public Vector2 leftBottomScreen;
    public Vector2 rightTopScreen;

    private void Awake()
    {
      CurrentGameState = new ReactiveProperty<GameState>(GameState.StartApp);
    }

    private async void Start()
    {
      GetScreenCoordinates();

      _factory = new Factory(this);
      await viewController.Construct(this, _factory);

      SelectLevel.Subscribe(i => SelectLevelAtIndex(i)
      ).AddTo(_disposableMenu);

      await LoadFromFile();
      //StartLevel();
      ShowMenu();
    }

    private void GetScreenCoordinates()
    {
      leftBottomScreen = Camera.main.ViewportToWorldPoint(Vector3.zero);
      rightTopScreen = Camera.main.ViewportToWorldPoint(Vector3.one);
    }

    private async Task LoadFromFile()
    {
      if (_clearData) await SaveLoader.Clear();
      _gameStateData = await SaveLoader.Load();
      
      _currentLevelData.Score = _gameStateData.Score;
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

    private async Task WinGame()
    {
      if (_currentLevelData.Level == _gameStateData.LastOpenLevel)
      {
        _gameStateData.LastOpenLevel = _currentLevelData.Level + 1;
      }

      _gameStateData.Score += _currentLevelData.Score;

      await SaveLoader.Save(_gameStateData);
    }

    void ShowMenu()
    {
      CurrentGameState.Value = GameState.Menu;
    }

    private void SelectLevelAtIndex(int level)
    {
      StartLevel(level);
    }

    async void StartLevel(int level)
    {
      await LoadLevelData(level);
      await CreateObjects();
      await Subscribes();
      CurrentGameState.Value = GameState.Game;
    }

    private async Task LoadLevelData(int level)
    {
      _currentLevelData = new CurrentData();

      LevelData levelData = _factory.GetDataLevel(level);

      _currentLevelData.Level = level;
      _currentLevelData.PlayerLifes.Value = 3;
      _currentLevelData.EnemiesDestroyed.Value = 0;
      _currentLevelData.AsteroidCooldown = levelData.AsteroidCooldown;
      _currentLevelData.AsteroidMaxView = levelData.MaxAsteriodView;

      GameObject.FindWithTag("Background").GetComponent<SpriteRenderer>().sprite = levelData.Background;

      //data saved
      if (SavedStateData.LevelsState.ContainsKey(level))
      {
        _currentLevelData.AsteroidTypes = SavedStateData.LevelsState[level].AsteriodTypes;
        _currentLevelData.EnemiesDestroyForWIn.Value = SavedStateData.LevelsState[level].AsteriodVictory;
      }
      //new data
      else
      {
        _currentLevelData.EnemiesDestroyForWIn.Value = Random.Range(
          levelData.MinAsteriodVictory,
          levelData.MaxAsteriodVictory
        );

        int asteroidTypes = Random.Range(
          levelData.MinAsteriodTypes,
          levelData.MaxAsteriodTypes
        );

        asteroidTypes = Mathf.Clamp(asteroidTypes, 1, _factory.AsteroidsMaxTypes);
        _currentLevelData.AsteroidTypes = asteroidTypes;

        SavedStateData.LevelsState[level] = new Level
        {
          AsteriodTypes = _currentLevelData.AsteroidTypes,
          AsteriodVictory = _currentLevelData.EnemiesDestroyForWIn.Value
        };
        SaveLoader.Save(SavedStateData);
      }
    }

    private async Task CreateObjects()
    {
      _asteroidSpawner = _factory.CreateAsteroidSpawner();
      _asteroidSpawner.Construct(this, _factory);

      _playerControl = _factory.CreatePlayer();
      _playerControl.Construct(this, _factory);
    }

    private async Task Subscribes()
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

      _currentLevelData.EnemiesDestroyed
        .ObserveEveryValueChanged(x => x.Value)
        .Subscribe(async x =>
        {
          if (_currentLevelData.EnemiesDestroyed.Value >= _currentLevelData.EnemiesDestroyForWIn.Value)
          {
            await WinGame();
            CurrentGameState.Value = GameState.Win;
          }
        })
        .AddTo(_disposable);
    }


    private void StopLevel()
    {
      _disposable.Clear();
      Destroy(_playerControl.gameObject);
      _factory.DestroyAllAsteroids();
      _factory.DestroyAllMissiles();
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
  }
}