using UniRx;
using UnityEngine.EventSystems;

namespace UI
{
  public class TouchButton : EventTrigger
  {
    public ReactiveProperty<bool> isFire { get; private set; }

    private void Awake()
    {
      isFire = new ReactiveProperty<bool>(false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
      base.OnPointerDown(eventData);
      isFire.Value = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
      base.OnPointerUp(eventData);
      isFire.Value = false;
    }
  }
}