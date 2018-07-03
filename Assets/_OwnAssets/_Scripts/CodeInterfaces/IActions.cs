using System.Collections.Generic;
using UnityEngine;

public interface IActions
{
    List<string> GetActions();

    void SetAction(string action);

    string GetName();

    Vector3 GetHighestPoint();

    Collider GetCollider();
}