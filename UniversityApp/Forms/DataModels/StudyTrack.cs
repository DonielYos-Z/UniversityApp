using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityApp.Forms.DataModels
{
    // Class StudyTrack represents a study track in the university's system
    internal class StudyTrack
    {
        // Attributes
        private String Name;
        private List<Course> Courses;
        private List<Student> Students;
        // Constructor
        public StudyTrack(string name, List<Course> courses, List<Student> students)
        {
            Name = name;
            Courses = courses ?? new List<Course>();
            Students = students ?? new List<Student>();
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
        public void SetCourses(List<Course> courses)
        {
            Courses = courses ?? new List<Course>();
        }
        public List<Course> GetCourses()
        {
            return Courses;
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
            return $"Track Name: {GetName()},\nCourses:\n\t{string.Join("\n\t", GetCourses())},\nStudents: \n\t{string.Join("\n\t", GetStudents())}";
        }
    }
}
