using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    public static PlayerSoundManager Instance;
    public AudioClip BasicAttackSfx;
    public AudioClip Skill1Sfx;
    public AudioClip Skill2Sfx;
    public AudioClip Skill3Sfx;
    public AudioClip Skill4Sfx;
    public AudioClip GetHitSfx;
    public AudioClip UseBottlesSfx;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySkillSfx(int skillNumber)
    {
        switch (skillNumber)
        {
            case 0:
                AudioManager.Instance.PlaySFX(BasicAttackSfx);
                break;
            case 1:
                AudioManager.Instance.PlaySFX(Skill1Sfx);
                break;
            case 2:
                AudioManager.Instance.PlaySFX(Skill2Sfx);
                break;
            case 3:
                AudioManager.Instance.PlaySFX(Skill3Sfx);
                break;
            case 4:
                AudioManager.Instance.PlaySFX(Skill4Sfx);
                break;
            default:
                Debug.LogWarning("Invalid skill number: " + skillNumber);
                break;
        }
    }

    public void PlayGetHitSfx()
    {
        AudioManager.Instance.PlaySFX(GetHitSfx);
    }

    public void PlayUseBottleSfx()
    {
        AudioManager.Instance.PlaySFX(UseBottlesSfx);
    }
}
