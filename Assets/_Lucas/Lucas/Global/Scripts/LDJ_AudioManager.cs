using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LDJ_AudioManager : MonoBehaviour
{
    #region Events
    // Event called when a sound is played
    public event Action OnPlaySound = null;
    #endregion

    #region Fields / Properties
    #region Audio Sources
    // Main audio sources
    [Header("Audio Sources :")]

    [SerializeField] private AudioSource cameraAudioSource = null;
    public AudioSource CameraAudioSource { get { return cameraAudioSource; } }

    [SerializeField] private AudioSource playerAudioSource = null;
    public AudioSource PlayerAudioSource { get { return playerAudioSource; } }
    #endregion

    // All the audio clips to use in the game
    [Header("Audio Clips :", order = 0)]

    #region Musics
    // All the musics
    [Header("Musics :", order = 1)]

    [SerializeField] private AudioClip fireCampTheme = null;
    public AudioClip FireCampTheme { get { return fireCampTheme; } }

    [SerializeField] private AudioClip mainTheme = null;
    public AudioClip MainTheme { get { return mainTheme; } }
    #endregion

    #region Player
    // All player's audio clips
    [Header("Player Audios :")]

    [SerializeField] private AudioClip[] playerDeath = new AudioClip[] { };
    public AudioClip[] PlayerDeath { get { return playerDeath; } }

    [SerializeField] private AudioClip playerEat = null;
    public AudioClip PlayerEat { get { return playerEat; } }

    [SerializeField] private AudioClip[] playerHit = new AudioClip[] { };
    public AudioClip[] PlayerHit { get { return playerHit; } }

    [SerializeField] private AudioClip playerInventory = null;
    public AudioClip PlayerInventory { get { return playerInventory; } }

    [SerializeField] private AudioClip playerSellInventory = null;
    public AudioClip PlayerSellInventory { get { return playerSellInventory; } }

    [SerializeField] private AudioClip playerUpdate = null;
    public AudioClip PlayerUpdate { get { return playerUpdate; } }
    #endregion

    #region Horde
    // All the horde audio clips
    [Header("Horde Audios :")]

    [SerializeField] private AudioClip goblinNoise = null;
    public AudioClip GoblinNoise { get { return goblinNoise; } }
    #endregion

    #region Interactable Elements
    // All the Iteractable elements audios
    [Header("Interactable Elements Audios :")]

    [SerializeField] private AudioClip chestOpen = null;
    public AudioClip ChestOpen { get { return chestOpen; } }

    [SerializeField] private AudioClip grabObject = null;
    public AudioClip GrabObject { get { return grabObject; } }

    [SerializeField] private AudioClip meatTreeCollect = null;
    public AudioClip MeatTreeCollect { get { return meatTreeCollect; } }
    #endregion

    #region Takable Objects
    // All the takable objects audios
    [Header("Takable Objects Audios :")]

    [SerializeField] private AudioClip axeHit = null;
    public AudioClip AxeHit { get { return axeHit; } }

    [SerializeField] private AudioClip axeThrow = null;
    public AudioClip AxeThrow { get { return axeThrow; } }

    [SerializeField] private AudioClip chairHit = null;
    public AudioClip ChairHit { get { return chairHit; } }

    [SerializeField] private AudioClip chairThrow = null;
    public AudioClip ChairThrow { get { return chairThrow; } }

    [SerializeField] private AudioClip coinHit = null;
    public AudioClip CoinHit { get { return coinHit; } }

    [SerializeField] private AudioClip coinThrow = null;
    public AudioClip CoinThrow { get { return coinThrow; } }

    [SerializeField] private AudioClip holyHit = null;
    public AudioClip HolyHit { get { return holyHit; } }

    [SerializeField] private AudioClip holyThrow = null;
    public AudioClip HolyThrow { get { return holyThrow; } }

    [SerializeField] private AudioClip meatHit = null;
    public AudioClip MeatHit { get { return meatHit; } }

    [SerializeField] private AudioClip meatThrow = null;
    public AudioClip MeatThrow { get { return meatThrow; } }

    [SerializeField] private AudioClip panHit = null;
    public AudioClip PanHit { get { return panHit; } }

    [SerializeField] private AudioClip panThrow = null;
    public AudioClip PanThrow { get { return panThrow; } }

    [SerializeField] private AudioClip plateHit = null;
    public AudioClip PlateHit { get { return plateHit; } }

    [SerializeField] private AudioClip plateThrow = null;
    public AudioClip PlateThrow { get { return plateThrow; } }

    [SerializeField] private AudioClip shieldHit = null;
    public AudioClip ShieldHit { get { return shieldHit; } }

    [SerializeField] private AudioClip shieldThrow = null;
    public AudioClip ShieldThrow { get { return shieldThrow; } }

    [SerializeField] private AudioClip spearHit = null;
    public AudioClip SpearHit { get { return spearHit; } }

    [SerializeField] private AudioClip spearThrow = null;
    public AudioClip SpearThrow { get { return spearThrow; } }

    [SerializeField] private AudioClip swordHit = null;
    public AudioClip SwordHit { get { return swordHit; } }

    [SerializeField] private AudioClip swordThrow = null;
    public AudioClip SwordThrow { get { return swordThrow; } }
    #endregion

    #region Other
    // Other audios
    [Header("Other Audios :")]
    [SerializeField] private AudioClip uiHighlight = null;
    public AudioClip UIHighlight { get { return uiHighlight; } }
    #endregion
    #endregion

    #region Singleton
    // The singleton instance of this script
    public static LDJ_AudioManager Instance = null;
    #endregion

    #region Methods
    #region Unity Methods
    void Awake()
    {
        // Set the singleton instance of this script as this if it is null, or destroy this
        if (!Instance) Instance = this;
        else
        {
            Destroy(this);
            return;
        }
    }

    private void OnDestroy()
    {
        // If this was the singleton instance, set it as null
        if (Instance == this) Instance = null;
    }

    // Use this for initialization
    void Start ()
    {
        // Get audio source components references if needed
        if (!cameraAudioSource) cameraAudioSource = Camera.main.GetComponent<AudioSource>();
        if (!playerAudioSource) playerAudioSource = LDJ_Player.Instance.GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    #endregion

    #region Original Methods
    /// <summary>
    /// Plays an audio clip at a specific audio source
    /// </summary>
    /// <param name="_clip">Clip to play</param>
    /// <param name="_source">Audio source where to play the clip</param>
    public void PlayAudio(AudioClip _clip, AudioSource _source)
    {
        if (_clip == null)
        {
            Debug.Log($"The audio clip you want to play is null !");
            return;
        }
        else if (_source == null)
        {
            Debug.Log($"The audio source you want to use is null !");
            return;
        }

        _source.PlayOneShot(_clip);

        OnPlaySound?.Invoke();
    }
    /// <summary>
    /// Plays an audio clip choosed randomly at a specific audio source
    /// </summary>
    /// <param name="_clip">Array of clip where to pick one randomly</param>
    /// <param name="_source">Audio source where to play the clip</param>
    public void PlayAudio(AudioClip[] _clip, AudioSource _source)
    {
        if (_clip.Length == 0)
        {
            Debug.Log($"The audio clip you want to play is null !");
            return;
        }
        else if (_source == null)
        {
            Debug.Log($"The audio source you want to use is null !");
            return;
        }

        _source.PlayOneShot(_clip[Random.Range(0, _clip.Length)]);

        OnPlaySound?.Invoke();
    }
    /// <summary>
    /// Plays an audio clip at a specific position
    /// </summary>
    /// <param name="_clip">Clip to play</param>
    /// <param name="_position">Position where to play the clip</param>
    public void PlayAudio(AudioClip _clip, Vector3 _position)
    {
        if (_clip == null)
        {
            Debug.Log($"The audio clip you want to play is null !");
            return;
        }

        AudioSource.PlayClipAtPoint(_clip, _position);

        OnPlaySound?.Invoke();
    }
    /// <summary>
    /// Plays an audio clip choosed randomly at a specific position
    /// </summary>
    /// <param name="_clip">Array of clip where to pick one randomly</param>
    /// <param name="_position">Position where to play the clip</param>
    public void PlayAudio(AudioClip[] _clip, Vector3 _position)
    {
        if (_clip.Length == 0)
        {
            Debug.Log($"The audio clip you want to play is null !");
            return;
        }

        AudioSource.PlayClipAtPoint(_clip[Random.Range(0, _clip.Length)], _position);

        OnPlaySound?.Invoke();
    }

    /// <summary>
    /// Set the audio clip of an specific audio source
    /// </summary>
    /// <param name="_source">Audio source to set the clip</param>
    /// <param name="_clip">Clip to use</param>
    public void SetAudio(AudioSource _source, AudioClip _clip)
    {
        if (_source == null)
        {
            Debug.Log($"The audio source you want to use is null !");
            return;
        }
        else if (_clip == null)
        {
            _source.Stop();
            return;
        }

        _source.clip = _clip;
        _source.Play();

        OnPlaySound?.Invoke();
    }
    #endregion
    #endregion
}
