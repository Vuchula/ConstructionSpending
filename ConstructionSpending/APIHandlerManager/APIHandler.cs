using ConstructionSpending.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConstructionSpending.APIHandlerManager
{
    public class APIHandler
    {
        //Fetching the construction api data.
        static string BASE_URL = "https://api.census.gov/data/timeseries/eits/";
        static string API_KEY = "eba8f314ba745df81e5653d412e873b20a487f25";

        HttpClient httpClient;

        //This is a constructor which gets Initialised when the API handler method is called
        //We don't have to give the API key in the header so we can skip that step.
        public APIHandler()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            //httpClient.DefaultRequestHeaders.Add("X-Api-Key", API_KEY);
            httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void GetHVData()
        {
            String HV_API = BASE_URL + "hv?get=data_type_code,cell_value,program_code,time_slot_date," +
                "seasonally_adj,time_slot_id,time_slot_name,category_code" +
                "&time=2019&key=" + API_KEY;
            String data = "";
            List<List<String>> responseModel;
            // connect to the IEXTrading API and retrieve information
            httpClient.BaseAddress = new Uri(HV_API);
            HttpResponseMessage response = httpClient.GetAsync(HV_API).GetAwaiter().GetResult();

            if (response.IsSuccessStatusCode)
            {
                data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            //System.Diagnostics.Debug.WriteLine("Data from the HV API " + data);

            if (!data.Equals(""))
            {
                // https://stackoverflow.com/a/46280739
                //JObject result = JsonConvert.DeserializeObject<JObject>(companyList);
                responseModel = JsonConvert.DeserializeObject<List<List<String>>>(data);
                
                System.Diagnostics.Debug.WriteLine("Data from the HV API " + responseModel.Count());
                List<Response> dataResponse = new List<Response>();
                for(int i = 0; i < responseModel.Count(); i++)
                {
                    Response res = new Response();
                    res.data_type_code = responseModel[i][0];
                    res.cell_value = responseModel[i][1];
                    res.program_code = responseModel[i][2];
                    res.time_slot_date = responseModel[i][3];
                    res.seasonally_adj = responseModel[i][4];
                    res.time_slot_id = responseModel[i][5];
                    res.time_slot_name = responseModel[i][6];
                    res.category_code = responseModel[i][7];
                    res.time = responseModel[i][8];
                    dataResponse.Add(res);
                    
                }
                System.Diagnostics.Debug.WriteLine("Data stored in the model " + dataResponse[1].data_type_code);
                System.Diagnostics.Debug.WriteLine("Data stored in the model " + dataResponse[1].cell_value);
                System.Diagnostics.Debug.WriteLine("Data stored in the model " + dataResponse[1].program_code);
                System.Diagnostics.Debug.WriteLine("Data stored in the model " + dataResponse[1].time_slot_date);
                System.Diagnostics.Debug.WriteLine("Data stored in the model " + dataResponse[1].seasonally_adj);
                System.Diagnostics.Debug.WriteLine("Data stored in the model " + dataResponse[1].time_slot_id);
                System.Diagnostics.Debug.WriteLine("Data stored in the model " + dataResponse[1].time_slot_name);
                System.Diagnostics.Debug.WriteLine("Data stored in the model " + dataResponse[1].category_code);
                System.Diagnostics.Debug.WriteLine("Data stored in the model " + dataResponse[1].time);
            }
        }
    }
}
