using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
  #region FIELDS

  [SerializeField] private TMP_Text _textButton;
  [SerializeField] private Button _button;

  private LogicController _logic;
  private int _levelNum;

  #endregion

  public void Construct(int index, LogicController logic)
  {
    _levelNum = index+1;
    _logic = logic;
    gameObject.name = $"level_{index + 1}";
    _textButton.text = (index + 1).ToString();

    _button.onClick.AddListener(() =>
    {
      _logic.SelectLevel.Execute(_levelNum);
    });
    
    CheckStatus();
  }

  public void CheckStatus()
  {
    _button.interactable = _logic.SavedStateData.LastOpenLevel >= _levelNum;
  }
}