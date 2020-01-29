using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mods
{
    public class TrackPath : MonoBehaviour
    {
        private List<Note> notes = new List<Note>();

        [SerializeField] protected Note prefabNote;

        private List<Note> finishedNotes = new List<Note>();    // Notes that have finished this update

        public void UpdatePath(float songTime, float pathLength)
        {
            foreach (Note note in notes)
                UpdateNote(note, songTime, pathLength);

            DestroyFinishedNotes();
        }

        public void SpawnNote(float startTime, float duration)
        {
            Note newNote = Instantiate(prefabNote, transform.position, Quaternion.identity);
            newNote.InitializeNote(startTime, startTime + duration);

            notes.Add(newNote);
        }

        private void UpdateNote(Note note, float songTime, float pathLength)
        {
            note.UpdateNote(songTime);
            if (note.cachedProgress >= 1f)
            {
                finishedNotes.Add(note);
            }

            Vector3 end = transform.position + (Vector3.forward * pathLength);
            note.transform.position = Vector3.Lerp(transform.position, end, note.cachedProgress);
        }

        private void DestroyFinishedNotes()
        {
            foreach (Note note in finishedNotes)
            {
                notes.Remove(note);
                Destroy(note.gameObject);
            }

            finishedNotes.Clear();
        }
    }
}
