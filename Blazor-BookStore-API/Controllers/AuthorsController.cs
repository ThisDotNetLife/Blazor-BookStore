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
using Microsoft.AspNetCore.Authorization;

namespace Blazor_BookStore_API.Controllers {
    /// <summary>
    /// REST endpoint that's used to interact with authors in database.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
            var location = GetControllerActionName();
            try {
                _logger.LogInfo($"{location}: Attempted Call");
                var authors = await _authorRepostory.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo($"{location}: Successfull");
                return Ok(response);
            }
            catch (Exception ex) {
                return InternalError(location, ex);
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
            var location = GetControllerActionName();
            try {
                _logger.LogInfo($"{location}: Attempted Call");
                var author = await _authorRepostory.FindById(id);
                if (author == null) {
                    _logger.LogWarn($"{location}: Author with id:{id} not found.");
                    return NotFound();
                } else {
                    var response = _mapper.Map<AuthorDTO>(author);
                    _logger.LogInfo($"{location}: Successfull");
                    return Ok(response);
                }
            }
            catch (Exception ex) {
                return InternalError(location, ex);
            }
        }

        /// <summary>
        /// Create new author.
        /// </summary>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult>Create([FromBody] AuthorCreateDTO authorDTO) {
            var location = GetControllerActionName();
            try {
                _logger.LogInfo($"{location}: Attempted Call");

                if (authorDTO == null) {
                    _logger.LogWarn($"{location}: Parameter (author) not provided.");
                    return BadRequest(ModelState);
                } else {
                    if (!ModelState.IsValid) {
                        _logger.LogWarn($"{location}: Author data is incomplete.");
                        return BadRequest(ModelState);
                    } else {
                        var author = _mapper.Map<Author>(authorDTO);
                        var isSuccess = await _authorRepostory.Create(author);
                        if (!isSuccess) {
                            _logger.LogError("{location}: Author creation failed.");
                            return StatusCode(500, "Author creation failed. Please contact customer support.");
                        } else {
                            _logger.LogInfo($"{location}: Successfull");
                            return Created("Create", new { author });
                        }
                    }
                }
            }
            catch (Exception ex) {
                return InternalError(location, ex);
            }
        }

        /// <summary>
        /// Update values for an existing author.
        /// </summary>
        /// <param name="authorUpdateDTO">Body containing properties the user can update, such as first name, last name, and bio.</param>
        /// <returns>Properties for given author after successful update.</returns>
        [HttpPut]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] AuthorUpdateDTO authorUpdateDTO) {
            var location = GetControllerActionName();
            try {
                _logger.LogInfo($"{location}: Attempted Call");

                if (authorUpdateDTO == null) {
                    _logger.LogWarn($"{location}: Parameter (author) not provided");
                    return BadRequest(ModelState);
                } else {
                    if (!ModelState.IsValid) {
                        _logger.LogWarn($"{location}: Author data is incomplete.");
                        return BadRequest(ModelState);
                    } else {
                        var author = _mapper.Map<Author>(authorUpdateDTO);
                        var isSuccess = await _authorRepostory.Update(author);
                        if (!isSuccess) {
                            _logger.LogError("{location}: Update for existing author failed.");
                            return StatusCode(500, "Update author failed. Please contact customer support.");
                        } else {
                            _logger.LogInfo($"{location}: Successfull");

                            // No Content means you didn't have any errors, but I don't have anything to show you.
                            return NoContent();
                        }
                    }
                }
            }
            catch (Exception ex) {
                return InternalError(location, ex);
            }
        }

        /// <summary>
        /// Delete author by ID.
        /// </summary>
        /// <returns>Delete based on ID.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult>Author(int id) {
            var location = GetControllerActionName();
            _logger.LogInfo($"{location}: Attempted Call");
            try {
                var authorExists = await _authorRepostory.Exists(id);
                if (!authorExists) {
                    _logger.LogWarn($"{location}: Author with id:{id} not found.");
                    return StatusCode(404, $"Author (id: {id}) not found.");
                } else {                    
                    var author = await _authorRepostory.FindById(id);
                    var isSuccess = await _authorRepostory.Delete(author);
                    if (!isSuccess) {
                        _logger.LogError("{location}: Attempt to delete author id: {id} failed.");
                        return StatusCode(500, "Delete author failed. Please contact customer support.");
                    } else {
                        _logger.LogInfo($"{location}: Successfull");
                        return NoContent();
                    }
                }
            }
            catch (Exception ex) {
                return InternalError(location, ex);
            }
        }

        private string GetControllerActionName() {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;
            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string location, Exception ex) {
            _logger.LogError($"{location}: {ex.Message} - {ex.InnerException}");
            return StatusCode(500, "Something went wrong. Please contact customer support.");
        }
    } /* End of Controller */
}
