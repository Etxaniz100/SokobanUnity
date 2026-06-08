using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

  private List<TurnData> m_tTurns;

  private int m_iCurrentLevel = -1;

  private int m_iNumberOfFlagsReached = 0;

  int m_iNumberOfSteps = 0;

  public delegate void StepsCountChanged(int iTotalSteps);
  public static StepsCountChanged OnStepsCountChanged;

  LevelManager m_rLevelManager;


  // Data structures
  public struct TurnData
  {
    public Vector3 vMoveDirection;
    public PlayerController rPlayerController;
    public IPushable rPushable;
  }

  private void Awake()
  {
    EventLibrary.OnStep += EventOnStep;
    EventLibrary.OnBoxOnEnd += FlagReached;
    EventLibrary.OnUndoMove += UndoMove;
    EventLibrary.OnRestartLevel += RestartLevel;
    EventLibrary.OnTurnDone += DoMove;
    EventLibrary.OnNextLevelInput += AdvanceLevel;
    EventLibrary.OnLoadLevelByIndex += LoadLevelByIndex;
  }

  private void Start()
  {
    m_tTurns = new List<TurnData>();

    m_rLevelManager = FindFirstObjectByType<LevelManager>();
    
    LoadLevel(m_rLevelManager?m_rLevelManager.GetCurrentLevel():0);  
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
    LoadLevel(m_iCurrentLevel);
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
    bool bCorrect = m_rLevelManager?m_rLevelManager.LoadLevel(_iLevel):false;
    
    if (bCorrect)
    {
      m_iCurrentLevel = _iLevel;
      
      m_tTurns.Clear();

      m_iNumberOfSteps = 0;
      OnStepsCountChanged?.Invoke(m_iNumberOfSteps);
      
      m_iNumberOfFlagsReached = 0;
      
      return true;
    }
    else
    {
      // TODO: No more levels
      return false;
    }
  }


  public void DoMove(TurnData rData)
  {
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
      
    rData.rPlayerController.MoveInDirection(-rData.vMoveDirection, false);

    if(rData.rPushable != null)
    {
      rData.rPushable.MoveInDirection(-rData.vMoveDirection);
    }

    m_iNumberOfSteps--;
    OnStepsCountChanged?.Invoke(m_iNumberOfSteps);

  }

  public void FlagReached(bool bReached)
  {
    m_iNumberOfFlagsReached += bReached?1:-1;

    //Debug.Log("Points: " + m_iNumberOfFlagsReached);

    if (m_rLevelManager.GetNumberOfFlagsNedded() == m_iNumberOfFlagsReached)
    {
      AdvanceLevel();
    }
  }

  public void EventOnStep()
  {
    m_iNumberOfSteps ++;
    OnStepsCountChanged?.Invoke(m_iNumberOfSteps);
  }
}
