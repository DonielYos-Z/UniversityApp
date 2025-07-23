using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityApp.Forms.DataModels
{
    // Super-class Student, extends from Person
    // When a student signs up for the university, they are saved in the system with all the details they gave the university, including personal info, a picture, and which study track they'll be taking.
    // When the student registers via Register.cs, the details they enter there (specifically, their ID number) will be used by the HoD to find them in the system, so that he can approve their registration.
    internal class Student : Person
    {
        // Attributes
        protected int StudentNumber;
        protected StudyTrack StudyTrack;
        protected String Specialization;
        protected int CurrentCreditPoints;
        protected int TotalEarnedPoints;
        // Constructor
        protected Student(
            string id, string name, int age, string phoneNumber, string email, List<Message> messageList, string filepath,
            int studentNumber, StudyTrack studyTrack, string specialization, int currentCreditPoints, int totalEarnedPoints)
            : base(id, name, age, phoneNumber, email, messageList, filepath)
        {
            SetStudentNumber(studentNumber);
            SetStudyTrack(studyTrack);
            SetSpecialization(specialization);
            SetCurrentCreditPoints(currentCreditPoints);
            SetTotalEarnedPoints(totalEarnedPoints);
        }
        // Setters, Getters, and ToString()
        public void SetStudentNumber(int studentNumber)
        {
            if (studentNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(studentNumber), "Student number must be positive.");
            StudentNumber = studentNumber;
        }
        public int GetStudentNumber()
        {
            return StudentNumber;
        }
        public void SetStudyTrack(StudyTrack studyTrack)
        {
            if (studyTrack == null)
                throw new ArgumentNullException(nameof(studyTrack), "Study track cannot be null.");
            StudyTrack = studyTrack;
        }
        public StudyTrack GetStudyTrack()
        {
            return StudyTrack;
        }
        public void SetSpecialization(string specialization)
        {
            if (string.IsNullOrWhiteSpace(specialization))
                throw new ArgumentException("Specialization cannot be empty.", nameof(specialization));
            Specialization = specialization;
        }
        public string GetSpecialization()
        {
            return Specialization;
        }
        public void SetCurrentCreditPoints(int currentCreditPoints)
        {
            if (currentCreditPoints < 0)
                throw new ArgumentOutOfRangeException(nameof(currentCreditPoints), "Credit points cannot be negative.");
            CurrentCreditPoints = currentCreditPoints;
        }
        public int GetCurrentCreditPoints()
        {
            return CurrentCreditPoints;
        }
        public void SetTotalEarnedPoints(int totalEarnedPoints)
        {
            if (totalEarnedPoints < 0)
                throw new ArgumentOutOfRangeException(nameof(totalEarnedPoints), "Total earned points cannot be negative.");
            TotalEarnedPoints = totalEarnedPoints;
        }
        public int GetTotalEarnedPoints()
        {
            return TotalEarnedPoints;
        }
        public override string ToString()
        {
            // ToString only contains the name of the study track to avoid loops or annoyingly long strings that are difficult to read
            return base.ToString() +
                $", Student Number: {GetStudentNumber()}, Study Track: {GetStudyTrack().GetName()}, Specialization: {GetSpecialization()},"+
                $"Current Credit Points: {GetCurrentCreditPoints()}, Total Earned Points: {GetTotalEarnedPoints()}";
        }
        // Call to abstract method from super-duper-class
        // This method returns a string of the courses in the student's study track
        public override string DisplayCourses()
        {
            // DisplayCourses only returns the names of the courses in the study track for the reasons mentioned in ToString (above)
            if (StudyTrack != null && StudyTrack.GetCourses() != null && StudyTrack.GetCourses().Count > 0)
            {
                return $"Courses learning: {string.Join(", ", StudyTrack.GetCourses().Select(course => course.GetName()))}";
            }
            return "No courses available.";
        }
    }
}