using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Range(0f, 1f)] [SerializeField] float minThrusterVolume = .5f;
    [Range(0f, 1f)] [SerializeField] float maxThrusterVolume = 1f;
    [SerializeField] float volumeModifier = 5f;

    [Header("Audio Sources")]
    [SerializeField] AudioSource mainThruster;
    [SerializeField] AudioSource sideThruster;
    [SerializeField] AudioSource clips;

    int _sideThrusterVolumeDirection;

    bool _canChangeSideThrusterVolume;
    bool _canStopMainThruster;

    void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            mainThruster.Play();
            mainThruster.volume = minThrusterVolume;
        }
    }

    void Update()
    {
        if (_canStopMainThruster)
        {
            ChangeVolume(mainThruster, -1);
            _canStopMainThruster = ProcessVolumeValue(mainThruster);
        }

        if (_canChangeSideThrusterVolume)
        {
            ChangeVolume(sideThruster, _sideThrusterVolumeDirection);
            _canChangeSideThrusterVolume = ProcessVolumeValue(sideThruster);
        }
    }

    void ChangeVolume(AudioSource audio, int direction)
    {
        float volumeChangedThisFrame = Time.deltaTime * direction * volumeModifier;
        float newVolumeValue = volumeChangedThisFrame + audio.volume;
        audio.volume = Mathf.Clamp(newVolumeValue, 0f, 1f);
    }

    bool ProcessVolumeValue(AudioSource audio)
    {
        if (audio.volume < Mathf.Epsilon)
        {
            audio.Stop();
        }
        else if (audio.volume >= 1f)
        {
            audio.volume = 1f;
        }
        else
        {
            return true;
        }
        return false;
    }

    public void ChangeMainThrusterVolume(int direction)
    {
        if (!mainThruster.isPlaying)
        {
            mainThruster.Play();
        }

        float volumeChangedThisFrame = Time.fixedDeltaTime * direction * volumeModifier;
        float newVolumeValue = volumeChangedThisFrame + mainThruster.volume;
        mainThruster.volume = Mathf.Clamp(newVolumeValue, minThrusterVolume, maxThrusterVolume);
    }

    public void StopAllThrusters()
    {
        if (mainThruster.isPlaying)
        {
            mainThruster.Stop();
        }
        if (sideThruster.isPlaying)
        {
            sideThruster.Stop();
        }
    }

    public void StopMainThruster()
    {
        if (!mainThruster.isPlaying) { return; }

        _canStopMainThruster = true;
    }

    public void PlaySideThruster()
    {
        if (!sideThruster.isPlaying)
        {
            sideThruster.Play();
        }
        sideThruster.volume = 1f;
    }

    public void StopSideThruster()
    {
        if (!sideThruster.isPlaying) { return; }

        _canChangeSideThrusterVolume = true;
        _sideThrusterVolumeDirection = -1;
    }

    public void PlayClip(AudioClip clip)
    {
        clips.PlayOneShot(clip);
    }
}
