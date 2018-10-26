using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DecisionWebApi.Exceptions;
using DecisionWebApi.Models;
using DecisionWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DecisionWebApi.Controllers
{
    [Route("/api/[controller]")]
    public class CarsController : Controller
    {
        private readonly CarsService _service;

        public CarsController(CarsService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var car = await _service.Get(id);

            if (car == null)
            {
                return NotFound();
            }

            return Json(car);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var cars = await _service.GetAll();
            if (cars == null)
            {
                return NotFound();
            }

            return Json(cars);
        }

        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] JObject car)
        {
            try
            {
                await _service.Update(car);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return BadRequest();
            }
            catch (ArgumentNullException)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Car car)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }

            try
            {
                await _service.Create(car);
            }
            catch (EntityExistException)
            {
                return BadRequest();
            }

            return CreatedAtAction("Create", car);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.Delete(id);
                return NoContent();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}