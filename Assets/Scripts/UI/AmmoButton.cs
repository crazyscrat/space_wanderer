using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  public class AmmoButton : MonoBehaviour
  {
    #region FIELDS

    public TouchButton TouchButton;
    
    [SerializeField] private Image _imageAmmo;

    #endregion
    
    public void SetImage(Sprite sprite)
    {
      _imageAmmo.sprite = sprite;
    }
  }
}