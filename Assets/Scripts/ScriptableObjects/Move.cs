using UnityEngine;

[CreateAssetMenu]
public class Move : ScriptableObject
{
    public new string name;
    public Type type;
    public int damage;
    public MoveCategory category; 
}

public enum MoveCategory
{
    Physical,
    Special
}
