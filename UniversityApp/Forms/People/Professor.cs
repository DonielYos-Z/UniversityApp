using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace UniversityApp.Forms.DataModels
{
    // Super-class Professor, extends from Person
    // When a professor is hired by the university, they are saved in the system with all the details they gave the university, including personal info, a picture, and which courses they'll be teaching.
    // When the professor registers via Register.cs, the details they enter there (specifically, their ID number) will be used by the HoD to find them in the system, so that he can approve their registration.
    internal class Professor:Person
    {
        // Attributes
        protected int EmployeeNumber;
        protected List<Course> CoursesTeaching;
        protected String Specialization;
        protected Double RatingFromStudents;
        // Constructor
        public Professor(
            string id, string name, int age, string phoneNumber, string email, List<Message> messageList, string filepath,
            int EmployeeNumber, List<Course> coursesTeaching, string specialization, double ratingFromStudents)
            : base(id, name, age, phoneNumber, email, messageList, filepath)
        {
            SetEmployeeNumber(EmployeeNumber);
            SetCoursesTeaching(coursesTeaching);
            SetSpecialization(specialization);
            SetRatingFromStudents(ratingFromStudents);
        }
        // Setters, Getters, and ToString()
        public void SetEmployeeNumber(int employeeNumber)
        {
            if (employeeNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(employeeNumber), "Employee number must be positive.");
            EmployeeNumber = employeeNumber;
        }
        public int GetEmployeeNumber()
        {
            return EmployeeNumber;
        }
        public void SetCoursesTeaching(List<Course> coursesTeaching)
        {
            if (coursesTeaching == null)
                throw new ArgumentNullException(nameof(coursesTeaching), "CoursesTeaching list cannot be null.");
            CoursesTeaching = coursesTeaching;
        }
        public List<Course> GetCoursesTeaching()
        {
            return CoursesTeaching;
        }
        public void SetSpecialization(string specialization)
        {
            if (string.IsNullOrWhiteSpace(specialization))
                throw new ArgumentException("Specialization cannot be null or empty.", nameof(specialization));
            Specialization = specialization;
        }
        public string GetSpecialization()
        {
            return Specialization;
        }
        public void SetRatingFromStudents(double ratingFromStudents)
        {
            if (ratingFromStudents < 0.0 || ratingFromStudents > 5.0)
                throw new ArgumentOutOfRangeException(nameof(ratingFromStudents), "Rating must be between 0.0 and 5.0.");
            RatingFromStudents = ratingFromStudents;
        }
        public double GetRatingFromStudents()
        {
            return RatingFromStudents;
        }
        public override string ToString()
        {
            return base.ToString() +
                $", Employee Number: {GetEmployeeNumber()}, Specialization: {GetSpecialization()}," +
                $"Rating: {GetRatingFromStudents()}, Courses:\n\t{string.Join("\n\n\t", GetCoursesTeaching())}";
        }
        // Abstract method inherited from Person
        // This method returns a string of the courses being taught by the professor and the students enrolled in each course
        public override string DisplayCourses()
        {
            if (CoursesTeaching == null || CoursesTeaching.Count == 0)
            {
                return "No courses being taught.";
            }
            StringBuilder sb = new StringBuilder();
            foreach (var course in CoursesTeaching)
            {
                // Only saves the name of the courses to avoid loops or annoyingly long strings that are difficult to read
                sb.AppendLine($"Course: {course.GetName()}");
                if (course.GetStudents() != null && course.GetStudents().Count > 0)
                {
                    // Only saves the name of the students for reasons mentioned above
                    sb.AppendLine($"\tStudents: {string.Join(", ", course.GetStudents().Select(s => s.GetName()))}");
                }
                else
                {
                    sb.AppendLine("\tNo students enrolled.");
                }
            }
            return sb.ToString();
        }
    }
}
