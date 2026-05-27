using UnityEngine;

public class LevelManager : MonoBehaviour
{
  // Start is called once before the first execution of Update after the MonoBehaviour is created

  public static LevelManager instance { get; private set; }

  private void Awake()
  {
    if (instance != null && instance != this)
    {
      Destroy(gameObject);
      return;
    }
    instance = this;

    DontDestroyOnLoad(gameObject);
  }
}