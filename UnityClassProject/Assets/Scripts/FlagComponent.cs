using UnityEngine;

public class FlagComponent : MonoBehaviour
{

  public void SetData()
  {

  }

  private void OnTriggerExit(Collider other)
  {
    if (other.gameObject.GetComponent<IPushable>() != null)
    {
      EventLibrary.CallOnOnBoxOnEnd(false);
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.GetComponent<IPushable>() != null)
    {
      EventLibrary.CallOnOnBoxOnEnd(true);
    }
  }
}
