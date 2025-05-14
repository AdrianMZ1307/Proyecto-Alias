using UnityEngine;

[CreateAssetMenu(menuName = "Character/Stats")]
public class CharacterStats : ScriptableObject
{
    public string characterName = "Alias";
    public float moveSpeed = 5f;
    public float runSpeed = 1f;
    public int extraJumps = 1;
}
