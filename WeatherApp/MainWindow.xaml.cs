using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace WeatherApp {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
		}
		XmlDocument GetDataFromServer(string cityname) {
			XmlDocument xml = new XmlDocument();
			xml.Load(string.Format("http://www.google.com/ig/api?weather={0}", cityname));
			return xml;
		}
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button Butt1 = sender as Button;
            Windowforecast Window = new Windowforecast();
            Window.Show();
            Window.forecast.ItemsSource = GetData(InputLocation.Text, Window);
        }
             
        //parser + html requester
        private List<WeatherData> GetData(string cityname, Windowforecast Window)
        {
            //get data
            String myUri = "http://api.openweathermap.org/data/2.5/forecast?q=" + cityname + ",us&mode=xml&APPID=92e4ca2d47dd0fa84408dcdf64b62231";
            XmlDocument MyXmlDocument = new XmlDocument();
            MyXmlDocument.Load(myUri);
            MyXmlDocument.Save("xml" + ".dat");
            XmlTextReader reader = new XmlTextReader("xml" + ".dat");

            // xml parsing

            List<WeatherData> MyWeatherData = new List<WeatherData>();
            String DateTime_ = "";
            String Temperature_day = "";
            String Temperature_night = "";
            String MinTemp = "";
            String MaxTemp = "";
            String Precipitation = "";
            String Clouds = "";
            String WindDirection = "";
            String WindSpeed = "";
            String Pressure = "";
            String Humidity = "";
            int a = 0;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.XmlDeclaration:
                        //  s.AppendLine("Описание XML: Name = " + reader.Name + " Value =" + reader.Value);
                        break;
                    case XmlNodeType.Element:
                        if (reader.Name == "name") a = 1;
                        if (reader.Name == "time")
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "day")
                                    DateTime_ = reader.Value;
                                if (reader.Name == "from")
                                    DateTime_ = reader.Value + " - ";
                                if (reader.Name == "to")
                                    DateTime_ += reader.Value;
                            }

                        if (reader.Name == "precipitation")
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "type")
                                    Precipitation = reader.Value;
                            }
                        if (reader.Name == "windDirection")
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "code")
                                    WindDirection = reader.Value;
                            }
                        if (reader.Name == "windSpeed")
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "mps")
                                    WindSpeed = reader.Value;
                            }
                        if (reader.Name == "temperature")
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "day")
                                    Temperature_day = reader.Value;
                                if (reader.Name == "night")
                                    Temperature_night = reader.Value;
                                if (reader.Name == "value")
                                    Temperature_day = reader.Value;
                                if (reader.Name == "max")
                                    MaxTemp = reader.Value;
                                if (reader.Name == "min")
                                    MinTemp = reader.Value;
                            }
                        if (reader.Name == "pressure")
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "value")
                                    Pressure = reader.Value + "hpa";
                            }
                        if (reader.Name == "humidity")
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "value")
                                    Humidity = reader.Value + "%";
                            }
                        if (reader.Name == "clouds")
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "all")
                                    Clouds = reader.Value + "%";
                            }
                        break;
                    case XmlNodeType.Text:
                        if (a == 1)
                        {
                            Window.Title = reader.Value + " - " + " 3 hours / 5 days  " + " forecast." + "on basis of openweathermap.org source";
                            a = 0;
                        }

                        break;
                }
                if (Clouds.Length > 1)
                {
                    MyWeatherData.Add(new WeatherData(DateTime_,
                                                        Temperature_day,
                                                        Temperature_night,
                                                        MinTemp,
                                                        MaxTemp,
                                                        Precipitation,
                                                        Clouds,
                                                        WindDirection,
                                                        WindSpeed,
                                                        Pressure,
                                                        Humidity));
                    DateTime_ = "";
                    Temperature_day = "";
                    Temperature_night = "";
                    Precipitation = "";
                    Clouds = "";
                    WindDirection = "";
                    WindSpeed = "";
                    Pressure = "";
                    Humidity = "";
                }
            }
            reader.Close();
            return MyWeatherData;
        }

     

	}
}
