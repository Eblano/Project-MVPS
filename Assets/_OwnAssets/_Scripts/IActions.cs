using System.Collections.Generic;

public interface IActions
{
    List<string> GetActions();

    void SetAction(string action);
}