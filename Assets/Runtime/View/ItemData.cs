using System;
using UnityEngine;

[Serializable]
public class ItemData
{
    public Guid Id;
    public string Name;
    public Sprite Icon;
    public Vector2 Position;

    public ItemData(Guid id, string name, Sprite icon, Vector2 position)
    {
        Id = id;
        Name = name;
        Icon = icon;
        Position = position;
    }
}
