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
    public partial class AssessmentDetail : ContentPage
    {
        private Assessment theAssessment;
        private SQLiteAsyncConnection _conn;
        public AssessmentDetail(Assessment assessment)
        {
            InitializeComponent();
            theAssessment = assessment;
            _conn = DependencyService.Get<ISQLite>().GetConnection();
        }
        private async void EdittheAssessment_Click(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new EditAssessment(theAssessment));
        }

        private async void DroptheAssessment_Click(object sender, EventArgs e)
        {
            var confirmation = await DisplayAlert("Alert", "Are you sure you want to delete this assessment?", "Yes", "No");
            if (confirmation)
            {
                await _conn.DeleteAsync(theAssessment);
                await Navigation.PopModalAsync();
            }
        }
        protected override void OnAppearing()
        {
            AssessmentName.Text = theAssessment.Title;
            StartDate.Text = theAssessment.StartDate.ToString("MM/dd/yy");
            EndDate.Text = theAssessment.EndDate.ToString("MM/dd/yy");
            AssessmentType.Text = theAssessment.Type;
            NotificationsEnabled.Text = theAssessment.NotificationEnabled == 1 ? "Yes" : "No";
            base.OnAppearing();
        }

        private async void OnButtonClick(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

    }
}