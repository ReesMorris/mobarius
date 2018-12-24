using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager Instance;

    public AudioSource globalSounds;
    public Sound[] sounds;

    PhotonView photonView;

    void Start() {
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    public void PlaySound(string soundName) {
        globalSounds.clip = GetSoundByName(soundName);
        globalSounds.Play();
    }

    public void PlayGlobalSound(string soundName) {
        photonView.RPC("GlobalSound", PhotonTargets.All, soundName);
    }

    public void PlayLocationalSound(string soundName, Vector3 position) {
        photonView.RPC("GlobalSound", PhotonTargets.All, soundName, position);
    }

    [PunRPC]
    void GlobalSound(string soundName) {
        globalSounds.clip = GetSoundByName(soundName);
        globalSounds.Play();
    }

    [PunRPC]
    void LocationalSound(string soundName, Vector3 position) {
        AudioSource.PlayClipAtPoint(GetSoundByName(soundName), position);
    }

    AudioClip GetSoundByName(string soundName) {
        foreach(Sound sound in sounds) {
            if (sound.name == soundName)
                return sound.audioClips[Random.Range(0, sound.audioClips.Length)];
        }
        return null;
    }
}
