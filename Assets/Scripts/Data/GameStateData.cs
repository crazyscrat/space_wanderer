using System;
using System.Collections.Generic;

[Serializable]
public class GameStateData
{
  public int LastOpenLevel = 1;
  public int Score = 0;

  public Dictionary<int,Level> LevelsState = new Dictionary<int, Level>();
}