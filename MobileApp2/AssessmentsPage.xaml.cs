using MobileApp2;
using MobileApp2.SQLite;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApp2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssessmentsPage : ContentPage
    {
        private Course theCurrentCourse;
        private SQLiteAsyncConnection _conn;
        private ObservableCollection<Assessment> theAssessmentList;
        public AssessmentsPage(Course currentCourse)
        {
            InitializeComponent();
            CourseName.Text = currentCourse.CourseName;
            theCurrentCourse = currentCourse;
            _conn = DependencyService.Get<ISQLite>().GetConnection();
            assessmentListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(Assessment_Tapped);

        }
        private async void AddtheAssessment_Click(object sender, EventArgs e)
        {
            await _conn.CreateTableAsync<Assessment>();
            var assessmentCount = await _conn.QueryAsync<Assessment>($"Select Type From Assessments Where Course = '{theCurrentCourse.Id}'");
            if (assessmentCount.Count == 2)
            {
                await DisplayAlert("Alert", "You cannot add more than two exams. Please remove an exam and try again", "Ok");
            }
            else await Navigation.PushModalAsync(new AddAssessment(theCurrentCourse));
        }
        private async void Assessment_Tapped(object sender, ItemTappedEventArgs e)
        {
            Assessment assessment = (Assessment)e.Item;
            await Navigation.PushModalAsync(new AssessmentDetail(assessment));
        }

        protected override async void OnAppearing()
        {
            CourseName.Text = theCurrentCourse.CourseName;
            await _conn.CreateTableAsync<Assessment>();
            var assessmentList = await _conn.QueryAsync<Assessment>($"Select * From Assessments Where Course = '{theCurrentCourse.Id}'");
            theAssessmentList = new ObservableCollection<Assessment>(assessmentList);
            assessmentListView.ItemsSource = theAssessmentList;
            base.OnAppearing();
        }

        private async void OnButtonClick(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

    }
}