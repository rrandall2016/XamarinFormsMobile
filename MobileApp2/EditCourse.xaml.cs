using MobileApp2;
using MobileApp2.SQLite;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApp2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditCourse : ContentPage
    {
        private SQLiteAsyncConnection _conn;
        private Course theCurrentCourse;
        public EditCourse(Course currentCourse)
        {
            InitializeComponent();
            theCurrentCourse = currentCourse;
            _conn = DependencyService.Get<ISQLite>().GetConnection();
        }
        private async void OnButtonClick(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        protected async override void OnAppearing()
        {
            await _conn.CreateTableAsync<Course>();

            CourseName.Text = theCurrentCourse.CourseName;
            CourseStatus.SelectedItem = theCurrentCourse.Status;
            StartDate.Date = theCurrentCourse.StartDate;
            EndDate.Date = theCurrentCourse.EndDate;
            InstructorName.Text = theCurrentCourse.InstructorName;
            InstructorPhone.Text = theCurrentCourse.InstructorPhone;
            InstructorEmail.Text = theCurrentCourse.InstructorEmail;
            Notes.Text = theCurrentCourse.Notes;
            if (theCurrentCourse.NotificationEnabled == 1)
            {
                EnableNotifications.On = true;
            }
            base.OnAppearing();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            theCurrentCourse.CourseName = CourseName.Text;
            theCurrentCourse.Status = (string)CourseStatus.SelectedItem;
            theCurrentCourse.StartDate = StartDate.Date;
            theCurrentCourse.EndDate = EndDate.Date;
            theCurrentCourse.InstructorName = InstructorName.Text;
            theCurrentCourse.InstructorEmail = InstructorEmail.Text;
            theCurrentCourse.InstructorPhone = InstructorPhone.Text;
            theCurrentCourse.Notes = Notes.Text;
            theCurrentCourse.NotificationEnabled = EnableNotifications.On == true ? 1 : 0;

            if (FieldCheck.IsNull(CourseName.Text) &&
                FieldCheck.IsNull(InstructorName.Text) &&
                FieldCheck.IsNull(InstructorPhone.Text))
            {
                if (FieldCheck.IsValidEmail(InstructorEmail.Text))
                {
                    if (theCurrentCourse.StartDate < theCurrentCourse.EndDate)
                    { 
                        await _conn.UpdateAsync(theCurrentCourse);
                        await Navigation.PopModalAsync();
                    }
                    else await DisplayAlert("Error.", "Start date must be before end date.", "Ok");
                }
                else await DisplayAlert("Error.", "Complete all fields.", "Ok");
            }
            else await DisplayAlert("Error.", "Valid email required", "Ok");
        }
    

    }
}