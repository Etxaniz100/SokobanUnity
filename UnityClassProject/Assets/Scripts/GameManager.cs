using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

  private List<TurnData> m_tTurns;

  public GameObject m_oBoxPrefab;
  public GameObject m_oWallPrefab;
  public GameObject m_oPlayerPrefab;
  public GameObject m_oEndFlagPrefab;

  private bool bWon = false;

  private List<LevelData> m_tLevels;
  private LevelData m_oCurrentLevel;
  private int m_iCurrentLevel = -1;

  public string m_sFilesPath = "LevelFiles/";
  [SerializeField]
  public List<string> m_tLevelsFilenameList;

  [SerializeField]
  private float m_fCellSize = 1;

  [SerializeField]
  private float m_fTimeToMove = 1;

  private int m_iNumberOfFlagsReached = 0;

  private float m_fLevelStartTime;
  // Object pools

  int m_iNumberOfSteps = 0;

  List<GameObject> m_tWallObjectPool;
  GameObject m_rPlayer;
  List<GameObject> m_tBoxObjectPool;
  List<GameObject> m_tFlagObjectPool;

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
    m_tTurns = new List<TurnData>();
    m_tLevels = new List<LevelData>();

    m_tWallObjectPool = new List<GameObject>();
    m_tBoxObjectPool = new List<GameObject>();
    m_tFlagObjectPool = new List<GameObject>();

    foreach (string file in m_tLevelsFilenameList)
    {
      TextAsset oLoadedFile = Resources.Load<TextAsset>(m_sFilesPath + file);
      if(oLoadedFile == null )
      {
        Debug.Log("File not found : " + m_sFilesPath + file);
        continue;
      }
      LevelData oCurrentData = new LevelData();

      Vector2 vCurrentPos = new Vector2(0, 0);
      foreach(char c in oLoadedFile.text)
      {
        switch(c)
        {
          case '#':
            oCurrentData.tWallPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
            vCurrentPos.x++;
            break;

          case 'P':
            oCurrentData.vPlayerStartPoint = new Vector2(vCurrentPos.x, vCurrentPos.y);
            vCurrentPos.x++;
            break;
          
          case 'S':
            oCurrentData.tBoxStartPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
            vCurrentPos.x++;
            break;

          case '$':
            oCurrentData.tBoxStartPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
            oCurrentData.tBoxEndPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
            vCurrentPos.x++;
            break;

          case '!':
            oCurrentData.tBoxEndPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
            vCurrentPos.x++;
            break;

          case '\n':
            vCurrentPos.y--;
            vCurrentPos.x = 0;
            break;

          case ' ':
          default:
            vCurrentPos.x++;
            break;
        }
      }

      m_tLevels.Add(oCurrentData);
    }


    EventLibrary.OnStep += EventOnStep;
    EventLibrary.OnBoxOnEnd += FlagReached;
    EventLibrary.OnUndoMove += UndoMove;


    if (m_tLevels.Count > 0)
    {
      m_iCurrentLevel = 0;
      m_oCurrentLevel = m_tLevels[m_iCurrentLevel];
      LoadLevel(m_oCurrentLevel);
    }
  }

  private void OnDestroy()
  {
    EventLibrary.OnStep -= EventOnStep;
    EventLibrary.OnBoxOnEnd -= FlagReached;
    EventLibrary.OnUndoMove -= UndoMove;
  }

  private void UnloadLevel()
  {

    foreach (GameObject rObject in m_tWallObjectPool)
    {
      rObject.SetActive(false);
    }

    foreach (GameObject rObject in m_tBoxObjectPool)
    {
      rObject.SetActive(false);
    }

    foreach (GameObject rObject in m_tFlagObjectPool)
    {
      rObject.SetActive(false);
    }

    if(m_rPlayer)
    {
      m_rPlayer.SetActive(false);
    }
  }

  void LoadLevel(LevelData rData)
  {

    UnloadLevel();

    m_tTurns.Clear();

    m_iNumberOfSteps = 0;

    m_oCurrentLevel = rData;

    // Set player
    if (m_rPlayer == null)
    {
      GameObject oPlayerObject = Instantiate(m_oPlayerPrefab);
      m_rPlayer = oPlayerObject;

      PlayerController rController = m_rPlayer.GetComponent<PlayerController>();
    }
    else
    {
      m_rPlayer.SetActive(true);
    }
    PlayerController oPlayerController = m_rPlayer.GetComponent<PlayerController>();
    oPlayerController.m_rManagerReference = this;
    oPlayerController.SetData(m_fCellSize, m_fTimeToMove, this);

    m_rPlayer.transform.position = new Vector3(rData.vPlayerStartPoint.x * m_fCellSize, 0.5f, rData.vPlayerStartPoint.y * m_fCellSize);

    // Set walls
    foreach (Vector2 vWallPosition in rData.tWallPosition)
    {

      GameObject oWallObject = null;
      bool bFound = false;

      foreach(GameObject rGameObject in m_tWallObjectPool)
      {
        if(!rGameObject.activeSelf)
        {
          rGameObject.SetActive(true);
          oWallObject = rGameObject;
          bFound = true;
          break;
        }
      }

      if(oWallObject == null)
      {
        oWallObject = Instantiate(m_oWallPrefab);
      }
      oWallObject.transform.position = new Vector3(vWallPosition.x * m_fCellSize, 0.5f, vWallPosition.y * m_fCellSize);
    
      if(!bFound)
      {
        m_tWallObjectPool.Add(oWallObject);
      }
    }

    // Set end flags
    foreach (Vector2 vFlagPosition in rData.tBoxEndPosition)
    {
      GameObject oFlagObject = null;
      bool bFound = false;

      foreach (GameObject rGameObject in m_tFlagObjectPool)
      {
        if (!rGameObject.activeSelf)
        {
          rGameObject.SetActive(true);
          oFlagObject = rGameObject;
          bFound = true;
          break;
        }
      }

      if (oFlagObject == null)
      {
        oFlagObject = Instantiate(m_oEndFlagPrefab);
      }

      oFlagObject.transform.position = new Vector3(vFlagPosition.x * m_fCellSize, 0f, vFlagPosition.y * m_fCellSize);

      FlagComponent rFlagComponent = oFlagObject.GetComponent< FlagComponent>();

      if(rFlagComponent != null)
      {
        rFlagComponent.SetData();
      }


      if (!bFound)
      {
        m_tFlagObjectPool.Add(oFlagObject);
      }

    }

    
    // Set boxes
    foreach (Vector2 vBoxPosition in rData.tBoxStartPosition)
    {
      GameObject oBoxObject = null;
      bool bFound = false;

      foreach (GameObject rGameObject in m_tBoxObjectPool)
      {
        if (!rGameObject.activeSelf)
        {
          rGameObject.SetActive(true);
          oBoxObject = rGameObject;
          bFound = true;
          break;
        }
      }

      if(oBoxObject == null)
      {
        oBoxObject = Instantiate(m_oBoxPrefab);
      }

      oBoxObject.transform.position = new Vector3(vBoxPosition.x * m_fCellSize, 0.5f, vBoxPosition.y * m_fCellSize);
      BoxComponent oComp = oBoxObject.GetComponent<BoxComponent>();
      if(oComp != null)
      {
        oComp.SetData(m_fCellSize, m_fTimeToMove, this);
      }

      if(!bFound)
      {
        m_tBoxObjectPool.Add(oBoxObject);
      }
    }

    m_fLevelStartTime = Time.realtimeSinceStartup;
    m_iNumberOfFlagsReached = 0;
  }

  public float GetLevelStartTime()
  {
    return m_fLevelStartTime;
  }

  public void DoMove(TurnData rData)
  {
    rData.iTurnId = m_tTurns.Count;
    //Debug.Log("Saved move (" + m_tTurns.Count.ToString() + "): " + rData.vMoveDirection.ToString() + " Box? " + (rData.rMovedBox == null ? "false" : "true"));
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

    //Debug.Log("Loaded move (" + rData.iTurnId.ToString() + "): " + rData.vMoveDirection.ToString() + " Box? " + (rData.rMovedBox == null ? "false" : "true"));
  }

  public void FlagReached(bool bReached)
  {
    m_iNumberOfFlagsReached += bReached?1:-1;

    Debug.Log("Points: " + m_iNumberOfFlagsReached);

    if (m_oCurrentLevel.tBoxEndPosition.Count == m_iNumberOfFlagsReached)
    {
      bWon = true;
      Debug.Log("Level cleared");

      m_iNumberOfFlagsReached = 0;
      m_iCurrentLevel += 1;

      EventLibrary.CallOnWin();

      if(m_iCurrentLevel >= m_tLevels.Count)
      {
        UnloadLevel();
      }
      else
      {
        LoadLevel(m_tLevels[m_iCurrentLevel]);
      }
    }
  }

  public void EventOnStep()
  {
    m_iNumberOfSteps ++;
    //print(m_iNumberOfSteps);
  }
}
