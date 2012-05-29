using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using Newtonsoft.Json;

namespace AsyncSilverlightMvvm.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        static int callCounter;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {            
            Cities = new ObservableCollection<City>();
            LoadCities();
        }

        public ObservableCollection<City> Cities { get; set; }

        
        private void OnSuccess(IEnumerable<City> cities)
        {
            if (cities == null)
                return;

            Cities.Clear();
            foreach (var city in cities)
            {
                Cities.Add(city);
            }
        }
        private void OnError(Exception exception)
        {
            //todo add your error handling here
        }

        private void LoadCities()
        {
            string cityApiUrl = "http://localhost:35531/api/city";

            HttpWebRequest request = CreateRequest(cityApiUrl);

            ExecuteRequest<City>(request, OnSuccess, OnError);
        }

        private HttpWebRequest CreateRequest(string url)
        {
            var uri = new Uri(url, UriKind.Absolute);
            var request = WebRequest.CreateHttp(uri);
            request.Method = "GET";
            request.Accept = "application/json";

            return request;
        }

        public void ExecuteRequest<T>(HttpWebRequest request, Action<IEnumerable<T>> OnSuccess, Action<Exception> OnError)
        {
            Interlocked.Increment(ref callCounter);
            request.BeginGetResponse(cb =>
            {
                try
                {
                    var resp = request.EndGetResponse(cb);
                    var content = GetContentFromStream(resp.GetResponseStream());
                    var data = JsonConvert.DeserializeObject<IEnumerable<T>>(content);
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                                              {
                                                                  OnSuccess(data);
                                                              });
                }
                catch (Exception ex)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                                              {
                                                                  OnError(ex);
                                                              });
                }
                finally
                {
                    Interlocked.Decrement(ref callCounter);   
                }                

            }, null);
        }        

        public static string GetContentFromStream(Stream responseStream)
        {
            string content = "";

            using (var reader = new StreamReader(responseStream))
            {
                content = reader.ReadToEnd();
                reader.Close();
            }

            return content;
        }        
    }
}