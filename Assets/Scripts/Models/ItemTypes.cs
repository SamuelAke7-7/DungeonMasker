using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public enum ItemType
{
    Permanent,//Mostly the masks
    Consumable,//It helps the player to make the players gameplay easier at the cost of loosing it
    Key//It's needed to advance
}

[Serializable]
public class ItemData
{
    public string id;
    public string displayName;
    public ItemType type;
    public Sprite iconId;
    public Sprite icon;

    public override bool Equals(object obj)
    {
        return obj is ItemData other && !string.IsNullOrEmpty(id) && id == other.id;
    }

    public override int GetHashCode()
    {
        return (id != null) ? id.GetHashCode() : base.GetHashCode();
    }
}