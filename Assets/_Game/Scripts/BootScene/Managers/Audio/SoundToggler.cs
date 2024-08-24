using UnityEngine;

public class SoundToggler : MonoBehaviour
{
    [SerializeField] private GameObject _soundOn;
    [SerializeField] private GameObject _soundOff;

    private void Awake()
    {
        _soundOn.SetActive(!AudioManager.Instance.Muted);
        _soundOff.SetActive(AudioManager.Instance.Muted);
    }

    public void ToggleSound()
    {
        AudioManager.Instance.SetVolume(_soundOn.activeInHierarchy);
        _soundOff.SetActive(_soundOn.activeInHierarchy);
        _soundOn.SetActive(!_soundOn.activeInHierarchy);
    }
}
