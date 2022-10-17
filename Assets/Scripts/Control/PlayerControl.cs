using System;
using System.Collections;
using Logic;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
  [SerializeField] private Collider2D _collider;
  [SerializeField] private float _speed = 1f;
  [SerializeField] private float _cooldown = 0.3f;
  private Vector3 _startPosition;
  private Factory _factory;

  private CompositeDisposable _disposable = new CompositeDisposable();
  private CompositeDisposable _disposableFire = new CompositeDisposable();
  private LogicController _logic;
  private bool _shooting = false;

  private bool _canShoot = true;
  [SerializeField] private MissileType _missileType = MissileType.Base;

  private Transform _transform;

  public void Construct(LogicController logic, Factory factory)
  {
    _transform = transform;
    _logic = logic;
    _factory = factory;
    _transform.position = GameObject.FindWithTag("PlayerPosition").transform.position;

    _collider.OnTriggerEnter2DAsObservable()
      .Where(t=>t.gameObject.layer == LayerMask.NameToLayer("Interaction"))
      .Subscribe(collision => TriggerEnter(collision))
      .AddTo(_disposable);

#if UNITY_EDITOR
    Observable.EveryUpdate()
        .Subscribe(x => 
          Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"))))
        .AddTo(_disposable);
      
      Observable.EveryUpdate()
        .Where(_=>Input.GetMouseButtonDown(0))
        .Subscribe(x => StartFire())
        .AddTo(_disposable);
      
      Observable.EveryUpdate()
        .Where(_=>Input.GetMouseButtonUp(0))
        .Subscribe(x => StopFire())
        .AddTo(_disposable);
#endif
  }

  public void Move(Vector2 value)
  {
    float dir = value.x != 0 ? (value.x > 0 ? 1 : -1) : 0;
    
    if(_transform != null)
    {
      _transform.Translate(Vector2.right * dir * _speed * Time.deltaTime);
      var position = _transform.position;
      float x = Mathf.Clamp(position.x, _logic.leftBottomScreen.x + 1, _logic.rightTopScreen.x - 1);
      position = new Vector3(x, position.y, position.z);
      _transform.position = position;
    }
  }

  void TimerFire()
  {
    SpawnMissile();
    Observable.Timer(System.TimeSpan.FromSeconds(_cooldown))
      .Repeat()
      .Subscribe(_ =>
      {
        if(_shooting) SpawnMissile();
        else
        {
          _canShoot = true;
          _disposableFire.Clear();
        }
      }).AddTo(_disposableFire);
  }
  
  public void StartFire()
  {
    if(!_canShoot) return;
    _canShoot = false;
    _shooting = true;
    TimerFire();
  }

  public void StopFire()
  {
    _shooting = false;
  }

  private void SpawnMissile()
  {
    if(_transform != null)
    {
      MissileControl missile = _factory.GetMissile((int) _missileType, _transform.position + Vector3.up);
      missile.Fly();
    }
  }
  
  private void TriggerEnter(Collider2D collision)
  {
    if (collision.tag.Equals("Enemy"))
    {
      _logic.CurrentLevelData.PlayerLifes.Value -= 1;
      _factory.DestroyAsteroid(collision.gameObject);
    }
  }

  private void OnDestroy()
  {
    _disposable.Clear();
  }
}
