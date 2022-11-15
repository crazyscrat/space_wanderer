  using Logic;
  using UniRx;

  public class StartAppState:IGameState
  {
    private LogicController _logicController;

    public StartAppState(LogicController logicController)
    {
      _logicController = logicController;
    }

    public void Enter()
    {
      _logicController.CurrentGameState = new ReactiveProperty<GameStateEnum>(GameStateEnum.StartApp);
    }

    public void Update()
    {
      
    }

    public void Exit()
    {
      
    }
  }
