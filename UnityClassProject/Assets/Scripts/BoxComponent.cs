using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoxComponent : MonoBehaviour
{

  private bool bMoving;
  private Vector3 vInitialPosition;
  private Vector3 vEndPosition;

  [SerializeField]
  private float fTimerTimeToMove;


  private float fTimeToMove;
  private float fCellSize;

  private void OnDrawGizmos()
  {

  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  //void Start()
  //{
  //      
  //}

  // Update is called once per frame
  //void Update()
  //{
  //
  //}

  public bool Push(Vector3 _vMoveDirection, float _fTimeToMove, float _fCellSize)
  {
    fTimeToMove = _fTimeToMove;
    fCellSize = _fCellSize;

    bool bCanMove = false;

    fTimerTimeToMove = 0;
    vInitialPosition = transform.position;
    vEndPosition = vInitialPosition + _vMoveDirection * fCellSize;
    RaycastHit oHit;

    //Debug.DrawLine(vInitialPosition, vEndPosition, new UnityEngine.Color(0, 1, 0), 1);

    if (Physics.Raycast(vInitialPosition, _vMoveDirection * fCellSize, out oHit, fCellSize))
    {

      //Debug.DrawLine(oHit.point, oHit.point + Vector3.up, new UnityEngine.Color(0, 0, 1), 1);
       
      if (oHit.collider.tag == "Wall")
      {
        bCanMove = false;
      }
      else if (oHit.collider.tag == "Box")
      {
        bCanMove = false;
        //BoxComponent oBox = null;
        //oHit.collider.gameObject.TryGetComponent<BoxComponent>(out oBox);
        //
        //if (oBox != null)
        //{
        //  bCanMove = oBox.Push(_vMoveDirection, fTimeToMove, fCellSize);
        //}
      }
      else
      {
        bCanMove = true;
      }

      //Debug.Log("Hit: " + oHit.collider.tag + " bCanMove : " + bCanMove);
    }
    else
    {
      bCanMove = true;
    }

    if(bCanMove)
    {
      StartCoroutine(MoveToCorroutine(vInitialPosition, vEndPosition, fTimeToMove));
    }

    return bCanMove;
  }

  public bool CanMove()
  {
    return !bMoving;
  }

  public bool MoveInDirection(Vector3 _vDirection)
  {
    if (!bMoving)
    {
      StartCoroutine(MoveToCorroutine(transform.position, transform.position + _vDirection * fCellSize, fTimeToMove));
    }
    return !bMoving;
  }

  IEnumerator MoveToCorroutine(Vector3 _vInitialPosition, Vector3 _vEndPosition, float _fDuration)
  {
    float fTimer = 0;
    bool bEnd = false;
    bMoving = true;

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

    bMoving = false;
  }


  IEnumerator Draw(Vector3 _vInitialPosition, Vector3 _vEndPosition, float _fDuration)
  {



    float fTimer = 0;
    bool bEnd = false;
    bMoving = true;

    while (!bEnd)
    {
      fTimer += Time.deltaTime;

      

      if (Mathf.Clamp(fTimer / _fDuration, 0, 1) >= 1)
      {
        //End movement
        transform.position = vEndPosition;
        bEnd = true;
      }

      yield return null;
    }

    bMoving = false;
  }

}


