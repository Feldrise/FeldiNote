using FeldiNote.Api.Models;
using FeldiNote.Api.Settings;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeldiNote.Api.Services
{
    public class NoteService
    {
        private readonly IMongoCollection<Note> _notes;
        private readonly IMongoDatabase _database;

        public NoteService(IMongoSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);

            _database = client.GetDatabase(settings.DatabaseName);
        }

        public Note GetNote(string collection, string noteId)
        {
            var notesCollection = _database.GetCollection<Note>(collection);

            return notesCollection.Find(note => note.Id == noteId).FirstOrDefault();
        }

        public async Task<List<Note>> GetNotesAsync(String collection)
        {
            var filter = new BsonDocument("name", collection);
            var collectionCursor = await _database.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
            var exist = await collectionCursor.AnyAsync();

            if (!exist)
            {
                throw new Exception("The notes collection can't be found");
            } 

            return _database.GetCollection<Note>(collection).Find(note => true).ToList(); 
        }

        public Note AddNote(string userId, Note noteIn)
        {
            var userNotesCollection = _database.GetCollection<Note>($"notes_{userId}");

            userNotesCollection.InsertOne(noteIn);

            return noteIn;
        }

        public void UpdateNote(string userId, Note noteIn)
        {
            var userNotesCollection = _database.GetCollection<Note>($"notes_{userId}");

            userNotesCollection.ReplaceOne(note => note.Id == noteIn.Id, noteIn);
        }

        public void RemoveNote(string userId, string noteId)
        {
            var userNotesCollection = _database.GetCollection<Note>($"notes_{userId}");

            userNotesCollection.DeleteOne(note => note.Id == noteId);
        }
    }
}
