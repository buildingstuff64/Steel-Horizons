using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "ProcedualData", menuName = "Data/Procedural")]
    public class ProceduralData : ScriptableObject
    {
        [SerializeField] public int xsize, zsize;

        [SerializeField] public List<float> noiseScales = new List<float>();
        [SerializeField] public List<float> Amplitides = new List<float>();

        [SerializeField] public float waterThreshold, grassThreshold, sandThreshold;
        [SerializeField] public float strength = 3;
        [SerializeField] public float amp = 3;

        [SerializeField] public float seed;
    }
}