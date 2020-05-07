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
        public async Task<ActionResult<List<Note>>> GetNotesAync()
        {
            List<Note> publicNotes;

            try
            {
                publicNotes = await _noteService.GetNotesAsync("PublicNotes");
            }
            catch
            {
                return BadRequest("The collection doesn't exist");
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
                return BadRequest("The collection doesn't exist");
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

            var id = idAndPassword.Substring(0, seperatorIndex);

            note = _noteService.AddNote(id, note);

            return note;
        }
    }
}