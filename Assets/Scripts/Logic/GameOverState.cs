using Logic;
using UnityEngine;

public class GameOverState : IGameState
{
  private Factory _factory;
  private LogicController _logic;

  public GameOverState(LogicController logic, Factory factory)
  {
    _logic = logic;
    _factory = factory;
  }

  public void Enter()
  {
    StopGame();
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
  }
}