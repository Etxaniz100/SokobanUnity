using UnityEngine;

public class EventLibrary
{
  public delegate void StepDelegate();
  public static event StepDelegate OnStep;
  public static void CallOnStep()
  {
    OnStep?.Invoke();
  }


  public delegate void BoxOnEndDelegate(bool bParam);
  public static event BoxOnEndDelegate OnBoxOnEnd;
  public static void CallOnOnBoxOnEnd(bool bParam)
  {
    OnBoxOnEnd?.Invoke(bParam);
  }


  public delegate void UndoMoveDelegate();
  public static event UndoMoveDelegate OnUndoMove;
  public static void CallOnUndoMove()
  {
    OnUndoMove?.Invoke();
  }


  public delegate void WinDelegate();
  public static event WinDelegate OnWin;
  public static void CallOnWin()
  {
    OnWin?.Invoke();
  }


  public delegate void RestartLevelDelegate();
  public static event RestartLevelDelegate OnRestartLevel;
  public static void CallOnRestartLevel()
  {
    OnRestartLevel?.Invoke();
  }


}
