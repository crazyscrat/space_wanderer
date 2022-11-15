using Control;
using Logic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class MissileControl : Ammo
{
    #region FIELDS

    [SerializeField] private Collider2D _collider;
    [SerializeField] private float _speed = 1f;
    
    private CompositeDisposable _disposable = new CompositeDisposable();
    private LogicController _logic;

    public AmmoPower Type => _type;

    private Transform _transform;

    #endregion

    private void Awake()
    {
        _transform = transform;
    }

    public override void Construct(LogicController logic)
    {
        _logic = logic;
    }
    
    public override Sprite GetSprite()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }
    
    public override void Fly()
    {
        _collider.OnTriggerEnter2DAsObservable()
            .Where(
                t => t.gameObject.tag.Equals("Enemy") 
                        || t.gameObject.tag.Equals("Finish")
                        )
            .Subscribe(collision => TriggerEnter(collision)).AddTo(_disposable);
        
        Observable.EveryUpdate().Subscribe(x => Move()).AddTo(_disposable);
    }

    private void Move()
    {
        _transform.Translate(Vector3.up * _speed * Time.deltaTime);
    }

    private void TriggerEnter(Collider2D collision)
    {
        if(collision.gameObject.tag.Equals("Enemy"))
        {
            collision.GetComponent<Enemy>().Hit();
            _logic.ModelData.EnemiesDestroyed.Value++;
        }
        Disable();
    }

    private void Disable()
    {
        _disposable.Clear();
        gameObject.SetActive(false);
    }

    public override void Destroy()
    {
        _disposable.Clear();
        Destroy(gameObject);
    }
}