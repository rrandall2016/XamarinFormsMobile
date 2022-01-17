using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileApp2;
using MobileApp2.SQLite;
using SQLite;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApp2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditAssessment : ContentPage
    {
        private Assessment theAssessment;
        private SQLiteAsyncConnection _conn;
        
        public EditAssessment(Assessment assessment)
        {
            InitializeComponent();
            theAssessment = assessment;
            _conn = DependencyService.Get<ISQLite>().GetConnection();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            theAssessment.Title = AssessmentName.Text;
            theAssessment.StartDate = StartDate.Date;
            theAssessment.EndDate = EndDate.Date;
            theAssessment.NotificationEnabled = EnableNotifications.On == true ? 1 : 0;

            if (FieldCheck.IsNull(AssessmentName.Text))
            {
                if (theAssessment.StartDate < theAssessment.EndDate)
                {
                    await _conn.UpdateAsync(theAssessment);
                    await Navigation.PopModalAsync();
                }
                else await DisplayAlert("Error.", "Please ensure start date is before end date.", "Ok");
            }
            else await DisplayAlert("Error.", "Please ensure all fields are completed.", "Ok");
        }

        protected async override void OnAppearing()
        {
            await _conn.CreateTableAsync<Assessment>();

            AssessmentName.Text = theAssessment.Title;
            StartDate.Date = theAssessment.StartDate;
            EndDate.Date = theAssessment.EndDate;
            
            if (theAssessment.NotificationEnabled == 1)
            {
                EnableNotifications.On = true;
            }
            base.OnAppearing();
        }



        private async void OnButtonClick(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}