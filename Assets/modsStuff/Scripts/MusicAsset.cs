using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mods
{
    public struct MusicData
    {
        public string trackNotes;
        public float trackLength;
    }

    [CreateAssetMenu(fileName = "modsMusicAsset", menuName = "mods/MusicAsset")]
    public class MusicAsset : ScriptableObject
    {
        [SerializeField] private string filepath;

        public MusicData LoadXML()
        {
            return new MusicData();
        }
    }
}
