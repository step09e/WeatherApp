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
//using System.Windows.Shapes;
using System.Web.Script.Serialization;
using System.IO;
using System.Net;

namespace WeatherApp {

	public partial class MainWindow : Window {

		public static readonly string JsonFileName = "ua_cities.json";
		string[] _locations = new string[0];
		List<string> _currentLocations = new List<string>();
		static readonly int MaxDrobHelpItems = 8;

		public MainWindow() {
			InitializeComponent();
			LoadCitiesBase();
			HelperDropBox.ItemsSource = _currentLocations;
		}

		void LoadCitiesBase() {
			var path = AppDomain.CurrentDomain.BaseDirectory;
			var root = Path.GetPathRoot(path);

			bool fileExists = false;
			if (File.Exists(Path.Combine(path, JsonFileName))) {
				fileExists = true;
			}
			else {
				while (path != "" && path != root) {
					path = Directory.GetParent(path).ToString();
					if (File.Exists(Path.Combine(path, JsonFileName))) {
						fileExists = true;
						break;
					}
				}
			}
			if (fileExists) {
				var resultList = new List<string>();
				using (var reader = new StreamReader(Path.Combine(path, JsonFileName))) {
					string line = "";
					while (( line = reader.ReadLine() ) != null) {
						JavaScriptSerializer js = new JavaScriptSerializer();
						var dict = js.Deserialize<Dictionary<string, string>>(line);
						if (dict.ContainsKey("city")) {
							resultList.Add(dict["city"]);
						}
					}
				}
				resultList = resultList.ConvertAll(x => x.ToLower());
				_locations = resultList.OrderBy(x => x).ToArray();
			}
			else {
				MessageBox.Show(JsonFileName + " not found");
			}
		}

		private async void Button_Click(object sender, RoutedEventArgs e) {
			var xmlStr = await GetXmlFromOpenWeather(InputField.Text);
			var doc = new XmlDocument();
			doc.LoadXml(xmlStr);
			var data = GetDataFromXml(doc);
			if (data.Forecasts.Count > 0) {
  				LocationLabel.Content = data.LocationName;
				var columns = data.GetColumns();
				for (int i = 0; i < columns.Length; i++) {
					forecastDataGridw.Columns.Add(columns[i]);
				}
				forecastDataGridw.ItemsSource = data.Forecasts;
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

			//парсинг с помощью xpath
			foreach (XmlNode node in elements) {
				var forecast = new WeatherData();

				var timeFromValue = node.SelectSingleNode("@from").Value;
				var timeToValue = node.SelectSingleNode("@to").Value;
				var timeFrom = DateTime.Parse(timeFromValue);
				var timeTo = DateTime.Parse(timeToValue);
				forecast.DateStr = timeFrom.ToShortDateString();
				forecast.TimeStr = timeFrom.ToShortTimeString() + " - " + timeTo.ToShortTimeString();
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

		private void InputField_TextChanged(object sender, TextChangedEventArgs e) {
			if (LocationLabel != null) {
				UpdateDropDown( InputField.Text );
			}
		}

		void UpdateDropDown(string query) {
			_currentLocations.Clear();
			var matchLocations = _locations.BinaryMinMaxIndexesSearch(query);
			if (matchLocations.Item1 >= 0 && matchLocations.Item2 >= 0 && matchLocations.Item1 < _locations.Length && matchLocations.Item2 < _locations.Length) {
				for (int i = matchLocations.Item1; i <= matchLocations.Item2; i++) {
					_currentLocations.Add(_locations[i]);
					if (_currentLocations.Count >= MaxDrobHelpItems) {
						break;
					}
				}
				HelperDropBox.Items.Refresh();
				HelperDropBox.IsDropDownOpen = true;
			}
			else {
				HelperDropBox.IsDropDownOpen = false;
			}
		}

		private void HelperDropBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			InputField.Text = (string)HelperDropBox.SelectedValue;
		}
	}

}

