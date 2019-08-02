using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NSSStudents.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace NSSStudents.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public StudentsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string firstName,
            [FromQuery] string lastName


            )
        {
            if (firstName == null) firstName = "";
            if (lastName == null) lastName = "";



            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT 
                            s.Id, 
                            c.CohortName, 
                            s.CohortId,
                            s.FirstName, 
                            s.LastName, 
                            s.SlackHandle FROM Student s
                        WHERE FirstName LIKE '%' + @firstName + '%'
                        AND LastName LIKE '%' + @lastName + '%'
    
                        LEFT JOIN Cohort c ON s.CohortId = c.Id";
                    cmd.Parameters.Add(new SqlParameter("@firstName", firstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", lastName));


                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<Student> students = new List<Student>();

                    while (await reader.ReadAsync())
                    {
                        Student student = new Student()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort()
                            {
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId"))
                            }
                        };

                        students.Add(student);
                    }
                    reader.Close();

                    return Ok(students);
                }
            }
        }

        [HttpGet("{id}", Name = "GetStudent")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    Student student = null;
                    cmd.CommandText = @"
                        SELECT 
                            s.Id, 
                            c.CohortName, 
                            s.CohortId,
                            s.FirstName, 
                            s.LastName, 
                            s.SlackHandle FROM Student s
                        WHERE s.Id = @id
                        LEFT JOIN Cohort c ON s.CohortId = c.Id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        student = new Student()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort()
                            {
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId"))
                            }
                        };


                    }
                    reader.Close();

                    if (student == null) return NotFound($"Student with id {id} not found.");

                    return Ok(student);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Student student)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Student (FirstName, LastName, SlackHandle, CohortId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@firstName, @lastName, @slackHandle, @cohortId)";
                    cmd.Parameters.Add(new SqlParameter("@firstName", student.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", student.LastName));
                    cmd.Parameters.Add(new SqlParameter("@slackHandle", student.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@cohortId", student.CohortId));


                    try
                    {
                        var result = await cmd.ExecuteScalarAsync();
                        int newId = (int)result;
                        student.Id = newId;
                        return CreatedAtRoute("GetStudent", new { id = newId }, null);
                    }
                    catch (Exception ex)
                    {
                        //return StatusCode(500, $"An error occurred: \n{ex.Message}");
                        return StatusCode(StatusCodes.Status422UnprocessableEntity);
                    }

                }
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Student student)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                                UPDATE Student 
                                    SET FirstName = @firstName,
                                        LastName = @lastName,
                                        SlackHandle = @slackHandle,
                                        CohortId = @cohortId
                                    WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@firstName", student.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", student.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slackHandle", student.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@cohortId", student.CohortId));
                        cmd.Parameters.Add(new SqlParameter("@id", id));


                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected.");

                    }
                }
            }
            catch (Exception)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Student WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception ex)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }



        private bool StudentExists(int id)
        {
            using (SqlConnection conn =Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id
                                            FROM Student
                                            WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}