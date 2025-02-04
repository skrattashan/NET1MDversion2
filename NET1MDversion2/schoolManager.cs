﻿using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NET1MDversion2
{
    public class schoolManager : IDataManager
    {
        private SchoolInfo _schoolinfo;
        private SchoolContext _schoolContext; //lai varētu save changes db
        private string _connectionString; 

        public schoolManager(SchoolInfo schoolinfo, string connectionString)
        {
            _schoolinfo = schoolinfo;
            _connectionString = connectionString;
            _schoolContext = new SchoolContext(_connectionString);
        }

        public schoolManager(SchoolInfo schoolinfo) //konstruktors, kas inicialize schoolinfo
        {
            _schoolinfo = new SchoolInfo();
        }

        public SchoolInfo SchoolInfo
        {
            get { return _schoolinfo; }
        }

        public string print() //mazliet izmainiju (VS automatiski piedavaja)
        {
            string result = _schoolinfo.printAllStudents();
            result += _schoolinfo.printAllTeachers();
            result += $"Courses: {_schoolinfo.countAllCourses()}\n";
            result += _schoolinfo.printAllAssignments();
            result += _schoolinfo.printAllSubmissions();
            return result;
        }

        //public async void save(string filePath = @"C:\Temp\schooldata.xml") //draugs ieteica izmantot xml
        //{
        //    try
        //    {
        //        string directoryPath = Path.GetDirectoryName(filePath);
        //        if (!Directory.Exists(directoryPath))
        //        {
        //            Directory.CreateDirectory(directoryPath);
        //        }

        //        XmlSerializer serializer = new XmlSerializer(typeof(SchoolInfo)); //System.IO.FileNotFoundException: 'Could not load file or assembly 'NET1MDversion2.XmlSerializers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null, processorArchitecture=MSIL'. The system cannot find the file specified.'

        //        using (TextWriter writer = new StreamWriter(filePath))
        //        {
        //            serializer.Serialize(writer, _schoolinfo);
        //        }

        //        Console.WriteLine($"Data saved successfully to {filePath}");

        //        // Verify the file exists and contains data - man bija problemas, tapec ir tik daudz mazliet bezjedzigu kludu parbauzu (visas parbaudes tika uzgeneretas ar AI riku)
        //        if (File.Exists(filePath))
        //        {
        //            string fileContent = File.ReadAllText(filePath);
        //            if (!string.IsNullOrEmpty(fileContent))
        //            {
        //                Console.WriteLine("Data saved successfully and file verified.");
        //            }
        //            else
        //            {
        //                Console.WriteLine("Data saved but file appears to be empty.");
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine("File not found after save attempt.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error saving data: {ex.Message}");
        //    }
        //}
        //public void load(string filePath = @"C:\Temp\schooldata.xml")
        //{
        //    try
        //    {
        //        if (File.Exists(filePath))
        //        {
        //            XmlSerializer serializer = new XmlSerializer(typeof(SchoolInfo));
        //            using (TextReader reader = new StreamReader(filePath))
        //            {
        //                _schoolinfo = (SchoolInfo)serializer.Deserialize(reader);
        //            }
        //            Console.WriteLine($"Data loaded successfully from {filePath}");
        //        }
        //        else
        //        {
        //            Console.WriteLine("File not found.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error loading data: {ex.Message}");
        //    }
        //}
        public void createTestData() //sis kods tika uzgenerets izmantojot AI riku
        {
            if (_schoolinfo == null) //initialize schoolinfo if it is null
            {
                _schoolinfo = new SchoolInfo();
            }
            var testData = SchoolInfo.GetTestData();

            // Add students one by one - jo ObservableCollection nevar pievienot visu sarakstu uzreiz (diemzel)
            foreach (var student in testData.Students)
            {
                _schoolinfo.Students.Add(student);
                _schoolContext.Students.Add(student); //saglaba izmainas db
            }

            // Add teachers one by one
            foreach (var teacher in testData.Teachers)
            {
                _schoolinfo.Teachers.Add(teacher);
                _schoolContext.Teachers.Add(teacher); 
            }

            // Add courses one by one
            foreach (var course in testData.Courses)
            {
                _schoolinfo.Courses.Add(course);
                _schoolContext.Courses.Add(course);
            }

            // Add assignments one by one
            foreach (var assignment in testData.Assignments)
            {
                _schoolinfo.Assignments.Add(assignment);
                _schoolContext.Assignments.Add(assignment);
            }

            // Add submissions one by one
            foreach (var submission in testData.Submissions)
            {
                _schoolinfo.Submissions.Add(submission);
                _schoolContext.Submissions.Add(submission);
            }

            _schoolContext.SaveChanges(); //saglabā izmaiņas db
        }
        public void reset()
        {
            _schoolinfo.Students.Clear();
            _schoolinfo.Teachers.Clear();
            _schoolinfo.Courses.Clear();
            _schoolinfo.Assignments.Clear();
            _schoolinfo.Submissions.Clear();

            //izdzēš visu no db
            _schoolContext.Students.RemoveRange(_schoolContext.Students);
            _schoolContext.Teachers.RemoveRange(_schoolContext.Teachers);
            _schoolContext.Courses.RemoveRange(_schoolContext.Courses);
            _schoolContext.Assignments.RemoveRange(_schoolContext.Assignments);
            _schoolContext.Submissions.RemoveRange(_schoolContext.Submissions); 

            _schoolContext.SaveChanges(); //saglabā izmaiņas db
        }

        public void addStudent(string name, string surname, Person.GenderType gender, string studentIDNumber) //jauna metode, kas lauj lietotajam pievienot jaunu studentu schoolInfo
        {
            _schoolinfo.Students.Add(new Student(name, surname, gender, studentIDNumber));
            _schoolContext.Students.Add(new Student(name, surname, gender, studentIDNumber)); //OLIVER, vai šis ir redundant?
            _schoolContext.SaveChanges(); //saglabā izmaiņas db
        }

        public ObservableCollection<Student> getStudents() //man nestradaja _schoolinfo var jo private
        {
            return _schoolinfo.Students;
        }
        public void addAssignment(DateTime deadline, Course course, string description) //jauna metode, kas lauj lietotajam pievienot jaunu Assignment schoolInfo
        {
            _schoolinfo.Assignments.Add(new Assignment(deadline, course, description));
            _schoolContext.SaveChanges(); //saglabā izmaiņas db
        }
        public void addSubmission(Assignment assignment, Student student, DateTime submissionDate, int score) //jauna metode, kas lauj lietotajam pievienot jaunu Submission schoolInfo
        {
            _schoolinfo.Submissions.Add(new Submission(assignment, student, submissionDate, score));
            _schoolContext.SaveChanges(); //saglabā izmaiņas db
        }

        //public void editAssignment(Assignment assignment, DateTime deadline, Course course, string description) //jauna metode, kas lauj lietotajam rediget Assignment schoolInfo
        //{
        //    assignment.Deadline = deadline;
        //    assignment.Course = course;
        //    assignment.Description = description;
        //    _schoolContext.SaveChanges(); //saglabā izmaiņas db
        //}
        // Kāpēc šo vajag you may ask?
        // man nav editSubmission un editAssignment šeit, tāpēc būs vnk jāuztaisa metode save un jāreferenco tajās lapās, kur notiek edit
        public void saveChanges() //jauna metode, kas lauj lietotajam saglabat izmainas db
        {
            _schoolContext.SaveChanges();
        }

        //private async void OnSaveClicked(object sender, EventArgs e)
        //{
        //    try
        //    { //save changes to db
        //        await SaveChangesAsync();
        //        await DisplayAlert("Success", "Changes have been saved!", "Ok");
        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("Error", $"Error saving changes: {ex.Message}", "Ok");
        //    }
        //}
        //private async Task SaveChangesAsync()
        //{ //implementation logic to save changes to db
        //    using (var context = new SchoolContext())
        //    {
        //        await context.SaveChangesAsync();
        //    }
        //}
    }
}
