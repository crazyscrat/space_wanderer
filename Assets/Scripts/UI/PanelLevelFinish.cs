using UI;
using UnityEngine;
using UnityEngine.UI;

public class PanelLevelFinish : MonoBehaviour
{
    [SerializeField] private Button _buttonMenu;
    private ViewController _viewController;

    public void Construct(ViewController viewController)
    {
        _viewController = viewController;
        
        _buttonMenu.onClick.AddListener(delegate
        {
            //_viewControl.ActivateMenu(true);
            _viewController.SetState(GameState.Menu);
        });
    }
}
