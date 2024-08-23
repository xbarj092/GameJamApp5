using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private SoundType _soundType;

    public void PlaySound()
    {
        AudioManager.Instance.Play(_soundType);
    }
}
