using System;
using UnityEngine;

public interface IPushable
{
    bool Push(Vector3 _vMoveDirection);

    bool CanBePushed(Vector3 _vMoveDirection);

    bool MoveInDirection(Vector3 _vMoveDirection);  

    bool IsAlreadyMoving();


    void SetSolved(bool _bSolved);

  int GetColor();


  event Action OnPush;
}
