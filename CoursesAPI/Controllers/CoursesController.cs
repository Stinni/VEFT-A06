using System.Collections.Generic;
using A06.CoursesAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace A06.CoursesAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class CoursesController : Controller
    {
        private static List<Course> _courses = new List<Course>
        {
            new Course
            {
                Id = 1,
                Name = "Vefþjónustur",
                Semester = "20163"
            }
        };

        // GET api/courses
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetCourses()
        {
            return Ok(_courses);
        }

        // POST api/courses
        [HttpPost]
        public IActionResult Post([FromBody]Course model)
        {
            _courses.Add(model);
            var location = Url.Link("GetCourseById", new { id = model.Id });
            return new CreatedResult(location, model);
        }

        // GET api/courses/5
        [HttpGet]
        [Route("{id}", Name = "GetCourseById")]
        public IActionResult GetCourseById(int id)
        {
            foreach(var c in _courses)
            {
                if (c.Id == id)
                {
                    return Ok(c);
                }
            }
            return NotFound();
        }
    }
}
