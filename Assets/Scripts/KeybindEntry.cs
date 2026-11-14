using System;
using UnityEngine;

public enum KeybindCategory
{
    Basic,
    Advanced,
    Skills,
    Interface,
    Others,
    Sounds
}

public class KeybindEntry
{
    public string DisplayName;
    public KeybindCategory Category;
    public Func<KeyCode> Getter;
    public Action<KeyCode> Setter;

    public KeybindEntry(string displayName, KeybindCategory category, Func<KeyCode> getter, Action<KeyCode> setter)
    {
        DisplayName = displayName;
        Category = category;
        Getter = getter;
        Setter = setter;
    }
}