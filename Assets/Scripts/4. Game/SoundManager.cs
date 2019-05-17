using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    This script handles playing sounds in the game
*/
/// <summary>
/// This script handles playing sounds in the game.
/// </summary>
public class SoundManager : MonoBehaviour {


    // Public variables
    public static SoundManager Instance;

    public AudioSource globalSounds;
    public Sound[] sounds;

    // Private variables
    PhotonView photonView;

    // Assign variables when the game starts.
    void Start() {
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    /// <summary>
    /// Plays a sound to the local player regardless of their position in the world.
    /// </summary>
    /// <param name="soundName">The name of the sound file to be played</param>
    public void PlaySound(string soundName) {
        globalSounds.clip = GetSoundByName(soundName);
        globalSounds.Play();
    }

    /// <summary>
    /// Plays a sound to all players regardless of their position in the world.
    /// </summary>
    /// <param name="soundName">The name of the sound file to be played</param>
    public void PlayGlobalSound(string soundName) {
        photonView.RPC("GlobalSound", PhotonTargets.All, soundName);
    }

    /// <summary>
    /// Plays a sound to all players near the position in the world.
    /// </summary>
    /// <param name="soundName">The name of the sound file to be played</param>
    /// <param name="position">The position in the world the sound is played from</param>
    public void PlayLocationalSound(string soundName, Vector3 position) {
        photonView.RPC("GlobalSound", PhotonTargets.All, soundName, position);
    }

    // Plays a global sound across the network
    [PunRPC]
    void GlobalSound(string soundName) {
        globalSounds.clip = GetSoundByName(soundName);
        globalSounds.Play();
    }

    // Plays a locational sound across the network
    [PunRPC]
    void LocationalSound(string soundName, Vector3 position) {
        AudioSource.PlayClipAtPoint(GetSoundByName(soundName), position);
    }

    // Returns an audio clip if one can be found with the same name
    AudioClip GetSoundByName(string soundName) {
        foreach(Sound sound in sounds) {
            if (sound.name == soundName)
                return sound.audioClips[Random.Range(0, sound.audioClips.Length)];
        }
        return null;
    }
}
