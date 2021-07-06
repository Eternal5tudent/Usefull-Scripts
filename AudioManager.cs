using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    struct AudioPlayer
    {
        public AudioSource source;
        public GameObject obj;
        public AudioPlayer(AudioSource source, GameObject obj)
        {
            this.source = source;
            this.obj = obj;
        }
    }
    public AudioManager Instance { get; private set; }
    private enum AudioType { Music, SFX, SFXLooped }


    [SerializeField] GameObject sfxPlayer;
    [SerializeField] GameObject sfxLoopedPlayer;
    [SerializeField] GameObject musicPlayer;

    private readonly List<AudioPlayer> musicSources = new List<AudioPlayer>();
    private readonly List<AudioPlayer> sfxSources = new List<AudioPlayer>();
    private readonly List<AudioPlayer> sfxLoopedSources = new List<AudioPlayer>();

    private readonly Dictionary<int, AudioPlayer> playingLoopedSFX = new Dictionary<int, AudioPlayer>();
    List<int> uniqueAudioIdUsed = new List<int>();

    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            //Generate  5 SFX audio sources
            GenerateSfxSource();

            //Generate 5 SFXLooped audio sources
            GenerateSfxLoopedSource();
        }

        //Generate one music audio sources
        GenerateMusicSource();
    }

    #region Generate Sources
    private AudioPlayer GenerateSfxLoopedSource()
    {
        GameObject newSfxLoopedObj = Instantiate(sfxLoopedPlayer, transform);
        newSfxLoopedObj.SetActive(false);
        AudioPlayer sfxLooped_player = new AudioPlayer(newSfxLoopedObj.GetComponent<AudioSource>(), newSfxLoopedObj);
        sfxLoopedSources.Add(sfxLooped_player);
        return sfxLooped_player;
    }

    private AudioPlayer GenerateMusicSource()
    {
        GameObject newMusicObj = Instantiate(musicPlayer, transform);
        newMusicObj.SetActive(false);
        AudioPlayer music_player = new AudioPlayer(newMusicObj.GetComponent<AudioSource>(), newMusicObj);
        musicSources.Add(music_player);
        return music_player;
    }

    private AudioPlayer GenerateSfxSource()
    {
        GameObject newSfxObj = Instantiate(sfxPlayer, transform);
        newSfxObj.SetActive(false);
        AudioPlayer sfx_player = new AudioPlayer(newSfxObj.GetComponent<AudioSource>(), newSfxObj);
        sfxSources.Add(sfx_player);
        return sfx_player;
    }
    #endregion

    private AudioPlayer GetAudioPlayer(AudioType type)
    {
        AudioPlayer CreateAudioPlayer(AudioType type)
        {
            if (type == AudioType.SFX)
            {
                return GenerateSfxSource();
            }
            else if (type == AudioType.Music)
            {
                return GenerateMusicSource();
            }
            else if (type == AudioType.SFXLooped)
            {
                return GenerateSfxLoopedSource();
            }
            throw new System.Exception("Can't create new audio player");
        }
        if (type == AudioType.SFX)
        {
            foreach (AudioPlayer player in sfxSources)
            {
                if (!player.obj.activeInHierarchy)
                {
                    return player;
                }
            }
        }
        else if (type == AudioType.Music)
        {
            foreach (AudioPlayer player in musicSources)
            {
                if (!player.obj.activeInHierarchy)
                {
                    return player;
                }
            }
        }
        else if (type == AudioType.SFXLooped)
        {
            foreach (AudioPlayer player in sfxLoopedSources)
            {
                if (!player.obj.activeInHierarchy)
                {
                    return player;
                }
            }
        }
        return CreateAudioPlayer(type);
    }
    public void PlaySFX(AudioClip clip)
    {
        AudioPlayer player = GetAudioPlayer(AudioType.SFX);
        player.obj.SetActive(true);
        player.source.PlayOneShot(clip);
        StartCoroutine(DisableGameObject(clip.length, player.obj));
    }
    public void PlaySFXLooped(AudioClip clip, int uniqueID)
    {
        AudioPlayer player = GetAudioPlayer(AudioType.SFXLooped);
        player.obj.SetActive(true);
        player.source.clip = clip;
        player.source.Play();
        playingLoopedSFX.Add(uniqueID, player);
    }

    public void StopSFXLooped(int uniqueID)
    {
        bool audioPlayerExists = playingLoopedSFX.TryGetValue(uniqueID, out AudioPlayer player);
        if (audioPlayerExists)
        {
            player.source.Stop();
            player.obj.SetActive(false);
            playingLoopedSFX.Remove(uniqueID);
        }

    }

    public void PlayMusic(AudioClip clip)
    {
        AudioPlayer player = GetAudioPlayer(AudioType.Music);
        player.obj.SetActive(true);
        player.source.PlayOneShot(clip);
        StartCoroutine(DisableGameObject(clip.length, player.obj));
    }

    private IEnumerator DisableGameObject(float timer, GameObject obj)
    {
        yield return new WaitForSeconds(timer);
        obj.SetActive(false);
    }

    public int GetUniqueAudioID()
    {
        for (int i = 0; i < 500; i++)
        {
            if (!uniqueAudioIdUsed.Contains(i))
            {
                return i;
            }
        }
        Debug.LogError("Couldn't find a free unique audio id!");
        return Random.Range(500, 1000);
    }

}
