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
    /// REST endpoint that's used to interact with books in database.
    /// </summary>
    [Route("api/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase {
        private readonly IBookRepository _bookRepostory;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public BooksController(IBookRepository bookRepository, ILoggerService logger, IMapper mapper) {
            _bookRepostory = bookRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all books.
        /// </summary>
        /// <returns>List of all books and their authors.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks() {
            var location = GetControllerActionName();
            try {
                _logger.LogInfo($"{location}: Attempted Call");
                var books = await _bookRepostory.FindAll();
                var response = _mapper.Map<IList<BookDTO>>(books);
                _logger.LogInfo($"{location}: Successfull");
                return Ok(response);
            }
            catch (Exception ex) {
                return InternalError(location, ex);
            }
        }

        /// <summary>
        /// Get book by ID.
        /// </summary>
        /// <returns>Book based on ID.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBook(int id) {
            var location = GetControllerActionName();
            try {
                _logger.LogInfo($"{location}: Attempted Call");
                var book = await _bookRepostory.FindById(id);
                if (book == null) {
                    _logger.LogWarn($"{location}: Book with id:{id} not found.");
                    return NotFound();
                } else {
                    var response = _mapper.Map<BookDTO>(book);
                    _logger.LogInfo($"{location}: Successfull");
                    return Ok(response);
                }
            }
            catch (Exception ex) {
                return InternalError(location, ex);
            }
        }

        /// <summary>
        /// Create new book.
        /// </summary>
        /// <param name="bookDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO bookDTO) {
            var location = GetControllerActionName();
            try {
                _logger.LogInfo($"{location}: Attempted Call");

                if (bookDTO == null) {
                    _logger.LogWarn($"{location}: Parameter (bookDTO) not provided.");
                    return BadRequest(ModelState);
                } else {
                    if (!ModelState.IsValid) {
                        _logger.LogWarn($"{location}: Book data is incomplete.");
                        return BadRequest(ModelState);
                    } else {
                        var book = _mapper.Map<Book>(bookDTO);
                        var isSuccess = await _bookRepostory.Create(book);
                        if (!isSuccess) {
                            _logger.LogError("{location}: Book creation failed.");
                            return StatusCode(500, "Book creation failed. Please contact customer support.");
                        } else {
                            _logger.LogInfo($"{location}: Successfull");
                            return Created("Create", new { book });
                        }
                    }
                }
            }
            catch (Exception ex) {
                return InternalError(location, ex);
            }
        }

        /// <summary>
        /// Update values for an existing book.
        /// </summary>
        /// <param name="bookUpdateDTO">Body containing properties the user can update.</param>
        /// <returns>When update is successful, nothing is returned. Action method will return HTTP Status Code 204.</returns>
        [HttpPut]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] BookUpdateDTO bookUpdateDTO) {
            var location = GetControllerActionName();
            try {
                _logger.LogInfo($"{location}: Attempted Call");

                if (bookUpdateDTO == null) {
                    _logger.LogWarn($"{location}: Parameter (bookUpdateDTO) not provided");
                    return BadRequest(ModelState);
                } else {
                    if (!ModelState.IsValid) {
                        _logger.LogWarn($"{location}: Book data is incomplete.");
                        return BadRequest(ModelState);
                    } else {
                        var book = _mapper.Map<Book>(bookUpdateDTO);
                        var isSuccess = await _bookRepostory.Update(book);
                        if (!isSuccess) {
                            _logger.LogError("{location}: Update for existing book failed.");
                            return StatusCode(500, "Update book failed. Please contact customer support.");
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
        /// Delete book by ID.
        /// </summary>
        /// <returns>Delete based on ID.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Book(int id) {
            var location = GetControllerActionName();
            _logger.LogInfo($"{location}: Attempted Call");
            try {
                var bookExists = await _bookRepostory.Exists(id);
                if (!bookExists) {
                    _logger.LogWarn($"{location}: Book with id:{id} not found.");
                    return StatusCode(404, $"Book (id: {id}) not found.");
                } else {
                    var book = await _bookRepostory.FindById(id);
                    var isSuccess = await _bookRepostory.Delete(book);
                    if (!isSuccess) {
                        _logger.LogError("{location}: Attempt to delete book id: {id} failed.");
                        return StatusCode(500, "Delete book failed. Please contact customer support.");
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
