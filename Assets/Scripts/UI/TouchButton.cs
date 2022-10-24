using UniRx;
using UnityEngine.EventSystems;

namespace UI
{
  public class TouchButton : EventTrigger
  {
    public ReactiveProperty<bool> isAction { get; private set; }

    private void Awake()
    {
      isAction = new ReactiveProperty<bool>(false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
      base.OnPointerDown(eventData);
      isAction.Value = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
      base.OnPointerUp(eventData);
      isAction.Value = false;
    }
  }
}