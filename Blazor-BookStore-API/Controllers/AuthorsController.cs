using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Blazor_BookStore_API.Controllers;
using Blazor_BookStore_API.Contracts;
using AutoMapper;
using Blazor_BookStore_API.DTOs;
using Blazor_BookStore_API.Data;

namespace Blazor_BookStore_API.Controllers {
    /// <summary>
    /// Endpoint used to interact with authors in database.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class AuthorsController : ControllerBase {
        private readonly IAuthorRepository _authorRepostory;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository, ILoggerService logger, IMapper mapper) {
            _authorRepostory = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all authors.
        /// </summary>
        /// <returns>List of authors.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors() {
            try {
                _logger.LogInfo("Attempted api/authors/GetAuthors");
                var authors = await _authorRepostory.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo("Successfully returned all authors from database.");
                return Ok(response);
            }
            catch (Exception ex) {
                return InternalError($"api/authors/GetAuthors", ex);
            }
        }

        /// <summary>
        /// Get author by ID.
        /// </summary>
        /// <returns>Author based on ID.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id) {
            try {
                _logger.LogInfo($"Attempted api/authors/GetAuthor/{id}");
                var author = await _authorRepostory.FindById(id);
                if (author == null) {
                    _logger.LogWarn($"Author with id:{id} not found.");
                    return NotFound();
                } else {
                    var response = _mapper.Map<AuthorDTO>(author);
                    _logger.LogInfo("Successfully returned author from database.");
                    return Ok(response);
                }
            }
            catch (Exception ex) {
                return InternalError($"api/authors/GetAuthor/{id}", ex);
            }
        }

        /// <summary>
        /// Create new author.
        /// </summary>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult>Create([FromBody] AuthorCreateDTO authorDTO) {
            try {
                _logger.LogInfo($"Attempted api/authors/Create");
                
                if (authorDTO == null) {
                    _logger.LogWarn($"Parameter (author) not provided.");
                    return BadRequest(ModelState);
                } else {
                    if (!ModelState.IsValid) {
                        _logger.LogWarn($"Author data is incomplete.");
                        return BadRequest(ModelState);
                    } else {
                        var author = _mapper.Map<Author>(authorDTO);
                        var isSuccess = await _authorRepostory.Create(author);
                        if (!isSuccess) {
                            _logger.LogError("Author creation failed.");
                            return StatusCode(500, "Author creation failed. Please contact customer support.");
                        } else {
                            _logger.LogInfo("Successfully created author in database.");
                            return Created("Create", new { author });
                        }
                    }
                }
            }
            catch (Exception ex) {
                return InternalError($"api/authors/Create", ex);
            }
        }

        /// <summary>
        /// Update values for an existing author.
        /// </summary>
        /// <param name="authorUpdateDTO">Body containing properties the user can update, such as first name, last name, and bio.</param>
        /// <returns>Properties for given author after successful update.</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] AuthorUpdateDTO authorUpdateDTO) {
            try {
                _logger.LogInfo($"Attempted api/authors/Update");

                if (authorUpdateDTO == null) {
                    _logger.LogWarn($"Parameter (author) not provided");
                    return BadRequest(ModelState);
                } else {
                    if (!ModelState.IsValid) {
                        _logger.LogWarn($"Author data is incomplete.");
                        return BadRequest(ModelState);
                    } else {
                        var author = _mapper.Map<Author>(authorUpdateDTO);
                        var isSuccess = await _authorRepostory.Update(author);
                        if (!isSuccess) {
                            _logger.LogError("Update attribues for an existing author failed.");
                            return StatusCode(500, "Update author failed. Please contact customer support.");
                        } else {
                            _logger.LogInfo("Successfully updated author in database.");

                            // No Content means you didn't have any errors, but I don't have anything to show you.
                            return NoContent();
                        }
                    }
                }
            }
            catch (Exception ex) {
                return InternalError($"api/authors/Update", ex);
            }
        }

        /// <summary>
        /// Delete author by ID.
        /// </summary>
        /// <returns>Delete based on ID.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult>Author(int id) {
            _logger.LogInfo($"Attempted Delete. api/authors/Author/{id}");
            try {
                var authorExists = await _authorRepostory.Exists(id);
                if (!authorExists) {
                    _logger.LogWarn($"Author with id:{id} not found.");
                    return StatusCode(404, $"Author (id: {id}) not found.");
                } else {                    
                    var author = await _authorRepostory.FindById(id);
                    var isSuccess = await _authorRepostory.Delete(author);
                    if (!isSuccess) {
                        _logger.LogError("Attempt to delete author id: {id} failed.");
                        return StatusCode(500, "Delete author failed. Please contact customer support.");
                    } else {
                        _logger.LogInfo("Successfully deleted author from database.");
                        return NoContent();
                    }
                }
            }
            catch (Exception ex) {
                return InternalError($"api/authors/Author/{id}", ex);
            }
        }

        private ObjectResult InternalError(string url, Exception ex) {
            _logger.LogError($"{url}: {ex.Message} - {ex.InnerException}");
            return StatusCode(500, "Something went wrong. Please contact customer support.");
        }
    }
}
