using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

public class LevelManager : MonoBehaviour
{
  // Start is called once before the first execution of Update after the MonoBehaviour is created

  public static LevelManager instance { get; private set; }
  private int iLoadedLevel = 0;
  private int iLastUnlockedLevel = 5;

  [SerializeField] bool bDeleteSavedData = false;

  private LeverLoader m_rLevelLoader;
  private LevelData m_rCurrentLevel;

  private void Awake()
  {
    if (instance != null && instance != this)
    {
      Destroy(gameObject);
      return;
    }
    instance = this;

    DontDestroyOnLoad(gameObject);

    m_rLevelLoader = GetComponent<LeverLoader>();

    if (bDeleteSavedData)
    {
      PlayerPrefs.DeleteAll();
    }

    iLastUnlockedLevel = PlayerPrefs.GetInt("iLastUnlockedLevel", 0);
  }

  void OnEnable()
  {
    SceneManager.sceneLoaded += OnSceneLoaded;
  }

  void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    m_rLevelLoader = GetComponent<LeverLoader>();
  }

  private void Start()
  {
    EventLibrary.OnLoadLevelByIndex += StartGameFromLevel;
  }

  void StartGameFromLevel(int iIndex)
  {
    iLoadedLevel = iIndex;
    SceneManager.LoadScene("Level1");
  }

  public bool IsLevelUnlocked(int iLevel)
  {
    return iLevel <= iLastUnlockedLevel;
  }  

  public bool LoadLevel(int iLevel)
  {
    if(iLevel < 0)
    {
      return false;
    }

    if(iLevel > iLastUnlockedLevel)
    {
      UnblockLevel(iLevel);
    }

    iLoadedLevel = iLevel;

    EventLibrary.CallOnLevelLoaded();

    m_rCurrentLevel = m_rLevelLoader.InstanciateLevel(iLoadedLevel);

    if (m_rCurrentLevel != null)
    {
      return true;
    }
    else
    {
      return false;
    }
  }

  public int GetNumberOfFlagsNedded()
  {
    return m_rCurrentLevel ? m_rCurrentLevel.tBoxEndPosition.Count : 0;
  }

  public int GetNumberOfLevels()
  {
    return m_rLevelLoader.GetNumberOfLevels();
  }

  public int GetCurrentLevel()
  {
    return iLoadedLevel;
  }

  // TODO: Unblock levels
  public void UnblockLevel(int iLevel)
  {
    iLastUnlockedLevel = iLevel;
    PlayerPrefs.SetInt("iLastUnlockedLevel", iLastUnlockedLevel);
  }
}