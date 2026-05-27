using UnityEngine;

public class LevelSelector : MonoBehaviour
{
  [SerializeField] LevelSelectorButton m_oButtonPrefab;

  [SerializeField] Transform m_rGridContent;

  void Start()
  {
    GenerateButtons();
  }

  void GenerateButtons()
  {
    for(int i = 0; i < 10; i++)
    {
      LevelSelectorButton oButton = Instantiate(m_oButtonPrefab, m_rGridContent);
      if (oButton != null)
      {
        oButton.SetUp(i);
      }
    }
  }
}
