using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace UniversityApp.Forms.DataModels
{
    // Extension-class HeadOfDepartment, extends from Professor
    internal class HeadOfDepartment:Professor
    {
        // Attributes
        private List<StudyTrack> TracksManaging;
        private List<Person> Supervising;
        private List<Student> StudentsInDepartment;
        // Constructor
        public HeadOfDepartment(
            string id, string name, int age, string phoneNumber, string email, List<Message> messageList, string filepath,
            int EmployeeNumber, List<Course> coursesTeaching, string specialization, double ratingFromStudents,
            List<StudyTrack> tracksManaging, List<Person> supervising, List<Student> studentsInDepartment)
            : base(id, name, age, phoneNumber, email, messageList, filepath,
                EmployeeNumber, coursesTeaching, specialization, ratingFromStudents)
        {
            SetTracksManaging(tracksManaging);
            SetSupervising(supervising);
            SetStudentsInDepartment(studentsInDepartment);
        }
        // Setters, Getters, and ToString()
        public void SetTracksManaging(List<StudyTrack> tracksManaging)
        {
            if (tracksManaging == null)
                throw new ArgumentNullException(nameof(tracksManaging), "TracksManaging list cannot be null.");
            TracksManaging = tracksManaging;
        }
        public List<StudyTrack> GetTracksManaging()
        {
            return TracksManaging;
        }
        public void SetSupervising(List<Person> supervising)
        {
            // To set the supervising list, it must not be null, and every person in the list must be either a StudentTeacher or Professor (not a HeadOfDepartment)
            if (supervising == null)
                throw new ArgumentNullException(nameof(supervising), "Supervising list cannot be null.");
            foreach (Person p in supervising)
            {
                if (!(p is StudentTeacher || p is Professor) || p is HeadOfDepartment)
                {
                    throw new ArgumentException("Supervising can only be a Student Teacher or Professor.");
                }
            }
            Supervising = supervising;
        }
        public List<Person> GetSupervising()
        {
            return Supervising;
        }
        public void SetStudentsInDepartment(List<Student> studentsInDepartment)
        {
            if (studentsInDepartment == null)
                throw new ArgumentNullException(nameof(studentsInDepartment), "StudentsInDepartment list cannot be null.");
            StudentsInDepartment = studentsInDepartment;
        }
        public List<Student> GetStudentsInDepartment()
        {
            return StudentsInDepartment;
        }
        public override string ToString()
        {
            return base.ToString()+
                $"Tracks Managing:\n\t{ string.Join("\n\t", GetTracksManaging())},\n" +
                $"Supervising:\n\t{string.Join("\n\t", GetSupervising())},\n" +
                $"Students in Department:\n\t{string.Join("\n\t", GetStudentsInDepartment())}";
        }
        // No need to call the abstract method since a call is inherited from Professor
        // Reminder: That method returns a string of the courses being taught (not managed) by the professor and the students enrolled in each course

        /* --- Track Management ---
         * These methods display all of the tracks the Head of Department is managing (name only to prevent loops or annoyingly long text, 
         * as well as to preserve the usefulness of other methods), add a track to the list of tracks being managed by him,
         * remove a track from that list, and update an existing track, swapping it out with a different one
        */
        public string DisplayTracksManaging()
        {
            // This method displays all of the tracks being managed by the HoD, returning a string of their names
            // If there are no tracks being managed, return a message indicating such
            if (TracksManaging == null || TracksManaging.Count == 0)
                return "No tracks being managed.";
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (var track in TracksManaging)
                // Only saves the name of the track to avoid loops or annoyingly long strings that are difficult to read
                sb.AppendLine("#"+count+": "+track.GetName());
            return sb.ToString();
        }
        public void AddTrack(StudyTrack track)
        {
            // Top add a track, it must not be null and it must not already exist in the list of tracks being managed
            if (track != null && !TracksManaging.Contains(track))
                TracksManaging.Add(track);
            else if (track == null)
                throw new ArgumentNullException(nameof(track), "Track cannot be null.");
            else
                throw new ArgumentException("Track already exists in the list of tracks being managed.");
        }
        public void RemoveTrack(StudyTrack track)
        {
            // To remove a track, it must not be null, no student can be taking it, and it must exist in the list of tracks being managed
            bool hasStudents = false;
            foreach (Student s in StudentsInDepartment)
            {
                if(s.GetStudyTrack() == track)
                {
                    hasStudents = true;
                    break;
                }
            }
            if (hasStudents)
            {
                throw new ArgumentNullException(nameof(track), "Track still has students.");
            }
            else if (track != null && TracksManaging.Contains(track))
                TracksManaging.Remove(track);
            else if (track == null)
                throw new ArgumentNullException(nameof(track), "Track cannot be null.");
            else
                throw new ArgumentException("Track not found in the list of tracks being managed.");
        }
        public void UpdateTrack(StudyTrack oldTrack, StudyTrack newTrack)
        {
            // To update a track, both the old and new tracks must not be null, and the old track must exist in the list of tracks being managed
            if (oldTrack == null)
                throw new ArgumentNullException(nameof(oldTrack), "Old track cannot be null.");
            if (newTrack == null)
                throw new ArgumentNullException(nameof(newTrack), "New track cannot be null.");
            int idx = TracksManaging.IndexOf(oldTrack);
            if (idx >= 0)
                TracksManaging[idx] = newTrack;
            else
                throw new ArgumentException("Old track not found in the list of tracks being managed.");
        }
        /* --- Course Management ---
         * These methods display all of the courses in the tracks the Head of Department is managing (name only for the reasons metioned above),
         * add a course to a specified track, remove a course from a specified track,
         * and update a specific course in a specified track, swapping it out for a different course
        */
        public string DisplayCoursesManaging()
        {
            // This method displays all of the courses in the tracks being managed by the HoD, returning a string of their names
            // If there are no tracks being managed, return a message indicating such
            if (TracksManaging == null || TracksManaging.Count == 0)
                return "No tracks being managed.";
            StringBuilder sb = new StringBuilder();
            foreach (var track in TracksManaging)
            {
                // Only saves the name of the track/courses to avoid loops or annoyingly long strings that are difficult to read
                sb.AppendLine($"Courses in {track.GetName()}: ");
                // If the track has courses, display their names, otherwise display a message indicating no courses in the track
                if (track.GetCourses() != null && track.GetCourses().Count > 0)
                    sb.AppendLine($"{string.Join(", ", track.GetCourses().Select(c => c.GetName()))}");
                else
                    sb.AppendLine("No courses in this track.");
            }
            return sb.ToString();
        }
        public void AddCourseToTrack(StudyTrack track, Course course)
        {
            // To add a course to a track, both the track and course must not be null, and the course must not already exist in the track's course list
            if (track != null && course != null && !track.GetCourses().Contains(course))
            {
                track.GetCourses().Add(course);
                // For each student enrolled in the track...
                foreach (Student student in track.GetStudents())
                {
                    // If the student is not already enrolled in the course, enroll them in it
                    if (!course.GetStudents().Contains(student))
                        course.GetStudents().Add(student);
                }
            }
            else if (track == null)
                throw new ArgumentNullException(nameof(track), "Track cannot be null.");
            else if (course == null)
                throw new ArgumentNullException(nameof(course), "Course cannot be null.");
            else
                throw new ArgumentException("This course already exists in the track's course list.");
        }
        public void RemoveCourseFromTrack(StudyTrack track, Course course)
        {
            // To remove a course from a track, both the track and course must not be null, and the course must exist in the track's course list
            if (track != null && course != null && track.GetCourses().Contains(course))
                track.GetCourses().Remove(course);
            else if (track == null)
                throw new ArgumentNullException(nameof(track), "Track cannot be null.");
            else if (course == null)
                throw new ArgumentNullException(nameof(course), "Course cannot be null.");
            else
                throw new ArgumentException("This course is not found in the track's course list.");
        }
        public void UpdateCourseInTrack(StudyTrack track, Course oldCourse, Course newCourse)
        {
            // To update a course in a track, both the old and new courses must not be null, the track must not be null,
            // and the old course must exist in the track's course list
            if (track == null)
                throw new ArgumentNullException(nameof(track), "Track cannot be null.");
            if (oldCourse == null)
                throw new ArgumentNullException(nameof(oldCourse), "Old course cannot be null.");
            if (newCourse == null)
                throw new ArgumentNullException(nameof(newCourse), "New course cannot be null.");
            if (TracksManaging.Contains(track))
            { 
                var courses = track.GetCourses();
                int idx = courses.IndexOf(oldCourse);
                if (idx >= 0)
                    courses[idx] = newCourse;
            }
            else
                throw new ArgumentException("Track not found in the list of tracks being managed.");
        }
        /* --- Teacher Management ---
         * These methods display all of the teachers (student teachers and professors) the Head of Department is supervising
         * (name and the courses they are teaching), add a teacher to the list of teachers they are supervising,
         * remove a teacher from the list, and update a specific teacher, swapping it out for a different teacher.
         * Teachers are added by an outside source by the university itself, not the HoD.
        */
        public string DisplayTeachersSupervising()
        {
            // This method displays all of the teachers being supervised by the HoD, returning a string of their names, id, and courses they are teaching
            // If there are no teachers being supervised, return a message indicating such
            if (Supervising == null || Supervising.Count == 0)
                return ("No teachers being supervised.");
            StringBuilder sb = new StringBuilder();
            foreach (var teacher in Supervising)
            {
                // Only saves the name and id of the teacher/course to avoid loops or annoyingly long strings that are difficult to read
                // Notes if teacher is a professor or student teacher
                if (teacher is Professor prof)
                {
                    sb.AppendLine($"Professor: {teacher.GetName()}, {teacher.GetID()}");
                    sb.AppendLine($"Courses Teaching: {string.Join(", ", prof.GetCoursesTeaching().Select(c => c.GetName()))}");
                }
                else if (teacher is StudentTeacher st)
                {
                    sb.AppendLine($"Student Teacher: {teacher.GetName()}, {teacher.GetID()}");
                    sb.AppendLine($"Courses Teaching: {string.Join(", ", st.GetCoursesTeaching().Select(c => c.GetName()))}");
                }
            }
            return (sb.ToString());
        }
        //The HoD doesn't hire teachers or assigns them to courses. That is the university's job. He does, however, approve the teachers to their position.
        public void ApproveTeacher(string idNum)
        {
            // To approve a teacher, the ID number must not be null, and the teacher must exist in the list of teachers being managed with a pending registration
            if (idNum == null)
                throw new ArgumentNullException(nameof(idNum), "ID cannot be null.");
            else
            {
                // Make sure the user file exists, and read it to find the teacher with the given ID number
                // If the file doesn't exist, return an appropriate message
                var path = "users.txt";
                if (!File.Exists(path))
                {
                    MessageBox.Show("User file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var lines = File.ReadAllLines(path).ToList();
                bool modified = false;
                for (int i = 0; i < lines.Count; i++)
                {
                    // Once the teacher is found, replace the "Approval: Pending" with "Approval: Approved"
                    // Add the HoD's ID to have a way for the Main App to have access to the object that represents the teacher in the university's system
                    // Add a slot for the date. This will be used to track when the teacher was approved, and can be used for future reference or statistics.
                    if (lines[i].Contains($"ID: {idNum}") && (lines[i].Contains($"Profession: Professor") || lines[i].Contains($"Profession: Student Teacher")) && lines[i].Contains("Approval: Pending"))
                    {
                        bool exists = false;
                        foreach (Person t in Supervising)
                        {
                            if(t.GetID() == idNum)
                            {
                                exists = true;
                                lines[i] = lines[i].Replace("Approval: Pending", $"Approval: Approved, Head of Department: {this.GetID()}, Date of Last Login: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                                MessageBox.Show($"Teacher {t.GetName()} has been approved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }
                        }
                        if (exists)
                        {
                            modified = true;
                            break;
                        }
                        else
                        {
                            MessageBox.Show("This teacher is not in the system.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                // If no teacher with the given ID was found, return a message indicating such
                if (!modified)
                {
                    MessageBox.Show("No pending registration found for that ID.");
                    return;
                }
                // Save changes to file
                File.WriteAllLines(path, lines);
            }
        }
        public void RemoveTeacher(Person teacher)
        {
            // To remove a teacher, it must not be null and it must exist in the list of teachers the HoD is supervising
            if (teacher != null && Supervising.Contains(teacher))
                Supervising.Remove(teacher);
            else if (teacher == null)
                throw new ArgumentNullException(nameof(teacher), "Teacher cannot be null.");
            else
                throw new ArgumentException("Teacher not found in the list of supervising teachers.");
        }
        public void UpdateTeacher(Person oldTeacher, Person newTeacher)
        {
            // To update a teacher, both the old and new teachers must not be null, the new teacher must be a non-HoD Professor or StudentTeacher,
            // and the old teacher must exist in the list of teachers the HoD is supervising
            if (oldTeacher == null)
                throw new ArgumentNullException(nameof(oldTeacher), "Old teacher cannot be null.");
            if (newTeacher == null)
                throw new ArgumentNullException(nameof(newTeacher), "New teacher cannot be null.");
            if (!(newTeacher is Professor || newTeacher is StudentTeacher) || newTeacher is HeadOfDepartment)
                throw new ArgumentException("New teacher must be a Professor or Student Teacher, and cannot be a Head of Department.");
            if (Supervising == null)
                throw new ArgumentNullException(nameof(Supervising), "Supervising list cannot be null.");
            if(!Supervising.Contains(oldTeacher))
                throw new ArgumentException("Old teacher not found in the list of supervising teachers.");
            else
            {
                int idx = Supervising.IndexOf(oldTeacher);
                if (idx >= 0)
                    Supervising[idx] = newTeacher;
            }
        }
        /* --- Student Management ---
         * These methods display all of the students in the Head of Department's department (name and the tracks they are learning),
         * approve an accuont of a student in that list of students in the HoF's department, remove a student from that list, 
         * and update a specific student, swapping it out for a different student (to change any details or make the student a student teacher).
         * Students are added by an outside source by the university itself, not the HoD.
        */
        public string DisplayStudentsInDepartment()
        {
            // This method displays all of the students in the HoD's department, returning a string of their names, id, and study tracks
            // If there are no students in the department, return a message indicating such
            if (StudentsInDepartment == null || StudentsInDepartment.Count == 0)
                return "No students in the department.";
            StringBuilder sb = new StringBuilder();
            // Only saves the name and id of the student/track to avoid loops or annoyingly long strings that are difficult to read
            foreach (var student in StudentsInDepartment)
            {
                sb.AppendLine($"Student: {student.GetName()}, {student.GetID()}");
                sb.AppendLine($"Track: {student.GetStudyTrack().GetName()}");
            }
            return sb.ToString();
        }
        public void ApproveStudent(string idNum)
        {
            // To approve a student, the ID number must not be null, and the student must exist in the list of students in the department with a pending registration
            if (idNum == null)
                throw new ArgumentNullException(nameof(idNum), "ID cannot be null.");
            else
            {
                // Make sure the user file exists, and read it to find the student with the given ID number
                // If the file doesn't exist, return an appropriate message
                var path = "users.txt";
                if (!File.Exists(path))
                {
                    MessageBox.Show("User file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var lines = File.ReadAllLines(path).ToList();
                bool modified = false;
                for (int i = 0; i < lines.Count; i++)
                {
                    // Once the student is found, replace the "Approval: Pending" with "Approval: Approved"
                    // Add the HoD's ID to have a way for the Main App to have access to the object that represents the student in the university's system
                    // Add a slot for the date. This will be used to track when the student was approved, and can be used for future reference or statistics.
                    if (lines[i].Contains($"ID: {idNum}") && lines[i].Contains($"Profession: Student") && lines[i].Contains("Approval: Pending"))
                    {
                        bool exists = false;
                        foreach (Student s in StudentsInDepartment)
                        {
                            if (s.GetID() == idNum)
                            {
                                exists = true;
                                lines[i] = lines[i].Replace("Approval: Pending", $"Approval: Approved, Head of Department: {this.GetID()}, Date of Last Login: {DateTime.Now:dd/MM/yyyy HH:mm:ss}, Study Track: {s.GetStudyTrack()}");
                                MessageBox.Show($"Student {s.GetName()} has been approved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }
                        }
                        if (exists)
                        {
                            modified = true;
                            break;
                        }
                        else
                        {
                            MessageBox.Show("This student is not in the system.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                // If no student with the given ID was found, return a message indicating such
                if (!modified)
                {
                    MessageBox.Show("No pending registration found for that ID.");
                    return;
                }
                // Save changes to file
                File.WriteAllLines(path, lines);
            }
        }
        public void RemoveStudent(Student student)
        {
            // To remove a student, it must not be null and it must exist in the list of students in the department
            if (student != null && StudentsInDepartment.Contains(student))
            {
                // Remove the student from all courses in their study track, and then remove them from the department
                RemoveStudentFromCourses(student);
                StudentsInDepartment.Remove(student);
            }
            else if (student == null)
                throw new ArgumentNullException(nameof(student), "Student cannot be null.");
            else
                throw new ArgumentException("Student not found in the list of students in the department.");
        }
        public void UpdateStudent(Student oldStudent, Student newStudent)
        {
            // To update a student, both the old and new students must not be null, and the old student must exist in the list of students in the department
            if (oldStudent == null)
                throw new ArgumentNullException(nameof(oldStudent), "Old student cannot be null.");
            if (newStudent == null)
                throw new ArgumentNullException(nameof(newStudent), "New student cannot be null.");
            if (StudentsInDepartment == null)
                throw new ArgumentNullException(nameof(StudentsInDepartment), "StudentsInDepartment list cannot be null.");
            if (!StudentsInDepartment.Contains(oldStudent))
                throw new ArgumentException("Old student not found in the list of students in the department.");
            else
            {
                int idx = StudentsInDepartment.IndexOf(oldStudent);
                if (idx >= 0)
                    StudentsInDepartment[idx] = newStudent;
            }
        }
        // --- Assignment Methods ---
        // The following methods assigns pre-existing students/teachers to their tracks/courses, or re-assigns them to a different one.
        public void AssignStudentToTrack(Student student, StudyTrack track)
        {
            // To assign a student to a track, both the student and track must not be null, and the student must not yet be in the track's student list.
            if (student != null && track != null && !track.GetStudents().Contains(student))
            {
                track.GetStudents().Add(student);
                // If the student is already enrolled in a different track, remove them from that track and unassign them from their courses, and add them to their new track, assigning them to all courses in that track.
                if (student.GetStudyTrack() != track)
                {
                    RemoveStudentFromCourses(student);
                    student.GetStudyTrack().GetStudents().Remove(student);
                    student.SetStudyTrack(track);
                    AssignStudentToCourses(student);
                }
            }
            else if (student != null && track != null && track.GetStudents().Contains(student))
                throw new ArgumentException("This student is already enrolled in the track.");
            else if (student == null)
                throw new ArgumentNullException(nameof(student), "Student cannot be null.");
            else if (track == null)
                throw new ArgumentNullException(nameof(track), "Track cannot be null.");
            else if (student.GetStudyTrack() != track)
                throw new ArgumentException("The student is not enrolled in this track.");
            else
                throw new ArgumentException("This student is already enrolled in the track.");
        }
        public void AssignStudentToCourses(Student student)
        {
            // This method assigns a student to all courses in their study track.
            // Make sure the student isn't null
            if (student == null)
                throw new ArgumentNullException(nameof(student), "Student cannot be null.");
            else
            {
                // For each course in their StudyTrack...
                foreach (Course c in student.GetStudyTrack().GetCourses())
                {
                    // If they aren't already enrolled, enroll them in the course. Otherwise, move on to the next course.
                    if (!c.GetStudents().Contains(student))
                        c.GetStudents().Add(student);
                    else
                        continue;
                }
            }
        }
        public void RemoveStudentFromCourses(Student student)
        {
            // This method removes a student from all courses in their study track.
            // Make sure the student isn't null
            if (student == null)
                throw new ArgumentNullException(nameof(student), "Student cannot be null.");
            else
            {
                // For each course in their StudyTrack...
                foreach (Course c in student.GetStudyTrack().GetCourses())
                {
                    // If they haven't already been removed, remove them from the course. Otherwise, move on to the next course.
                    if (c.GetStudents().Contains(student))
                        c.GetStudents().Remove(student);
                    else
                        continue;
                }
            }
        }
        public void AssignTeacherToCourse(Person teacher, Course course)
        {
            if (teacher == null)
                throw new ArgumentNullException(nameof(teacher), "Teacher cannot be null.");
            if (course == null)
                throw new ArgumentNullException(nameof(course), "Course cannot be null.");
            if (teacher is Professor || teacher is StudentTeacher)
            {
                // If the teacher is head of the department, a professor, or a student teacher, add the course to the list of courses they teach, if it isn't already there
                // If it is already there, or the teacher isn't head of the department, a professor, or a student teacher, throw an appropriate exception.
                if (teacher is HeadOfDepartment hod)
                {
                    if (!hod.GetCoursesTeaching().Contains(course))
                        hod.GetCoursesTeaching().Add(course);
                    else
                        throw new ArgumentException("This course is already being taught by the head of the department.");
                }
                else if (teacher is Professor prof)
                {
                    if (!prof.GetCoursesTeaching().Contains(course))
                        prof.GetCoursesTeaching().Add(course);
                    else
                        throw new ArgumentException("This course is already being taught by this professor.");
                } 
                else if (teacher is StudentTeacher st)
                {
                    if(!st.GetCoursesTeaching().Contains(course))
                        st.GetCoursesTeaching().Add(course);
                    else
                        throw new ArgumentException("This course is already being taught by this student teacher.");
                }   
                else
                    throw new ArgumentException("Teacher must be head of the department, a professor, or a student teacher.");
            }
        }
    }
}