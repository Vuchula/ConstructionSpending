using ConstructionSpending.DataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using ConstructionSpending.Models;
using System.Threading.Tasks;

namespace ConstructionSpending.APIHandlerManager
{
    public class APIHandler
    {
        //Fetching the construction api data.
        static string BASE_URL = "https://api.census.gov/data/timeseries/eits/";
        static string API_KEY = "eba8f314ba745df81e5653d412e873b20a487f25";

        //HttpClient httpClient;

        //This is a constructor which gets Initialised when the API handler method is called
        //We don't have to give the API key in the header so we can skip that step.
        public APIHandler()
        {
            /*httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            //httpClient.DefaultRequestHeaders.Add("X-Api-Key", API_KEY);
            httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));*/
        }

        public T GetData<T>(string type, string year, HttpClient httpClient)
        {
            String API = BASE_URL + type+"?get=data_type_code,cell_value,program_code,time_slot_date," +
                "seasonally_adj,time_slot_id,time_slot_name,category_code" +
                "&time="+year+"&key=" + API_KEY;
            String data = "";
            List<List<String>> responseModel;
            // connect to the IEXTrading API and retrieve information
            //httpClient.BaseAddress = new Uri(API);
            HttpResponseMessage response = httpClient.GetAsync(API).GetAwaiter().GetResult();

            if (response.IsSuccessStatusCode)
            {
                data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            //System.Diagnostics.Debug.WriteLine("Data from the HV API " + data);
            List<Response> dataResponse = new List<Response>();
            List<ResponseVip> responseVips = new List<ResponseVip>();
            if (!data.Equals(""))
            {
                // https://stackoverflow.com/a/46280739
                //JObject result = JsonConvert.DeserializeObject<JObject>(companyList);
                responseModel = JsonConvert.DeserializeObject<List<List<String>>>(data);
                
                System.Diagnostics.Debug.WriteLine("Data from the API " + responseModel.Count());

                if (type.Equals("hv"))
                {
                    for (int i = 1; i < responseModel.Count(); i++)
                    {
                        System.Diagnostics.Debug.WriteLine("Data value " + responseModel[i][1]);
                        
                        Response res = new Response();

                        res.data_type_code = responseModel[i][0];
                        double num;
                        bool isNum = double.TryParse(responseModel[i][1], out num);
                        if (isNum)
                        {
                            res.cell_value = Double.Parse(responseModel[i][1]);
                        }
                        else
                        {
                            res.cell_value = (double)0;
                        }
                        //res.cell_value = Double.Parse(responseModel[i][1]);
                        res.program_code = responseModel[i][2];
                        res.time_slot_date = DateTime.Parse(responseModel[i][3]);
                        res.seasonally_adj = responseModel[i][4];
                        res.time_slot_id = int.Parse(responseModel[i][5]);
                        res.time_slot_name = responseModel[i][6];
                        res.category_code = responseModel[i][7];
                        res.time = responseModel[i][8];

                        dataResponse.Add(res);
                    }
                }
                else if(type.Equals("vip"))
                {
                    for (int i = 1; i < responseModel.Count(); i++)
                    {
                        ResponseVip res = new ResponseVip
                        {
                            data_type_code = responseModel[i][0],
                            cell_value = double.Parse(responseModel[i][1]),
                            program_code = responseModel[i][2],
                            time_slot_date = DateTime.Parse(responseModel[i][3]),
                            seasonally_adj = responseModel[i][4],
                            time_slot_id = int.Parse(responseModel[i][5]),
                            time_slot_name = responseModel[i][6],
                            category_code = responseModel[i][7],
                            time = responseModel[i][8],
                        };
                        responseVips.Add(res);
                    }
                }

                
            }
            if (type.Equals("hv"))
                return (T)Convert.ChangeType(dataResponse, typeof(T));
            else
                return (T)Convert.ChangeType(responseVips, typeof(T));
        }
    }
}
