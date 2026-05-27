using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

  LeverLoader m_rLeverLoader;

  private List<TurnData> m_tTurns;

  private LevelData m_oCurrentLevel;
  private int m_iCurrentLevel = -1;


  private int m_iNumberOfFlagsReached = 0;

  private float m_fLevelStartTime;

  int m_iNumberOfSteps = 0;


  public delegate void StepsCountChanged(int iTotalSteps);
  public static StepsCountChanged OnStepsCountChanged;



  // Data structures
  public struct TurnData
  {
    public Vector3 vMoveDirection;
    public PlayerController rPlayerController;
    public IPushable rPushable;
    public int iTurnId;
  }

 
  private void Start()
  {
    m_rLeverLoader = GetComponent<LeverLoader>();

    if(m_rLeverLoader == null)
    {
      throw new Exception();
    }

    m_tTurns = new List<TurnData>();

    EventLibrary.OnStep += EventOnStep;
    EventLibrary.OnBoxOnEnd += FlagReached;
    EventLibrary.OnUndoMove += UndoMove;
    EventLibrary.OnRestartLevel += RestartLevel;
    EventLibrary.OnTurnDone += DoMove;
    EventLibrary.OnNextLevelInput += AdvanceLevel;
    EventLibrary.OnLoadLevelByIndex += LoadLevelByIndex;

    LoadLevel(0);
    
  }

  private void OnDestroy()
  {
    EventLibrary.OnStep -= EventOnStep;
    EventLibrary.OnBoxOnEnd -= FlagReached;
    EventLibrary.OnUndoMove -= UndoMove;
    EventLibrary.OnRestartLevel -= RestartLevel;
    EventLibrary.OnTurnDone -= DoMove;
    EventLibrary.OnNextLevelInput -= AdvanceLevel;
  }

  void RestartLevel()
  {
    m_rLeverLoader.InstanciateLevel(m_iCurrentLevel);
  }

  public void LoadLevelByIndex(int iIndex)
  {
    LoadLevel(iIndex);
  }

  void AdvanceLevel()
  {
    LoadLevel(m_iCurrentLevel + 1);
  }

  bool LoadLevel(int _iLevel)
  {
    m_iCurrentLevel = _iLevel;

    EventLibrary.CallOnLevelLoaded();
    
    m_oCurrentLevel = m_rLeverLoader.InstanciateLevel(m_iCurrentLevel);

    if (m_oCurrentLevel != null)
    {
      m_tTurns.Clear();

      m_fLevelStartTime = Time.realtimeSinceStartup;
      
      m_iNumberOfSteps = 0;
      OnStepsCountChanged.Invoke(m_iNumberOfSteps);
      
      m_iNumberOfFlagsReached = 0;
      
      return true;
    }
    else
    {
      // TODO: No more levels
      return false;
    }
  }

  public float GetLevelStartTime()
  {
    return m_fLevelStartTime;
  }

  public void DoMove(TurnData rData)
  {
    rData.iTurnId = m_tTurns.Count;
    m_tTurns.Add(rData);
  }

  public void UndoMove()
  {
    if (m_tTurns.Count <= 0)
    {
      return;
    }

    TurnData rData = m_tTurns[m_tTurns.Count - 1];

    if(rData.rPlayerController == null)
    {
      m_tTurns.Remove(rData);
      return;
    }

    bool bApplyUndo = true;

    // If box exists, and cannot move, cant apply undo
    if(rData.rPushable != null && rData.rPushable.IsAlreadyMoving())
    {
      bApplyUndo = false;
    }

    // If after reaching here, we can apply the undo, check if the player allows it
    if (bApplyUndo && !rData.rPlayerController.CanMove())
    {
      bApplyUndo = false;
    }

    if(!bApplyUndo)
    {
      return;
    }

    m_tTurns.Remove(rData);
      
    rData.rPlayerController.MoveInDirection(-rData.vMoveDirection);

    if(rData.rPushable != null)
    {
      rData.rPushable.MoveInDirection(-rData.vMoveDirection);
    }

    m_iNumberOfSteps--;
    OnStepsCountChanged.Invoke(m_iNumberOfSteps);

  }

  public void FlagReached(bool bReached)
  {
    m_iNumberOfFlagsReached += bReached?1:-1;

    //Debug.Log("Points: " + m_iNumberOfFlagsReached);

    if (m_oCurrentLevel.tBoxEndPosition.Count == m_iNumberOfFlagsReached)
    {
      AdvanceLevel();
    }
  }

  public void EventOnStep()
  {
    m_iNumberOfSteps ++;
    OnStepsCountChanged.Invoke(m_iNumberOfSteps);
  }
}
