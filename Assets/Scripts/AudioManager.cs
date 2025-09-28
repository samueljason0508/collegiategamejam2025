using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("—— Audio Sources ——")]
    [SerializeField] private AudioSource musicSource; // for BGM/ambience
    [SerializeField] private AudioSource sfxSource;   // for one-shots

    [Header("—— Audio Clips (assign in Inspector) ——")]
    public AudioClip background;
    public AudioClip death;
    public AudioClip checkpoint;
    public AudioClip wallTouch;
    public AudioClip portalIn;
    public AudioClip portalOut;

    [Header("Optional: Mixer Routing")]
    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxMixer;

    [Header("Keys (optional, for modular lookups)")]
    [SerializeField] private List<NamedClip> extraClips = new(); // add “key + clip” rows

    private Dictionary<string, AudioClip> _clipMap;

    [System.Serializable]
    public struct NamedClip
    {
        public string key;
        public AudioClip clip;
    }

    private void Awake()
    {
        // simple singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // setup sources if missing
        if (!musicSource) musicSource = gameObject.AddComponent<AudioSource>();
        if (!sfxSource)   sfxSource   = gameObject.AddComponent<AudioSource>();

        musicSource.playOnAwake = false;
        sfxSource.playOnAwake   = false;

        musicSource.loop = true;
        sfxSource.loop   = false;

        musicSource.spatialBlend = 0f; // 2D
        sfxSource.spatialBlend   = 0f; // 2D

        if (musicMixer) musicSource.outputAudioMixerGroup = musicMixer;
        if (sfxMixer)   sfxSource.outputAudioMixerGroup   = sfxMixer;

        // build key lookup
        _clipMap = new Dictionary<string, AudioClip>();
        AddIfValid("background", background);
        AddIfValid("death", death);
        AddIfValid("checkpoint", checkpoint);
        AddIfValid("wallTouch", wallTouch);
        AddIfValid("portalIn", portalIn);
        AddIfValid("portalOut", portalOut);
        foreach (var nc in extraClips)
            AddIfValid(nc.key, nc.clip);
    }

    private void Start()
    {
        // auto-start background if assigned
        if (background)
        {
            musicSource.clip = background;
            musicSource.Play();
        }
    }

    void AddIfValid(string key, AudioClip clip)
    {
        if (!string.IsNullOrWhiteSpace(key) && clip && !_clipMap.ContainsKey(key))
            _clipMap.Add(key, clip);
    }

    // --------- Public API (simple) ---------

    // like the tutorial: call from other scripts or UI
    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (!clip) return;
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, volume);
    }

    // Play by key (so you can add more later without changing code)
    public void PlaySFX(string key, float volume = 1f, float pitch = 1f)
    {
        if (key != null && _clipMap.TryGetValue(key, out var clip))
            PlaySFX(clip, volume, pitch);
        else
            Debug.LogWarning($"AudioManager: key '{key}' not found.");
    }

    // Swap music immediately
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (!clip) return;
        musicSource.loop = loop;
        musicSource.clip = clip;
        musicSource.Play();
    }

    // Optional: fade music
    public void FadeToMusic(AudioClip clip, float fadeSeconds = 0.5f, bool loop = true)
    {
        if (!clip) return;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(clip, fadeSeconds, loop));
    }

    IEnumerator FadeRoutine(AudioClip next, float t, bool loop)
    {
        float startVol = musicSource.volume;
        float half = Mathf.Max(0.01f, t * 0.5f);

        // fade out
        for (float x = 0; x < half; x += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVol, 0f, x / half);
            yield return null;
        }
        musicSource.volume = 0f;

        // switch & fade in
        musicSource.clip = next;
        musicSource.loop = loop;
        musicSource.Play();

        for (float x = 0; x < half; x += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, startVol, x / half);
            yield return null;
        }
        musicSource.volume = startVol;
    }
}
