using MobileApp2;
using MobileApp2.SQLite;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApp2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTerm : ContentPage
    {
        public MainPage _mainPage;
        private SQLiteAsyncConnection _conn;
        private async void OnButtonClick(object sender, EventArgs e)
        {

            await Navigation.PopModalAsync();
        }

        public AddTerm(MainPage mainPage)
        {
            InitializeComponent();
            _mainPage = mainPage;
            _conn = DependencyService.Get<ISQLite>().GetConnection();
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            var term = new Term();
            term.Title = TermTitle.Text;
            term.StartDate = startDate.Date;
            term.EndDate = endDate.Date;

            if (FieldCheck.IsNull(term.Title))
            {
                if (term.StartDate < term.EndDate)
                {
                    await _conn.InsertAsync(term);
                    _mainPage._termList.Add(term);
                    await Navigation.PopModalAsync();
                }
                else await DisplayAlert("Error.", "Start date must be before end date.", "Ok");
            }
            else await DisplayAlert("Error.", "Fill in all fields.", "Ok");
        }
    }
}