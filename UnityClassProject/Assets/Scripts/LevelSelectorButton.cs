using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectorButton : MonoBehaviour
{
  [SerializeField] TMP_Text m_rText;
  [SerializeField] Button m_rButton;
  int m_iLevelIndex = 0;

  public void SetUp(int _iLevel, bool _bIsUnlocked)
  {
    m_rButton.onClick.AddListener(OnClick);
    m_iLevelIndex = _iLevel;
    m_rText.text = "Level " + m_iLevelIndex;

    m_rButton.interactable = _bIsUnlocked;
    
  }

  void OnClick()
  {
    EventLibrary.CallOnLoadLevelByIndex(m_iLevelIndex);
  }
}
