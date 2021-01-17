using Acr.UserDialogs;
using Newtonsoft.Json;
using SaveTheWorldApp.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SaveTheWorldApp.Helpers
{
    public class WebApiHelper
    {
        public async Task<T> GetAsync<T>(string url) where T : class
        {
            try
            {
                HttpClient client = new HttpClient();
                UserDialogs.Instance.ShowLoading("Please wait..");
                var response = await client.GetAsync(url);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    if (jsonResponse.Contains("MSG"))
                    {
                        var res = JsonConvert.DeserializeObject<BaseResponse[]>(jsonResponse);
                        if (res != null && res.Length > 0)
                            UserDialogs.Instance.Alert(res[0].MSG);
                        UserDialogs.Instance.HideLoading();
                        return null;
                    }
                    T responseModel = JsonConvert.DeserializeObject<T>(jsonResponse);
                    UserDialogs.Instance.HideLoading();
                    return responseModel;
                }
                else
                {
                    UserDialogs.Instance.HideLoading();
                    UserDialogs.Instance.Alert("Something went wrong please try again");
                    return null;
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                UserDialogs.Instance.Alert("Something went wrong please try again");
                return null;
            }
        }

        public async Task<T> PostAsync<T>(string url, object data) where T : class
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Please wait..");
                HttpClient client = new HttpClient();
                StringContent content = null;
                if (data != null)
                {
                    string jsonContent = JsonConvert.SerializeObject(data);
                    content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                }
                var response = await client.PostAsync(url, content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    if (jsonResponse.Contains("MSG"))
                    {
                        var res = JsonConvert.DeserializeObject<BaseResponse[]>(jsonResponse);
                        if (res != null && res.Length > 0)
                            UserDialogs.Instance.Alert(res[0].MSG);
                        UserDialogs.Instance.HideLoading();
                        return null;
                    }
                    T responseModel = JsonConvert.DeserializeObject<T>(jsonResponse);
                    UserDialogs.Instance.HideLoading();
                    return responseModel;
                }
                else
                {
                    UserDialogs.Instance.HideLoading();
                    UserDialogs.Instance.Alert("Something went wrong please try again");
                    return null;
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                UserDialogs.Instance.Alert("Something went wrong please try again");
                return null;
            }
        }
    }
}
