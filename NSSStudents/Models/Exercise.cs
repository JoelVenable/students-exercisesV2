using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NSSStudents.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string ExerciseName { get; set; }
        [MaxLength(50)]

        public string ExerciseLanguage { get; set; }
    }
}
