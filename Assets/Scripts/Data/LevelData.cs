using System;
using UnityEngine;

[CreateAssetMenu(order = 51, fileName = "LevelData", menuName = "Data/LevelData")]
public class LevelData : ScriptableObject
{
    [SerializeField] private int _level;
    [SerializeField] private Sprite _background;
    [SerializeField] private int _minAsteriodTypes = 0;
    [SerializeField] private int _maxAsteriodTypes = 5;
    [SerializeField] private int _minAsteriodVictory = 10;
    [SerializeField] private int _maxAsteriodVictory = 50;
    [SerializeField] private int _maxAsteriodView = 10;
    [SerializeField] private float _asteroidCooldown = 1f;
    
    public int Level => _level;
    public int MinAsteriodTypes => _minAsteriodTypes;
    public int MaxAsteriodTypes => _maxAsteriodTypes;
    public int MinAsteriodVictory => _minAsteriodVictory;
    public int MaxAsteriodVictory => _maxAsteriodVictory;
    public int MaxAsteriodView => _maxAsteriodView;
    public float AsteroidCooldown => _asteroidCooldown;
    public Sprite Background => _background;
}
