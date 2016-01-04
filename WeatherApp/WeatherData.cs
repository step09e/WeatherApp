using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace WeatherApp {

	class ForecastsList {
		public string LocationName = "";
		public string TemperatureUnits = "";
		public string CloudsUnits = "";
		public string HumidityUnits = "";
		public string PressureUnits = "";
		public string PrecipitationUnits = "";
		public string WindUnits = "";
		public List<WeatherData> Forecasts = new List<WeatherData>();

		public DataGridTextColumn[] GetColumns() {
			var result = new DataGridTextColumn[12];
			for (int i = 0; i < result.Length; i++) {
				result[i] = new DataGridTextColumn();
				//result[i].
				result[i].IsReadOnly = true;
			}
			result[0].Header = "TimeFrom";
			result[0].Binding = new Binding("TimeFrom");
			result[1].Header = "TimeTo";
			result[1].Binding = new Binding("TimeTo");
			result[2].Header = "Temperature" + ( TemperatureUnits == "" ? "" : " (" + TemperatureUnits + ")" );
			result[2].Binding = new Binding("Temperature");
			result[3].Header = "Precipitation Value" + ( PrecipitationUnits == "" ? "" : " (" + PrecipitationUnits + ")" );
			result[3].Binding = new Binding("Precipitation");
			result[4].Header = "Precipitation Descr";
			result[4].Binding = new Binding("PrecipitationDescription");
			result[5].Header = "Clouds Value" + ( CloudsUnits == "" ? "" : " (" + CloudsUnits + ")" );
			result[5].Binding = new Binding("Clouds");
			result[6].Header = "Clouds Descr";
			result[6].Binding = new Binding("CloudsDescription");
			result[7].Header = "Wind Direction";
			result[7].Binding = new Binding("WindDirection");
			result[8].Header = "Wind Speed" + ( WindUnits == "" ? "" : " (" + WindUnits + ")" );
			result[8].Binding = new Binding("WindSpeed");
			result[9].Header = "Wind Descr";
			result[9].Binding = new Binding("WindDescription");
			result[10].Header = "Pressure" + ( PressureUnits == "" ? "" : " (" + PressureUnits + ")" );
			result[10].Binding = new Binding("Pressure");
			result[11].Header = "Humidity" + ( HumidityUnits == "" ? "" : " (" + HumidityUnits + ")" );
			result[11].Binding = new Binding("Humidity");
			return result;
		}

		public void ParseUnitsFromXml(XmlDocument doc) {
			if (doc != null) {
				LocationName = doc.SelectSingleNode("//location/name").InnerText;
				TemperatureUnits = doc.SelectSingleNode("//temperature/@unit").Value;
				CloudsUnits = doc.SelectSingleNode("//clouds/@unit").Value;
				HumidityUnits = doc.SelectSingleNode("//humidity/@unit").Value;
				PressureUnits = doc.SelectSingleNode("//forecast/time/pressure/@unit").Value;
				WindUnits = "mps";
				PrecipitationUnits = doc.SelectSingleNode("//precipitation/@unit").Value;
			}
		}
	}

	class WeatherData {
		public string TimeFrom { get; set;} // не пишите транслитом, пожалуйста. никогда.
		public string TimeTo { get;	set;}
		public string Temperature {	get; set;}
		public string Precipitation { get; set;}
		public string PrecipitationDescription { get; set;}
		public string Clouds { get; set;}
		public string CloudsDescription { get; set;}
		public string WindDirection { get; set;}
		public string WindSpeed { get; set;}
		public string WindDescription { get; set;}
		public string Pressure { get; set;}
		public string Humidity { get; set;}
	}
}
