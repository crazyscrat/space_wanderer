using System;
using System.Collections;
using System.Collections.Generic;
using Control;
using Logic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class AsteroidControl : Enemy
{
    public string nameAsteroid = "Asteroid_";
    [HideInInspector] public int index = 0;
    [SerializeField] private int _scoreAdd = 1;
    [SerializeField] private float _speed = 1;

    private CompositeDisposable _disposable = new CompositeDisposable();
    private AsteroidSpawner _spawner;
    private Collider2D _collider;

    private Transform _transform;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _transform = transform;
    }

    public void Construct(AsteroidSpawner spawner)
    {
        _spawner = spawner;
    }
    
    public void SetName(int index)
    {
        this.index = index;
        nameAsteroid += index.ToString();
        gameObject.name = nameAsteroid;
    }

    public int GetScore()
    {
        return _scoreAdd;
    }
    
    public void Fly()
    {
        Observable.EveryUpdate().Subscribe(x => Move()).AddTo(_disposable);
        
        _collider.OnTriggerEnter2DAsObservable()
            .Where(t => t.gameObject.tag.Equals("Finish")
            )
            .Subscribe(collision => Hit()).AddTo(_disposable);
    }

    private void Move()
    {
        _transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    public override void Hit()
    {
        _spawner.DestroyAsteroid(this); 
        _disposable.Clear();
        gameObject.SetActive(false);
    }

    public void Destroy()
    {
        _disposable.Clear();
        Destroy(gameObject);
    }
}