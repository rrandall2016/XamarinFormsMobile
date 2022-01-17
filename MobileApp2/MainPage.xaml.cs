using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.LocalNotifications;
using System.Runtime.InteropServices;
using MobileApp2.SQLite;

namespace MobileApp2
{
    [Table("Assessments")]
    public class Assessment
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Type { get; set; }
        public int Course { get; set; }
        public int NotificationEnabled { get; set; }
    }
    [Table("Courses")]
    public class Course
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public int Term { get; set; }
        public string CourseName { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string InstructorName { get; set; }
        public string InstructorPhone { get; set; }
        public string InstructorEmail { get; set; }
        public string Notes { get; set; }
        public int NotificationEnabled { get; set; }
    }
    [Table("Terms")]
    public class Term
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        async private void Term_Click(object sender, ItemTappedEventArgs e)
        {
            Term term = (Term)e.Item;
            await Navigation.PushModalAsync(new TermDetails(term));
        }
        private async void OnButtonClick(object sender, EventArgs e)
        {

            await Navigation.PushModalAsync(new AddTerm(this));
        }

        private SQLiteAsyncConnection _conn;
        public ObservableCollection<Term> _termList;
        private bool pushNotification = true;
        public MainPage()
        {
            InitializeComponent();
            _conn = DependencyService.Get<ISQLite>().GetConnection();
            termListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(Term_Click);

        }
        protected override async void OnAppearing()
        {
            await _conn.CreateTableAsync<Term>();
            await _conn.CreateTableAsync<Course>();
            await _conn.CreateTableAsync<Assessment>();

            var termList = await _conn.Table<Term>().ToListAsync();
            var courseList = await _conn.Table<Course>().ToListAsync();
            var assessmentList = await _conn.Table<Assessment>().ToListAsync();

            //Add mock data 
            if (!termList.Any())
            {
                var mockTerm = new Term();
                mockTerm.Title = "Term 1";
                mockTerm.StartDate = new DateTime(2021, 01, 01);
                mockTerm.EndDate = new DateTime(2021, 06, 30);
                await _conn.InsertAsync(mockTerm);
                termList.Add(mockTerm);

                var mockCourse = new Course();
                mockCourse.CourseName = "Mobile Application Development Using C#";
                mockCourse.StartDate = new DateTime(2021, 06, 11);
                mockCourse.StartDate = new DateTime(2021, 07, 11);
                mockCourse.Status = "In-Progress";
                mockCourse.InstructorName = "Randy Randall";
                mockCourse.InstructorPhone = "123-123-1234";
                mockCourse.InstructorEmail = "testing@gmail.com";
                mockCourse.NotificationEnabled = 1;
                mockCourse.Notes = "Notes for course";
                mockCourse.Term = mockTerm.Id;
                await _conn.InsertAsync(mockCourse);

                var mockOA = new Assessment();
                mockOA.Title = "Assessment 1";
                mockOA.StartDate = new DateTime(2021, 06, 11);
                mockOA.EndDate = new DateTime(2021, 06, 12);
                mockOA.Course = mockCourse.Id;
                mockOA.Type = "Objective";
                mockOA.NotificationEnabled = 1;
                await _conn.InsertAsync(mockOA);

                var mockPA = new Assessment();
                mockPA.Title = "Test Assessment 2";
                mockPA.StartDate = new DateTime(2021, 06, 12);
                mockPA.EndDate = new DateTime(2021, 06, 13);
                mockPA.Course = mockCourse.Id;
                mockPA.Type = "Performance";
                mockPA.NotificationEnabled = 1;
                await _conn.InsertAsync(mockPA);
            }

            if (pushNotification == true)
            {
                pushNotification = false;
                int courseId = 0;
                foreach (Course course in courseList)
                {
                    courseId++;
                    if (course.NotificationEnabled == 1)
                    {
                        if (course.StartDate == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Reminder", $"{course.CourseName} begins today!", courseId);
                        if (course.EndDate == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Reminder", $"{course.CourseName} ends today!", courseId);
                    }
                }

                int assessmentId = courseId;
                foreach (Assessment assessment in assessmentList)
                {
                    assessmentId++;
                    if (assessment.NotificationEnabled == 1)
                    {
                        if (assessment.StartDate == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Reminder", $"{assessment.Title} begins today!", assessmentId);
                        if (assessment.EndDate == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Reminder", $"{assessment.Title} ends today!", assessmentId);
                    }
                }
            }

            _termList = new ObservableCollection<Term>(termList);
            termListView.ItemsSource = _termList;
            base.OnAppearing();
        }
    }
}
