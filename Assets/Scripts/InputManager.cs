using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [Header("Movimentação")]
    public KeyCode moveUp = KeyCode.W;
    public KeyCode moveDown = KeyCode.S;
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode jump = KeyCode.Space;

    [Header("Skills")]
    public List<KeyCode> skillKeys = new List<KeyCode>
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5
    };

    [Header("Janelas e Menu")]
    public KeyCode openInventory = KeyCode.I;
    public KeyCode openStatus = KeyCode.C;
    public KeyCode pause = KeyCode.Pause;
    public KeyCode closeMenu = KeyCode.Escape;

    [Header("Interação")]
    public KeyCode interact = KeyCode.F;

    [Header("Poções")]
    public KeyCode useHealthPotion = KeyCode.Q;
    public KeyCode useManaPotion = KeyCode.E;
    public KeyCode useSpeedPotion = KeyCode.R;

    [Header("Câmera")]
    public KeyCode resetCamera = KeyCode.F12;

    [Header("Targeting")]
    public KeyCode selectTarget = KeyCode.Tab;

    [Header("Combate")]
    public KeyCode toggleAutoAttack = KeyCode.G;

    // Chaves padrão para as skills, usadas para resetar ou como fallback
    static readonly KeyCode[] defaultSkillKeys =
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadKeybinds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveKeybinds()
    {
        PlayerPrefs.SetInt("moveUp", (int)moveUp);
        PlayerPrefs.SetInt("moveDown", (int)moveDown);
        PlayerPrefs.SetInt("moveLeft", (int)moveLeft);
        PlayerPrefs.SetInt("moveRight", (int)moveRight);
        PlayerPrefs.SetInt("jump", (int)jump);

        for (int i = 0; i < skillKeys.Count; i++)
            PlayerPrefs.SetInt($"skillKey_{i}", (int)skillKeys[i]);

        PlayerPrefs.SetInt("openInventory", (int)openInventory);
        PlayerPrefs.SetInt("openStatus", (int)openStatus);
        PlayerPrefs.SetInt("pause", (int)pause);
        PlayerPrefs.SetInt("closeMenu", (int)closeMenu);
        PlayerPrefs.SetInt("interact", (int)interact);
        PlayerPrefs.SetInt("useHealthPotion", (int)useHealthPotion);
        PlayerPrefs.SetInt("useManaPotion", (int)useManaPotion);
        PlayerPrefs.SetInt("useSpeedPotion", (int)useSpeedPotion);
        PlayerPrefs.SetInt("resetCamera", (int)resetCamera);
        PlayerPrefs.SetInt("selectTarget", (int)selectTarget);
        PlayerPrefs.SetInt("toggleAutoAttack", (int)toggleAutoAttack);
        PlayerPrefs.Save();
    }

    public void LoadKeybinds()
    {
        moveUp = (KeyCode)PlayerPrefs.GetInt("moveUp", (int)moveUp);
        moveDown = (KeyCode)PlayerPrefs.GetInt("moveDown", (int)moveDown);
        moveLeft = (KeyCode)PlayerPrefs.GetInt("moveLeft", (int)moveLeft);
        moveRight = (KeyCode)PlayerPrefs.GetInt("moveRight", (int)moveRight);
        jump = (KeyCode)PlayerPrefs.GetInt("jump", (int)jump);

        for (int i = 0; i < skillKeys.Count; i++)
            skillKeys[i] = (KeyCode)PlayerPrefs.GetInt($"skillKey_{i}", (int)skillKeys[i]);

        openInventory = (KeyCode)PlayerPrefs.GetInt("openInventory", (int)openInventory);
        openStatus = (KeyCode)PlayerPrefs.GetInt("openStatus", (int)openStatus);
        pause = (KeyCode)PlayerPrefs.GetInt("pause", (int)pause);
        closeMenu = (KeyCode)PlayerPrefs.GetInt("closeMenu", (int)closeMenu);
        interact = (KeyCode)PlayerPrefs.GetInt("interact", (int)interact);
        useHealthPotion = (KeyCode)PlayerPrefs.GetInt("useHealthPotion", (int)useHealthPotion);
        useManaPotion = (KeyCode)PlayerPrefs.GetInt("useManaPotion", (int)useManaPotion);
        useSpeedPotion = (KeyCode)PlayerPrefs.GetInt("useSpeedPotion", (int)useSpeedPotion);
        resetCamera = (KeyCode)PlayerPrefs.GetInt("resetCamera", (int)resetCamera);
        selectTarget = (KeyCode)PlayerPrefs.GetInt("selectTarget", (int)selectTarget);
        toggleAutoAttack = (KeyCode)PlayerPrefs.GetInt("toggleAutoAttack", (int)toggleAutoAttack);
    }

    public void ResetToDefaults()
    {
        moveUp = KeyCode.W;
        moveDown = KeyCode.S;
        moveLeft = KeyCode.A;
        moveRight = KeyCode.D;
        jump = KeyCode.Space;

        skillKeys = new List<KeyCode>(defaultSkillKeys);

        openInventory = KeyCode.I;
        openStatus = KeyCode.C;
        pause = KeyCode.Pause;
        closeMenu = KeyCode.Escape;
        interact = KeyCode.F;
        useHealthPotion = KeyCode.Q;
        useManaPotion = KeyCode.E;
        useSpeedPotion = KeyCode.R;
        resetCamera = KeyCode.F12;
        selectTarget = KeyCode.Tab;
        toggleAutoAttack = KeyCode.G;
        SaveKeybinds();
    }

    public List<KeybindEntry> GetKeybindEntries()
    {
        var entries = new List<KeybindEntry>();

        // Basic Controls
        entries.Add(new KeybindEntry("Mover para Frente", KeybindCategory.Basic, () => moveUp, k => moveUp = k));
        entries.Add(new KeybindEntry("Mover para Trás", KeybindCategory.Basic, () => moveDown, k => moveDown = k));
        entries.Add(new KeybindEntry("Mover para Esquerda", KeybindCategory.Basic, () => moveLeft, k => moveLeft = k));
        entries.Add(new KeybindEntry("Mover para Direita", KeybindCategory.Basic, () => moveRight, k => moveRight = k));
        entries.Add(new KeybindEntry("Pular", KeybindCategory.Basic, () => jump, k => jump = k));
        entries.Add(new KeybindEntry("Interagir", KeybindCategory.Basic, () => interact, k => interact = k));

        // Advanced Controls
        entries.Add(new KeybindEntry("Poção de Vida", KeybindCategory.Advanced, () => useHealthPotion, k => useHealthPotion = k));
        entries.Add(new KeybindEntry("Poção de Mana", KeybindCategory.Advanced, () => useManaPotion, k => useManaPotion = k));
        entries.Add(new KeybindEntry("Poção de Velocidade", KeybindCategory.Advanced, () => useSpeedPotion, k => useSpeedPotion = k));
        entries.Add(new KeybindEntry("Resetar Câmera", KeybindCategory.Advanced, () => resetCamera, k => resetCamera = k));
        entries.Add(new KeybindEntry("Selecionar Alvo", KeybindCategory.Advanced, () => selectTarget, k => selectTarget = k));
        entries.Add(new KeybindEntry("Ataque Automático", KeybindCategory.Advanced, () => toggleAutoAttack, k => toggleAutoAttack = k));

        // Skill Shortcuts
        for (int i = 0; i < skillKeys.Count; i++)
        {
            int index = i; // captura local, senão todo lambda aponta pro último i do loop
            entries.Add(new KeybindEntry($"Skill {index + 1}", KeybindCategory.Skills,
                () => skillKeys[index],
                k => skillKeys[index] = k));
        }

        // Interface Shortcuts
        entries.Add(new KeybindEntry("Abrir Inventário", KeybindCategory.Interface, () => openInventory, k => openInventory = k));
        entries.Add(new KeybindEntry("Abrir Status", KeybindCategory.Interface, () => openStatus, k => openStatus = k));
        entries.Add(new KeybindEntry("Pausar", KeybindCategory.Interface, () => pause, k => pause = k));
        entries.Add(new KeybindEntry("Fechar Menu", KeybindCategory.Interface, () => closeMenu, k => closeMenu = k));

        return entries;
    }
}