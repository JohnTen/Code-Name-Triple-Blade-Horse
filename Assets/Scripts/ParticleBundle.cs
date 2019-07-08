using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleBladeHorse.Animation;
public class ParticleBundle : MonoBehaviour
{
    [SerializeField]
    List<string> particleNames;
    [SerializeField]
    List<ParticleSystem> particleObjs;
    private Dictionary<string, ParticleSystem> particles;
    private FSM fSM;

    private void Start()
    {
        int i = 0;
        fSM = this.GetComponent<FSM>();
        particleNames = new List<string>();
        particleObjs = new List<ParticleSystem>();
        particles = new Dictionary<string, ParticleSystem>();
        foreach(var particleName in particleNames)
        {
            particles.Add(particleName, particleObjs[i]);
            i++;
        }
    }

    private void Update()
    {

    }
}
