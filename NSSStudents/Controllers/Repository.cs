using NSSStudents.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NSSStudents.Controllers
{
    static public class Repository
    {
        static public async Task<List<Student>> GetStudents(
            SqlConnection conn, 
            int cohortId = 0, 
            string firstName = "",
            string lastName = ""
            )
        {
            using (conn)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    
                    if (cohortId != 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@cohortId", cohortId));
                        cmd.CommandText = @"
                        SELECT
                            s.Id,
                            s.CohortId,
                            s.FirstName,
                            s.LastName,
                            e.ExerciseName,
                            e.ExerciseLanguage,
                            s.SlackHandle FROM Student s
                        WHERE CohortId = @cohortId
                        AND LastName LIKE '%' + @lastName + '%'
                        AND FirstName LIKE '%' + @firstName + '%'
                        LEFT JOIN StudentExercise se ON s.Id = se.StudentId
                        LEFT JOIN Exercise e on e.Id = se.ExerciseId";
                    }
                    else cmd.CommandText = @"
                        SELECT
                            s.Id,
                            s.CohortId,
                            s.FirstName,
                            s.LastName,
                            c.CohortName,
                            e.ExerciseName,
                            e.ExerciseLanguage,
                            s.SlackHandle FROM Student s
                        WHERE LastName LIKE '%' + @lastName + '%'
                        AND FirstName LIKE '%' + @firstName + '%'
                        LEFT JOIN Cohort c ON s.CohortId = c.Id
                        LEFT JOIN StudentExercise se ON s.Id = se.StudentId
                        LEFT JOIN Exercise e on e.Id = se.ExerciseId";

                    cmd.Parameters.Add(new SqlParameter("@firstName", firstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", lastName));

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<Student> students = new List<Student>();

                    while (await reader.ReadAsync())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("Id"));
                        int foundStudentId = students.FindIndex(s => s.Id == id);
                        if (foundStudentId == -1)
                        {

                        }

                        Student student = new Student()
                        {
                            Id = id,
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle"))
                        };
                        if (cohortId == 0) student.Cohort = new Cohort()
                        {
                            CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                        };

                        if (reader.GetString(reader.GetOrdinal("ExerciseName")) != null)
                        {
                            student.Exercises.Add(new Exercise()
                            {
                                ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                ExerciseLanguage = reader.GetString(reader.GetOrdinal("ExerciseLanguage"))
                            });

                        }

                        students.Add(student);
                    }
                    reader.Close();

                    return students;
                }
            }


        }

        static public async Task<List<Instructor>> GetInstructors(SqlConnection conn, int cohortId = 0)
        {
            using (conn)
            {

                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (cohortId != 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@cohortId", cohortId));
                        cmd.CommandText = @"
                        SELECT
                            Id,
                            CohortId,
                            FirstName,
                            LastName,
                            Specialty,
                            SlackHandle FROM Instructor
                        WHERE CohortId = @cohortId";
                    }
                    else cmd.CommandText = @"
                        SELECT
                            i.Id,
                            i.CohortId,
                            i.FirstName,
                            i.LastName,
                            c.CohortName
                            i.Specialty
                            i.SlackHandle FROM Instructor i
                        LEFT JOIN Cohort c ON i.CohortId = c.Id";

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
                        if (cohortId == 0) instructor.Cohort = new Cohort()
                        {
                            CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                        };

                        instructors.Add(instructor);
                    }
                    reader.Close();

                    return instructors;
                }
            }

        }


        static public async Task<bool> ItemExists(string itemName, int id, SqlConnection conn)
        {
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id
                                            FROM @itemName
                                            WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@itemName", itemName));
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    return await reader.ReadAsync();
                }
            }
        }
    }
}
