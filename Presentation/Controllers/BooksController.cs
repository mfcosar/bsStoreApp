﻿using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/books")]
   
        public class BooksController : ControllerBase
        {
            //Servis katmanı ekleniyor
            private readonly IServiceManager _manager;
            public BooksController(IServiceManager manager)
            {
                _manager = manager;
            }


            /* private readonly IRepositoryManager _manager;  //RepositoryContext _context; ==> artık manager kullanılsın istiyoruz.

            public BooksController(IRepositoryManager manager)
            {
                _manager = manager; //_manager üzerinden repolara erişim sağlanır
            }*/


            /* public BooksController(RepositoryContext context)
            {//register yapılmış olan context burda resolve edilir

                _context = context;
            }*/


            [HttpGet]
            public async Task<IActionResult> GetAllBooksAsync()
            {
                //var books = _manager.BookRepo.GetAllBooks(false);   //_context.Books.ToList();
                var books = await _manager.BookService.GetAllBooksAsync(false);
                return Ok(books);
            }

            [HttpGet("{id:int}")]
            public async Task<IActionResult> GetOneBookAsync([FromRoute(Name = "id")] int id)
            {
                var book = await _manager.BookService.GetOneBookByIdAsync(id, false);        //BookRepo.GetOneBookById(id,false);
                                                                                  //_context.Books.Where(b => b.Id.Equals(id)).SingleOrDefault();
            /* if (book is null)
                throw new BookNotFoundException(id); */
                return Ok(book);
            }

            [HttpPost]
            public async Task<IActionResult> FormOneBookAsync([FromBody] BookDtoForInsertion bookDto)
            {
                if (bookDto is null)
                    return BadRequest(); //400
                if (!ModelState.IsValid)
                    return UnprocessableEntity(ModelState); //422

            //_manager.BookRepo.CreateOneBook(book);  //_context.Books.Add(book);
            var book = await _manager.BookService.CreateOneBookAsync(bookDto);
                //_manager.Save();    //_context.SaveChanges();
                return StatusCode(201, book); //CreatedAtRoute()

            }

            [HttpPut("{id:int}")]
            public async Task<IActionResult> UpdateOneBookAsync([FromRoute(Name = "id")] int id, [FromBody] BookDtoForUpdate bookDto)
            {
                if (bookDto is null)
                    return BadRequest(); //400
                if (!ModelState.IsValid)
                  return UnprocessableEntity(ModelState); //422

                await _manager.BookService.UpDdateOneBookAsync(id, bookDto, false);
                return NoContent(); //204

                                    //var entity = _manager.BookRepo.GetOneBookById(id, true);
                                    //_context.Books.Where(b => b.Id.Equals(id)).SingleOrDefault();


                //check id: update edilecek book ile route'dan gelen id eşit değilse BadRequest
                /*if (id != book.Id)
                    return BadRequest(); // 400

                //map işlemine ihtiyaç var:
                entity.Title = book.Title;
                entity.Price = book.Price;
                _manager.Save();  //_context.SaveChanges();
                return Ok(book);*/
            }


            [HttpDelete("{id:int}")]
            public async Task<IActionResult> DeleteOneBookAsync([FromRoute(Name = "id")] int id)
            {

                /*//var entity = _manager.BookRepo.GetOneBookById(id, false);
                var entity = _manager.BookService.GetOneBookById(id, false);
                    //_context.Books.Where(b => b.Id.Equals(id)).SingleOrDefault();

                if (entity is null)
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = $"Book with id:{id} could not be found."
                    }); // 404
                */
                /*_manager.BookRepo.DeleteOneBook(entity);    //_context.Books.Remove(entity);
                _manager.Save();                            //_context.SaveChanges();*/
                await _manager.BookService.DeleteOneBookAsync(id, false);

                return NoContent();
            }

            [HttpPatch("{id:int}")]
            public async  Task<IActionResult> PartiallyUpdateOneBookAsync([FromRoute(Name = "id")] int id,
                                            [FromBody] JsonPatchDocument<BookDtoForUpdate> bookPatch)
            {

            // validation if bookPatch is valid
            if (bookPatch is null)
                return BadRequest();//400
            var result = await _manager.BookService.GetOneBookForPatchAsync(id, false);

            //check entity, if book exists
            //var entity = _manager.BookRepo.GetOneBookById(id, true);
            //_context.Books.Where(b => b.Id.Equals(id)).SingleOrDefault();

            //var bookDto = _manager.BookService.GetOneBookById(id, true);

                bookPatch.ApplyTo(result.bookDtoForUpdate, ModelState);
            //_manager.BookRepo.Update(entity);    //_context.SaveChanges();
            //_manager.BookService.UpDdateOneBook(id, entity, true); 
            //Dto'dan ötürü hata verir. Patch yapısından ötürü mapping yapmadık. Bunu aşmak için:

            TryValidateModel(result.bookDtoForUpdate);
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            //save changes for patch
            await _manager.BookService.saveChangesForPatchAsync(result.bookDtoForUpdate, result.book);
            return NoContent(); //204
            }
        }
    }

