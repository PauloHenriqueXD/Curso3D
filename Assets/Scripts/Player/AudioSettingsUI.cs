using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Referências")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private void OnEnable()
    {
        // Garante que o slider mostra o valor salvo sempre que o painel é aberto
        musicSlider.SetValueWithoutNotify(AudioManager.Instance.musicVolume);
        sfxSlider.SetValueWithoutNotify(AudioManager.Instance.sfxVolume);
    }

    private void Start()
    {
        musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
    }
}