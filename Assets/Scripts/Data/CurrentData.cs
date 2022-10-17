using System;
using UniRx;
using UnityEngine;

namespace Data
{
  [Serializable]
  public class CurrentData
  {
    public ReactiveProperty<int> PlayerLifes = new ReactiveProperty<int>(3);
    public ReactiveProperty<int> EnemiesDestroyed = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> EnemiesDestroyForWIn = new ReactiveProperty<int>(0);
    public int AsteroidTypes = 0;
    public int AsteroidMaxView = 0;
    public float AsteroidCooldown = 0f;
    public int Level = 0;
    public int Score = 0;
  }
}