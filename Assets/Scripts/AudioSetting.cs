using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer; // gán AudioMixer ở Inspector
    [SerializeField] private Slider volumeSlider;   // gán Slider ở Inspector

    void Start()
    {
        // Load âm lượng lưu trước đó nếu có
        if (PlayerPrefs.HasKey("Volume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("Volume");
            volumeSlider.value = savedVolume;
            SetVolume(savedVolume);
        }
    }



    public void OnVolumeChanged(float sliderValue)
    {
        sliderValue = volumeSlider.value;

        SetVolume(sliderValue);
        PlayerPrefs.SetFloat("Volume", sliderValue);
    }

    private void SetVolume(float sliderValue)
    {
        float volumeDb = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 40f;
        audioMixer.SetFloat("MasterVolume", volumeDb);
    }

}
