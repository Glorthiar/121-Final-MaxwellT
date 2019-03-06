using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : MonoBehaviour
{

    AudioSource myAudio;
    [SerializeField] AudioClip[] clips;
    // Use this for initialization
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void PlaySound(int soundID)
    {
        myAudio.PlayOneShot(clips[soundID], 1);
    }
}
