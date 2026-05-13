using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoxComponent : MonoBehaviour, IPushable
{

  private bool bIsMoving;

  [SerializeField]
  private float fTimerTimeToMove;


  private float m_fCellSize = 1;
  private float m_fTimeToMove = 1;
    public void SetData(float _fCellSize, float _fTimeToMove, GameManager _rManager)
  {
    m_fCellSize = _fCellSize;
    m_fTimeToMove = _fTimeToMove;
  }



  public bool Push(Vector3 _vMoveDirection)
  {
    bool bCanBePushed = CanBePushed(_vMoveDirection);

    if(!bCanBePushed)
    {
      return false;
    }

    StartCoroutine(MoveToCorroutine(transform.position, transform.position + _vMoveDirection * m_fCellSize, m_fTimeToMove));
    
    return true;
  }

  public bool IsAlreadyMoving()
  {
    return bIsMoving;
  }

  public bool MoveInDirection(Vector3 _vDirection)
  {
    if (!bIsMoving)
    {
      StartCoroutine(MoveToCorroutine(transform.position, transform.position + _vDirection * m_fCellSize, m_fTimeToMove));
    }
    return !bIsMoving;
  }

  IEnumerator MoveToCorroutine(Vector3 _vInitialPosition, Vector3 _vEndPosition, float _fDuration)
  {
    float fTimer = 0;
    bool bEnd = false;
    bIsMoving = true;

    while (!bEnd)
    {
      fTimer += Time.deltaTime;

      transform.position = Vector3.Lerp(_vInitialPosition, _vEndPosition, Mathf.Clamp(fTimer / _fDuration, 0, 1));

      if (Mathf.Clamp(fTimer / _fDuration, 0, 1) >= 1)
      {
        //End movement
        transform.position = _vEndPosition;
        bEnd = true;
      }

      yield return null;
    }

    bIsMoving = false;
  }

  public bool CanBePushed(Vector3 _vMoveDirection)
  {
    bool bCanMove = false;

    RaycastHit oHit;

    if (Physics.Raycast(transform.position, _vMoveDirection * m_fCellSize, out oHit, m_fCellSize))
    {
      if (oHit.collider.tag == "Wall")
      {
        bCanMove = false;
      }
      else if (oHit.collider.tag == "Box")
      {
        bCanMove = false;
      }
      else
      {
        bCanMove = true;
      }
    }
    else
    {
      bCanMove = true;
    }

    return bCanMove;
  }


  //private void OnTriggerExit(Collider other)
  //{
  //  if (other.gameObject.tag == "Flag")
  //  {
  //    m_rManagerReference.FlagReached(false);
  //  }
  //}
  //
  //private void OnTriggerEnter(Collider other) 
  //{
  //  if (other.gameObject.tag == "Flag")
  //  {
  //    m_rManagerReference.FlagReached(true);
  //  }
  //}

  // TODO: Change material color on flag reached

}


