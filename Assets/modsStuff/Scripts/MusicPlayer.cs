using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace mods
{
    [System.Serializable]
    public struct TempNoteStruct
    {
        [Min(0f)] public float spawnTime;
        public float duration;

        public int pathIndex;
    }

    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private MusicAsset musicAsset;

        private AudioSource trackSource;
        private List<TrackPath> paths;

        // temp
        public AudioClip music;
        public TrackPath[] pathPrefabs;
        public float pathSpread;
        public float pathLength;
        public TempNoteStruct[] notes;
        public float firstNoteDelay;
        private List<TempNoteStruct> notesCopy;
        
        [Header("Editor")]
        public int editorNumBeats;
        public float editorBeatTime;
        public int editorNumPaths;
        public float editorNoteDuration;

        void Awake()
        {
            if (musicAsset)
                InitializeFromMusicData(musicAsset.LoadXML());
        }

        void Start()
        {
            if (trackSource)
                trackSource.Play();

            SpawnNotes(0f);
        }

        void Update()
        {
            if (trackSource == null)
                return;

            // First spawn any new notes so they update this frame
            SpawnNotes(trackSource.time);

            // Now update each path
            foreach (TrackPath path in paths)
                path.UpdatePath(trackSource.time, pathLength);
        }

        private void InitializeFromMusicData(MusicData musicData)
        {
            paths = new List<TrackPath>();
            for (int i = 0; i < pathPrefabs.Length; ++i)
            {
                TrackPath newPath = Instantiate(pathPrefabs[i]);

                Vector3 pathPos = transform.position + (Vector3.right * pathSpread * i);
                newPath.transform.position = pathPos;

                paths.Add(newPath);
            }

            // Make sure these are sorted by time, we assume it is sorted
            // when checking when notes can spawn
            notesCopy = new List<TempNoteStruct>(notes.Length);
            notesCopy.AddRange(notes);

            trackSource = gameObject.AddComponent<AudioSource>();
            trackSource.spatialize = false;
            trackSource.spatialBlend = 0f;
            trackSource.clip = music;
        }

        private void SpawnNotes(float time)
        {
            int activateNotes = 0;
            for (int i = 0; i < notesCopy.Count; ++i)
            {
                TempNoteStruct noteStruct = notesCopy[i];

                // Haven't reach spawn time for not yet
                if (noteStruct.spawnTime + firstNoteDelay > time)
                {
                    break;
                }

                TrackPath trackPath = paths[noteStruct.pathIndex];
                trackPath.SpawnNote(noteStruct.spawnTime + firstNoteDelay, noteStruct.duration);
                ++activateNotes;
            }

            // Remove notes we just activated
            if (activateNotes > 0)
                notesCopy.RemoveRange(0, activateNotes);
        }

        private void OnDrawGizmos()
        {         
            for (int i = 0; i < pathPrefabs.Length; ++i)
            {
                Vector3 point = transform.position + (Vector3.right * pathSpread * i);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(point, 0.5f);
                Gizmos.DrawLine(point, point + Vector3.forward * pathLength);
            }
        }
    }

    [CustomEditor(typeof(MusicPlayer))]
    public class MusicPlayerEditor : Editor
    {
        SerializedProperty notes;

        void OnEnable()
        {
            notes = serializedObject.FindProperty("notes");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            if (GUILayout.Button("Set from Editor Vars"))
            {
                MusicPlayer player = serializedObject.targetObject as MusicPlayer;

                TempNoteStruct[] newNotes = new TempNoteStruct[player.editorNumBeats];
                for (int i = 0; i < player.editorNumBeats; ++i)
                {
                    newNotes[i].duration = player.editorNoteDuration;
                    newNotes[i].spawnTime = player.editorBeatTime * i;

                    newNotes[i].pathIndex = Random.Range(0, player.editorNumPaths);
                }

                player.notes = newNotes;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
