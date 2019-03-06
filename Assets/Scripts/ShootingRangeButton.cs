using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingRangeButton : Pressable

{

    [SerializeField] Animator TargetAnimator;
    [SerializeField] AudioClip Button;
    private AudioSource MyAudio;

    private void Start()
    {
        MyAudio = this.GetComponent<AudioSource>();
    }

    public override void Toggle()
    {
        MyAudio.PlayOneShot(Button);
        TargetAnimator.SetTrigger("Start");

    }
}
