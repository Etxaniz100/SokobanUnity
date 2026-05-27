using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
  [SerializeField] TMP_Text m_rStepsText;
  [SerializeField] TMP_Text m_rTimeText;
  [SerializeField] Image m_rFadeImage;

  private void Start()
  {
    GameManager.OnStepsCountChanged += UpdateSteps;
    EventLibrary.OnLevelLoaded += OnWin;
  }


  private void OnDestroy()
  {
    if(GameManager.OnStepsCountChanged != null)
    {
      GameManager.OnStepsCountChanged -= UpdateSteps;
    }
    EventLibrary.OnLevelLoaded -= OnWin;
  }

  void UpdateSteps(int _iTotalSteps)
  {
    if (m_rStepsText != null)
    {
      m_rStepsText.text = "Steps: " + _iTotalSteps.ToString();
    }
  }

  void OnWin()
  {
    m_fTiempo = 0;
    if (m_rTimeText != null)
    {
      int iMins = Mathf.FloorToInt(m_fTiempo / 60);
      int iSec = Mathf.FloorToInt(m_fTiempo % 60);
      m_rTimeText.text = $"{iMins:00}:{iSec:00}";
    }

    StartCoroutine(FadeInOut(1));
  }

  float m_fTiempo;
  private void Update()
  {
    m_fTiempo += Time.deltaTime;
    if (m_rTimeText != null)
    {
      int iMins = Mathf.FloorToInt(m_fTiempo / 60);
      int iSec = Mathf.FloorToInt(m_fTiempo % 60);
      m_rTimeText.text = $"{iMins:00}:{iSec:00}";
    }
    
  }



  IEnumerator FadeInOut(float _fDuration)
  {
    if(m_rFadeImage != null)
    {
      float fTimer = 0;

      float fFadeInTime = _fDuration / 4;
      float fFadeOutTime = _fDuration - fFadeInTime;


      //while (fTimer < fFadeInTime)
      //{
      //  fTimer += Time.deltaTime;
      //  m_rFadeImage.color = new Color(m_rFadeImage.color.r, m_rFadeImage.color.g, m_rFadeImage.color.b, fTimer / fFadeInTime);
      //  yield return null;
      //}

      fTimer = 0;




      while (fTimer < fFadeOutTime)
      {
        fTimer += Time.deltaTime;
        m_rFadeImage.color = new Color(m_rFadeImage.color.r, m_rFadeImage.color.g, m_rFadeImage.color.b, 1- fTimer / fFadeOutTime);
        yield return null;
      }

      m_rFadeImage.color = new Color(m_rFadeImage.color.r, m_rFadeImage.color.g, m_rFadeImage.color.b, 0);
    }
  }


}
