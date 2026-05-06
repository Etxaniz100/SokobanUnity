using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public Vector2 vPlayerStartPoint;
    public List<Vector2> tBoxStartPosition;
    public List<Vector2> tBoxEndPosition;
    public List<Vector2> tWallPosition;

    public LevelData()
    {
      tBoxStartPosition = new List<Vector2>();
      tBoxEndPosition = new List<Vector2>();
      tWallPosition = new List<Vector2>();
    }
}
