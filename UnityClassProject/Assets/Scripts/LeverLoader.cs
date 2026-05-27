using System.Collections.Generic;
using UnityEngine;

public class LeverLoader : MonoBehaviour
{
  // Level loading
  public string m_sFilesPath = "LevelFiles/";
  [SerializeField]
  public List<string> m_tLevelsFilenameList;
  
  // Loaded levels
  private List<LevelData> m_tLevels;
  private LevelData m_oCurrentLevel;
  private bool m_bCurrentLevelInstanciated;

  // Prefabs
  [Header("Prefabs")]
  [SerializeField] GameObject m_oBoxPrefab;
  [SerializeField] GameObject m_oWallPrefab;
  [SerializeField] GameObject m_oPlayerPrefab;
  [SerializeField] GameObject m_oEndFlagPrefab;


  [Header("General Settings")]
  [SerializeField] float m_fCellSize = 1;
  [SerializeField] float m_fTimeToMove = 1;
  [SerializeField] List<Color> m_tBoxColors;

  // Object pools
  List<GameObject> m_tWallObjectPool;
  GameObject m_rPlayer;
  List<GameObject> m_tBoxObjectPool;
  List<GameObject> m_tFlagObjectPool;

  // Camera
  public Transform m_rCameraTransform;

  private void Awake()
  {
    m_tLevels = new List<LevelData>();
    m_tWallObjectPool = new List<GameObject>();
    m_tBoxObjectPool = new List<GameObject>();
    m_tFlagObjectPool = new List<GameObject>();

    LoadLevels();
  }

  private void LoadLevels()
  {
    foreach (string sFile in m_tLevelsFilenameList)
    {
      LevelData oLoadedLevel = ReadLevelFromFile(sFile);
      if(oLoadedLevel != null)
      {
        m_tLevels.Add(oLoadedLevel);
      }
    }
  }

  private LevelData ReadLevelFromFile(string _sFile)
  {
    TextAsset oLoadedFile = Resources.Load<TextAsset>(m_sFilesPath + _sFile);
    if (oLoadedFile == null)
    {
      Debug.Log("File not found : " + m_sFilesPath + _sFile);
      return null;
    }

    LevelData oCurrentData = new LevelData();

    int iCurrentLayer = 0;
    Vector2 vCurrentPos = new Vector2(0, 0);
    foreach (char c in oLoadedFile.text)
    {
      switch (c)
      {
        case '#': // Wall
          if (iCurrentLayer == 0)
          {
            oCurrentData.tWallPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
          }
          vCurrentPos.x++;
          break;

        case 'P': // Player
          oCurrentData.vPlayerStartPoint = new Vector2(vCurrentPos.x, vCurrentPos.y);
          vCurrentPos.x++;
          break;

        case 'S': // Box
          oCurrentData.tBoxStartPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
          oCurrentData.tBoxColor.Add(0);
          vCurrentPos.x++;
          break;

        case '$': // Box in goal
          oCurrentData.tBoxStartPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
          oCurrentData.tBoxEndPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
          oCurrentData.tBoxColor.Add(0);
          oCurrentData.tGoalColor.Add(0);

          vCurrentPos.x++;
          break;

        case '!': // Goal
          oCurrentData.tBoxEndPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
          oCurrentData.tGoalColor.Add(0);
          vCurrentPos.x++;
          break;

        case '\n': // Line jump
          vCurrentPos.y--;
          vCurrentPos.x = 0;
          break;

        case '-': // New layer
          vCurrentPos.x = 0;
          vCurrentPos.y = 1;
          iCurrentLayer += 1;
          break;

        case '0':
          if (iCurrentLayer == 0)
          {
            oCurrentData.tBoxStartPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
            oCurrentData.tBoxColor.Add(0);
          }
          else
          {
            oCurrentData.tBoxEndPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
            oCurrentData.tGoalColor.Add(0);
          }
          vCurrentPos.x++;
          break;

        case '1':
          if (iCurrentLayer == 0)
          {
            oCurrentData.tBoxStartPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
            oCurrentData.tBoxColor.Add(1);
          }
          else
          {
            oCurrentData.tBoxEndPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
            oCurrentData.tGoalColor.Add(1);
          }
          vCurrentPos.x++;
          break;

        case '2':
          if (iCurrentLayer == 0)
          {
            oCurrentData.tBoxStartPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
            oCurrentData.tBoxColor.Add(2);
          }
          else
          {
            oCurrentData.tBoxEndPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
            oCurrentData.tGoalColor.Add(2);
          }
          vCurrentPos.x++;
          break;

        case '3':
          if (iCurrentLayer == 0)
          {
            oCurrentData.tBoxStartPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
            oCurrentData.tBoxColor.Add(3);
          }
          else
          {
            oCurrentData.tBoxEndPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
            oCurrentData.tGoalColor.Add(3);
          }
          vCurrentPos.x++;
          break;

        case '4':
          if (iCurrentLayer == 0)
          {
            oCurrentData.tBoxStartPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
            oCurrentData.tBoxColor.Add(4);
          }
          else
          {
            oCurrentData.tBoxEndPosition.Add(new Vector2(vCurrentPos.x, vCurrentPos.y));
            oCurrentData.tGoalColor.Add(4);
          }
          vCurrentPos.x++;
          break;

        case ' ':
        default:
          vCurrentPos.x++;
          break;
      }

      oCurrentData.vLevelCenter.x = oCurrentData.vLevelCenter.x < vCurrentPos.x ? vCurrentPos.x : oCurrentData.vLevelCenter.x;
      oCurrentData.vLevelCenter.y = oCurrentData.vLevelCenter.y > vCurrentPos.y ? vCurrentPos.y : oCurrentData.vLevelCenter.y;
    }

    return oCurrentData;
  }

