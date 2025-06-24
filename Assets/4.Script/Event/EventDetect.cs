using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInspector;
[CreateAssetMenu(menuName = "GameEvent/EventDetect")]
public class EventDetect : GameEvent<EventDetect>
{
    public override EventDetect Item => this;
     [Title("Detectable Radius")]
    public float DetectRadius;
    [ReadOnly]public Node occuredNode;

}
