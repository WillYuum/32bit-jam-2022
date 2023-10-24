using System.Collections.Generic;
using UnityEngine;
using Utils.GenericSingletons;
using AudioClasses;

public class AudioManager : MonoBehaviourSingleton<AudioManager>
{
    [SerializeField] private List<AudioConfig> _sfxConfigs;
    [SerializeField] private List<AudioConfig> _bgmConfigs;
    private List<Audio> _sfx;
    private List<Audio> _bgm;


    public void Load()
    {
        Debug.Log("Loading AudioManager");

        _sfx = new List<Audio>();
        _bgm = new List<Audio>();


        void LoadAudioConfigs(List<AudioConfig> configs, List<Audio> audios)
        {
            configs.ForEach(audioConfig =>
           {
               GameObject spawnedAudioObject = new GameObject
               {
                   name = audioConfig.Name,
                   transform = { parent = transform }
               };
               AudioSource audioSource = spawnedAudioObject.AddComponent<AudioSource>();

#if UNITY_EDITOR
               if (audioSource == null) Debug.LogError("audioSource is null");
               if (audioConfig == null) Debug.LogError("audioConfig is null");
#endif

               audios.Add(new Audio(audioSource, audioConfig));
           });
        }

        LoadAudioConfigs(_sfxConfigs, _sfx);
        LoadAudioConfigs(_bgmConfigs, _bgm);

    }

    public void PlaySFX(string name) => Play(name, _sfx);

    public void PlayBGM(string name)
    {
        _bgm.ForEach(audio => audio.Stop());
        Play(name, _bgm);
    }

    public void StopAllBGM()
    {
        _bgm.ForEach(audio => audio.Stop());
    }





    private void Play(string audioName, List<Audio> audios)
    {
        Audio audio = audios.Find(x => x.Name == audioName);

#if UNITY_EDITOR
        if (audio == null)
        {
            Debug.LogError("Aduio not found: " + audioName);
            return;
        }
#endif

        audio.Play();
    }




}




namespace AudioClasses
{

    [System.Serializable]
    public class AudioConfig
    {
        [SerializeField] public string Name = "";
        [SerializeField] public AudioClip Clip;
        [SerializeField][Range(0f, 1.0f)] public float Volume = 1.0f;
        [SerializeField] public bool Loop = false;
        [SerializeField] public bool RandPitch = false;
    }



    public class Audio
    {
        private string _name;
        private AudioSource _source;


        public string Name { get => _name; }
        public bool isPlaying { get => _source.isPlaying; }

        private bool _randPitch = false;

        public Audio(AudioSource source, AudioConfig audioConfig)
        {
            _source = source;

            _name = audioConfig.Name;
            _source.volume = audioConfig.Volume;
            _source.clip = audioConfig.Clip;
            _source.loop = audioConfig.Loop;
            _randPitch = audioConfig.RandPitch;
        }

        public void SetVolume(float volume)
        {
            _source.volume = volume;
        }

        public void Play()
        {
            _source.Play();
            if (_randPitch)
            {
                _source.pitch = Random.Range(0.8f, 1.0f);
            }
        }

        public void Stop()
        {
            _source.Stop();
        }
    }
}