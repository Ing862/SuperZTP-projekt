﻿using SuperZTP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperZTP.Command
{
    public class AddNote : ICommand
    {
        private List<Note> notes;
        private Note newNote;

        public AddNote(List<Note> notes, Note newNote)
        {
            this.notes = notes;
            this.newNote = newNote;
        }

        public void Execute()
        {
            var noteCopy = new Note
            {
                Id = newNote.Id,
                Title = newNote.Title,
                Description = newNote.Description,
                Tag = newNote.Tag,
                Category = newNote.Category
            };

            notes.Add(noteCopy);
        }

        public void Undo()
        {
            notes.Remove(newNote);
        }
    }

    public class EditNote : ICommand
    {
        private List<Note> notes;
        private Note newNoteCopy;
        private Note oldNote;
        private int id;

        public EditNote(List<Note> notes, Note editNote, int id)
        {
            this.notes = notes;
            this.id = id;

            if(id >= 0 && id <= notes.Count)
            {
                oldNote = new Note
                {
                    Id = notes[id].Id,
                    Title = notes[id].Title,
                    Description = notes[id].Description,
                    Tag = notes[id].Tag,
                    Category = notes[id].Category
                };

                newNoteCopy = new Note
                {
                    Id = editNote.Id,
                    Title = editNote.Title,
                    Description = editNote.Description,
                    Tag = editNote.Tag,
                    Category = editNote.Category
                };
            }
        }

        public void Execute()
        {
            if(id >= 0 && id <= notes.Count)
            {
                notes[id] = newNoteCopy;
            }
        }

        public void Undo()
        {
            if(oldNote != null && id >= 0 && id <= notes.Count)
            {
                notes[id] = oldNote;
            }
        }
    }

    public class DeleteNote : ICommand
    {
        private List<Note> notes;
        private Note noteCopy;
        private int id;

        public DeleteNote(List<Note> notes, Note deleteNote, int id)
        {
            this.notes = notes;
            this.id = id;
            if(id >= 0 && id <= notes.Count)
            {
                noteCopy = new Note
                {
                    Id = deleteNote.Id,
                    Title = deleteNote.Title,
                    Description = deleteNote.Description,
                    Tag = deleteNote.Tag,
                    Category = deleteNote.Category
                };
            }
        }

        public void Execute() 
        { 
            if(id >= 0 && id <= notes.Count)
            {
                notes.RemoveAt(id);
            }
        }

        public void Undo()
        {
            if(noteCopy != null && id >= 0 && id <= notes.Count)
            {
                notes.Insert(id, noteCopy);
            }
        }
    }

    public class GroupNotes
    {
        public List<IGrouping<string, Note>> GroupNotesByCategory(List<Note> notes)
        {
            var groupedNotes = notes
                .GroupBy(note => note.Category?.Name ?? "Brak kategorii")
                .OrderBy(group => group.Key)
                .ToList();
            return groupedNotes;
        }

        public List<IGrouping<string, Note>> GroupNotesByTag(List<Note> notes)
        {
            var groupedNotes = notes
                .GroupBy(note => note.Tag?.Name ?? "Brak tagu")
                .OrderBy(group => group.Key)
                .ToList();
            return groupedNotes;
        }
    }

    public class SortNotes
    {
        public List<Note> SortNotesByTitle(List<Note> notes, bool ascending = true)
        {
            var sortedNotes = ascending
                ? notes.OrderBy(note => note.Title).ToList()
                : notes.OrderByDescending(note => note.Title).ToList();

            notes = sortedNotes;
            return notes;
        }
    }
}
