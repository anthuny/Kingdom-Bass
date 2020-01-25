using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mods
{
    public class Note : MonoBehaviour
    {
        private float startedAt;
        private float finishAt;

        public float cachedProgress { get; protected set; }
        private float duration { get { return finishAt - startedAt; } }

        public void UpdateNote(float songTime)
        {
            CacheProgressTime(songTime);
        }

        public void InitializeNote(float startAt, float finAt)
        {
            startedAt = startAt;
            finishAt = finAt;

            CacheProgressTime(Time.time);
        }

        private void CacheProgressTime(float time)
        {
            cachedProgress = Mathf.Clamp01((time - startedAt) / duration);
        }
    }
}
