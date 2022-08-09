using AuthorsAPI.DTOs;
using AuthorsAPI.IRepository;
using AuthorsAPI.Model;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthorsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BooksController> _logger;
        private readonly IMapper _mapper;
        public BooksController(IUnitOfWork unitOfWork, ILogger<BooksController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;   
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            try
            {
                var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
                var currentUser = await _unitOfWork.Authors.Get(p => p.Email == currentUserEmail);
                var books = await _unitOfWork.Books.GetAll(t => t.AuthorId == currentUser.Id);
                var results = _mapper.Map<IList<BookDTO>>(books);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{Id}")]

        public async Task<IActionResult> GetOne(int Id)
        {
            try
            {
                if (Id == 0)
                {
                    return NotFound();
                }
               
                var book = await _unitOfWork.Books.Get(x => x.Id == Id);
                if (book == null)
                {
                    return NotFound();
                }
                var result = _mapper.Map<BookDTO>(book);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]

        public async Task<IActionResult> PostBook([FromBody] CreateBookDTO bookdto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var book = _mapper.Map<BookModel>(bookdto);
                    var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
                    var currentUser = await _unitOfWork.Authors.Get(p => p.Email == currentUserEmail);
                    book.AuthorId = currentUser.Id;
                    await _unitOfWork.Books.Insert(book);
                    await _unitOfWork.Save();
                    return StatusCode(200);

                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, $"Something went wrong");
                    return StatusCode(500, "Internal server error");
                }
              
            }
            _logger.LogError($"Invalid Post attempt");
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateBook(int id, [FromBody] CreateBookDTO bookdto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var book = await _unitOfWork.Books.Get(x => x.Id == id);
                    if(book == null)
                    {
                        return BadRequest("Data is invalid");
                    }
                     _mapper.Map(bookdto, book);
                     _unitOfWork.Books.Update(book);
                    await _unitOfWork.Save();


                    return NoContent();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Something went wrong");
                    return StatusCode(500, "Internal server error");
                }

            }
            _logger.LogError($"Invalid Post attempt");
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id < 1)
            {
                return NotFound();
            }
            try
            {
                var book = await _unitOfWork.Books.Get(x => x.Id == id);
                if (book == null)
                {
                    return BadRequest("Data is invalid");
                }
                await _unitOfWork.Books.Delete(id);
                await _unitOfWork.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong");
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
