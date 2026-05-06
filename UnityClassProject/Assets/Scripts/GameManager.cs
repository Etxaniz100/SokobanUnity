using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.IO;

public class GameManager : MonoBehaviour
{

  private List<TurnData> m_tTurns;

  public GameObject m_oBoxPrefab;
  public GameObject m_oWallPrefab;
  public GameObject m_oPlayerPrefab;

  private bool bWon = false;

  private List<LevelData> m_tLevels;
  private LevelData m_oCurrentLevel;
  private int m_iCurrentLevel = -1;

  public string m_sFilesPath = "LevelFiles/";
  [SerializeField]
  public List<string> m_tFurnitureFilenameList;


  // Data structures
  public struct TurnData
  {
    public Vector3 vMoveDirection;
    public PlayerController rPlayerController;
    public BoxComponent rMovedBox;
    public int iTurnId;
  }



  private void Start()
  {
    m_tTurns = new List<TurnData>();
    m_tLevels = new List<LevelData>();

    foreach(string file in m_tFurnitureFilenameList)
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

    if(m_tLevels.Count > 0)
    {
      m_iCurrentLevel = 0;
      m_oCurrentLevel = m_tLevels[m_iCurrentLevel];
      LoadLevel(m_oCurrentLevel);
    }
  }

  void LoadLevel(LevelData rData)
  {
    // Set player
    GameObject oPlayerObject = Instantiate(m_oPlayerPrefab);
    PlayerController oPlayerController = oPlayerObject.GetComponent<PlayerController>();
    oPlayerController.m_rManagerReference = this;
    oPlayerObject.transform.position = new Vector3(rData.vPlayerStartPoint.x, 0.5f, rData.vPlayerStartPoint.y);

    // Set walls
    foreach (Vector2 vWallPosition in rData.tWallPosition)
    {
      GameObject oWallObject = Instantiate(m_oWallPrefab);
      oWallObject.transform.position = new Vector3(vWallPosition.x, 0.5f, vWallPosition.y);
    }

    // Set boxes
    foreach (Vector2 vBoxPosition in rData.tBoxStartPosition)
    {
      GameObject oWallObject = Instantiate(m_oBoxPrefab);
      oWallObject.transform.position = new Vector3(vBoxPosition.x, 0.5f, vBoxPosition.y);
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (bWon || m_iCurrentLevel < 0)
    {
      return;
    }
    int iBoxInPosition = 0;

    foreach(Vector2 vEndPosition in m_oCurrentLevel.tBoxEndPosition)
    {
      RaycastHit oHit;

      bool bHit = false;
      if (Physics.Raycast(new Vector3(vEndPosition.x, 10, vEndPosition.y), Vector3.down * 10, out oHit))
      {
        if (oHit.collider.tag == "Wall")
        {
          Debug.Log("How??");
        }
        else if (oHit.collider.tag == "Box")
        {
          bHit = true;
          iBoxInPosition += 1;
        }
      }

      Debug.DrawLine(new Vector3(vEndPosition.x, 10, vEndPosition.y), new Vector3(vEndPosition.x, 10, vEndPosition.y) + Vector3.down * 10, bHit?new UnityEngine.Color(0, 1, 0): new UnityEngine.Color(1, 0, 0));
    }

    if (iBoxInPosition == m_oCurrentLevel.tBoxEndPosition.Count)
    {
      bWon = true;
    }
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
    if(rData.rMovedBox != null && !rData.rMovedBox.CanMove())
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

    if(rData.rMovedBox != null)
    {
      rData.rMovedBox.MoveInDirection(-rData.vMoveDirection);
    }

    //Debug.Log("Loaded move (" + rData.iTurnId.ToString() + "): " + rData.vMoveDirection.ToString() + " Box? " + (rData.rMovedBox == null ? "false" : "true"));
  }
}
