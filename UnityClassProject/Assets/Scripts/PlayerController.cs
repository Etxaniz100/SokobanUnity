using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

  private bool m_bMoving;
  private bool m_bInitialized = false;
  private Vector3 vMoveDirection;
  private Vector3 vInitialPosition;
  private Vector3 vEndPosition;

  private float m_fTimeToMove;
  private float fTimerTimeToMove;

  private float m_fLevelStartTime;

  private float m_fCellSize;

  [SerializeField]
  private float fSphereCastRadius;

  public InputActionReference m_rMoveInputAction;
  public InputActionReference m_rUndoInputAction;

  public GameManager m_rManagerReference;

  private void Start()
  {
    EventLibrary.OnWin += LevelWon;
  }

  private void OnDestroy()
  {
    EventLibrary.OnWin -= LevelWon;
  }


  public void RestartLevel(InputAction.CallbackContext _cbc)
  {
    if (!this.isActiveAndEnabled)
    {
      return;
    }

    EventLibrary.CallOnRestartLevel();
  }

  // TODO: Move camera on level start
  public void UndoMove(InputAction.CallbackContext _cbc)
  {
    if (!this.isActiveAndEnabled)
    {
      return;
    }
    if (m_bMoving)
    {
      return;
    }

    //Debug.Log(m_rUndoInputAction.action.ReadValue<bool>());
    //Debug.Log(_cbc);

    EventLibrary.CallOnUndoMove();
  }

  public void SetData(float _fCellSize, float _fTimeToMove, GameManager _rManager)
  {
    m_fCellSize = _fCellSize;
    m_fTimeToMove = _fTimeToMove;
    m_rManagerReference = _rManager;
    m_bMoving = false;
    m_fLevelStartTime = Time.realtimeSinceStartup;
    m_bInitialized = true;
  }

  public void Move(InputAction.CallbackContext _cbc)
  {
    if(!this.isActiveAndEnabled)
    {
      return;
    }

    if (m_bMoving)
    {
      return;
    }

    if(Time.realtimeSinceStartup - m_fLevelStartTime < m_fTimeToMove)
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
    vEndPosition = vInitialPosition + vMoveDirection * m_fCellSize;

    RaycastHit oHit;

    bool bCanMove = false;
    bool bBoxPushed = false;
    IPushable oBox = null;

    //Debug.DrawLine(vInitialPosition, vEndPosition, new UnityEngine.Color(0, 1, 0), 1);

    if (Physics.Raycast(vInitialPosition, vMoveDirection * m_fCellSize, out oHit, m_fCellSize))
    {
      //Debug.DrawLine(oHit.point, oHit.point + Vector3.up, new UnityEngine.Color(0, 1, 0), 1);
      if (oHit.collider.tag == "Wall")
      {
        bCanMove = false;
      }
      else if (oHit.collider.tag == "Box")
      {
        oHit.collider.gameObject.TryGetComponent<IPushable>(out oBox);

        if (oBox != null)
        {
          bCanMove = oBox.Push(vMoveDirection);
          
          bBoxPushed = bCanMove;
        }
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

    if(bCanMove)
    {
      MoveTo(vInitialPosition, vEndPosition);

      EventLibrary.CallOnStep();

      // TODO: Send info (of action) to game manager
      GameManager.TurnData data = new GameManager.TurnData();
      data.rPlayerController = this;
      data.vMoveDirection = vMoveDirection;
      data.rPushable = oBox;
      m_rManagerReference.DoMove(data);
    }
  }

  public bool MoveTo(Vector3 _vInitialPosition, Vector3 _vEndPosition)
  {
    if(!m_bMoving && this.enabled)
    {
      StartCoroutine(MoveToCorroutine(_vInitialPosition, _vEndPosition, m_fTimeToMove));
    }
    return !m_bMoving;
  }

  public bool CanMove()
  {
    return !m_bMoving;
  }
  public bool MoveInDirection(Vector3 _vDirection)
  {
    if (!m_bMoving)
    {
      vInitialPosition = transform.position;
      vEndPosition = vInitialPosition + _vDirection * m_fCellSize;
      StartCoroutine(MoveToCorroutine(vInitialPosition, vEndPosition, m_fTimeToMove));
    }
    return !m_bMoving;
  }

  IEnumerator MoveToCorroutine(Vector3 _vInitialPosition, Vector3 _vEndPosition, float _fDuration)
  {
    float fTimer = 0;
    bool bEnd = false;
    m_bMoving = true;

    while (m_bMoving && !bEnd)
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

    m_bMoving = false;
  }

  void LevelWon()
  {
    m_bInitialized = false;
    m_bMoving = false;
  }
}
