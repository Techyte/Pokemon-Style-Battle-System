using System;
using UnityEngine;

[CreateAssetMenu]
public class Move : ScriptableObject
{
    public new string name;
    public Type type;
    public int damage;
    public MoveCategory category;

    public delegate void MoveMethod(Battler target);
    public MoveMethod moveMethod;
}

public enum MoveCategory
{
    Physical,
    Special,
    Status
}
