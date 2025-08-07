using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using UniversityApp.Forms.DataModels;
using Message = UniversityApp.Forms.DataModels.Message;
// List of example accounts:
// Student- Username: Doniel, Password: Zaner
// Professor- Username: Shahar, Password: IMEvil
// Hod- Username: Head, Password: HoD
namespace UniversityApp
{
    public partial class MainApp : Form
    {
        Form form;
        // Example fields (if you want to keep references)
        private Student student1, student2;
        private Professor professor;
        private StudentTeacher studentTeacher;
        private HeadOfDepartment hod;
        private Course course1, course2;
        private StudyTrack studyTrack;
        private void CreateExampleData()
        {
            // Create students
            student1 = new Student(
                id: "332497635",
                name: "Doniel Yosef Zaner",
                age: 23,
                phoneNumber: "058-409-2801",
                email: "dyz@gmail.com",
                messageList: new List<Message>(),
                filepath: "dyz.jpeg",
                studentNumber: 80085169,
                studyTrack: null, // will set after track is created
                specialization: "Mathematics",
                currentCreditPoints: 30,
                totalEarnedPoints: 60
            );

            student2 = new Student(
                id: "S002",
                name: "Bob",
                age: 21,
                phoneNumber: "987-654-3210",
                email: "bob@student.edu",
                messageList: new List<Message>(),
                filepath: "dyz.jpeg",
                studentNumber: 10000002,
                studyTrack: null, // will set after track is created
                specialization: "Physics",
                currentCreditPoints: 25,
                totalEarnedPoints: 50
            );

            // Create courses
            course1 = new Course("Python", new List<Student>());
            course2 = new Course("Java", new List<Student>());

            // Create study track and assign students and courses
            studyTrack = new StudyTrack(
                name: "Computer Science",
                courses: new List<Course> { course1, course2 },
                students: new List<Student> { student1, student2 }
            );

            // Set the study track for students
            student1.SetStudyTrack(studyTrack);
            student2.SetStudyTrack(studyTrack);

            // Add students to courses
            course1.SetStudents(new List<Student> { student1 });
            course2.SetStudents(new List<Student> { student2 });

            // Create Professor
            professor = new Professor(
                id: "123456789",
                name: "Evil Shahar",
                age: 45,
                phoneNumber: "555-1234",
                email: "evsh@haha.com",
                messageList: new List<Message>(),
                filepath: "satan.jpg",
                EmployeeNumber: 20001,
                coursesTeaching: new List<Course> { course1 },
                specialization: "Mathematics",
                ratingFromStudents: 4.8
            );

            // Create StudentTeacher
            studentTeacher = new StudentTeacher(
                id: "ST001",
                name: "Charlie",
                age: 23,
                phoneNumber: "555-5678",
                email: "charlie@student.edu",
                messageList: new List<Message>(),
                filepath: "satan.jpg",
                studentNumber: 10000003,
                studyTrack: studyTrack,
                specialization: "Mathematics",
                CurrentCreditPoints: 40,
                TotalEarnedPoints: 80,
                coursesTeaching: new List<Course> { course2 }
            );

            // Create HeadOfDepartment
            hod = new HeadOfDepartment(
                id: "666666666",
                name: "Head of Department",
                age: 55,
                phoneNumber: "555-0000",
                email: "HoD@gmail.com",
                messageList: new List<Message>(),
                filepath: "head.png",
                EmployeeNumber: 30001,
                coursesTeaching: new List<Course>(), // HoD may or may not teach courses
                specialization: "STEM",
                ratingFromStudents: 4.9,
                tracksManaging: new List<StudyTrack> { studyTrack },
                supervising: new List<Person> { professor, studentTeacher },
                studentsInDepartment: new List<Student> { student1, student2 }
            );

            // Optionally, assign teachers to courses in the HoD's context
            professor.SetCoursesTeaching(new List<Course> { course1 });
            studentTeacher.SetCoursesTeaching(new List<Course> { course2 });
            // Initialize the list if it hasn't been already
            if (allHeadsOfDepartment == null)
                allHeadsOfDepartment = new List<HeadOfDepartment>();

            // Add the hod to the list
            allHeadsOfDepartment.Add(hod);
        }
        // Creating the List of Heads of the Departments
        private List<HeadOfDepartment> allHeadsOfDepartment;
        // A method to find a specific Head via their ID number
        private HeadOfDepartment IdentifyHoD(string id)
        {
            foreach (HeadOfDepartment hod in allHeadsOfDepartment)
            {
                if (hod.GetID() == id)
                {
                    return hod;
                }
            }
            return null;
        }
        public MainApp(Login Login)
        {
            CreateExampleData();
            this.form = Login;
            string[] details = Login.LoggedInLine.Split(',');
            this.FormClosed += this.closeEveryThing;
            InitializeComponent();
            // Name, ID, Email, Last Login, and Profession
            textBox1.Text = details[2].Split(':')[1].Trim();// Name
            string id = details[4].Split(':')[1].Trim();
            textBox2.Text = id;// ID
            textBox4.Text = details[3].Split(':')[1].Trim();// Email
            textBox5.Text = details[8].Split(':')[1].Trim();// Last Login
            // Change the last_login in users.txt to the current date and time
            // This way, the date and time of the previous login is saved, and the current date and time is immediately updated into the file, for the next time the user logs in.
            try
            {
                // First we try to find the file users.txt. If we can't find it, we throw an exception
                string path = "users.txt";
                if (!File.Exists(path))
                    throw new FileNotFoundException("The users.txt file was not found.");
                // Next we create a string array of the lines in the file
                string[] lines = File.ReadAllLines(path);
                // We scan each line untill we find a line with the id of the current user
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains($"ID: {id}"))
                    {
                        //Find the starting index of "Date of Last Login" in the line. If none is found, throw an exception
                        int loginIndex = lines[i].IndexOf("Date of Last Login:");
                        if (loginIndex == -1)
                            throw new FormatException("The 'Date of Last Login' field was not found in the user entry.");
                        //Find the end of the "Date of Last Login" field. If none is found (for some reason, error or otherwise), assume that the end of the line is the appropriate end of the field.
                        int endIndex = lines[i].IndexOf(',', loginIndex);
                        if (endIndex == -1) endIndex = lines[i].Length;
                        // Replace the old date with the current date and time, then exit the loop (once the update is done, we don't need to go through the rest of the file)
                        string oldLoginField = lines[i].Substring(loginIndex, endIndex - loginIndex);
                        string newLoginField = $"Date of Last Login: {DateTime.Now:dd/MM/yyyy HH:mm}";
                        lines[i] = lines[i].Replace(oldLoginField, newLoginField);
                        break;
                    }
                }

