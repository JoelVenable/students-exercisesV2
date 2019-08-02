//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Data;
//using System.Linq;
//using System.Threading.Tasks;
//using NSSStudents.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;

//namespace CoffeeShop.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class StudentsController : ControllerBase
//    {
//        private readonly IConfiguration _config;

//        public StudentsController(IConfiguration config)
//        {
//            _config = config;
//        }

//        public SqlConnection Connection
//        {
//            get
//            {
//                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
//            }
//        }

//        [HttpGet]
//        public async Task<IActionResult> Get(
//            [FromQuery] string cohortName

//            )
//        {
//            if (cohortName == null) cohortName = "";


//            using (SqlConnection conn = Connection)
//            {
//                conn.Open();
//                using (SqlCommand cmd = conn.CreateCommand())
//                {
//                    cmd.CommandText = @"
//                        SELECT 
//                            c.Id, 
//                            c.CohortName, 
//                            s.FirstName, 
//                            s.LastName, 
//                            s.SlackHandle FROM Cohort c
//                        WHERE CohortName LIKE '%' + @cohortName + '%'
    
//                        LEFT JOIN Student s ON c.Id = s.CohortId";
//                    cmd.Parameters.Add(new SqlParameter("@cohortName", cohortName));

//                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
//                    List<Cohort> cohorts = new List<Cohort>();

//                    while (await reader.ReadAsync())
//                    {
//                        Cohort cohort = new Cohort()
//                        {
//                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
//                            CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
//                        };

//                        cohorts.Add(cohort);
//                    }
//                    reader.Close();

//                    return Ok(cohorts);
//                }
//            }
//        }

//        [HttpGet("{id}", Name = "GetCoffee")]
//        public async Task<IActionResult> Get([FromRoute] int id)
//        {
//            using (SqlConnection conn = Connection)
//            {
//                conn.Open();
//                using (SqlCommand cmd = conn.CreateCommand())
//                {
//                    Coffee coffee = null;
//                    cmd.CommandText = @"SELECT Id, Title, BeanType FROM Coffee WHERE Id = @id";
//                    cmd.Parameters.Add(new SqlParameter("@id", id));
//                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

//                    if (await reader.ReadAsync())
//                    {
//                        coffee = new Coffee
//                        {
//                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
//                            Title = reader.GetString(reader.GetOrdinal("Title")),
//                            BeanType = reader.GetString(reader.GetOrdinal("BeanType"))
//                        };


//                    }
//                    reader.Close();

//                    if (coffee == null) return NotFound($"Coffee with id {id} not found.");

//                    return Ok(coffee);
//                }
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> Post([FromBody] Student student)
//        {
//            using (SqlConnection conn = Connection)
//            {
//                conn.Open();
//                using (SqlCommand cmd = conn.CreateCommand())
//                {
//                    cmd.CommandText = @"INSERT INTO Coffee (Title, BeanType)
//                                        OUTPUT INSERTED.Id
//                                        VALUES (@title, @beanType)";
//                    cmd.Parameters.Add(new SqlParameter("@title", coffee.Title));
//                    cmd.Parameters.Add(new SqlParameter("@beanType", coffee.BeanType));

//                    try
//                    {
//                        var result = await cmd.ExecuteScalarAsync();
//                        int newId = (int)result;
//                        coffee.Id = newId;
//                        return CreatedAtRoute("GetCoffee", new { id = newId }, null);
//                    }
//                    catch (Exception ex)
//                    {
//                        //return StatusCode(500, $"An error occurred: \n{ex.Message}");
//                        return StatusCode(StatusCodes.Status422UnprocessableEntity);
//                    }

//                }
//            }
//        }


//        [HttpPut("{id}")]
//        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Coffee coffee)
//        {
//            try
//            {
//                using (SqlConnection conn = Connection)
//                {
//                    await conn.OpenAsync();
//                    using (SqlCommand cmd = conn.CreateCommand())
//                    {
//                        cmd.CommandText = @"UPDATE Coffee
//                                            SET Title = @title,
//                                                BeanType = @beanType
//                                            WHERE Id = @id";
//                        cmd.Parameters.Add(new SqlParameter("@title", coffee.Title));
//                        cmd.Parameters.Add(new SqlParameter("@beanType", coffee.BeanType));
//                        cmd.Parameters.Add(new SqlParameter("@id", id));

//                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
//                        if (rowsAffected > 0)
//                        {
//                            return new StatusCodeResult(StatusCodes.Status204NoContent);
//                        }
//                        throw new Exception("No rows affected.");

//                    }
//                }
//            }
//            catch (Exception)
//            {
//                if (!CoffeeExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete([FromRoute] int id)
//        {
//            try
//            {
//                using (SqlConnection conn = Connection)
//                {
//                    await conn.OpenAsync();
//                    using (SqlCommand cmd = conn.CreateCommand())
//                    {
//                        cmd.CommandText = @"DELETE FROM Coffee WHERE Id = @id";
//                        cmd.Parameters.Add(new SqlParameter("@id", id));

//                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
//                        if (rowsAffected > 0)
//                        {
//                            return new StatusCodeResult(StatusCodes.Status204NoContent);
//                        }
//                        throw new Exception("No rows affected");
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                if (!CoffeeExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }
//        }



//        private bool CoffeeExists(int id)
//        {
//            using (SqlConnection conn =Connection)
//            {
//                conn.Open();
//                using (SqlCommand cmd = conn.CreateCommand())
//                {
//                    cmd.CommandText = @"SELECT Id, Title, BeanType
//                                            FROM Coffee
//                                            WHERE Id = @id";
//                    cmd.Parameters.Add(new SqlParameter("@id", id));
//                    SqlDataReader reader = cmd.ExecuteReader();
//                    return reader.Read();
//                }
//            }
//        }
//    }
//}