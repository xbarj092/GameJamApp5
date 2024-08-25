using System;

public static class GameEvents
{
    public static event Action<BossEnemy> OnBossEnemySpawned;
    public static void OnBossEnemySpawnedInvoke(BossEnemy enemy)
    {
        OnBossEnemySpawned?.Invoke(enemy);
    }

    public static event Action<BossEnemy> OnBossEnemyKilled;
    public static void OnBossEnemyKilledInvoke(BossEnemy enemy)
    {
        OnBossEnemyKilled?.Invoke(enemy);
    }

    public static event Action OnScoreChanged;
    public static void OnScoreChangedInvoke()
    {
        OnScoreChanged?.Invoke();
    }
}
