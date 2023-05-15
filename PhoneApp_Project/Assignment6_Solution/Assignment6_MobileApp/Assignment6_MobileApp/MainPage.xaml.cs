using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Assignment6_MobileApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        CacheProvider cache = new CacheProvider();

        public MainPage()
        {
            InitializeComponent();
        }
        private int count = 0;
        private void Button_Clicked(object sender, EventArgs e)
        {
            ((Button)sender).Text = $"You clicked me {++count} times.";
        }
        
        //This method handles what happens when the Add button is pressed
        // This method uses Caching to reduce the amount of calls made to the service.
        private async void Add_Clicked(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            string result;
            bool cached = false;

            //This if statement prevents the caches from being accessed when they are still Null values
                
            if (cache.Get1<String>("add_cache_num1") == null || cache.Get2<String>("add_cache_num2") == null)
            {

            }
            else
            {
                //Look at the values in the caches and the Entry fields. If both match use the cached value
                if (cache.Get1<String>("add_cache_num1").Equals(Num1Entry.Text) && cache.Get2<String>("add_cache_num2").Equals(Num2Entry.Text))
                {
                    cached = true;
                }
            }

            if (cached != true)
            {
                try
                {
                    var response = await client.GetAsync(
                        "https://venus.sod.asu.edu/WSRepository/Services/WcfRestService4/Service1/add2?x=" + Num1Entry.Text + "&y=" + Num2Entry.Text);
                    response.EnsureSuccessStatusCode();
                    result = (await response.Content.ReadAsStringAsync()).Replace(@"""", "");
                }
                catch (HttpRequestException ex)
                {
                    result = ex.ToString();
                }
                cache.Set1<String>("add_cache_num1", Num1Entry.Text, DateTimeOffset.Now.AddSeconds(10));
                cache.Set2<String>("add_cache_num2", Num2Entry.Text, DateTimeOffset.Now.AddSeconds(10));

                ResultLabel.Text = "Result = " + result;
            }
            else
            {
                ResultLabel.Text = ResultLabel.Text + " (cached value)";
            }
        }

        //This method handles what happens when the Generate button is clicked
        // The GetRandomString ASU restful service is called.
        private async void Generate_Clicked(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            string result;
            try
            {
                var response = await client.GetAsync(
                    "https://venus.sod.asu.edu/WSRepository/Services/RandomString/Service.svc/GetRandomString/" + NumEntry.Text);
                response.EnsureSuccessStatusCode();
                result = (await response.Content.ReadAsStringAsync()).Replace(@"""", "");
            }
            catch (HttpRequestException ex)
            {
                result = ex.ToString();
            }

            //Get the string from within the XML tag
            for (int x = 0; x < result.Length; x++)
            {
                if (result[x] == '<')
                {
                    for (int y = x + 1; y < result.Length; y++)
                    {
                        if (result[y] == '>')
                        {
                            if (result[0] == '<')
                            {
                                y++;
                                result = result.Remove(x, y);
                            }
                            else
                            {
                                result = result.Remove(x);
                            }
                        }
                    }
                }
            }

            GeneratedResult.Text = "Generated string: " + result;
        }
        
    }
    public class CacheProvider
    {
        private readonly IMemoryCache _cache_num1;
        private readonly IMemoryCache _cache_num2;

        //Constructor
        public CacheProvider()
        {
            _cache_num1 = new MemoryCache(new MemoryCacheOptions() { });
            _cache_num2 = new MemoryCache(new MemoryCacheOptions() { });
        }

        //Num1 Get/Set methods
        public void Set1<String>(string key, string value, DateTimeOffset absoluteExpiry)
        {
            _cache_num1.Set(key, value, absoluteExpiry);
        }

        public string Get1<String>(string key)
        {
            if (_cache_num1.TryGetValue(key, out string value))
                return value;
            else
                return default(string);
        }

        //Num 2 Get/Set methods
        public void Set2<String>(string key, string value, DateTimeOffset absoluteExpiry)
        {
            _cache_num2.Set(key, value, absoluteExpiry);
        }

        public string Get2<String>(string key)
        {
            if (_cache_num2.TryGetValue(key, out string value))
                return value;
            else
                return default(string);
        }
    }
}