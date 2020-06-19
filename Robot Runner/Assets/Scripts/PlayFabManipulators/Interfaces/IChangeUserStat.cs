using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIncreaseUserStat
{
    IIncreaseUserStat IncreaseStat(string username, string fieldStatName,string value);
}
