using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

  private bool bMoving;
  private Vector3 vMoveDirection;
  private Vector3 vInitialPosition;
  private Vector3 vEndPosition;

  [SerializeField]
  private float fTimeToMove;
  private float fTimerTimeToMove;
  
  [SerializeField]
  private float fCellSize;


  [SerializeField]
  private float fSphereCastRadius;

  public InputActionReference m_rMoveInputAction;
  public InputActionReference m_rUndoInputAction;

  [SerializeField]
  public GameManager m_rManagerReference;


  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
        
  }

  private void OnDrawGizmos()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  // TODO: Add IPushable interface for boxes
  // TODO: Move camera on level start
  public void UndoMove(InputAction.CallbackContext _cbc)
  {
    if (bMoving)
    {
      return;
    }

    //Debug.Log(m_rUndoInputAction.action.ReadValue<bool>());
    //Debug.Log(_cbc);
    

    m_rManagerReference.UndoMove();
  }


  public void Move(InputAction.CallbackContext _cbc)
  {
    if (bMoving)
    {
      return;
    }

    vMoveDirection = new Vector3(m_rMoveInputAction.action.ReadValue<Vector2>().x, 0, m_rMoveInputAction.action.ReadValue<Vector2>().y);

    // The input allows diagonals, so we prioritize the x axis 
    if(vMoveDirection.x != 0)
    {
      // As it allows diagonal, if x is pressed we pass it to 1
      vMoveDirection.x = vMoveDirection.x>0?1:-1;
      vMoveDirection.z = 0;
    }
    else if(vMoveDirection.z != 0)
    {
      vMoveDirection.x = 0;
      vMoveDirection.z = vMoveDirection.z > 0 ? 1 : -1;
    }

    if(vMoveDirection.x == 0 && vMoveDirection.z == 0)
    {
      return;
    }

    fTimerTimeToMove = 0;
    vInitialPosition = transform.position;
    vEndPosition = vInitialPosition + vMoveDirection * fCellSize;

    RaycastHit oHit;

    bool bCanMove = false;
    bool bBoxPushed = false;
    BoxComponent oBox = null;

    //Debug.DrawLine(vInitialPosition, vEndPosition, new UnityEngine.Color(0, 1, 0), 1);

    if (Physics.Raycast(vInitialPosition, vMoveDirection * fCellSize, out oHit, fCellSize))
    {
      //Debug.DrawLine(oHit.point, oHit.point + Vector3.up, new UnityEngine.Color(0, 1, 0), 1);
      if (oHit.collider.tag == "Wall")
      {
        bCanMove = false;
      }
      else if (oHit.collider.tag == "Box")
      {
        oHit.collider.gameObject.TryGetComponent<BoxComponent>(out oBox);

        if (oBox != null)
        {
          bCanMove = oBox.Push(vMoveDirection, fTimeToMove, fCellSize);
          bBoxPushed = bCanMove;
        }
      }
    }
    else
    {
      bCanMove = true;
    }

    if(bCanMove)
    {
      MoveTo(vInitialPosition, vEndPosition);

      // TODO: Send info (of action) to game manager
      GameManager.TurnData data = new GameManager.TurnData();
      data.rPlayerController = this;
      data.vMoveDirection = vMoveDirection;
      data.rMovedBox = oBox;
      m_rManagerReference.DoMove(data);
    }
  }

  public bool MoveTo(Vector3 _vInitialPosition, Vector3 _vEndPosition)
  {
    if(!bMoving)
    {
      StartCoroutine(MoveToCorroutine(_vInitialPosition, _vEndPosition, fTimeToMove));
    }
    return !bMoving;
  }

  public bool CanMove()
  {
    return !bMoving;
  }
  public bool MoveInDirection(Vector3 _vDirection)
  {
    if (!bMoving)
    {
      vInitialPosition = transform.position;
      vEndPosition = vInitialPosition + _vDirection * fCellSize;
      StartCoroutine(MoveToCorroutine(vInitialPosition, vEndPosition, fTimeToMove));
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
}
