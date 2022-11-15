using Logic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
  #region FIELDS

  [SerializeField] private Collider2D _collider;
  [SerializeField] private float _speed = 1f;
  [SerializeField] private float _cooldown = 0.3f;
  private Vector3 _startPosition;

  private CompositeDisposable _disposable = new CompositeDisposable();
  private CompositeDisposable _disposableFire = new CompositeDisposable();
  private LogicController _logic;
  private bool _shooting = false;

  private bool _canShoot = true;
  [SerializeField] private AmmoPower ammoPower = AmmoPower.Base;

  private Transform _transform;

  #endregion

  public void Construct(LogicController logic)
  {
    _transform = transform;
    _logic = logic;
    _transform.position = GameObject.FindWithTag("PlayerPosition").transform.position;

    _collider.OnTriggerEnter2DAsObservable()
      .Where(t=>t.gameObject.layer == LayerMask.NameToLayer("Interaction"))
      .Subscribe(collision => TriggerEnter(collision))
      .AddTo(_disposable);
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
      Ammo ammo = _logic.Factory.GetMissile(_transform.position + Vector3.up);
      ammo.Fly();
    }
  }
  
  private void TriggerEnter(Collider2D collision)
  {
    if (collision.tag.Equals("Enemy"))
    {
      _logic.ModelData.PlayerLifes.Value -= 1;
      _logic.Factory.DestroyAsteroid(collision.gameObject);
    }
  }

  private void OnDestroy()
  {
    _disposable.Clear();
  }
}
