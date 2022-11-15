using UI;
using UnityEngine;
using UnityEngine.UI;

public class PanelLevelFinish : MonoBehaviour
{
    #region FIELDS

    [SerializeField] private Button _buttonMenu;
    private ViewController _viewController;

    #endregion

    public void Construct(ViewController viewController)
    {
        _viewController = viewController;
        
        _buttonMenu.onClick.AddListener(delegate
        {
            //_viewControl.ActivateMenu(true);
            _viewController.SetState(GameStateEnum.Menu);
        });
    }
}
