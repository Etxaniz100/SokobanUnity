using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class BoxComponent : MonoBehaviour, IPushable
{

  private bool bIsMoving;

  [SerializeField]
  private float fTimerTimeToMove;


  private float m_fCellSize = 1;
  private float m_fTimeToMove = 1;

  public event Action OnPush;

  public int m_iColor = 0;

  public Color m_oColor;

  public void SetData(float _fCellSize, float _fTimeToMove, Color _oColor, int _iColor = 0)
  {
    m_fCellSize = _fCellSize;
    m_fTimeToMove = _fTimeToMove;
    m_iColor = _iColor;
    m_oColor = _oColor;
    SetColor(false);
  }


  public void SetSolved(bool _bSolved)
  {
    SetColor(_bSolved);

  }

  public int GetColor()
  {
    return m_iColor;
  }

  public void SetColor(bool _bSolved)
  {
    Material material = GetComponent<MeshRenderer>().material;

    Color oColor = m_oColor;

    if (_bSolved) 
    {
      oColor = oColor / 2;
    }

    material.color = oColor;
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
    OnPush?.Invoke();

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


