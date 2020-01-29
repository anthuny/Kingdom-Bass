using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mods
{
    public class MusicKey
    {
        public float activateTime;
    }

    public class MusicBeat : MusicKey
    {
        public float beatElapse;
    }

    public class MusicBeatPattern
    {
        public List<MusicBeat> beats;
    }

    public class MusicNote : MusicKey
    {
        public float overrideDuration;  // 0 if duration is beat
    }

    public class MusicTrack
    {

    }
}
