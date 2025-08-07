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

namespace UniversityApp
{
    public partial class SendMessage : Form
    {
        public SendMessage(string recipientEmail)
        {
            InitializeComponent();
            textBox1.Text = recipientEmail;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string subject = textBox2.Text;
            string content = textBox3.Text;

            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show("Please fill in both the subject and the message.");
                return;
            }

            string message = $"To: {textBox1.Text}\nSubject: {subject}\nContent: {content}\nDate: {DateTime.Now}\n";
            string path = $"messages_{textBox1.Text}.txt"; // Or a central messages store
            File.AppendAllText(path, message + "\n---\n");

            MessageBox.Show("Message sent successfully!");
            this.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Are you sure you want to cancel?", "Confirm", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}
