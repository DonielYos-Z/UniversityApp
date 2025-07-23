using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityApp.Forms.DataModels
{
    // Abstract super-class Person
    // I like to call it the super-duper-class, because it extends to other super-classes
    internal abstract class Person
    {
        // Attributes
        protected string ID;
        protected string Name;
        protected int Age;
        protected string PhoneNumber;
        protected string Email;
        protected List<Message> MessageList;
        protected string Filepath;
        // Constructor
        // The filepath is a string that points to the image file of the person
        protected Person(string id, string name, int age, string phoneNumber, string email, List<Message> messageList, string filepath)
        {
            SetID(id);
            SetName(name);
            SetAge(age);
            SetPhoneNumber(phoneNumber);
            SetEmail(email);
            SetMessageList(messageList);
            SetFilepath(filepath);
        }
        // Setters, Getters, and ToString()
        public string GetID()
        {
            return ID;
        }
        public void SetID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("ID cannot be null or empty.", nameof(id));
            ID = id;
        }
        public string GetName()
        {
            return Name;
        }
        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            Name = name;
        }
        public int GetAge()
        {
            return Age;
        }
        public void SetAge(int age)
        {
            if (age < 0)
                throw new ArgumentOutOfRangeException(nameof(age), "Age cannot be negative.");
            Age = age;
        }
        public string GetPhoneNumber()
        {
            return PhoneNumber;
        }
        public void SetPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be null or empty.", nameof(phoneNumber));
            PhoneNumber = phoneNumber;
        }
        public string GetEmail()
        {
            return Email;
        }
        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            Email = email;
        }
        public List<Message> GetMessageList()
        {
            return MessageList;
        }
        public void SetMessageList(List<Message> messageList)
        {
            MessageList = messageList;
        }
        public string GetFilepath()
        {
            return Filepath;
        }
        public void SetFilepath(string filepath)
        {
            if (string.IsNullOrWhiteSpace(filepath))
                throw new ArgumentException("Filepath cannot be null or empty.", nameof(filepath));
            if (!System.IO.File.Exists(filepath))
                throw new System.IO.FileNotFoundException($"File not found at path: {filepath}", filepath);
            Filepath = filepath;
        }
        public override string ToString()
        {
            return $"ID: {GetID()}, Name: {GetName()}, Age: {GetAge()}, Phone: {GetPhoneNumber()}, Email: {GetEmail()}," +
                $"Messages: [\n\t{string.Join("\n\t", GetMessageList())}\n], Image: {GetFilepath()}";
        }
        // Abstract method
        // This method is abstract, along with the class, meaning that it must be implemented by non-abstract extended classes
        public abstract string DisplayCourses();
    }
}
