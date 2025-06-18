using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : BehaviourSingleton<GameManager>
{
    protected override bool IsDontdestroy() => true;

    public float startDelay = 1f;
}
