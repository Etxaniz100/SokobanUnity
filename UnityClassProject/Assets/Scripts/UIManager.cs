using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
  [SerializeField] TMP_Text m_rStepsText;

  private void Start()
  {
    GameManager.OnStepsCountChanged += UpdateSteps;
  }

  void UpdateSteps(int _iTotalSteps)
  {
    if (m_rStepsText != null)
    {
      m_rStepsText.text = _iTotalSteps.ToString();
    }
  }


}
