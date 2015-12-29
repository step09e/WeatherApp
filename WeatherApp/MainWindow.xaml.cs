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
using System.Net;

namespace WeatherApp {

	public partial class MainWindow : Window {

		public MainWindow() {
			InitializeComponent();
		}

		private async void Button_Click(object sender, RoutedEventArgs e) {
			var xmlStr = await GetXmlFromOpenWeather(InputField.Text);
			var doc = new XmlDocument();
			doc.LoadXml(xmlStr);
			var data = GetDataFromXml(doc);
			if (data.Forecasts.Count > 0) {
				Windowforecast window = new Windowforecast();
				window.Title = data.LocationName;
				LocationLabel.Content = data.LocationName;
				window.Show();
				window.forecastDataGrid.Columns.Clear();
				var columns = data.GetColumns();
				for (int i = 0; i < columns.Length; i++) {
					window.forecastDataGrid.Columns.Add(columns[i]);
				}
				window.forecastDataGrid.ItemsSource = data.Forecasts;
			}
			else {
				MessageBox.Show("no data received");
			}
		}

		async Task<string> GetXmlFromOpenWeather(string cityname) {
			var uri = new Uri("http://api.openweathermap.org/data/2.5/forecast?q=" + cityname + "&mode=xml&APPID=92e4ca2d47dd0fa84408dcdf64b62231"); ///Вот тут таился чёрт. ",us" добавлялся ко всем запросам, а мы все смотрели на эту строчку, и не замечали...
			var xml = await new WebClient().DownloadStringTaskAsync(uri);
			return xml;
		}

		/// <summary>
		/// Parse xml from openweathermap.org
		/// </summary>
		ForecastsList GetDataFromXml(XmlDocument doc) {
			doc.Save("forecast.xml"); //сохранять больше не требуется, но будем и дальше это делать для сущей красоты
			var result = new ForecastsList();
			result.ParseUnitsFromXml(doc);

			var elements = doc.SelectNodes("//time");

			foreach (XmlNode node in elements)
			{
				//парсинг с помощью xpath
				var forecast = new WeatherData();
				var timeFrom = node.SelectSingleNode("@from").Value;
				var timeTo = node.SelectSingleNode("@to").Value;
				forecast.TimeFrom = /*DateTime.Parse(*/timeFrom;//);
				forecast.TimeTo = /*DateTime.Parse(*/timeTo;//);
				forecast.Temperature = /*float.Parse(*/node.SelectSingleNode("temperature/@value").Value;//);
				forecast.WindDirection = node.SelectSingleNode("windDirection/@name").Value;
				forecast.WindSpeed = /*float.Parse(*/node.SelectSingleNode("windSpeed/@mps").Value;//);
				forecast.WindDescription = node.SelectSingleNode("windSpeed/@name").Value;
				forecast.Pressure = /*float.Parse(*/node.SelectSingleNode("pressure/@value").Value;//);
				var _precipitationType = node.SelectSingleNode("precipitation/@type");
				forecast.PrecipitationDescription = _precipitationType == null ? "" : _precipitationType.Value;
				if (forecast.PrecipitationDescription == "show") {
					forecast.PrecipitationDescription = "snow";
				}
				var _precValue = /*float.Parse(*/node.SelectSingleNode("precipitation/@value");//);
				forecast.Precipitation = _precValue == null ? "" : _precValue.Value;
				forecast.Humidity = /*float.Parse(*/node.SelectSingleNode("humidity/@value").Value;//);
				forecast.CloudsDescription = node.SelectSingleNode("clouds/@value").Value;
				forecast.Clouds = node.SelectSingleNode("clouds/@all").Value;
				result.Forecasts.Add(forecast);
			}		
			return result;
		}

		[Obsolete()]
		/// <summary>
		/// deprecated
		/// </summary>
		WeatherData[] GetDataFromXml(string xml){
			
			//if (xml == ""){
				return new WeatherData[0];//вместо null лучше возвращать пустой массив в таких случаях
			//}
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
			doc.Save("xml.dat");
			XmlTextReader reader = new XmlTextReader("xml.dat");
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
			string realname = "";
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.XmlDeclaration:
						//  s.AppendLine("Описание XML: Name = " + reader.Name + " Value =" + reader.Value);
						break;
					case XmlNodeType.Element:
						if (reader.Name == "name")
							a = 1;
						else if (reader.Name == "time")
							while (reader.MoveToNextAttribute()) {
								if (reader.Name == "day")
									DateTime_ = reader.Value;
								if (reader.Name == "from")
									DateTime_ = reader.Value + " - ";
								if (reader.Name == "to")
									DateTime_ += reader.Value;
							}

						else if (reader.Name == "precipitation")
							while (reader.MoveToNextAttribute()) {
								if (reader.Name == "type")
									Precipitation = reader.Value;
							}
						else if (reader.Name == "windDirection")
							while (reader.MoveToNextAttribute()) {
								if (reader.Name == "code")
									WindDirection = reader.Value;
							}
						else if (reader.Name == "windSpeed")
							while (reader.MoveToNextAttribute()) {
								if (reader.Name == "mps")
									WindSpeed = reader.Value;
							}
						else if (reader.Name == "temperature")
							while (reader.MoveToNextAttribute()) {
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
						else if (reader.Name == "pressure")
							while (reader.MoveToNextAttribute()) {
								if (reader.Name == "value")
									Pressure = reader.Value + "hpa";
							}
						else if (reader.Name == "humidity")
							while (reader.MoveToNextAttribute()) {
								if (reader.Name == "value")
									Humidity = reader.Value + "%";
							}
						else if (reader.Name == "clouds")
							while (reader.MoveToNextAttribute()) {
								if (reader.Name == "all")
									Clouds = reader.Value + "%";
							}
						break;
					case XmlNodeType.Text:
						if (a == 1) {
							//Window.Title = reader.Value + " - " + " 3 hours / 5 days  " + " forecast." + "on basis of openweathermap.org source";
							//не подходящее место для изменения заголовка. нужно немного подумать и изменить архитектуру так, чтобы это можно было сделать вне функции.
							realname = reader.Value; //временное решение. я потом всё поменяю
							a = 0;
						}

						break;
				}
				if (Clouds == "") {
					continue;
				}
				//MyWeatherData.Add(new WeatherData {
				//	DateTime = DateTime_,
				//	Temperature = Temperature_day,
				//	Temperature_night = Temperature_night,
				//	MinTemp = MinTemp,
				//	MaxTemp = MaxTemp,
				//	Precipitation = Precipitation,
				//	Clouds = Clouds,
				//	WindDirection = WindDirection,
				//	WindSpeed = WindSpeed,
				//	Pressure = Pressure,
				//	Humidity = Humidity
				//});

				DateTime_ = "";
				Temperature_day = "";
				Temperature_night = "";
				MinTemp = "";
				MaxTemp = "";
				Precipitation = "";
				Clouds = "";
				WindDirection = "";
				WindSpeed = "";
				Pressure = "";
				Humidity = "";


			}
			//reader.Close();
			//MyWeatherData.Add(new WeatherData() {
			//	//Clouds = realname //будем знать, что в конце списка лежит имя. это временное решение, так делать не стоит
			//});
			return MyWeatherData.ToArray();
		}
	}

}

