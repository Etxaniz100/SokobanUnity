using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public Vector2 vPlayerStartPoint;
    public List<Vector2> tBoxStartPosition;
    public List<int> tBoxColor;
    public List<Vector2> tBoxEndPosition;
    public List<int> tGoalColor;
    public List<Vector2> tWallPosition;

    public LevelData()
    {
      tBoxStartPosition = new List<Vector2>();
      tBoxColor = new List<int>();
      tGoalColor = new List<int>();
      tBoxEndPosition = new List<Vector2>();
      tWallPosition = new List<Vector2>();
    }
}
