using Logic;
using TMPro;
using UnityEngine;

public class PanelMenu : MonoBehaviour
{
    [SerializeField] private Transform _parentLevelButtons;
    [SerializeField] private TMP_Text _textScore;
    
    private Factory _factory;
    private LevelButton[] _buttons;

    public void Construct(Factory factory)
    {
        _factory = factory;
    }
    
    public void GenerateLevelButtons()
    {
        if (_buttons != null)
        {
            foreach (LevelButton button in _buttons)
            {
                button.CheckStatus();
            }
        }
        else
        {
            _buttons = _factory.GenerateLevelButtons(_parentLevelButtons);
        }
    }

    public void SetScore(int score)
    {
        _textScore.text = $"Score: {score}";
    }
}
