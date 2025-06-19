using UnityEngine;



[CreateAssetMenu(menuName = "GameEvent/EventCursorHover")]
public class EventCursorHover : GameEvent<EventCursorHover>
{

  public override EventCursorHover Item => this;

   public PlayerControl player;
 

}