                File.WriteAllLines(path, lines);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("You do not have permission to access users.txt. Please check file permissions.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"A file access error occurred: {ex.Message}", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException ex)
            {
                MessageBox.Show($"Formatting issue: {ex.Message}", "Format Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            string profession = details[5].Split(':')[1].Trim();
            textBox8.Text = profession;// Profession
            if (profession != "Head of Department")
            {
                // Head of Department
                tabControl1.TabPages.Remove(tabPage1);
                string headID = details[7].Split(':')[1].Trim();
                HeadOfDepartment headOfDepartment = IdentifyHoD(headID);
                // Student-based info and complex info (Study track, picture, age, and messages)
                if (profession == "Student")
                {
                    // Hide the courses groupbox and the HoD tab
                    groupBox6.Visible = false;
                    tabPage1.Hide();
                    label12.Text = "Study Track: ";
                    textBox7.Text = details[9].Split(':')[1].Trim();// Study Track
                    foreach (Student student in headOfDepartment.GetStudentsInDepartment())
                    {
                        if (student.GetID() == id)
                        {
                            string path = student.GetFilepath();
                            if (File.Exists(path))
                            {
                                pictureBox1.Image = Image.FromFile(path);// Picture
                                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                            }
                            else
                            {
                                MessageBox.Show("Profile image not found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            textBox3.Text = student.GetAge().ToString();// Age
                            // Messages
                            LatestMessages.Items.Clear();
                            StarredMessages.Items.Clear();
                            Messages.Items.Clear();
                            // For each message, add the message to the list of all messages. If it's starred, add it to the starred messages list. If it was sent within the past 30 days, add it to the recent messages list.
                            foreach (Message msg in student.GetMessageList())
                            {
                                Messages.Items.Add(msg);
                                if (msg.IsStarred())
                                {
                                    StarredMessages.Items.Add(msg);
                                }
                                if (msg.GetDateSent().AddDays(30) >= DateTime.Now)
                                {
                                    LatestMessages.Items.Add(msg);
                                }
                            }
                            break;
                        }
                    }
                }
                // StudentTeacher-based info and complex info (Study track, picture, age, messages, and courses)
                if (profession == "Student Teacher")
                {
                    // Hide the the HoD tab
                    tabPage1.Hide();
                    label12.Text = "Study Track: ";
                    textBox7.Text = details[9].Split(':')[1].Trim();// Study Track
                    foreach (Student student in headOfDepartment.GetStudentsInDepartment())
                    {
                        // Save as a Student Teacher
                        StudentTeacher st = student as StudentTeacher;
                        if (st.GetID() == id)
                        {
                            string path = st.GetFilepath();
                            if (File.Exists(path))
                            {
                                pictureBox1.Image = Image.FromFile(path);// Picture
                                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                            }
                            else
                            {
                                MessageBox.Show("Profile image not found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            textBox3.Text = st.GetAge().ToString();// Age
                            // Messages
                            LatestMessages.Items.Clear();
                            StarredMessages.Items.Clear();
                            Messages.Items.Clear();
                            // For each message, add the message to the list of all messages. If it's starred, add it to the starred messages list. If it was sent within the past 30 days, add it to the recent messages list.
                            foreach (Message msg in st.GetMessageList())
                            {
                                Messages.Items.Add(msg);
                                if (msg.IsStarred())
                                {
                                    StarredMessages.Items.Add(msg);
                                }
                                if (msg.GetDateSent().AddDays(30) >= DateTime.Now)
                                {
                                    LatestMessages.Items.Add(msg);
                                }
                            }
                            // Courses
                            foreach (Course c in st.GetCoursesTeaching())
                            {
                                Courses.Items.Add(c);
                            }
                            break;
                        }
                    }
                }
                // Professor-based info and complex info (, picture, age, messages, and courses)
                if (profession == "Professor")
                {
                    // Hide the Study Track textbox and the HoD tab
                    textBox7.Visible = false;
                    tabPage1.Hide();
                    foreach (Professor prof in headOfDepartment.GetSupervising())
                    {
                        if (prof.GetID() == id)
                        {
                            string path = prof.GetFilepath();
                            if (File.Exists(path))
                            {
                                pictureBox1.Image = Image.FromFile(path);// Picture
                                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                            }
                            else
                            {
                                MessageBox.Show("Profile image not found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            textBox3.Text = prof.GetAge().ToString();// Age
                            // Messages
                            LatestMessages.Items.Clear();
                            StarredMessages.Items.Clear();
                            Messages.Items.Clear();
                            // For each message, add the message to the list of all messages. If it's starred, add it to the starred messages list. If it was sent within the past 30 days, add it to the recent messages list.
                            foreach (Message msg in prof.GetMessageList())
                            {
                                Messages.Items.Add(msg);
                                if (msg.IsStarred())
                                {
                                    StarredMessages.Items.Add(msg);
                                }
                                if (msg.GetDateSent().AddDays(30) >= DateTime.Now)
                                {
                                    LatestMessages.Items.Add(msg);
                                }
                            }
                            // Courses
                            foreach (Course c in prof.GetCoursesTeaching())
                            {
                                Courses.Items.Add(c);
                            }
                            break;
                        }
                    }
                }
            }
            // Professor-based info and complex info (, picture, age, messages, and courses)
            if (profession == "Head of Department")
            {
                // Hide the Study Track textbox, the listBox for displaying details, and the panel for selecting details
                textBox7.Visible = false;
                ToDisplay.Visible = false;
                ToRemove.Visible = false;
                // Identify HoD
                HeadOfDepartment HoD = IdentifyHoD(id);
                {
                    if (HoD.GetID() == id)
                    {
                        string path = HoD.GetFilepath();
                        if (File.Exists(path))
                        {
                            pictureBox1.Image = Image.FromFile(path);// Picture
                            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                        }
                        else
                        {
                            MessageBox.Show("Profile image not found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        textBox3.Text = HoD.GetAge().ToString();// Age
                        // Messages
                        LatestMessages.Items.Clear();
                        StarredMessages.Items.Clear();
                        Messages.Items.Clear();
                        // For each message, add the message to the list of all messages. If it's starred, add it to the starred messages list. If it was sent within the past 30 days, add it to the recent messages list.
                        foreach (Message msg in HoD.GetMessageList())
                        {
                            Messages.Items.Add(msg);
                            if (msg.IsStarred())
                            {
                                StarredMessages.Items.Add(msg);
                            }
                            if (msg.GetDateSent().AddDays(30) >= DateTime.Now)
                            {
                                LatestMessages.Items.Add(msg);
                            }
                        }
                        // Courses
                        if (HoD.GetCoursesTeaching().Count() == 0)
                        {
                            Courses.Items.Add("No courses being taught.");
                        }
                        else
                        {
                            foreach (Course c in HoD.GetCoursesTeaching())
                            {
                                Courses.Items.Add(c);
                            }
                        }
                    }
                }
            }
        }
        private void closeEveryThing(object sender, EventArgs e)
        {
            this.form.Close();
        }
        // Sort the messages in the listbox by date sent
        private void button1_Click(object sender, EventArgs e)
        {
            List<Message> msgs = new List<Message>();
            foreach (var item in Messages.Items)
            {
                if (item is Message msg)
                    msgs.Add(msg);
            }
            msgs = msgs.OrderBy(m => m.GetDateSent()).ToList();
            Messages.Items.Clear();
            foreach (var msg in msgs)
            {
                Messages.Items.Add(msg);
            }
        }
        // Sort the messages in the listbox by starred status
        private void button2_Click(object sender, EventArgs e)
        {
            List<Message> msgs = new List<Message>();
            foreach (var item in Messages.Items)
            {
                if (item is Message msg)
                    msgs.Add(msg);
            }
            msgs = msgs.OrderByDescending(m => m.IsStarred()).ToList();
            Messages.Items.Clear();
            foreach (var msg in msgs)
            {
                Messages.Items.Add(msg);
            }
        }
        // Sort the messages in the listbox by sender type (HoD, Professor, ST, Student)
        private void button3_Click(object sender, EventArgs e)
        {
            List<Message> msgs = new List<Message>();
            foreach (var item in Messages.Items)
            {
                if (item is Message msg)
                    msgs.Add(msg);
            }
            Dictionary<Type, int> typeOrder = new Dictionary<Type, int>
            {
                { typeof(HeadOfDepartment), 0 },
                { typeof(Professor), 1 },
                { typeof(StudentTeacher), 2 },
                { typeof(Student), 3 }
            };
            msgs = msgs.OrderBy(m =>
            {
                var senderType = m.GetSender().GetType();
                return typeOrder.TryGetValue(senderType, out int order) ? order : int.MaxValue;
            }).ToList(); Messages.Items.Clear();
            foreach (var msg in msgs)
            {
                Messages.Items.Add(msg);
            }
        }
        // Color Settings
        public enum ThemeType
        {
            Red,
            Blue,
            Green,
            Dark
        }
        private Dictionary<ThemeType, Dictionary<Type, (Color BackColor, Color ForeColor)>> themeSettings = new Dictionary<ThemeType, Dictionary<Type, (Color, Color)>>()
        {
            [ThemeType.Red] = new Dictionary<Type, (Color, Color)>
            {
                // Red Theme Settings
                [typeof(Form)] = (Color.Maroon, Color.Black),
                [typeof(TabPage)] = (Color.LightCoral, Color.Black),
                [typeof(Panel)] = (Color.MistyRose, Color.Black),
                [typeof(Label)] = (Color.Transparent, Color.Black),
                [typeof(TextBox)] = (Color.Bisque, Color.Black),
                [typeof(GroupBox)] = (Color.Coral, Color.Black),
                [typeof(ListBox)] = (Color.DarkSalmon, Color.Black),
                [typeof(VScrollBar)] = (Color.LightGray, Color.Black),
                [typeof(PictureBox)] = (Color.Transparent, Color.Black),
                [typeof(Button)] = (Color.LightPink, Color.Black),
                [typeof(ComboBox)] = (Color.BlanchedAlmond, Color.Black),
                [typeof(RadioButton)] = (Color.Transparent, Color.Black),
                [typeof(CheckBox)] = (Color.Transparent, Color.Black),
                [typeof(TabControl)] = (Color.LightCoral, Color.Black)
            },
            [ThemeType.Blue] = new Dictionary<Type, (Color, Color)>
            {
                // Blue Theme Settings
                [typeof(Form)] = (Color.Navy, Color.Black),
                [typeof(TabPage)] = (Color.CornflowerBlue, Color.Black),
                [typeof(Panel)] = (Color.Lavender, Color.Black),
                [typeof(Label)] = (Color.Transparent, Color.Black),
                [typeof(TextBox)] = (Color.Bisque, Color.Black),
                [typeof(GroupBox)] = (Color.DarkTurquoise, Color.Black),
                [typeof(ListBox)] = (Color.DarkCyan, Color.Black),
                [typeof(VScrollBar)] = (Color.LightGray, Color.Black),
                [typeof(PictureBox)] = (Color.Transparent, Color.Black),
                [typeof(Button)] = (Color.LightSkyBlue, Color.Black),
                [typeof(ComboBox)] = (Color.BlanchedAlmond, Color.Black),
                [typeof(RadioButton)] = (Color.Transparent, Color.Black),
                [typeof(CheckBox)] = (Color.Transparent, Color.Black),
                [typeof(TabControl)] = (Color.LightCyan, Color.Black)
            },
            [ThemeType.Green] = new Dictionary<Type, (Color, Color)>
            {
                // Green Theme Settings
                [typeof(Form)] = (Color.DarkGreen, Color.Black),
                [typeof(TabPage)] = (Color.SeaGreen, Color.Black),
                [typeof(Panel)] = (Color.Honeydew, Color.Black),
                [typeof(Label)] = (Color.Transparent, Color.Black),
                [typeof(TextBox)] = (Color.Bisque, Color.Black),
                [typeof(GroupBox)] = (Color.LimeGreen, Color.Black),
                [typeof(ListBox)] = (Color.LightGreen, Color.Black),
                [typeof(VScrollBar)] = (Color.LightGray, Color.Black),
                [typeof(PictureBox)] = (Color.Transparent, Color.Black),
                [typeof(Button)] = (Color.PaleGreen, Color.Black),
                [typeof(ComboBox)] = (Color.BlanchedAlmond, Color.Black),
                [typeof(RadioButton)] = (Color.Transparent, Color.Black),
                [typeof(CheckBox)] = (Color.Transparent, Color.Black),
                [typeof(TabControl)] = (Color.Honeydew, Color.Black)
            },
            [ThemeType.Dark] = new Dictionary<Type, (Color, Color)>
            {
                // Dark Theme Settings
                [typeof(Form)] = (Color.Black, Color.White),
                [typeof(TabPage)] = (Color.DarkGray, Color.Black),
                [typeof(Panel)] = (Color.DimGray, Color.White),
                [typeof(Label)] = (Color.White, Color.Black),
                [typeof(TextBox)] = (Color.Silver, Color.Black),
                [typeof(GroupBox)] = (Color.Black, Color.White),
                [typeof(ListBox)] = (Color.Gray, Color.White),
                [typeof(VScrollBar)] = (Color.Black, Color.White),
                [typeof(PictureBox)] = (Color.Transparent, Color.White),
                [typeof(Button)] = (Color.FromArgb(70, 70, 70), Color.White),
                [typeof(ComboBox)] = (Color.Gainsboro, Color.Black),
                [typeof(RadioButton)] = (Color.Gainsboro, Color.Black),
                [typeof(CheckBox)] = (Color.Gainsboro, Color.Black),
                [typeof(TabControl)] = (Color.Gainsboro, Color.White)
            }
        };
        // Apply the theme settings to the control and its children
        private void ApplyThemeRecursive(Control control, ThemeType theme)
        {
            foreach (var pair in themeSettings[theme])
            {
                if (pair.Key.IsInstanceOfType(control))
                {
                    control.BackColor = pair.Value.BackColor;
                    control.ForeColor = pair.Value.ForeColor;
                    break;
                }
            }
            foreach (Control child in control.Controls)
            {
                ApplyThemeRecursive(child, theme);
            }
        }
        // Set the theme to Red by default
        private ThemeType currentTheme = ThemeType.Red;
        private ThemeType previousTheme = ThemeType.Red;
        // Change the theme based on the user's choice
        private void SetTheme(ThemeType theme)
        {
            currentTheme = theme;
            previousTheme = theme;
            ApplyThemeRecursive(this, currentTheme);
        }
        // Dark Mode activation
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                previousTheme = currentTheme;
                ApplyThemeRecursive(this, ThemeType.Dark);
            }
            else
            {
                SetTheme(previousTheme);
            }
        }
        // Color Theme Selection
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!checkBox1.Checked) // Only apply if not in dark mode
            {
                switch (comboBox2.SelectedItem.ToString())
                {
                    case "Red":
                        SetTheme(ThemeType.Red);
                        break;
                    case "Blue":
                        SetTheme(ThemeType.Blue);
                        break;
                    case "Green":
                        SetTheme(ThemeType.Green);
                        break;
                }
            }
        }
        // Font Size Settings
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if a font size is selected
            // If not, show an error message
            if (comboBox4.SelectedItem != null)
            {
                // Try to parse the selected item as a float
                // If parsing fails, show an error message
                if (float.TryParse(comboBox4.SelectedItem.ToString(), out float newFontSize))
                {
                    ChangeFontSize(this, newFontSize);
                }
                else
                {
                    MessageBox.Show("Invalid font size selected.");
                }
            }
            else
            {
                MessageBox.Show("Please select a font size.");
            }
        }
        // Apply the new font size to the controlS and their children
        private void ChangeFontSize(Control control, float size)
        {
            // Only change if control has text
            if (!(control is Form)) // Don't change the actual form
            {
                Font oldFont = control.Font;
                control.Font = new Font(oldFont.FontFamily, size, oldFont.Style);
            }
            foreach (Control child in control.Controls)
            {
                ChangeFontSize(child, size);
            }
        }
        // Searching for People
        private void button4_Click(object sender, EventArgs e)
        {
            // Reset the listBox before searching
            SearchResults.Items.Clear();
            // Store the information of what to search for
            string searchText = textBox6.Text.Trim().ToLower();// Whatever was written in the textBox
            string searchBy = comboBox1.SelectedItem?.ToString();// Name, Student Number, or Employee Number
            string currentUserProfession = textBox8.Text.Trim();// The profession of the current user
            string currentUserID = textBox2.Text.Trim();// The ID of the current user
            // Check if searchText and searchBy are not empty
            if (string.IsNullOrEmpty(searchText) || string.IsNullOrEmpty(searchBy))
            {
                MessageBox.Show("Please enter a search term and select a search method.");
                return;
            }
            // A list for all of people to search, currently empty, and a HashSet to keep track of added emails, to avoid adding duplicates
            List<Person> peopleToSearch = new List<Person>();
            HashSet<string> addedEmails = new HashSet<string>();
            // Make sure a radioButton is checked
            if (!radioButton1.Checked && !radioButton2.Checked)
            {
                MessageBox.Show("Please select a search type (Own Type or All Types).");
                return;
            }
            // Check for which radioButton is checked and add the appropriate people to search
            else if (radioButton1.Checked)// Own type only
            {
                // If the user isn't a Head of Department, add everyone in the system that matches the current user's profession that isn't the user themselves
                // Implement the HashSet to avoid adding duplicates
                if (currentUserProfession == "Student" || currentUserProfession == "Student Teacher" || currentUserProfession == "Professor")
                {
                    // First cover StudentTeacher to avoid adding them when covering Student
                    if (currentUserProfession == "Student Teacher")
                    {
                        foreach (HeadOfDepartment hod in allHeadsOfDepartment)
                        {
                            foreach (Student s in hod.GetStudentsInDepartment())
                            {
                                if (s.GetType() == typeof(StudentTeacher) && s.GetID() != currentUserID && addedEmails.Add(s.GetEmail()))
                                {
                                    peopleToSearch.Add(s);
                                }
                            }
                            foreach (Person p in hod.GetSupervising())
                            {
                                if (p.GetType() == typeof(StudentTeacher) && p.GetID() != currentUserID && addedEmails.Add(p.GetEmail()))
                                {
                                    peopleToSearch.Add(p);
                                }
                            }
                        }
                    }
                    else if (currentUserProfession == "Student")// Student
                    {
                        foreach (HeadOfDepartment hod in allHeadsOfDepartment)
                        {
                            foreach (Student s in hod.GetStudentsInDepartment())
                            {
                                if (s.GetType() != typeof(StudentTeacher) && s.GetID() != currentUserID && addedEmails.Add(s.GetEmail()))
                                {
                                    peopleToSearch.Add(s);
                                }
                            }
                        }
                    }
                    else if (currentUserProfession == "Professor")// Professor
                    {
                        foreach (HeadOfDepartment hod in allHeadsOfDepartment)
                        {
                            foreach (Person p in hod.GetSupervising())
                            {
                                if (p.GetType() == typeof(Professor) && p.GetID() != currentUserID && addedEmails.Add(p.GetEmail()))
                                {
                                    peopleToSearch.Add(p);
                                }
                            }
                        }
                    }
                }
                // Otherwise, just add all of the Heads of Departments to the search list, save for the current user
                else if (currentUserProfession == "Head of Department")
                {
                    foreach (var hod in allHeadsOfDepartment)
                    {
                        if (hod.GetID() != currentUserID && addedEmails.Add(hod.GetEmail()))
                        {
                            peopleToSearch.Add(hod);
                        }
                    }
                }
            }
            else if (radioButton2.Checked)// All types
            {
                // Add all students, student teachers, professors, and HoDs in the system to the list, except the current user
                foreach (HeadOfDepartment hod in allHeadsOfDepartment)
                {
                    foreach (Student s in hod.GetStudentsInDepartment())
                    {
                        if (s.GetID() != currentUserID && addedEmails.Add(s.GetEmail()))
                        {
                            peopleToSearch.Add(s);
                        }
                    }
                    foreach (Person p in hod.GetSupervising())
                    {
                        if (p.GetID() != currentUserID && addedEmails.Add(p.GetEmail()))
                        {
                            peopleToSearch.Add(p);
                        }
                    }
                    if (hod.GetID() != currentUserID && addedEmails.Add(hod.GetEmail()))
                    {
                        peopleToSearch.Add(hod);
                    }
                }
            }
            // If the searchBy is not valid, show an error message
            // Otherwise, for each person in the peopleToSearch list, check if they match the search criteria
            foreach (var person in peopleToSearch)
            {
                bool match = false;
                switch (searchBy)
                {
                    case "Name":
                        match = person.GetName().ToLower().Contains(searchText);
                        break;
                    case "Student Number":
                        if (person is Student s && s.GetStudentNumber().ToString().Contains(searchText))
                            match = true;
                        break;
                    case "Employee Number":
                        if (person is Professor p && p.GetEmployeeNumber().ToString().Contains(searchText))
                            match = true;
                        break;
                }
                // If they match the criteria, display them in the listBox
                if (match)
                {
                    SearchResults.Items.Add($"{person.GetName()}, {person.GetEmail()}");
                }
            }
            if (SearchResults.Items.Count == 0)
                MessageBox.Show("No matches found.");
            else if (SearchResults.Items.Count == 1)
                MessageBox.Show("1 match found.");
            else
                MessageBox.Show($"{SearchResults.Items.Count} matches found.");
        }
        // Sending a message to a selected person
        private void button5_Click(object sender, EventArgs e)
        {
            // Make sure that a recipient is selected
            if (SearchResults.SelectedItem == null)
            {
                MessageBox.Show("Please select a user to message.");
                return;
            }
            // Open a messageForm already addressed to the email of the selected user, ready for the current user to insewrt their message
            string selected = SearchResults.SelectedItem.ToString();
            string[] parts = selected.Split(',');
            if (parts.Length < 2)
            {
                MessageBox.Show("Invalid selection.");
                return;
            }
            string recipientEmail = parts[1].Trim();
            SendMessage messageForm = new SendMessage(recipientEmail);
            messageForm.Show();
        }
        // Head of Department tab
        private void Resolve_Click(object sender, EventArgs e)
        {
            HeadOfDepartment self = IdentifyHoD(textBox2.Text.Trim());
            string action = comboBox3.SelectedItem?.ToString();// Display, Approve, Remove, Update, Create
            string target = comboBox5.SelectedItem?.ToString();// Course, Study Track, Student, Teacher
            if (string.IsNullOrEmpty(action) || string.IsNullOrEmpty(target))
            {
                MessageBox.Show("Please enter an action and a target.");
                return;
            }
            else
            {
                ToDisplay.Visible = true;
                if (action == "Display")
                {
                    if (target == "Course")
                    {
                        // Display all courses
                        ToDisplay.Items.Clear();
                        string courses = self.DisplayCoursesManaging();
                        if (string.IsNullOrEmpty(courses))
                        {
                            ToDisplay.Items.Add("No courses found.");
                        }
                        else
                        {
                            ToDisplay.Items.Add(courses);
                        }
                    }
                    else if (target == "Study Track")
                    {
                        // Display all study tracks
                        ToDisplay.Items.Clear();
                        string studyTracks = self.DisplayTracksManaging();
                        if (string.IsNullOrEmpty(studyTracks))
                        {
                            ToDisplay.Items.Add("No study tracks found.");
                        }
                        else
                        {
                            ToDisplay.Items.Add(studyTracks);
                        }
                    }
                    else if (target == "Student")
                    {
                        // Display all students
                        ToDisplay.Items.Clear();
                        string students = self.DisplayStudentsInDepartment();
                        if (string.IsNullOrEmpty(students))
                        {
                            ToDisplay.Items.Add("No students found.");
                        }
                        else
                        {
                            ToDisplay.Items.Add(students);
                        }
                    }
                    else
                    {
                        // Display all teachers
                        ToDisplay.Items.Clear();
                        string teachers = self.DisplayTeachersSupervising();
                        if (string.IsNullOrEmpty(teachers))
                        {
                            ToDisplay.Items.Add("No teachers found.");
                        }
                        else
                        {
                            ToDisplay.Items.Add(teachers);
                        }
                    }
                }
                else if (action == "Approve")
                {
                    if (target == "Course" || target == "Study Track")
                    {
                        MessageBox.Show("The 'Approve' action is only applicable to students and teachers.");
                        return;
                    }
                    else if (target == "Student")
                    {
                        // First display the students so that the user can select one to approve
                        // Reset the listBox before displaying, and then save the students in a string
                        ToDisplay.Items.Clear();
                        string students = self.DisplayStudentsInDepartment();
                        // If there are no students, display an appropriate message. Otherwise, add the string to the listBox
                        if (string.IsNullOrEmpty(students))
                        {
                            ToDisplay.Items.Add("No students found.");
                        }
                        else
                        {
                            ToDisplay.Items.Add(students);
                        }
                        // Check if a student is selected
                        if (ToDisplay.SelectedItem == null)
                        {
                            MessageBox.Show("Please select a user to approve.");
                            return;
                        }
                        // Once a student is selected, get their ID from the listBox
                        else
                        {
                            string selectedStudent = ToDisplay.SelectedItem.ToString();
                            string[] parts = selectedStudent.Split(',');
                            string studentID = parts[1].Trim();
                            // Approve the student
                            self.ApproveStudent(studentID);
                        }
                    }
                    else
                    {
                        // First display the teachers so that the user can select one to approve
                        // Reset the listBox before displaying, and then save the teachers in a string
                        ToDisplay.Items.Clear();
                        string teachers = self.DisplayTeachersSupervising();
                        // If there are no teachers, display an appropriate message. Otherwise, add the string to the listBox
                        if (string.IsNullOrEmpty(teachers))
                        {
                            ToDisplay.Items.Add("No teachers found.");
                        }
                        else
                        {
                            ToDisplay.Items.Add(teachers);
                        }
                        // Check if a teacher is selected
                        if (ToDisplay.SelectedItem == null)
                        {
                            MessageBox.Show("Please select a user to approve.");
                            return;
                        }
                        // Once a teacher is selected, get their ID from the listBox
                        else
                        {
                            string selectedTeacher = ToDisplay.SelectedItem.ToString();
                            string[] parts = selectedTeacher.Split(',');
                            string teacherID = parts[1].Trim();
                            // Approve the teacher
                            self.ApproveTeacher(teacherID);
                        }
                    }
                }
                else if (action == "Remove")
                {
                    ToRemove.Visible = true;
                    ToRemove01.Visible = false;
                    ToRemove02.Visible = false;
                    ToRemove03.Visible = false;
                    ToRemove04.Visible = false;
                    ToRemove05.Visible = false;
                    ToRemove06.Visible = false;
                    ToRemove07.Visible = false;
                    ToRemove08.Visible = false;
                    ToRemove09.Visible = false;
                    ToRemove10.Visible = false;
                    ToRemove11.Visible = false;
                    if (target == "Course")
                    {
                        // First, the user needs to select a study track to remove a course from
                        // Make the first combobox visilbe and fill it with the names of the tracks in the list of tracks the user's managing
                        ToRemove01.Visible = true;
                        ToRemove01.Items.Clear();
                        ToRemove01.Text = "Select a study track";
                        StudyTrack selectedTrack = null;
                        foreach (StudyTrack track in self.GetTracksManaging())
                        {
                            ToRemove01.Items.Add(track.GetName());
                        }
                        // Make sure that a track was selected
                        if (ToRemove01.SelectedItem == null || ToRemove01.Text == "Select a study track")
                        {
                            MessageBox.Show("Please select a track from which you will be removing a course.");
                            return;
                        }
                        // Identify the selected track and save it as a track by finding it in the list
                        foreach (StudyTrack track in self.GetTracksManaging())
                        {
                            if (track.GetName() == ToRemove01.SelectedItem)
                            {
                                selectedTrack = track;
                                break;
                            }
                        }
                        // Now we do the same process with the courses in that track on the combobox
                        if (selectedTrack != null)
                        {
                            ToRemove02.Visible = true;
                            ToRemove02.Items.Clear();
                            ToRemove02.Text = "Select a course to remove";
                            Course selectedCourse = null;
                            foreach (Course c in selectedTrack.GetCourses())
                            {
                                ToRemove02.Items.Add(c.GetName());
                            }
                            // Make sure that a course was selected
                            if (ToRemove02.SelectedItem == null || ToRemove02.Text == "Select a course to remove")
                            {
                                MessageBox.Show("Please select a course to remove.");
                                return;
                            }
                            // Identify the selected course and save it as a course by finding it in the list
                            foreach (Course c in selectedTrack.GetCourses())
                            {
                                if (c.GetName() == ToRemove02.SelectedItem)
                                {
                                    selectedCourse = c;
                                    break;
                                }
                            }
                            if (selectedCourse != null)
                            {
                                // Remove the course from the track
                                self.RemoveCourseFromTrack(selectedTrack, selectedCourse);
                                // Display a message to the user
                                MessageBox.Show($"'{selectedCourse.GetName()}' was removed from '{selectedTrack.GetName()}'.");
                            }
                        }
                    }
                    else if (target == "Study Track")
                    {
                        // First, the user needs to select a study track to remove
                        // Make the first combobox visilbe and fill it with the names of the tracks in the list of tracks the user's managing
                        ToRemove01.Visible = true;
                        ToRemove01.Items.Clear();
                        ToRemove01.Text = "Select a study track to remove";
                        StudyTrack selectedTrack = null;
                        foreach (StudyTrack track in self.GetTracksManaging())
                        {
                            ToRemove01.Items.Add(track.GetName());
                        }
                        // Make sure that a track was selected
                        if (ToRemove01.SelectedItem == null || ToRemove01.Text == "Select a study track to remove")
                        {
                            MessageBox.Show("Please select a track you will be removing.");
                            return;
                        }
                        // Identify the selected track and save it as a track by finding it in the list
                        foreach (StudyTrack track in self.GetTracksManaging())
                        {
                            if (track.GetName() == ToRemove01.SelectedItem)
                            {
                                selectedTrack = track;
                                break;
                            }
                        }
                        if (selectedTrack != null)
                        {
                            // Remove the study track
                            self.RemoveTrack(selectedTrack);
                            // Display a message to the user
                            MessageBox.Show($"'{selectedTrack.GetName()}' was removed.");
                        }
                    }
                    else if (target == "Student")
                    {
                        // First, the user needs to select a student to remove
                        ToRemove01.Visible = true;
                        ToRemove01.Items.Clear();
                        ToRemove01.Text = "Select a student to remove";
                        Student selectedStudent = null;
                        foreach (Student s in self.GetStudentsInDepartment())
                        {
                            ToRemove01.Items.Add(s.GetName() + ", " + s.GetID());
                        }
                        // Make sure that a student was selected
                        if (ToRemove01.SelectedItem == null || ToRemove01.Text == "Select a student to remove")
                        {
                            MessageBox.Show("Please select a student to remove.");
                            return;
                        }
                        // Identify the selected student and save it as a student by finding it in the list
                        foreach (Student s in self.GetStudentsInDepartment())
                        {
                            if (s.GetName() + ", " + s.GetID() == ToRemove01.SelectedItem.ToString())
                            {
                                selectedStudent = s;
                                break;
                            }
                        }
                        if (selectedStudent != null)
                        {
                            // Remove the student from the department
                            self.RemoveStudent(selectedStudent);
                        }
                    }
                    else
                    {
                        // First, the user needs to select a teacher to remove
                        ToRemove01.Visible = true;
                        ToRemove01.Items.Clear();
                        ToRemove01.Text = "Select a teacher to remove";
                        Person selectedTeacher = null;
                        foreach (Person p in self.GetSupervising())
                        {
                            ToRemove01.Items.Add(p.GetName() + ", " + p.GetID());
                        }
                        // Make sure that a teacher was selected
                        if (ToRemove01.SelectedItem == null || ToRemove01.Text == "Select a teacher to remove")
                        {
                            MessageBox.Show("Please select a teacher to remove.");
                            return;
                        }
                        // Identify the selected teacher and save it as a teacher by finding it in the list
                        foreach (Person p in self.GetSupervising())
                        {
                            if (p.GetName() + ", " + p.GetID() == ToRemove01.SelectedItem.ToString())
                            {
                                selectedTeacher = p;
                                break;
                            }
                        }
                        if (selectedTeacher != null)
                        {
                            // Remove the teacher from the department
                            self.RemoveTeacher(selectedTeacher);
                        }
                    }
                }
                else if (action == "Update")
                {
                    ToRemove.Visible = true;
                    ToRemove01.Visible = false;
                    ToRemove02.Visible = false;
                    ToRemove03.Visible = false;
                    ToRemove04.Visible = false;
                    ToRemove05.Visible = false;
                    ToRemove06.Visible = false;
                    ToRemove07.Visible = false;
                    ToRemove08.Visible = false;
                    ToRemove09.Visible = false;
                    ToRemove10.Visible = false;
                    ToRemove11.Visible = false;
                    if (target == "Course")
                    {
                        // First, the user needs to select a study track to update a course in
                        // Make the first combobox visilbe and fill it with the names of the tracks in the list of tracks the user's managing
                        ToRemove01.Visible = true;
                        ToRemove01.Items.Clear();
                        ToRemove01.Text = "Select a study track";
                        StudyTrack selectedTrack = null;
                        foreach (StudyTrack track in self.GetTracksManaging())
                        {
                            ToRemove01.Items.Add(track.GetName());
                        }
                        // Make sure that a track was selected
                        if (ToRemove01.SelectedItem == null || ToRemove01.Text == "Select a study track")
                        {
                            MessageBox.Show("Please select a track from which you will be updating a course.");
                            return;
                        }
                        // Identify the selected track and save it as a track by finding it in the list
                        foreach (StudyTrack track in self.GetTracksManaging())
                        {
                            if (track.GetName() == ToRemove01.SelectedItem)
                            {
                                selectedTrack = track;
                                break;
                            }
                        }
                        // Now we do the same process with the courses in that track on the combobox
                        if (selectedTrack != null)
                        {
                            ToRemove02.Visible = true;
                            ToRemove02.Items.Clear();
                            ToRemove02.Text = "Select a course to update";
                            Course selectedCourse = null;
                            foreach (Course c in selectedTrack.GetCourses())
                            {
                                ToRemove02.Items.Add(c.GetName());
                            }
                            // Make sure that a course was selected
                            if (ToRemove02.SelectedItem == null || ToRemove02.Text == "Select a course to update")
                            {
                                MessageBox.Show("Please select a course to update.");
                                return;
                            }
                            // Identify the selected course and save it as a course by finding it in the list
                            foreach (Course c in selectedTrack.GetCourses())
                            {
                                if (c.GetName() == ToRemove02.SelectedItem)
                                {
                                    selectedCourse = c;
                                    break;
                                }
                            }
                            if (selectedCourse != null)
                            {
                                // The only details to update in a course is its name.
                                // We already add/remove students to/from the course when we add/remove them to/from the track
                                // Update the course from the track
                                Course newCourse = selectedCourse;
                                ToRemove03.Visible = true;
                                ToRemove03.Items.Clear();
                                ToRemove03.Text = "Insert the new name of the course";
                                if (ToRemove03.Text == null || ToRemove03.Text == "Insert the new name of the course")
                                {
                                    MessageBox.Show("Please insert a new name for the course.");
                                    return;
                                }
                                string name = ToRemove03.Text;
                                newCourse.SetName(name);
                                // Update the course and display a message to the user
                                self.UpdateCourseInTrack(selectedTrack, selectedCourse, newCourse);
                                MessageBox.Show($"'{selectedCourse.GetName()}' was replaced with '{name}' from '{selectedTrack.GetName()}'.");
                            }
                        }
                    }
                    else if (target == "Study Track")
                    {
                        // First, the user needs to select a study track to update
                        // Make the first combobox visilbe and fill it with the names of the tracks in the list of tracks the user's managing
                        ToRemove01.Visible = true;
                        ToRemove01.Items.Clear();
                        ToRemove01.Text = "Select a track to update";
                        StudyTrack selectedTrack = null;
                        foreach (StudyTrack track in self.GetTracksManaging())
                        {
                            ToRemove01.Items.Add(track.GetName());
                        }
                        // Make sure that a track was selected
                        if (ToRemove01.SelectedItem == null || ToRemove01.Text == "Select a track to update")
                        {
                            MessageBox.Show("Please select a track you will be updating.");
                            return;
                        }
                        // Identify the selected track and save it as a track by finding it in the list
                        foreach (StudyTrack track in self.GetTracksManaging())
                        {
                            if (track.GetName() == ToRemove01.SelectedItem)
                            {
                                selectedTrack = track;
                                break;
                            }
                        }
                        if (selectedTrack != null)
                        {
                            // The only details to update in a track is its name.
                            // We add/remove students/courses to/from the track when adding/removing the student/course
                            // Update the course from the track
                            StudyTrack newTrack = selectedTrack;
                            ToRemove02.Visible = true;
                            ToRemove02.Items.Clear();
                            ToRemove02.Text = "Insert the new name of the track";
                            if (ToRemove02.SelectedItem == null || ToRemove02.Text == "Insert the new name of the track")
                            {
                                MessageBox.Show("Please insert a new name for the track.");
                                return;
                            }
                            string name = ToRemove02.Text;
                            newTrack.SetName(name);
                            // Update the track and display a message to the user
                            self.UpdateTrack(selectedTrack, newTrack);
                            MessageBox.Show($"'{selectedTrack.GetName()}' was replaced with '{name}' from '{selectedTrack.GetName()}'.");
                        }
                    }
                    else if (target == "Student")
                    {
                        // First, the user needs to select a student to update
                        // Make the first combobox visilbe and fill it with the names and ID of the students in the list of students in the user's department
                        ToRemove01.Visible = true;
                        ToRemove01.Items.Clear();
                        ToRemove01.Text = "Select a student to update";
                        Student selectedStudent = null;
                        foreach (Student s in self.GetStudentsInDepartment())
                        {
                            ToRemove01.Items.Add(s.GetName() + ", " + s.GetID());
                        }
                        // Make sure that a student was selected
                        if (ToRemove01.SelectedItem == null || ToRemove01.Text == "Select a student to update")
                        {
                            MessageBox.Show("Please select a student to update.");
                            return;
                        }
                        // Identify the selected student and save it as a student by finding it in the list
                        foreach (Student s in self.GetStudentsInDepartment())
                        {
                            if (s.GetName() + ", " + s.GetID() == ToRemove01.SelectedItem.ToString())
                            {
                                selectedStudent = s;
                                break;
                            }
                        }
                        if (selectedStudent != null)
                        {
                            // For a student, we can update their age, phone number, picture, study track, specialization, currentCreditPoints, and totalEarnedPoints
                            ToRemove02.Visible = true;
                            ToRemove02.Items.Clear();
                            ToRemove02.Items.Add("Age");
                            ToRemove02.Items.Add("Phone Number");
                            ToRemove02.Items.Add("Picture");
                            ToRemove02.Items.Add("Study Track");
                            ToRemove02.Items.Add("Specialization");
                            ToRemove02.Items.Add("Current Credit Points");
                            ToRemove02.Items.Add("Total Credit Points");
                            ToRemove02.Items.Add("Make Student Teacher");
                            ToRemove02.Text = "Select a detail to update";
                            Student newStudent = selectedStudent;
                            // Make sure that a detail was selected
                            if (ToRemove02.SelectedItem == null || ToRemove02.Text == "Select a detail to update")
                            {
                                MessageBox.Show("Please select a detail to update.");
                                return;
                            }
                            // Identify the selected detail and save it as a string by finding it in the list
                            string selectedDetail = ToRemove02.SelectedItem.ToString();
                            if (selectedDetail == "Age")
                            {
                                ToRemove03.Visible = true;
                                ToRemove03.Items.Clear();
                                ToRemove03.Text = "Insert the new age of the student";
                                if (int.TryParse(ToRemove03.Text, out int newAge) && newAge > 0)
                                {
                                    newStudent.SetAge(newAge);
                                    self.UpdateStudent(selectedStudent, newStudent);
                                    MessageBox.Show($"'{selectedStudent.GetName()}' was updated with age '{newAge}'.");
                                }
                                else
                                {
                                    MessageBox.Show("Please insert a valid age.");
                                    return;
                                }
                            }
                            else if (selectedDetail == "Phone Number")
                            {
                                ToRemove03.Visible = true;
                                ToRemove03.Items.Clear();
                                ToRemove03.Text = "Insert the new phone number of the student";
                                string newPhoneNumber = ToRemove03.Text;
                                newStudent.SetPhoneNumber(newPhoneNumber);
                                self.UpdateStudent(selectedStudent, newStudent);
                                MessageBox.Show($"'{selectedStudent.GetName()}' was updated with phone number '{newPhoneNumber}'.");
                            }
                            else if (selectedDetail == "Picture")
                            {
                                // For picture, we can open a file dialog to select a new picture
                                OpenFileDialog openFileDialog = new OpenFileDialog();
                                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                                if (openFileDialog.ShowDialog() == DialogResult.OK)
                                {
                                    string newFilepath = openFileDialog.FileName;
                                    newStudent.SetFilepath(newFilepath);
                                    self.UpdateStudent(selectedStudent, newStudent);
                                    MessageBox.Show($"'{selectedStudent.GetName()}' was updated with a new picture.");
                                }
                            }
                            else if (selectedDetail == "Study Track")
                            {
                                // For study track, we can open a combobox to select a new track
                                ToRemove03.Visible = true;
                                ToRemove03.Items.Clear();
                                ToRemove03.Text = "Select a new study track for the student";
                                StudyTrack selectedTrack = null;
                                foreach (StudyTrack track in self.GetTracksManaging())
                                {
                                    ToRemove03.Items.Add(track.GetName());
                                }
                                // Make sure that a track was selected
                                if (ToRemove03.SelectedItem == null || ToRemove03.Text == "Select a new study track for the student")
                                {
                                    MessageBox.Show("Please select a new study track for the student.");
                                    return;
                                }
                            }
                            else if (selectedDetail == "Specialization")
                            {
                                ToRemove03.Visible = true;
                                ToRemove03.Items.Clear();
                                ToRemove03.Text = "Insert the new specialization of the student";
                                string newSpecialization = ToRemove03.Text;
                                newStudent.SetSpecialization(newSpecialization);
                                self.UpdateStudent(selectedStudent, newStudent);
                                MessageBox.Show($"'{selectedStudent.GetName()}' was updated with specialization '{newSpecialization}'.");
                            }
                            else if (selectedDetail == "Current Credit Points")
                            {
                                ToRemove03.Visible = true;
                                ToRemove03.Items.Clear();
                                ToRemove03.Text = "Insert the new current credit points of the student";
                                if (int.TryParse(ToRemove03.Text, out int newCurrentCreditPoints) && newCurrentCreditPoints >= 0)
                                {
                                    newStudent.SetCurrentCreditPoints(newCurrentCreditPoints);
                                    self.UpdateStudent(selectedStudent, newStudent);
                                    MessageBox.Show($"'{selectedStudent.GetName()}' was updated with current credit points '{newCurrentCreditPoints}'.");
                                }
                                else
                                {
                                    MessageBox.Show("Please insert a valid number of current credit points.");
                                    return;
                                }
                            }
                            else if (selectedDetail == "Total Credit Points")
                            {
                                ToRemove03.Visible = true;
                                ToRemove03.Items.Clear();
                                ToRemove03.Text = "Insert the new total credit points of the student";
                                if (int.TryParse(ToRemove03.Text, out int newTotalEarnedPoints) && newTotalEarnedPoints >= 0)
                                {
                                    newStudent.SetTotalEarnedPoints(newTotalEarnedPoints);
                                    self.UpdateStudent(selectedStudent, newStudent);
                                    MessageBox.Show($"'{selectedStudent.GetName()}' was updated with total credit points '{newTotalEarnedPoints}'.");
                                }
                                else
                                {
                                    MessageBox.Show("Please insert a valid number of total credit points.");
                                    return;
                                }
                            }
                            else if (selectedDetail == "Make Student Teacher")
                            {
                                ToRemove03.Visible = true;
                                ToRemove03.Items.Clear();
                                ToRemove03.Text = "Select a Head of Department to supervise the new Student Teacher";
                                foreach (HeadOfDepartment hod in allHeadsOfDepartment)
                                {
                                    ToRemove03.Items.Add(hod.GetName() + ", " + hod.GetID());
                                }
                                if (ToRemove03.SelectedItem == null || ToRemove03.Text == "Select a Head of Department to supervise the new Student Teacher")
                                {
                                    MessageBox.Show("Please select a Head of Department to supervise the new Student Teacher.");
                                    return;
                                }
                                // Find the selected HoD object
                                HeadOfDepartment selectedHoD = null;
                                string selectedHoDString = ToRemove03.SelectedItem.ToString();
                                foreach (HeadOfDepartment hod in allHeadsOfDepartment)
                                {
                                    if ((hod.GetName() + ", " + hod.GetID()) == selectedHoDString)
                                    {
                                        selectedHoD = hod;
                                        break;
                                    }
                                }
                                if (selectedHoD == null)
                                {
                                    MessageBox.Show("Selected Head of Department not found.");
                                    return;
                                }
                                ToRemove04.Visible = true;
                                ToRemove04.Items.Clear();
                                ToRemove04.Text = $"Select a course to assign to '{selectedStudent.GetName()}'";
                                // Fill ToRemove04 with all courses in all tracks managed by the selected HoD
                                HashSet<string> courseNames = new HashSet<string>();
                                foreach (StudyTrack track in selectedHoD.GetTracksManaging())
                                {
                                    foreach (Course course in track.GetCourses())
                                    {
                                        // Avoid duplicates by course name
                                        if (courseNames.Add(course.GetName()))
                                        {
                                            ToRemove04.Items.Add(course.GetName());
                                        }
                                    }
                                }
                                if (ToRemove04.SelectedItem == null || ToRemove04.Text == $"Select a course to assign to '{selectedStudent.GetName()}'")
                                {
                                    MessageBox.Show("Please select a course to assign to the new Student Teacher.");
                                    return;
                                }
                                // Find the selected course object
                                Course selectedCourse = null;
                                foreach (StudyTrack track in selectedHoD.GetTracksManaging())
                                {
                                    foreach (Course course in track.GetCourses())
                                    {
                                        if (course.GetName() == ToRemove04.SelectedItem.ToString())
                                        {
                                            // Check if any teacher is already teaching this course
                                            bool isTaught = false;
                                            foreach (Person teacher in selectedHoD.GetSupervising())
                                            {
                                                if (teacher is Professor prof && prof.GetCoursesTeaching().Contains(course))
                                                {
                                                    isTaught = true;
                                                    break;
                                                }
                                                else if (teacher is StudentTeacher st && st.GetCoursesTeaching().Contains(course))
                                                {
                                                    isTaught = true;
                                                    break;
                                                }
                                            }
                                            if (isTaught)
                                            {
                                                MessageBox.Show("This course is already being taught by another teacher. Please select a different course.");
                                                selectedCourse = null;
                                                break;
                                            }
                                            selectedCourse = course;
                                            break;
                                        }
                                    }
                                    if (selectedCourse != null) break;
                                }
                                if (selectedCourse == null)
                                {
                                    MessageBox.Show("Selected course not found.");
                                    return;
                                }
                                // Create a new StudentTeacher with the same info as the selected student
                                StudentTeacher newST = new StudentTeacher(
                                    selectedStudent.GetID(),
                                    selectedStudent.GetName(),
                                    selectedStudent.GetAge(),
                                    selectedStudent.GetPhoneNumber(),
                                    selectedStudent.GetEmail(),
                                    new List<Message>(selectedStudent.GetMessageList()),
                                    selectedStudent.GetFilepath(),
                                    selectedStudent.GetStudentNumber(),
                                    selectedStudent.GetStudyTrack(),
                                    selectedStudent.GetSpecialization(),
                                    selectedStudent.GetCurrentCreditPoints(),
                                    selectedStudent.GetTotalEarnedPoints(),
                                    new List<Course> { selectedCourse } // Assign the selected course
                                );
                                // Replace the student with the new StudentTeacher in the department
                                self.UpdateStudent(selectedStudent, newST);
                                // Add the new StudentTeacher to the selected HoD's Supervising list
                                var supervisingList = selectedHoD.GetSupervising();
                                supervisingList.Add(newST);
                                selectedHoD.SetSupervising(supervisingList);
                                // Optionally, assign the course to the new StudentTeacher as a teacher
                                selectedHoD.AssignTeacherToCourse(newST, selectedCourse);
                                MessageBox.Show($"'{newST.GetName()}' is now a Student Teacher, assigned to '{selectedCourse.GetName()}' and supervised by '{selectedHoD.GetName()}'.");
                            }
                        }
                        else// if(target == "Teacher")
                        {
                            // First, the user needs to select a teacher to update
                            // Make the first combobox visilbe and fill it with the names and ID of the teachers in the list of teachers the user's supervising
                            ToRemove01.Visible = true;
                            ToRemove01.Items.Clear();
                            ToRemove01.Text = "Select a teacher to update";
                            Person selectedTeacher = null;
                            foreach (Person t in self.GetSupervising())
                            {
                                ToRemove01.Items.Add(t.GetName() + ", " + t.GetID());
                            }
                            // Make sure that a teacher was selected
                            if (ToRemove01.SelectedItem == null || ToRemove01.Text == "Select a teacher to update")
                            {
                                MessageBox.Show("Please select a teacher to update.");
                                return;
                            }
                            // Identify the selected student and save it as a student by finding it in the list
                            foreach (Person t in self.GetSupervising())
                            {
                                if (t.GetName() + ", " + t.GetID() == ToRemove01.SelectedItem.ToString())
                                {
                                    selectedTeacher = t;
                                    break;
                                }
                            }
                            if (selectedTeacher != null)
                            {
                                if (selectedTeacher is Professor p)
                                {
                                    // For a professor, we can update their age, phone number, picture, list of courses assigned to them, and specialization
                                    ToRemove02.Visible = true;
                                    ToRemove02.Items.Clear();
                                    ToRemove02.Items.Add("Age");
                                    ToRemove02.Items.Add("Phone Number");
                                    ToRemove02.Items.Add("Picture");
                                    ToRemove02.Items.Add("Courses Teaching");
                                    ToRemove02.Items.Add("Specialization");
                                    if (ToRemove02.SelectedItem == null || ToRemove02.Text == "Select a detail to update")
                                    {
                                        MessageBox.Show("Please select a detail to update.");
                                        return;
                                    }
                                    // Age
                                    string selectedDetail = ToRemove02.SelectedItem.ToString();
                                    Professor newProf = p;
                                    if (selectedDetail == "Age")
                                    {
                                        ToRemove03.Visible = true;
                                        ToRemove03.Items.Clear();
                                        ToRemove03.Text = "Insert the new age of the professor";
                                        if (int.TryParse(ToRemove03.Text, out int newAge) && newAge > 0)
                                        {
                                            newProf.SetAge(newAge);
                                            self.UpdateTeacher(p, newProf);
                                            MessageBox.Show($"'{p.GetName()}' was updated with age '{newAge}'.");
                                        }
                                        else
                                        {
                                            MessageBox.Show("Please insert a valid age.");
                                            return;
                                        }
                                    }
                                    // Phone Number
                                    else if (selectedDetail == "Phone Number")
                                    {
                                        ToRemove03.Visible = true;
                                        ToRemove03.Items.Clear();
                                        ToRemove03.Text = "Insert the new phone number of the professor";
                                        string newPhoneNumber = ToRemove03.Text;
                                        newProf.SetPhoneNumber(newPhoneNumber);
                                        self.UpdateTeacher(p, newProf);
                                        MessageBox.Show($"'{p.GetName()}' was updated with phone number '{newPhoneNumber}'.");
                                    }
                                    else if (selectedDetail == "Picture")
                                    {
                                        OpenFileDialog openFileDialog = new OpenFileDialog();
                                        openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                                        {
                                            string newFilepath = openFileDialog.FileName;
                                            newProf.SetFilepath(newFilepath);
                                            self.UpdateTeacher(p, newProf);
                                            MessageBox.Show($"'{p.GetName()}' was updated with a new picture.");
                                        }
                                    }
                                    // Courses Teaching
                                    else if (selectedDetail == "Courses Teaching")
                                    {
                                        ToRemove03.Visible = true;
                                        ToRemove03.Items.Clear();
                                        ToRemove03.Text = "Do you wish to assign or unassign a course?";
                                        ToRemove03.Items.Add("Assigne Course");
                                        ToRemove03.Items.Add("Unassign Course");
                                        if (ToRemove03.SelectedItem == null || ToRemove03.Text == "Do you wish to assign or unassign a course?")
                                        {
                                            MessageBox.Show("Please select whether you want to assign or unassign a course.");
                                            return;
                                        }
                                        if (ToRemove03.SelectedItem == "Assign Course")
                                        {
                                            ToRemove04.Visible = true;
                                            ToRemove04.Items.Clear();
                                            ToRemove04.Text = "Select a course";
                                            foreach (StudyTrack track in self.GetTracksManaging())
                                            {
                                                foreach (Course course in track.GetCourses())
                                                {
                                                    ToRemove04.Items.Add(course.GetName());
                                                }
                                            }
                                            if (ToRemove04.SelectedItem == null || ToRemove04.Text == "Select a course to assign")
                                            {
                                                MessageBox.Show("Please select a course to assign.");
                                                return;
                                            }
                                            // Identify the selected course and save it as a course by finding it in the list
                                            Course selectedCourse = null;
                                            foreach (StudyTrack track in self.GetTracksManaging())
                                            {
                                                foreach (Course course in track.GetCourses())
                                                {
                                                    if (course.GetName() == ToRemove04.SelectedItem.ToString())
                                                    {
                                                        selectedCourse = course;
                                                        break;
                                                    }
                                                }
                                            }
                                            if (selectedCourse != null)
                                            {
                                                // Make sure no teachers are aleady assigned to the course, and if any are, unassign them
                                                foreach (Person teacher in self.GetSupervising())
                                                {
                                                    if (teacher is Professor prof)
                                                    {
                                                        if (prof.GetCoursesTeaching().Contains(selectedCourse))
                                                        {
                                                            self.UnassignTeacherFromCourse(prof, selectedCourse);
                                                            break;
                                                        }
                                                    }
                                                    else if (teacher.GetType().Name == "StudentTeacher")
                                                    {
                                                        var st = (StudentTeacher)teacher;
                                                        if (st.GetCoursesTeaching().Contains(selectedCourse))
                                                        {
                                                            self.UnassignTeacherFromCourse(st, selectedCourse);
                                                            break;
                                                        }
                                                    }
                                                }
                                                self.AssignTeacherToCourse(newProf, selectedCourse);
                                                self.UpdateTeacher(p, newProf);
                                                MessageBox.Show($"'{p.GetName()}' was assigned to course '{selectedCourse.GetName()}'.");
                                            }
                                        }
                                        else // if (ToRemove03.SelectedItem == "Unassign Course")
                                        {
                                            ToRemove04.Visible = true;
                                            ToRemove04.Items.Clear();
                                            ToRemove04.Text = "Select a course";
                                            // List the courses
                                            foreach (Course c in p.GetCoursesTeaching())
                                            {
                                                ToRemove04.Items.Add(c.GetName());
                                            }
                                            if (ToRemove04.SelectedItem == null || ToRemove04.Text == "Select a course to unassign")
                                            {
                                                MessageBox.Show("Please select a course to unassign.");
                                                return;
                                            }
                                            // Identify the course selected
                                            Course selectedCourse = null;
                                            foreach (Course c in p.GetCoursesTeaching())
                                            {
                                                if (c.GetName() == ToRemove04.SelectedItem.ToString())
                                                {
                                                    selectedCourse = c;
                                                    break;
                                                }
                                            }
                                            if (selectedCourse != null)
                                            {
                                                self.UnassignTeacherFromCourse(newProf, selectedCourse);
                                                self.UpdateTeacher(p, newProf);
                                                MessageBox.Show($"'{p.GetName()}' was assigned to course '{selectedCourse.GetName()}'.");
                                            }
                                        }
                                    }
                                    else if (selectedDetail == "Specialization")
                                    {
                                        ToRemove03.Visible = true;
                                        ToRemove03.Items.Clear();
                                        ToRemove03.Text = "Insert the new specialization of the professor";
                                        string newSpecialization = ToRemove03.Text;
                                        newProf.SetSpecialization(newSpecialization);
                                        self.UpdateTeacher(p, newProf);
                                        MessageBox.Show($"'{p.GetName()}' was updated with specialization '{newSpecialization}'.");
                                    }
                                }
                                else if (selectedTeacher is StudentTeacher st)
                                {
                                    // For a student teacher, we can update their age, phone number, picture, study track, specialization, currentCreditPoints, totalEarnedPoints,
                                    // and the list of courses assigned to them
                                    ToRemove02.Visible = true;
                                    ToRemove02.Items.Clear();
                                    ToRemove02.Items.Add("Age");
                                    ToRemove02.Items.Add("Phone Number");
                                    ToRemove02.Items.Add("Picture");
                                    ToRemove02.Items.Add("Study Track");
                                    ToRemove02.Items.Add("Specialization");
                                    ToRemove02.Items.Add("Current Credit Points");
                                    ToRemove02.Items.Add("Total Credit Points");
                                    ToRemove02.Items.Add("Courses Teaching");
                                    if (ToRemove02.SelectedItem == null || ToRemove02.Text == "Select a detail to update")
                                    {
                                        MessageBox.Show("Please select a detail to update.");
                                        return;
                                    }
                                    string selectedDetail = ToRemove02.SelectedItem.ToString();
                                    StudentTeacher newST = st;
                                    if (selectedDetail == "Age")
                                    {
                                        ToRemove03.Visible = true;
                                        ToRemove03.Items.Clear();
                                        ToRemove03.Text = "Insert the new age of the student teacher";
                                        if (int.TryParse(ToRemove03.Text, out int newAge) && newAge > 0)
                                        {
                                            newST.SetAge(newAge);
                                            self.UpdateTeacher(st, newST);
                                            MessageBox.Show($"'{st.GetName()}' was updated with age '{newAge}'.");
                                        }
                                        else
                                        {
                                            MessageBox.Show("Please insert a valid age.");
                                            return;
                                        }
                                    }
                                    else if (selectedDetail == "Phone Number")
                                    {
                                        ToRemove03.Visible = true;
                                        ToRemove03.Text = "Insert the new phone number of the student teacher";
                                        string newPhoneNumber = ToRemove03.Text;
                                        newST.SetPhoneNumber(newPhoneNumber);
                                        self.UpdateTeacher(st, newST);
                                        MessageBox.Show($"'{st.GetName()}' was updated with phone number '{newPhoneNumber}'.");
                                    }
                                    else if (selectedDetail == "Picture")
                                    {
                                        OpenFileDialog openFileDialog = new OpenFileDialog();
                                        openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                                        {
                                            string newFilepath = openFileDialog.FileName;
                                            newST.SetFilepath(newFilepath);
                                            self.UpdateTeacher(st, newST);
                                            MessageBox.Show($"'{st.GetName()}' was updated with a new picture.");
                                        }
                                    }
                                    else if (selectedDetail == "Study Track")
                                    {
                                        ToRemove03.Visible = true;
                                        ToRemove03.Items.Clear();
                                        ToRemove03.Text = "Select a new study track for the student teacher";
                                        StudyTrack selectedTrack = null;
                                        foreach (StudyTrack track in self.GetTracksManaging())
                                        {
                                            ToRemove03.Items.Add(track.GetName());
                                        }
                                        if (ToRemove03.SelectedItem == null || ToRemove03.Text == "Select a new study track for the student teacher")
                                        {
                                            MessageBox.Show("Please select a new study track for the student teacher.");
                                            return;
                                        }
                                        foreach (StudyTrack track in self.GetTracksManaging())
                                        {
                                            if (track.GetName() == ToRemove03.SelectedItem.ToString())
                                            {
                                                selectedTrack = track;
                                                break;
                                            }
                                        }
                                        if (selectedTrack != null)
                                        {
                                            self.AssignStudentToTrack(newST, selectedTrack);
                                            self.UpdateTeacher(st, newST);
                                            MessageBox.Show($"'{st.GetName()}' was updated with new study track '{selectedTrack.GetName()}'.");
                                        }
                                    }
                                    else if (selectedDetail == "Specialization")
                                    {
                                        ToRemove03.Visible = true;
                                        ToRemove03.Text = "Insert the new specialization of the student teacher";
                                        string newSpecialization = ToRemove03.Text;
                                        newST.SetSpecialization(newSpecialization);
                                        self.UpdateTeacher(st, newST);
                                        MessageBox.Show($"'{st.GetName()}' was updated with specialization '{newSpecialization}'.");
                                    }
                                    else if (selectedDetail == "Current Credit Points")
                                    {
                                        ToRemove03.Visible = true;
                                        ToRemove03.Text = "Insert the new current credit points of the student teacher";
                                        if (int.TryParse(ToRemove03.Text, out int newCurrentCreditPoints) && newCurrentCreditPoints >= 0)
                                        {
                                            newST.SetCurrentCreditPoints(newCurrentCreditPoints);
                                            self.UpdateTeacher(st, newST);
                                            MessageBox.Show($"'{st.GetName()}' was updated with current credit points '{newCurrentCreditPoints}'.");
                                        }
                                        else
                                        {
                                            MessageBox.Show("Please insert a valid number of current credit points.");
                                            return;
                                        }
                                    }
                                    else if (selectedDetail == "Total Credit Points")
                                    {
                                        ToRemove03.Visible = true;
                                        ToRemove03.Text = "Insert the new total credit points of the student teacher";
                                        if (int.TryParse(ToRemove03.Text, out int newTotalEarnedPoints) && newTotalEarnedPoints >= 0)
                                        {
                                            newST.SetTotalEarnedPoints(newTotalEarnedPoints);
                                            self.UpdateTeacher(st, newST);
                                            MessageBox.Show($"'{st.GetName()}' was updated with total credit points '{newTotalEarnedPoints}'.");
                                        }
                                        else
                                        {
                                            MessageBox.Show("Please insert a valid number of total credit points.");
                                            return;
                                        }
                                    }
                                    else if (selectedDetail == "Courses Teaching")
                                    {
                                        ToRemove03.Visible = true;
                                        ToRemove03.Items.Clear();
                                        ToRemove03.Text = "Do you wish to assign or unassign a course?";
                                        ToRemove03.Items.Add("Assigne Course");
                                        ToRemove03.Items.Add("Unassign Course");
                                        if (ToRemove03.SelectedItem == null || ToRemove03.Text == "Do you wish to assign or unassign a course?")
                                        {
                                            MessageBox.Show("Please select whether you want to assign or unassign a course.");
                                            return;
                                        }
                                        if (ToRemove03.SelectedItem == "Assign Course")
                                        {
                                            ToRemove04.Visible = true;
                                            ToRemove04.Items.Clear();
                                            ToRemove04.Text = "Select a course";
                                            foreach (StudyTrack track in self.GetTracksManaging())
                                            {
                                                foreach (Course course in track.GetCourses())
                                                {
                                                    ToRemove04.Items.Add(course.GetName());
                                                }
                                            }
                                            if (ToRemove04.SelectedItem == null || ToRemove04.Text == "Select a course to assign")
                                            {
                                                MessageBox.Show("Please select a course to assign.");
                                                return;
                                            }
                                            // Identify the selected course and save it as a course by finding it in the list
                                            Course selectedCourse = null;
                                            foreach (StudyTrack track in self.GetTracksManaging())
                                            {
                                                foreach (Course course in track.GetCourses())
                                                {
                                                    if (course.GetName() == ToRemove04.SelectedItem.ToString())
                                                    {
                                                        selectedCourse = course;
                                                        break;
                                                    }
                                                }
                                            }
                                            if (selectedCourse != null)
                                            {
                                                // Make sure no teachers are aleady assigned to the course, and if any are, unassign them
                                                foreach (Person teacher in self.GetSupervising())
                                                {
                                                    if (teacher is Professor prof)
                                                    {
                                                        if (prof.GetCoursesTeaching().Contains(selectedCourse))
                                                        {
                                                            self.UnassignTeacherFromCourse(prof, selectedCourse);
                                                            break;
                                                        }
                                                    }
                                                    else if (teacher is StudentTeacher studT)
                                                    {
                                                        if (studT.GetCoursesTeaching().Contains(selectedCourse))
                                                        {
                                                            self.UnassignTeacherFromCourse(studT, selectedCourse);
                                                            break;
                                                        }
                                                    }
                                                }
                                                self.AssignTeacherToCourse(newST, selectedCourse);
                                                self.UpdateTeacher(st, newST);
                                                MessageBox.Show($"'{newST.GetName()}' was assigned to course '{selectedCourse.GetName()}'.");
                                            }
                                        }
                                        else
                                        {
                                            ToRemove04.Visible = true;
                                            ToRemove04.Items.Clear();
                                            ToRemove04.Text = "Select a course to unassign";
                                            foreach (Course c in st.GetCoursesTeaching())
                                            {
                                                ToRemove04.Items.Add(c.GetName());
                                            }
                                            if (ToRemove04.SelectedItem == null || ToRemove04.Text == "Select a course to unassign")
                                            {
                                                MessageBox.Show("Please select a course to unassign.");
                                                return;
                                            }
                                            Course selectedCourse = null;
                                            foreach (Course c in st.GetCoursesTeaching())
                                            {
                                                if (c.GetName() == ToRemove04.SelectedItem.ToString())
                                                {
                                                    selectedCourse = c;
                                                    break;
                                                }
                                            }
                                            if (selectedCourse != null)
                                            {
                                                self.UnassignTeacherFromCourse(newST, selectedCourse);
                                                self.UpdateTeacher(st, newST);
                                                MessageBox.Show($"'{st.GetName()}' was assigned to course '{selectedCourse.GetName()}'.");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // if (action == "Create")
                    else
                    {
                        ToRemove.Visible = true;
                        ToRemove01.Visible = false;
                        ToRemove02.Visible = false;
                        ToRemove03.Visible = false;
                        ToRemove04.Visible = false;
                        ToRemove05.Visible = false;
                        ToRemove06.Visible = false;
                        ToRemove07.Visible = false;
                        ToRemove08.Visible = false;
                        ToRemove09.Visible = false;
                        ToRemove10.Visible = false;
                        ToRemove11.Visible = false;
                        if (target == "Course")
                        {
                            ToRemove02.Items.Clear();
                            ToRemove03.Items.Clear();
                            ToRemove04.Items.Clear();
                            ToRemove02.Visible = true;
                            ToRemove02.Text = "Enter the name of the new course";
                            ToRemove03.Visible = true;
                            ToRemove03.Items.Clear();
                            ToRemove03.Text = "Select a study track to assign the course to";
                            foreach (StudyTrack track in self.GetTracksManaging())
                            {
                                ToRemove03.Items.Add(track.GetName());
                            }
                            ToRemove04.Visible = true;
                            ToRemove04.Items.Clear();
                            ToRemove04.Text = "Select a teacher to assign this course to";
                            foreach (Person teacher in self.GetSupervising())
                            {
                                ToRemove04.Items.Add(teacher.GetName() + ", " + teacher.GetID());
                            }
                            // Validation
                            if (string.IsNullOrWhiteSpace(ToRemove02.Text) || ToRemove02.Text == "Enter the name of the new course")
                            {
                                MessageBox.Show("Please enter a valid course name.");
                                return;
                            }
                            if (ToRemove03.SelectedItem == null || ToRemove03.Text == "Select a study track to assign the course to")
                            {
                                MessageBox.Show("Select a study track");
                                return;
                            }
                            if (ToRemove04.SelectedItem == null || ToRemove04.Text == "Select a teacher to assign this course to")
                            {
                                MessageBox.Show("Select a teacher");
                                return;
                            }
                            // Identify selected study track
                            StudyTrack selectedTrack = null;
                            foreach (StudyTrack track in self.GetTracksManaging())
                            {
                                if (track.GetName() == ToRemove03.SelectedItem.ToString())
                                {
                                    selectedTrack = track;
                                    break;
                                }
                            }
                            if (selectedTrack == null)
                            {
                                MessageBox.Show("Selected study track not found.");
                                return;
                            }
                            // Identify selected teacher
                            Person selectedTeacher = null;
                            foreach (Person teacher in self.GetSupervising())
                            {
                                if ((teacher.GetName() + ", " + teacher.GetID()) == ToRemove04.SelectedItem.ToString())
                                {
                                    selectedTeacher = teacher;
                                    break;
                                }
                            }
                            if (selectedTeacher == null)
                            {
                                MessageBox.Show("Selected teacher not found.");
                                return;
                            }
                            // Create the new course with the same student list as the study track
                            string courseName = ToRemove02.Text.Trim();
                            List<Student> courseStudents = new List<Student>(selectedTrack.GetStudents());
                            Course newCourse = new Course(courseName, courseStudents);
                            // Add the course to the study track
                            selectedTrack.GetCourses().Add(newCourse);
                            // Assign the course to the selected teacher
                            self.AssignTeacherToCourse(selectedTeacher, newCourse);
                            // Confirmation
                            MessageBox.Show($"Course '{courseName}' created, assigned to '{selectedTeacher.GetName()}', and added to study track '{selectedTrack.GetName()}'.");
                        }
                        else if (target == "Study Track")
                        {
                            // --- Study Track Creation Sub-Segment ---
                            ToRemove02.Items.Clear();
                            ToRemove03.Items.Clear();
                            ToRemove04.Items.Clear();
                            ToRemove02.Visible = true;
                            ToRemove02.Text = "Enter the name of the new study track";
                            ToRemove03.Visible = true;
                            ToRemove03.Text = "Select a course to add to the new track";
                            foreach (StudyTrack track in self.GetTracksManaging())
                            {
                                foreach (Course course in track.GetCourses())
                                {
                                    ToRemove03.Items.Add(course.GetName());
                                }
                            }
                            ToRemove04.Visible = true;
                            ToRemove04.Text = "Select a student to add to the new track";
                            foreach (Student student in self.GetStudentsInDepartment())
                            {
                                ToRemove04.Items.Add(student.GetName() + ", " + student.GetID());
                            }
                            // Validation
                            if (string.IsNullOrWhiteSpace(ToRemove02.Text) || ToRemove02.Text == "Enter the name of the new study track")
                            {
                                MessageBox.Show("Please enter a valid study track name.");
                                return;
                            }
                            if (ToRemove03.SelectedItem == null || ToRemove03.Text == "Select a course to add to the new track")
                            {
                                MessageBox.Show("Please select a course to add.");
                                return;
                            }
                            if (ToRemove04.SelectedItem == null || ToRemove04.Text == "Select a student to add to the new track")
                            {
                                MessageBox.Show("Please select a student to add.");
                                return;
                            }
                            // Identify selected course
                            Course selectedCourse = null;
                            foreach (StudyTrack track in self.GetTracksManaging())
                            {
                                foreach (Course course in track.GetCourses())
                                {
                                    if (course.GetName() == ToRemove03.SelectedItem.ToString())
                                    {
                                        selectedCourse = course;
                                        break;
                                    }
                                }
                                if (selectedCourse != null) break;
                            }
                            if (selectedCourse == null)
                            {
                                MessageBox.Show("Selected course not found.");
                                return;
                            }
                            // Identify selected student
                            Student selectedStudent = null;
                            foreach (Student student in self.GetStudentsInDepartment())
                            {
                                if ((student.GetName() + ", " + student.GetID()) == ToRemove04.SelectedItem.ToString())
                                {
                                    selectedStudent = student;
                                    break;
                                }
                            }
                            if (selectedStudent == null)
                            {
                                MessageBox.Show("Selected student not found.");
                                return;
                            }
                            // Create the new study track with the selected course and student
                            string trackName = ToRemove02.Text.Trim();
                            List<Course> trackCourses = new List<Course> { selectedCourse };
                            List<Student> trackStudents = new List<Student> { selectedStudent };
                            StudyTrack newTrack = new StudyTrack(trackName, trackCourses, trackStudents);
                            // Add the new track to the HoD's managed tracks
                            self.AddTrack(newTrack);
                            // Confirmation and invitation
                            MessageBox.Show(
                                $"Study track '{trackName}' created with course '{selectedCourse.GetName()}' and student '{selectedStudent.GetName()}'.\n" +
                                "You may now assign or create more courses and assign more students for this track."
                            );
                        }
                        else if (target == "Student")
                        {
                            // --- Student Creation Sub-Segment ---
                            ToRemove01.Visible = true; // Student ID
                            ToRemove02.Visible = true; // Name
                            ToRemove03.Visible = true; // Age
                            ToRemove04.Visible = true; // Phone Number
                            ToRemove05.Visible = true; // Email
                            ToRemove06.Visible = true; // Filepath
                            ToRemove07.Visible = true; // Study Track
                            ToRemove08.Visible = true; // Specialization
                            ToRemove09.Visible = true; // Total Credit Points
                            ToRemove01.Items.Clear();
                            ToRemove02.Items.Clear();
                            ToRemove03.Items.Clear();
                            ToRemove04.Items.Clear();
                            ToRemove05.Items.Clear();
                            ToRemove06.Items.Clear();
                            ToRemove07.Items.Clear();
                            ToRemove08.Items.Clear();
                            ToRemove09.Items.Clear();
                            ToRemove01.Text = "Enter the student's ID";
                            ToRemove02.Text = "Enter the student's name";
                            ToRemove03.Text = "Enter the studdent's age";
                            ToRemove04.Text = "Enter the student's phone number";
                            ToRemove05.Text = "Enter the student's email";
                            ToRemove06.Text = "Enter the student's image filepath";
                            ToRemove07.Text = "Select a study track";
                            foreach (StudyTrack track in self.GetTracksManaging())
                            {
                                ToRemove07.Items.Add(track.GetName());
                            }
                            ToRemove08.Text = "Enter the student's specialization";
                            ToRemove09.Text = "Enter the student's total credit points";
                            // Validation
                            if (string.IsNullOrWhiteSpace(ToRemove01.Text))
                            {
                                MessageBox.Show("Please enter the student's ID.");
                                return;
                            }
                            if (string.IsNullOrWhiteSpace(ToRemove02.Text))
                            {
                                MessageBox.Show("Please enter the student's name.");
                                return;
                            }
                            if (string.IsNullOrWhiteSpace(ToRemove04.Text) || !int.TryParse(ToRemove04.Text, out int age) || age <= 0)
                            {
                                MessageBox.Show("Please enter the student's age.");
                                return;
                            }
                            if (string.IsNullOrWhiteSpace(ToRemove04.Text))
                            {
                                MessageBox.Show("Please enter the student's phone number.");
                                return;
                            }
                            if (string.IsNullOrWhiteSpace(ToRemove05.Text))
                            {
                                MessageBox.Show("Please enter the student's email.");
                                return;
                            }
                            if (string.IsNullOrWhiteSpace(ToRemove06.Text))
                            {
                                MessageBox.Show("Please enter the student's image filepath.");
                                return;
                            }
                            if (ToRemove07.SelectedItem == null)
                            {
                                MessageBox.Show("Please select a study track.");
                                return;
                            }
                            if (string.IsNullOrWhiteSpace(ToRemove08.Text))
                            {
                                MessageBox.Show("Please enter the student's specialization.");
                                return;
                            }
                            if (string.IsNullOrWhiteSpace(ToRemove09.Text) || !int.TryParse(ToRemove09.Text, out int totalCreditPoints) || totalCreditPoints <= 0)
                            {
                                MessageBox.Show("Please enter a valid total credit points value.");
                                return;
                            }
                            // Identify selected study track
                            StudyTrack selectedTrack = null;
                            foreach (StudyTrack track in self.GetTracksManaging())
                            {
                                if (track.GetName() == ToRemove07.SelectedItem.ToString())
                                {
                                    selectedTrack = track;
                                    break;
                                }
                            }
                            if (selectedTrack == null)
                            {
                                MessageBox.Show("Selected study track not found.");
                                return;
                            }
                            // --- Ensure Student ID is Unique ---
                            string enteredId = ToRemove01.Text.Trim();
                            bool unique = true;
                            foreach (HeadOfDepartment hod in allHeadsOfDepartment)
                            {
                                foreach (Student student in hod.GetStudentsInDepartment())
                                {
                                    if (student.GetID() == enteredId)
                                    {
                                        unique = false;
                                        break;
                                    }
                                }
                                if (!unique) break;
                            }
                            if (!unique)
                            {
                                MessageBox.Show("This student ID is already in use. Please enter a unique ID.");
                                return;
                            }
                            // Generate a unique 8-digit student number
                            Random rand = new Random();
                            int studentNumber;
                            unique = true;
                            do
                            {
                                studentNumber = rand.Next(10000000, 100000000); // 8-digit number
                                unique = true;
                                foreach (HeadOfDepartment hod in allHeadsOfDepartment)
                                {
                                    foreach (Student student in hod.GetStudentsInDepartment())
                                    {
                                        if (student.GetStudentNumber() == studentNumber)
                                        {
                                            unique = false;
                                            break;
                                        }
                                    }
                                    if (!unique) break;
                                }
                            } while (!unique);
                            // Create the new student
                            string id = enteredId;
                            string name = ToRemove02.Text.Trim();
                            string phoneNumber = ToRemove04.Text.Trim();
                            string email = ToRemove05.Text.Trim();
                            string filepath = ToRemove06.Text.Trim();
                            List<Message> messageList = new List<Message>();
                            string specialization = ToRemove08.Text.Trim();
                            int currentCreditPoints = 0;

                            Student newStudent = new Student(
                                id, name, age, phoneNumber, email, messageList, filepath,
                                studentNumber, selectedTrack, specialization, currentCreditPoints, totalCreditPoints
                            );
                            // Add the student to the department and study track
                            self.GetStudentsInDepartment().Add(newStudent);
                            selectedTrack.GetStudents().Add(newStudent);
                            // Confirmation
                            MessageBox.Show(
                                $"Student '{name}' created and assigned to study track '{selectedTrack.GetName()}'.\n" +
                                $"Student Number: {studentNumber}\nYou may now assign courses or update student details."
                            );
                        }
                        // if (target == "Teacher")
                        else
                        {
                            // Step 1: Select type of teacher
                            ToRemove01.Visible = true;
                            ToRemove01.Items.Clear();
                            ToRemove01.Text = "Select teacher type";
                            ToRemove01.Items.Add("Professor");
                            ToRemove01.Items.Add("Student Teacher");

                            // If user selects "Student Teacher"
                            if (ToRemove01.SelectedItem != null && ToRemove01.SelectedItem.ToString() == "Student Teacher")
                            {
                                // Show and prepare all fields for Student Teacher creation
                                ToRemove02.Visible = true; // ID
                                ToRemove03.Visible = true; // Name
                                ToRemove04.Visible = true; // Age
                                ToRemove05.Visible = true; // Phone Number
                                ToRemove06.Visible = true; // Email
                                ToRemove07.Visible = true; // Filepath
                                ToRemove08.Visible = true; // HoD
                                ToRemove09.Visible = true; // Study Track
                                ToRemove10.Visible = true; // Specialization
                                ToRemove11.Visible = true; // Total Credit Points
                                ToRemove01.Items.Clear();
                                ToRemove02.Items.Clear();
                                ToRemove03.Items.Clear();
                                ToRemove04.Items.Clear();
                                ToRemove05.Items.Clear();
                                ToRemove06.Items.Clear();
                                ToRemove07.Items.Clear();
                                ToRemove08.Items.Clear();
                                ToRemove09.Items.Clear();
                                ToRemove10.Items.Clear();
                                ToRemove11.Items.Clear();
                                ToRemove01.Text = "Enter the student's ID";
                                ToRemove02.Text = "Enter the student's name";
                                ToRemove03.Text = "Enter the studdent's age";
                                ToRemove04.Text = "Enter the student's phone number";
                                ToRemove05.Text = "Enter the student's email";
                                ToRemove06.Text = "Enter the student's image filepath";
                                ToRemove08.Text = "Select a Head of Department";
                                ToRemove09.Text = "Select a study track";
                                ToRemove10.Text = "Enter the student's specialization";
                                ToRemove11.Text = "Enter the student's total credit points";
                                // Setup HoDs
                                ToRemove08.Items.Clear();
                                ToRemove08.Text = "Select a Head of Department";
                                foreach (HeadOfDepartment hod in allHeadsOfDepartment)
                                {
                                    ToRemove08.Items.Add(hod.GetName() + ", " + hod.GetID());
                                }
                                // Find selected HoD
                                HeadOfDepartment selectedHoD = null;
                                string selectedHoDString = ToRemove08.SelectedItem.ToString();
                                foreach (HeadOfDepartment hod in allHeadsOfDepartment)
                                {
                                    if ((hod.GetName() + ", " + hod.GetID()) == selectedHoDString)
                                    {
                                        selectedHoD = hod;
                                        break;
                                    }
                                }
                                if (selectedHoD == null)
                                {
                                    MessageBox.Show("Selected Head of Department not found.");
                                    return;
                                }
                                // Setup study tracks
                                ToRemove09.Items.Clear();
                                ToRemove09.Text = "Select a study track";
                                foreach (StudyTrack track in selectedHoD.GetTracksManaging())
                                {
                                    ToRemove09.Items.Add(track.GetName());
                                }
                                // Find selected study track
                                StudyTrack selectedTrack = null;
                                foreach (StudyTrack track in selectedHoD.GetTracksManaging())
                                {
                                    if (track.GetName() == ToRemove09.SelectedItem.ToString())
                                    {
                                        selectedTrack = track;
                                        break;
                                    }
                                }
                                if (selectedTrack != null)
                                {
                                    MessageBox.Show("Selected study track not found.");
                                    return;
                                }
                                // Validation
                                if (string.IsNullOrWhiteSpace(ToRemove02.Text))
                                {
                                    MessageBox.Show("Please enter the ID.");
                                    return;
                                }
                                if (string.IsNullOrWhiteSpace(ToRemove03.Text))
                                {
                                    MessageBox.Show("Please enter the name.");
                                    return;
                                }
                                if (string.IsNullOrWhiteSpace(ToRemove04.Text) || !int.TryParse(ToRemove04.Text, out int age) || age <= 0)
                                {
                                    MessageBox.Show("Please enter a valid age.");
                                    return;
                                }
                                if (string.IsNullOrWhiteSpace(ToRemove05.Text))
                                {
                                    MessageBox.Show("Please enter the phone number.");
                                    return;
                                }
                                if (string.IsNullOrWhiteSpace(ToRemove06.Text))
                                {
                                    MessageBox.Show("Please enter the email.");
                                    return;
                                }
                                if (string.IsNullOrWhiteSpace(ToRemove07.Text))
                                {
                                    MessageBox.Show("Please enter the image filepath.");
                                    return;
                                }
                                if (ToRemove08.SelectedItem == null)
                                {
                                    MessageBox.Show("Please select a Head of Department.");
                                    return;
                                }
                                if (ToRemove09.SelectedItem == null)
                                {
                                    MessageBox.Show("Please select a study track.");
                                    return;
                                }
                                if (string.IsNullOrWhiteSpace(ToRemove10.Text))
                                {
                                    MessageBox.Show("Please enter the specialization.");
                                    return;
                                }
                                if (string.IsNullOrWhiteSpace(ToRemove11.Text) || !int.TryParse(ToRemove11.Text, out int totalCreditPoints) || totalCreditPoints <= 0)
                                {
                                    MessageBox.Show("Please enter a valid total credit points value.");
                                    return;
                                }
                                // Ensure unique student number
                                Random rand = new Random();
                                int studentNumber;
                                bool unique;
                                do
                                {
                                    studentNumber = rand.Next(10000000, 100000000); // 8-digit number
                                    unique = true;
                                    foreach (HeadOfDepartment hod in allHeadsOfDepartment)
                                    {
                                        foreach (Student student in hod.GetStudentsInDepartment())
                                        {
                                            if (student.GetStudentNumber() == studentNumber)
                                            {
                                                unique = false;
                                                break;
                                            }
                                        }
                                        if (!unique) break;
                                    }
                                } while (!unique);
                                // Create the StudentTeacher
                                StudentTeacher newST = new StudentTeacher(
                                    ToRemove02.Text.Trim(), // id
                                    ToRemove03.Text.Trim(), // name
                                    age,
                                    ToRemove05.Text.Trim(), // phone number
                                    ToRemove06.Text.Trim(), // email
                                    new List<Message>(),    // message list
                                    ToRemove07.Text.Trim(), // filepath
                                    studentNumber,
                                    selectedTrack,
                                    ToRemove09.Text.Trim(), // specialization
                                    0,                      // currentCreditPoints
                                    totalCreditPoints,
                                    new List<Course>()      // courses teaching (empty at creation)
                                );
                                // Add to HoD's Supervising list
                                var supervisingList = selectedHoD.GetSupervising();
                                supervisingList.Add(newST);
                                selectedHoD.SetSupervising(supervisingList);
                                // Add to HoD's StudentsInDepartment list
                                var studentsList = selectedHoD.GetStudentsInDepartment();
                                studentsList.Add(newST);
                                selectedHoD.SetStudentsInDepartment(studentsList);
                                MessageBox.Show($"Student Teacher '{newST.GetName()}' created and assigned to '{selectedTrack.GetName()}', supervised by '{selectedHoD.GetName()}'.\nStudent Number: {studentNumber}");
                            }
                        }
                    }
                }
            }
        }
    }
}
