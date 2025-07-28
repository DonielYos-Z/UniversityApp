using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using UniversityApp.Forms.DataModels;
using Message = UniversityApp.Forms.DataModels.Message;
// List of example accounts:
// Student- Username: Doniel, Password: Zaner
// Professor- Username: Shahar, Password: IMEvil
// Hod- Username: HeadOfDepartment, Password: HoD
namespace UniversityApp
{
    public partial class MainApp : Form
    {
        Form form;
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
                // Hide the Study Track textbox, and the listBox for displaying details
                textBox7.Visible = false;
                ToDisplay.Visible = false;
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
                        if(HoD.GetCoursesTeaching().Count() == 0)
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
            else if(SearchResults.Items.Count == 1)
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
            if(string.IsNullOrEmpty(action) || string.IsNullOrEmpty(target))
            {
                MessageBox.Show("Please enter an action and a target.");
                return;
            }
            else
            {
                if (action == "Display") {
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
                if (action == "Approve")
                {
                    if (target == "Course" || target == "Study Track")
                    {
                        MessageBox.Show("The 'Approve' action is only applicable to students and teachers.");
                        return;
                    }
                    else if(target == "Student")
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
                if (action == "Remove")
                {
                    if (target == "Course") { }
                    else if (target == "Study Track") { }
                    else if (target == "Student") { }
                    else { }
                }
                if (action == "Update")
                {
                    if (target == "Course") { }
                    else if (target == "Study Track") { }
                    else if (target == "Student") { }
                    else { }
                }
                if (action == "Create")
                {
                    if (target == "Course") { }
                    else if (target == "Study Track") { }
                    else if (target == "Student") { }
                    else { }
                }
            }
        }
    }
}
