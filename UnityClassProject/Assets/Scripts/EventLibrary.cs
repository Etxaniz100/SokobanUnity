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


  public delegate void LevelLoadedDelegate();
  public static event LevelLoadedDelegate OnLevelLoaded;
  public static void CallOnLevelLoaded()
  {
    OnLevelLoaded?.Invoke();
  }


  public delegate void RestartLevelDelegate();
  public static event RestartLevelDelegate OnRestartLevel;
  public static void CallOnRestartLevel()
  {
    OnRestartLevel?.Invoke();
  }

  public delegate void TurnDoneDelegate(GameManager.TurnData rTurn);
  public static event TurnDoneDelegate OnTurnDone;
  public static void CallOnTurnDone(GameManager.TurnData rTurn)
  {
    OnTurnDone?.Invoke(rTurn);
  }

  public delegate void NextLevelInputDelegate();
  public static event NextLevelInputDelegate OnNextLevelInput;
  public static void CallOnNextLevelInput()
  {
    OnNextLevelInput?.Invoke();
  }



  public delegate void LoadLevelByIndexDelegate(int iIndex);
  public static event LoadLevelByIndexDelegate OnLoadLevelByIndex;
  public static void CallOnLoadLevelByIndex(int iIndex)
  {
    OnLoadLevelByIndex?.Invoke(iIndex);
  }
}
