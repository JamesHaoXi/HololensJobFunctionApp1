using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System;

namespace HololensJobFunctionApp1
{
    public static class Function1
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("Function1")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "ConnectionString")]EventData message, ILogger log)
//        public static void Run([IoTHubTrigger("messages/events", Connection = "IoTHubTriggerConnection", ConsumerGroup = "FuncGroup")]EventData message, ILogger log)
        {
            string msg = Encoding.UTF8.GetString(message.Body.Array);
            log.LogInformation($"C# IoT Hub trigger function processed a message: {msg}");
            dynamic data = JsonConvert.DeserializeObject<query_para>(msg);
            //name = name ?? data?.name;
            //mac = mac ?? data?.mac;
            log.LogInformation($"C# IoT Hub trigger function processed a message: {data.deviceID}");

            //new, insert

            //exist, update
            // Get the connection string from app settings and use it to create a connection.
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            //string tmp_str = "";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                //var text = "UPDATE SalesLT.SalesOrderHeader " +
                //        "SET [Status] = 5  WHERE ShipDate < GetDate();";
                //var text = "SELECT * " +
                //        "FROM Job " +
                //        "WHERE Done = 0 AND deviceID='" + data.deviceID + "' " +
                //        "ORDER BY CreateTime DESC;";
                var text = "SELECT COUNT(*) " +
                        "FROM Job " +
                        "WHERE deviceID = '" + data.deviceID + "' AND Done = 0;";
                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    // Execute the command and log the # rows affected.

                    var rows = cmd.ExecuteScalar();
                    log.LogInformation($"{rows} rows were get from Job table.");
                    //SqlDataReader reader = cmd.ExecuteReader();
                    //string tmp_str = "";
                    //while (reader.Read())
                    //{
                    //    log.LogInformation(reader["deviceID"].ToString() + "," + reader["CreateTime"].ToString());

                    //}
                    //reader.Close();
                    if (((int)rows <= 0)&&(data.brightness<185))
                    {
                        text = "INSERT INTO Job (deviceID, CreateTime, Done)" +
                        "VALUES ( '" + data.deviceID + "','" + data.timestamp + "',0);";
                        SqlCommand cmd_insert = new SqlCommand(text, conn);

                        var rows_insert = cmd_insert.ExecuteNonQuery();
                        log.LogInformation($"{rows_insert} rows were inserted to Job table.");
                    }


                }
                conn.Close();
            }
        }
        public class query_para
        {
            public string deviceID { get; set; }
            public string timestamp { get; set; }
            public double brightness { get; set; }
            public double vdd { get; set; }
            public int status { get; set; }

        }
    }
}