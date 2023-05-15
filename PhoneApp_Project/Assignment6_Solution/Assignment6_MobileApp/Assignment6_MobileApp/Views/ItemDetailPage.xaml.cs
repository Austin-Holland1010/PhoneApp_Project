using Assignment6_MobileApp.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace Assignment6_MobileApp.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}