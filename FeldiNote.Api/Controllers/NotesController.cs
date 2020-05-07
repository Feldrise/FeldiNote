using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeldiNote.Api.Models;
using FeldiNote.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace FeldiNote.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly NoteService _noteService;

        public NotesController(NoteService noteService)
        {
            _noteService = noteService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Note>>> GetNotesAync([FromHeader] string authorization)
        {
            string encodedUsernamePassword = authorization.Substring("Basic ".Length).Trim();
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string idAndPassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

            int seperatorIndex = idAndPassword.IndexOf(':');

            var userId = idAndPassword.Substring(0, seperatorIndex);

            List<Note> publicNotes;

            try
            {
                publicNotes = await _noteService.GetNotesAsync($"notes_{userId}");
            }
            catch
            {
                return NotFound();
            }

            return Ok(publicNotes);
        }

        [HttpGet("{collection}")]
        public async Task<ActionResult<List<Note>>> GetNotesForCollectionAsync(string collection)
        {
            List<Note> publicNotes;

            try
            {
                publicNotes = await _noteService.GetNotesAsync(collection);
            }
            catch
            {
                return NotFound();
            }

            return Ok(publicNotes);
        }

        [HttpPost("add")]
        public ActionResult<Note> AddNote([FromBody] Note note, [FromHeader] string authorization)
        {
            string encodedUsernamePassword = authorization.Substring("Basic ".Length).Trim();
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string idAndPassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

            int seperatorIndex = idAndPassword.IndexOf(':');

            var userId = idAndPassword.Substring(0, seperatorIndex);

            note = _noteService.AddNote(userId, note);

            return note;
        }

        [HttpPut("update/{id:length(24)}")]
        public IActionResult Update(string id, [FromBody] Note noteIn, [FromHeader] string authorization)
        {
            string encodedUsernamePassword = authorization.Substring("Basic ".Length).Trim();
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string idAndPassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

            int seperatorIndex = idAndPassword.IndexOf(':');

            var userId = idAndPassword.Substring(0, seperatorIndex);

            var note = _noteService.GetNote($"notes_{userId}", id);

            if (note == null)
            {
                return NotFound();
            }

            _noteService.UpdateNote(userId, noteIn);

            return NoContent();
        }

        [HttpDelete("remove/{id:length(24)}")]
        public IActionResult Delete(string id, [FromHeader] string authorization)
        {
            string encodedUsernamePassword = authorization.Substring("Basic ".Length).Trim();
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string idAndPassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

            int seperatorIndex = idAndPassword.IndexOf(':');

            var userId = idAndPassword.Substring(0, seperatorIndex);

            var note = _noteService.GetNote($"notes_{userId}", id);

            if (note == null)
            {
                return NotFound();
            }

            _noteService.RemoveNote(userId, note.Id);

            return NoContent();
        }
    }
}