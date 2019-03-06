using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTargets : MonoBehaviour
{
    [SerializeField] GameObject[] Targets;
    [SerializeField] Light[] Lights;
    // Start is called before the first frame update

    // Update is called once per frame
    void reset()
    {

        Lights[0].enabled = false;
        Lights[1].enabled = false;
        Lights[2].enabled = false;
        Lights[3].enabled = false;
        
        Targets[0].GetComponent<TargetHit>().Used = false;
        Targets[1].GetComponent<TargetHit>().Used = false;
        Targets[2].GetComponent<TargetHit>().Used = false;
        Targets[3].GetComponent<TargetHit>().Used = false;

        this.GetComponent<Animator>().ResetTrigger("Start");
    }
}
