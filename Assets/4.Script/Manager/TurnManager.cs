using UnityEngine;

public class TurnManager : BehaviourSingleton<TurnManager>
{
    protected override bool IsDontdestroy() => false;

    public uint currentTurn = 0;
    public uint previousTurn = 0;

    void Update()
    {
        if (currentTurn == previousTurn) return;

        NotifyEnemies();
        previousTurn = currentTurn;
    }

   private void NotifyEnemies()
{
    var enemies = FindObjectsOfType<EnemyControl>(true);

    foreach (var enemy in enemies)
    {
        enemy.OnTurnChanged(currentTurn);
    }

}
}
