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
        public void Post([FromBody]string value)
        {
        }

        // GET api/courses/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
    }
}
