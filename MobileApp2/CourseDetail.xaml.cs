using MobileApp2;
using MobileApp2.SQLite;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApp2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseDetail : ContentPage
    {
        private SQLiteAsyncConnection _conn;
        private Course theCurrentCourse;
        public CourseDetail(Course course)
        {
            InitializeComponent();
            theCurrentCourse = course;
            _conn = DependencyService.Get<ISQLite>().GetConnection();
        }
        private async void Drop_Course_Click(object sender, EventArgs e)
        {
            var confirmation = await DisplayAlert("Alert", "Are you sure you want to drop this course?", "Yes", "No");
            if (confirmation)
            {
                await _conn.DeleteAsync(theCurrentCourse);
                await Navigation.PopModalAsync();
            }
        }
        protected override void OnAppearing()
        {
            courseName.Text = theCurrentCourse.CourseName;
            Status.Text = theCurrentCourse.Status;
            StartDate.Text = theCurrentCourse.StartDate.ToString("MM/dd/yy");
            EndDate.Text = theCurrentCourse.EndDate.ToString("MM/dd/yy");
            InstructorName.Text = theCurrentCourse.InstructorName;
            InstructorPhone.Text = theCurrentCourse.InstructorPhone;
            InstructorEmail.Text = theCurrentCourse.InstructorEmail;
            Notes.Text = theCurrentCourse.Notes;
            NotificationsEnabled.Text = theCurrentCourse.NotificationEnabled == 1 ? "Yes" : "No";
            base.OnAppearing();

            
        }
        private async void Assessments_Click(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AssessmentsPage(theCurrentCourse));
        }
        private async void ShareButton_Clicked(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = Notes.Text,
                Title = "Share your notes from this course!"
            });
        }
        private async void OnButtonClick(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void Edit_Course_Click(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new EditCourse(theCurrentCourse));
        }

    }
}