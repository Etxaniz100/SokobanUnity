using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

  private bool m_bMoving;
  private bool m_bCanMove;
  private Vector3 vMoveDirection;
  private Vector3 vInitialPosition;
  private Vector3 vEndPosition;

  private float m_fTimeToMove;
  private float m_fCellSize;

  private float m_fLevelStartTime;


  public InputActionReference m_rMoveInputAction;
  public InputActionReference m_rUndoInputAction;
  public InputActionReference m_rResetInputAction;


  private void Start()
  {
    EventLibrary.OnLevelLoaded += LevelWon;
    //m_rResetInputAction.action.performed += RestartLevel;
  }

  private void OnDestroy()
  {
    EventLibrary.OnLevelLoaded -= LevelWon;
    //m_rResetInputAction.action.performed -= RestartLevel;
  }

  public void BackToLevelSelector(InputAction.CallbackContext _cbc)
  {
    if (!this.isActiveAndEnabled || _cbc.phase != InputActionPhase.Performed)
    {
      return;
    }

    SceneManager.LoadScene("MainMenu");
  }

  public void RestartLevel(InputAction.CallbackContext _cbc)
  {
    if (!this.isActiveAndEnabled || _cbc.phase != InputActionPhase.Performed)
    {
      return;
    }

    EventLibrary.CallOnRestartLevel();
  }

  public void NextLevel(InputAction.CallbackContext _cbc)
  {
    if (!this.isActiveAndEnabled || _cbc.phase != InputActionPhase.Performed)
    {
      return;
    }

    EventLibrary.CallOnNextLevelInput();
  }


  public void UndoMove(InputAction.CallbackContext _cbc)
  {
    if (!this.isActiveAndEnabled || !m_bCanMove || m_bMoving || _cbc.phase != InputActionPhase.Performed)
    {
      return;
    }

    EventLibrary.CallOnUndoMove();
  }

  public void SetData(float _fCellSize, float _fTimeToMove)
  {
    m_fCellSize = _fCellSize;
    m_fTimeToMove = _fTimeToMove;
    m_bMoving = false;
    m_fLevelStartTime = Time.realtimeSinceStartup;
    m_bCanMove = true;
  }

  public void Move(InputAction.CallbackContext _cbc)
  {
    if(!this.isActiveAndEnabled || m_bMoving || Time.realtimeSinceStartup - m_fLevelStartTime < m_fTimeToMove || !m_bCanMove)
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

    vInitialPosition = transform.position;
    vEndPosition = vInitialPosition + vMoveDirection * m_fCellSize;

    RaycastHit oHit;

    bool bBoxPushed = false;
    IPushable oBox = null;

    bool bCanMove = true;

    if (Physics.Raycast(vInitialPosition, vMoveDirection * m_fCellSize, out oHit, m_fCellSize))
    {
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
    }

    if(bCanMove)
    {
      MoveTo(vInitialPosition, vEndPosition);

      EventLibrary.CallOnStep();

      GameManager.TurnData data = new GameManager.TurnData();
      data.rPlayerController = this;
      data.vMoveDirection = vMoveDirection;
      data.rPushable = oBox;

      EventLibrary.CallOnTurnDone(data);
    }
  }

  public bool MoveTo(Vector3 _vInitialPosition, Vector3 _vEndPosition)
  {
    if(m_bCanMove && !m_bMoving && this.enabled)
    {
      StartCoroutine(MoveToCorroutine(_vInitialPosition, _vEndPosition, m_fTimeToMove));
    }
    return !m_bMoving;
  }

  public bool CanMove()
  {
    return m_bCanMove && !m_bMoving;
  }
  public bool MoveInDirection(Vector3 _vDirection)
  {
    if (m_bCanMove && !m_bMoving)
    {
      vInitialPosition = transform.position;
      vEndPosition = vInitialPosition + _vDirection * m_fCellSize;
      StartCoroutine(MoveToCorroutine(vInitialPosition, vEndPosition, m_fTimeToMove));

      return true;
    }
    return false;
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
    m_bCanMove = false;
    m_bMoving = false;
  }


  public struct TurnData
  {
    public Vector3 vDir;

  }
}
