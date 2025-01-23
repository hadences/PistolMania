using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public static AudioClip shootSound;
    public static AudioClip reloadSound;
    public static AudioClip happySound;
    public static AudioClip hurtSound;
    public static AudioClip ghostDeathSound;

    void Awake(){
        // add the singleton pattern to the SoundManager
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        // register the sounds
        registerSounds();
    }

    private void registerSounds() {
        shootSound = registerSound("Sounds/Shoot");
        reloadSound = registerSound("Sounds/Reload");
        happySound = registerSound("Sounds/Happy");
        hurtSound = registerSound("Sounds/Hurt");
        ghostDeathSound = registerSound("Sounds/GhostDeath");

    }

    public AudioClip registerSound(String path){
        // load the audio clip from the path
        return Resources.Load<AudioClip>(path);
    }

    public void playSound(AudioClip sound, float volume, float pitch){
        // create a new game object to play the sound
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = sound;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.Play();
        Destroy(soundGameObject, sound.length);
    }

    public void playSound(AudioClip sound){
        playSound(sound, 1, 1);
    }

}