  private void DeinstanciateLevel()
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

    if (m_rPlayer)
    {
      m_rPlayer.SetActive(false);
    }

    m_bCurrentLevelInstanciated = false;
  }
  
  public LevelData InstanciateLevel(int iLevelIndex)
  {
    if(m_tLevels.Count <= iLevelIndex)
    {
      return null;
    }

    InstanciateLevel(m_tLevels[iLevelIndex]);

    return m_tLevels[iLevelIndex];
  }

  private void InstanciateLevel(LevelData rData)
  {
    if(m_bCurrentLevelInstanciated)
    {
      DeinstanciateLevel();
    }

    if (m_rCameraTransform)
    {
      m_rCameraTransform.position = new Vector3(rData.vLevelCenter.x / 2f, m_rCameraTransform.position.y, -5 + (rData.vLevelCenter.y / 2f));
    }

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
    oPlayerController.SetData(m_fCellSize, m_fTimeToMove);

    m_rPlayer.transform.position = new Vector3(rData.vPlayerStartPoint.x * m_fCellSize, 0.5f, rData.vPlayerStartPoint.y * m_fCellSize);

    // Set walls
    foreach (Vector2 vWallPosition in rData.tWallPosition)
    {

      GameObject oWallObject = null;
      bool bFound = false;

      foreach (GameObject rGameObject in m_tWallObjectPool)
      {
        if (!rGameObject.activeSelf)
        {
          rGameObject.SetActive(true);
          oWallObject = rGameObject;
          bFound = true;
          break;
        }
      }

      if (oWallObject == null)
      {
        oWallObject = Instantiate(m_oWallPrefab);
      }
      oWallObject.transform.position = new Vector3(vWallPosition.x * m_fCellSize, 0.5f, vWallPosition.y * m_fCellSize);

      if (!bFound)
      {
        m_tWallObjectPool.Add(oWallObject);
      }
    }

    int i = 0;
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

      FlagComponent rFlagComponent = oFlagObject.GetComponent<FlagComponent>();

      int iFlagColor = (i < rData.tGoalColor.Count) ? rData.tGoalColor[i] : 0;
      iFlagColor = iFlagColor < m_tBoxColors.Count ? iFlagColor : 0;

      if (rFlagComponent != null)
      {
        rFlagComponent.SetData(m_tBoxColors[iFlagColor], iFlagColor);
      }


      if (!bFound)
      {
        m_tFlagObjectPool.Add(oFlagObject);
      }

      i++;
    }

    i = 0;
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

      if (oBoxObject == null)
      {
        oBoxObject = Instantiate(m_oBoxPrefab);
      }

      oBoxObject.transform.position = new Vector3(vBoxPosition.x * m_fCellSize, 0.5f, vBoxPosition.y * m_fCellSize);
      BoxComponent oComp = oBoxObject.GetComponent<BoxComponent>();

      int iBoxColor = (i < rData.tBoxColor.Count) ? rData.tBoxColor[i] : 0;
      iBoxColor = iBoxColor < m_tBoxColors.Count ? iBoxColor : 0;


      if (oComp != null)
      {
        oComp.SetData(m_fCellSize, m_fTimeToMove, m_tBoxColors[iBoxColor], iBoxColor);
      }

      if (!bFound)
      {
        m_tBoxObjectPool.Add(oBoxObject);
      }

      i++;
    }

    m_bCurrentLevelInstanciated = true;
  }

}
