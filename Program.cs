using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Timers;
using Newtonsoft.Json;

namespace Time_Steelseries
{
    public class CoreProps
    {
        public string address { get; set; }
        public string encrypted_address { get; set; }
        public CoreProps(string _address, string _encrypted_address)
        {
            address = _address;
            encrypted_address = _encrypted_address;
        }
    }
    class Program
    {
        static HttpWebRequest gameEventRequest;
        static CultureInfo timeCulture;
        static Timer timer = new Timer(5000);
        static int numberOfLoops = 1;
        static string coreProps; // The coreProps file with the ip and port information in string format
        static string ipAddress;
        static bool timerInitialized = false;

        static public void Main(string[] args)
        {
            coreProps = File.ReadAllText("C:/ProgramData/SteelSeries/SteelSeries Engine 3/coreProps.json");
            CoreProps corePropsDeserialized = JsonConvert.DeserializeObject<CoreProps>(coreProps);
            ipAddress = "http://" + corePropsDeserialized.address;

            Menu();
        }

        static void Menu()
        {
            int response;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("What would you like to do?\n" +
                "0 - Start Sending Clock Updates to engine\n" +
                "1 - Install Engine App\n" +
                "2 - Uninstall Engine App");
                bool success = Int32.TryParse(Console.ReadLine(), out response);
                if (success)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("The inputed value is not an integer, try again");
                }
            }
            switch (response)
            {
                case 0:
                    Console.WriteLine("Starting UpdateClock Loop... Press any key to end the loop");
                    StartUpdateClockLoop();
                    break;
                case 1:
                    RegisterMetadata();
                    Console.WriteLine("Press any key to return to the menu");
                    Console.ReadKey(true);
                    Menu();
                    break;
                case 2:
                    RemoveGame();
                    Console.WriteLine("Press any key to return to the menu");
                    Console.ReadKey(true);
                    Menu();
                    break;
                default:
                    Menu();
                    break;
            }
        }

        static void StartUpdateClockLoop()
        {
            if(!timerInitialized)
            {
                timerInitialized = true;
                timeCulture = CultureInfo.CreateSpecificCulture("es-ES");

                timer.Elapsed += UpdateClock;
                timer.AutoReset = true;
            }
            timer.Enabled = true;

            Console.ReadKey(true);
            timer.Enabled = false;
            numberOfLoops = 1;
            Menu();
        }

        static void UpdateClock(Object source, ElapsedEventArgs e)
        {
            try
            {
                gameEventRequest = (HttpWebRequest)WebRequest.Create($"{ipAddress}/game_event");
                gameEventRequest.ContentType = "application/json";
                gameEventRequest.Method = "POST";

                Console.WriteLine($"Running UpdateClock for the {numberOfLoops} time");
                numberOfLoops++;
                using (StreamWriter streamWriter = new StreamWriter(gameEventRequest.GetRequestStream()))
                {
                    string json = "{\"game\": \"CLOCK\"," +
                                  "\"event\": \"TIME\"," +
                                  "\"data\": { \"value\": 1, " +
                                  "\"frame\": {\"textvalue\": \"" + DateTime.Now.ToString("t", timeCulture) + "\"} } }";

                    streamWriter.Write(json);
                }
                HttpWebResponse gameEventResponse = (HttpWebResponse)gameEventRequest.GetResponse();
                Console.WriteLine($"Response: {gameEventResponse.StatusCode}");
                gameEventRequest.Abort();
                gameEventResponse.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        static void BindEvent()
        {
            try
            {
                HttpWebRequest bindRequest = (HttpWebRequest)WebRequest.Create($"{ipAddress}/bind_game_event");
                bindRequest.ContentType = "application/json";
                bindRequest.Method = "POST";
                
                using (StreamWriter streamWriter = new StreamWriter(bindRequest.GetRequestStream()))
                {
                    string json = "{\"game\": \"CLOCK\", " +
                        "\"event\": \"TIME\", " +
                        "\"min_value\": 0, " +
                        "\"max_value\": 1, " +
                        "\"icon_id\": 15, " +
                        "\"value_optional\": true, " +
                        "\"handlers\": [{" +
                        "\"device-type\": \"screened\", " +
                        "\"mode\": \"screen\", " +
                        "\"zone\": \"one\", " +
                        "\"datas\": [{" +
                        "\"has-text\": true, " +
                        "\"context-frame-key\": \"textvalue\", " +
                        "\"icon-id\": 15" +
                        "}]}]}";

                    streamWriter.Write(json);
                }
                HttpWebResponse bindResponse = (HttpWebResponse)bindRequest.GetResponse();
                Console.WriteLine($"Response: {bindResponse.StatusCode}");
                Console.WriteLine("The event is bound");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        static void RegisterMetadata()
        {
            try
            {
                HttpWebRequest registerRequest = (HttpWebRequest)WebRequest.Create($"{ipAddress}/game_metadata");
                registerRequest.ContentType = "application/json";
                registerRequest.Method = "POST";

                using (StreamWriter streamWriter = new StreamWriter(registerRequest.GetRequestStream()))
                {
                    string json = "{\"game\": \"CLOCK\", " +
                        "\"game_display_name\": \"Clock\", " + 
                        "\"developer\": \"TechnOllieG\"}";

                    streamWriter.Write(json);
                }
                HttpWebResponse registerResponse = (HttpWebResponse)registerRequest.GetResponse();
                Console.WriteLine($"Response: {registerResponse.StatusCode}");
                Console.WriteLine("The metadata has been registered");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            BindEvent();
        }

        static void RemoveGame()
        {
            try
            {
                HttpWebRequest removalRequest = (HttpWebRequest)WebRequest.Create($"{ipAddress}/remove_game");
                removalRequest.ContentType = "application/json";
                removalRequest.Method = "POST";

                using (StreamWriter streamWriter = new StreamWriter(removalRequest.GetRequestStream()))
                {
                    string json = "{\"game\": \"CLOCK\"}";

                    streamWriter.Write(json);
                }
                HttpWebResponse removalResponse = (HttpWebResponse)removalRequest.GetResponse();
                Console.WriteLine($"Response: {removalResponse.StatusCode}");
                Console.WriteLine("The app has been uninstalled");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
