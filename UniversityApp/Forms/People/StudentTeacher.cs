using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityApp.Forms.DataModels
{
    // Extension-class StudentTeacher, extends from Student
    internal class StudentTeacher : Student
    {
        // Attributes
        private List<Course> CoursesTeaching;
        // Constructor
        public StudentTeacher(
            string id, string name, int age, string phoneNumber, string email, List<Message> messageList, string filepath,
            int studentNumber, StudyTrack studyTrack, string specialization, int CurrentCreditPoints, int TotalEarnedPoints,
            List<Course> coursesTeaching)
            : base(id, name, age, phoneNumber, email, messageList, filepath,
                studentNumber, studyTrack, specialization, CurrentCreditPoints, TotalEarnedPoints)
        {
            SetCoursesTeaching(coursesTeaching);
        }
        // Setters, Getters, and ToString()
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
        public override string ToString()
        {
            return base.ToString() + $",\nCourses Teaching:\n\t{string.Join("\n\t", GetCoursesTeaching())}";
        }
        // No need to call the abstract method since a call is inherited from Student
        // Reminder: That method returns a string of the courses in the student's study track

        // This method returns a string of the courses being taught by the student teacher and the students enrolled in each course
        public string DisplayStudentsTeaching()
        {
            if (CoursesTeaching == null || CoursesTeaching.Count == 0)
            {
                return "No courses being taught.";
            }
            StringBuilder sb = new StringBuilder();
            foreach (var course in CoursesTeaching)
            {
                // Only saves the name of the copurses to avoid loops or annoyingly long strings that are difficult to read
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
