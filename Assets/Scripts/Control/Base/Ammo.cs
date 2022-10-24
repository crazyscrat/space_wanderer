using Logic;
using UnityEngine;

public abstract class Ammo : MonoBehaviour
{
  [SerializeField]
  protected string NameAmmo;
  public AmmoPower _type = AmmoPower.Base;
  public AmmoType AmmoType = AmmoType.Missile;
  public int id = 0;
  
  public abstract void Fly();
  public abstract void Destroy();
  public abstract void Construct(LogicController logic);
  public abstract Sprite GetSprite();
}
