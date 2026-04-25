using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Game Jam/Character Stats")]
public class CharacterStats : ScriptableObject
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Fizik Ayarları")]
    public float mass = 1f;
    public float gravityScale = 1f;
}