using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeldiNote.Api.Models;
using FeldiNote.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}