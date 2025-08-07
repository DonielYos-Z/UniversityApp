using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityApp.Forms.DataModels
{
    internal class Course
    {
        // Class Course represents a course in the university's system
        // Attributes
        private String Name;
        private List<Student> Students;
        // Constructor
        public Course(string Name, List<Student> students)
        {
            this.Name = Name;
            this.Students = students ?? new List<Student>();
        }
        // Setters, Getters, and ToString()
        public void SetName(string name)
        {
            Name = name;
        }
        public string GetName()
        {
            return Name;
        }
        public void SetStudents(List<Student> students)
        {
            Students = students ?? new List<Student>();
        }
        public List<Student> GetStudents()
        {
            return Students;
        }
        public override string ToString()
        {
            return $"Course Name: {GetName()}, Students:\n\t{string.Join("\n\t", GetStudents())}";
        }
    }
}
