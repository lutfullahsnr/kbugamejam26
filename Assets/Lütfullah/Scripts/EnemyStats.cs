using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Game Jam/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Damage Ayarları")]
    public int damage = 50;
}