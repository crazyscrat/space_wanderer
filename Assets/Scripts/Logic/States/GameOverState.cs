using Logic;
using UnityEngine;

public class GameOverState : IGameState
{
  private LogicController _logic;

  public GameOverState(LogicController logic)
  {
    _logic = logic;
  }

  public void Enter()
  {
    StopGame();
  }

  private void StopGame()
  {
    if (_logic.PlayerControl != null)
    {
      Object.Destroy(_logic.PlayerControl.gameObject);
    }
    _logic.Factory.DestroyAllAsteroids();
    _logic.Factory.DestroyAllAmmo();
  }

  public void Update()
  {
  }

  public void Exit()
  {
  }
}