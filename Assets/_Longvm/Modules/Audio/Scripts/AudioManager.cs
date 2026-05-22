using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    [Serializable]
    public class MusicDict : SerializedDictionary<MusicType, AudioClipConfig> { }
    [Serializable]
    public class SoundDict : SerializedDictionary<SoundType, AudioClipConfig> { }

    [SerializeField] float crossfadeDuration = 0.5f;
    [SerializeField] Transform _transform;
    public new Transform transform => _transform;
    [SerializeField] AudioSource musicSource;
    [SerializeField] SoundSource soundSourcePrefab;
    [SerializeField] MusicDict musicDict;
    [SerializeField] SoundDict soundDict;

    IEnumerator crossfadeCoroutine;

    public void Init()
    {
        musicSource.enabled = AudioPref.Ins.IsMusicOn;
    }

    public void ToggleMusic()
    {
        AudioPref.Ins.IsMusicOn = !AudioPref.Ins.IsMusicOn;
        musicSource.enabled = AudioPref.Ins.IsMusicOn;
    }

    public void ToggleSound()
    {
        AudioPref.Ins.IsSoundOn = !AudioPref.Ins.IsSoundOn;
    }

    public void PlayMusic(MusicType type)
    {
        if (!AudioPref.Ins.IsMusicOn || !musicDict.ContainsKey(type)) return;
        Crossfade(musicSource, musicDict[type]);
    }

    void Crossfade(AudioSource source, AudioClipConfig clipConfig)
    {
        if (crossfadeCoroutine != null)
        {
            StopCoroutine(crossfadeCoroutine);
        }
        crossfadeCoroutine = CrossfadeCoroutine(source, clipConfig);
        StartCoroutine(crossfadeCoroutine);
    }

    IEnumerator CrossfadeCoroutine(AudioSource source, AudioClipConfig clipConfig)
    {
        float volume = musicSource.volume;
        float timeElapsed = 0;
        while (timeElapsed < crossfadeDuration)
        {
            source.volume = Mathf.Lerp(volume, 0, timeElapsed / crossfadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        source.volume = 0;
        source.Stop();
        source.clip = clipConfig.Clip;
        source.Play();

        volume = clipConfig.Volume;
        timeElapsed = 0;
        while (timeElapsed < crossfadeDuration)
        {
            source.volume = Mathf.Lerp(0, volume, timeElapsed / crossfadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        source.volume = 1;
    }

    public void StopMusic()
    {
        if (!AudioPref.Ins.IsMusicOn) return;
        musicSource.Stop();
        if (crossfadeCoroutine != null)
        {
            StopCoroutine(crossfadeCoroutine);
        }
        musicSource.volume = 1;
    }

    public SoundSource PlaySound(SoundType type)
    {
        if (!AudioPref.Ins.IsSoundOn || !soundDict.ContainsKey(type)) return null;
        SoundSource soundSource = soundSourcePrefab.GetInstance(transform);
        soundSource.PlayOneShot(soundDict[type]);
        return soundSource;
    }

#if UNITY_EDITOR
    [Header("Editor")]
    [SerializeField, FolderPath] string folderPath;
    [SerializeField] AudioClip[] audioClips;

    [Button]
    public void GenConfigs()
    {
        foreach (AudioClip clip in audioClips)
        {
            AudioClipConfig config = ScriptableObject.CreateInstance<AudioClipConfig>();
            config.Clip = clip;
            string path = Path.Combine(folderPath, $"{clip.name}.asset");
            UnityEditor.AssetDatabase.CreateAsset(config, path);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    } 
#endif
}
