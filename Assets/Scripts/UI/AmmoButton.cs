using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  public class AmmoButton : MonoBehaviour
  {
    public TouchButton TouchButton;
    
    [SerializeField] private Image _imageAmmo;
    public void SetImage(Sprite sprite)
    {
      _imageAmmo.sprite = sprite;
    }
  }
}