using UnityEngine;
using UnityEngine.UI;

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

    LevelManager[] tLevelManagerList = FindObjectsByType<LevelManager>(FindObjectsSortMode.InstanceID);

    foreach(LevelManager rLevelManager in tLevelManagerList)
    {
        // Al obtener el LevelManager, puede estar siendo destruido
        if(rLevelManager == null || rLevelManager.m_bToBeDeleted)
        {
            continue;
        }

		int iNumberOfLevels = rLevelManager?rLevelManager.GetNumberOfLevels():0;

        for (int i = 0; i < iNumberOfLevels; i++)
        {
            LevelSelectorButton oButton = Instantiate(m_oButtonPrefab, m_rGridContent);
            if (oButton != null)
            {
            oButton.SetUp(i, rLevelManager.IsLevelUnlocked(i));
            }
        }

        break;
    }
  }
}
