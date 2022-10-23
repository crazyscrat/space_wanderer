using System;
using Control;
using Logic;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;

public class MissileControl : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private MissileType _type = MissileType.Base;
    
    private CompositeDisposable _disposable = new CompositeDisposable();
    private LogicController _logic;

    public MissileType Type => _type;

    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    public void Construct(LogicController logic)
    {
        _logic = logic;
    }
    
    public void Fly()
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
            //TODO add score
            _logic.ModelData.EnemiesDestroyed.Value++;
        }
        Disable();
    }

    private void Disable()
    {
        _disposable.Clear();
        gameObject.SetActive(false);
    }

    public void Destroy()
    {
        _disposable.Clear();
        Destroy(gameObject);
    }
}