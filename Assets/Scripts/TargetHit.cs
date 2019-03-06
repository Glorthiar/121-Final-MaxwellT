using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHit : MonoBehaviour
{
    [SerializeField] Light[] Lights;
    [SerializeField] AudioClip Buzz;
    [SerializeField] AudioSource Buzzer;
    public bool Used;
    // Start is called before the first frame update
    void Start()
    {
        Used = false;
        Lights[0].enabled = false;
        Lights[1].enabled = false;
        Lights[2].enabled = false;
        Lights[3].enabled = false;

    }

    // Update is called once per frame
    void HitByRay()
    {
        if (Used == false){ 
        if (Lights[0].enabled == false) { Lights[0].enabled = true; }else
        if (Lights[1].enabled == false) { Lights[1].enabled = true; }else
        if (Lights[2].enabled == false) { Lights[2].enabled = true; }else
        if (Lights[3].enabled == false) { Lights[3].enabled = true; }
        Buzzer.PlayOneShot(Buzz);
        Used = true;
        }
    }
}
