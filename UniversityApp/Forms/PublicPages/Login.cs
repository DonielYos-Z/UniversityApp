using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace UniversityApp
{
    public partial class Login : Form
    {
        // This line is for after the user logs in, it will be used to show the logged in user in the main app
        public string LoggedInLine { get; private set; }
        public Login()
        {
            InitializeComponent();
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                // Convert the byte array to a string.
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                    builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string username = textBox1.Text.Trim();
                string password = textBox2.Text.Trim();
                string filePath = "users.txt";
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    throw new ArgumentException("Please enter both username and password!");
                }

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("User file not found.");
                }
                bool found = false;
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string hashedPassword = HashPassword(password);
                    if (line.Contains($"Username: {username}") && line.Contains($"Password: {hashedPassword}"))
                    {
                        if (line.Contains("Approval: Approved"))
                        {
                            found = true;
                            LoggedInLine = line;
                            break;
                        }
                        else
                        {
                            throw new UnauthorizedAccessException("Your account is not approved yet. Please wait for approval.");
                        }
                    }
                }
                if (found)
                {
                    this.Hide();
                    MainApp mainForm = new MainApp(this);
                    mainForm.Show();
                }
                else
                {
                    throw new UnauthorizedAccessException("Invalid username or password.");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Clear();
                textBox2.Clear();
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Clear();
                textBox2.Clear();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Register registerForm = new Register(this);
            registerForm.Show();
        }
    }
}
