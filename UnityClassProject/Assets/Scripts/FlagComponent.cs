using UnityEngine;

public class FlagComponent : MonoBehaviour
{

  IPushable m_CurrentPushable;
  int m_iColor = 0;
  Color m_oColor;

  public void SetData(Color _oColor, int _iColor = 0)
  {
    if(m_CurrentPushable != null)
    {
      m_CurrentPushable.OnPush -= OnBoxMoved;
      m_CurrentPushable = null;
    }

    m_oColor = _oColor;
    m_iColor = _iColor;

    SetColor();
  }

  public void SetColor()
  {
    Material material = GetComponent<MeshRenderer>().material;

    material.color = m_oColor;
  }


  private void OnTriggerEnter(Collider other)
  {
    IPushable rComponet = other.gameObject.GetComponent<IPushable>();
    if (rComponet != null)
    {
      if (rComponet.GetColor() == m_iColor)
      {
        m_CurrentPushable = rComponet;
        m_CurrentPushable.OnPush += OnBoxMoved;
        m_CurrentPushable.SetSolved(true);
        EventLibrary.CallOnOnBoxOnEnd(true);
      }
    }
  }

  private void OnBoxMoved()
  {
    if(m_CurrentPushable == null)
    {
      return;
    }
    m_CurrentPushable.OnPush -= OnBoxMoved;
    m_CurrentPushable.SetSolved(false);
    m_CurrentPushable = null;
    EventLibrary.CallOnOnBoxOnEnd(false);

  }
}
