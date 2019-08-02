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
        private IConfiguration _config;

        public StudentsController(IConfiguration config) => _config = config;

        public SqlConnection Connection => new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string cohortName

            )
        {
            if (cohortName == null) cohortName = "";


            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT 
                            c.Id, 
                            c.CohortName FROM Cohort c
                        WHERE CohortName LIKE '%' + @cohortName + '%'";
                    cmd.Parameters.Add(new SqlParameter("@cohortName", cohortName));

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<Cohort> cohorts = new List<Cohort>();

                    while (await reader.ReadAsync())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("Id"));
                        Cohort cohort = new Cohort()
                        {
                            Id = id,
                            CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                            Students = await GetStudentsFromCohortId(id),
                            Instructors = await GetInstructorsFromCohortId(id),

                        };

                        cohorts.Add(cohort);
                    }
                    reader.Close();

                    //cohorts.ForEach(async cohort => cohort.Students = await GetStudentsFromCohortId(cohort.Id));
                    //cohorts.ForEach(async cohort => cohort.Instructors = await GetInstructorsFromCohortId(cohort.Id, conn));


                    return Ok(cohorts);
                }
            }
        }


        public async Task<List<Student>> GetStudentsFromCohortId(int cohortId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT 
                            Id, 
                            CohortId, 
                            FirstName, 
                            LastName, 
                            SlackHandle FROM Student
                        WHERE CohortId = @cohortId";
                    cmd.Parameters.Add(new SqlParameter("@cohortId", cohortId));

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<Student> students = new List<Student>();

                    while (await reader.ReadAsync())
                    {
                        Student student = new Student()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle"))

                        };

                        students.Add(student);
                    }
                    reader.Close();

                    return students;
                }
            }


        }

        public async Task<List<Instructor>> GetInstructorsFromCohortId(int cohortId)
        {
            using (SqlConnection conn = Connection)
            {

                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"
                        SELECT 
                            Id, 
                            CohortId, 
                            FirstName, 
                            LastName, 
                            Specialty,
                            SlackHandle FROM Instructor
                        WHERE CohortId = @cohortId";
                    cmd.Parameters.Add(new SqlParameter("@cohortId", cohortId));

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<Instructor> instructors = new List<Instructor>();

                    while (await reader.ReadAsync())
                    {
                        Instructor instructor = new Instructor()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty"))


                        };

                        instructors.Add(instructor);
                    }
                    reader.Close();

                    return instructors;
                }
            }

        }

        [HttpGet("{id}", Name = "GetCoffee")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        SELECT 
                            Id, 
                            CohortName FROM Cohort
                        WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        List<Cohort> cohorts = new List<Cohort>();

                        while (await reader.ReadAsync())
                        {
                            Cohort cohort = new Cohort()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                            };

                            cohorts.Add(cohort);
                        }
                        reader.Close();

                        //cohorts.ForEach(async cohort => cohort.Students = await GetStudentsFromCohortId(cohort.Id, conn));
                        //cohorts.ForEach(async cohort => cohort.Instructors = await GetInstructorsFromCohortId(cohort.Id, conn));


                        return Ok(cohorts);
                    }
                }
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> Post([FromBody] Student student)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"INSERT INTO Coffee (Title, BeanType)
        //                                OUTPUT INSERTED.Id
        //                                VALUES (@title, @beanType)";
        //            cmd.Parameters.Add(new SqlParameter("@title", coffee.Title));
        //            cmd.Parameters.Add(new SqlParameter("@beanType", coffee.BeanType));

        //            try
        //            {
        //                var result = await cmd.ExecuteScalarAsync();
        //                int newId = (int)result;
        //                coffee.Id = newId;
        //                return CreatedAtRoute("GetCoffee", new { id = newId }, null);
        //            }
        //            catch (Exception ex)
        //            {
        //                //return StatusCode(500, $"An error occurred: \n{ex.Message}");
        //                return StatusCode(StatusCodes.Status422UnprocessableEntity);
        //            }

        //        }
        //    }
        //}


        //[HttpPut("{id}")]
        //public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Coffee coffee)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = Connection)
        //        {
        //            await conn.OpenAsync();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"UPDATE Coffee
        //                                    SET Title = @title,
        //                                        BeanType = @beanType
        //                                    WHERE Id = @id";
        //                cmd.Parameters.Add(new SqlParameter("@title", coffee.Title));
        //                cmd.Parameters.Add(new SqlParameter("@beanType", coffee.BeanType));
        //                cmd.Parameters.Add(new SqlParameter("@id", id));

        //                int rowsAffected = await cmd.ExecuteNonQueryAsync();
        //                if (rowsAffected > 0)
        //                {
        //                    return new StatusCodeResult(StatusCodes.Status204NoContent);
        //                }
        //                throw new Exception("No rows affected.");

        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        if (!CoffeeExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete([FromRoute] int id)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = Connection)
        //        {
        //            await conn.OpenAsync();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"DELETE FROM Coffee WHERE Id = @id";
        //                cmd.Parameters.Add(new SqlParameter("@id", id));

        //                int rowsAffected = await cmd.ExecuteNonQueryAsync();
        //                if (rowsAffected > 0)
        //                {
        //                    return new StatusCodeResult(StatusCodes.Status204NoContent);
        //                }
        //                throw new Exception("No rows affected");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (!CoffeeExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}



        //private bool CoffeeExists(int id)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"SELECT Id, Title, BeanType
        //                                    FROM Coffee
        //                                    WHERE Id = @id";
        //            cmd.Parameters.Add(new SqlParameter("@id", id));
        //            SqlDataReader reader = cmd.ExecuteReader();
        //            return reader.Read();
        //        }
        //    }
        //}
    }
}