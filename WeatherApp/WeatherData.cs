using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    class WeatherData
        {
            //public String Location { get; set; }
            public String DateTime { get; set; } // data i vremia, dla po4asovogo nado otobrazhatj i vremia
            public String Temperature { get; set; } //Temprature
            public String Temperature_night { get; set; } //Temprature
            public String MinTemp { get; set; } //Temprature
            public String MaxTemp { get; set; } //Temprature
            public String Precipitation { get; set; }
            public String Clouds { get; set; }  //Clouds
            public String WindDirection { get; set; }
            public String WindSpeed { get; set; }  //WSpeed
            public String Pressure { get; set; }  //
            public String Humidity { get; set; }  //
            public WeatherData(String dt, String td, String tn, String mint, String maxt, String Precip, String c, String wd, String ws, String p, String h)
            {
                // this.Location=Loc;    
                this.DateTime = dt;
                this.Temperature = td;
                this.Temperature_night = tn;
                this.MinTemp = mint;
                this.MaxTemp = maxt;
                this.Precipitation = Precip;
                this.Clouds = c;
                this.WindDirection = wd;
                this.WindSpeed = ws;
                this.Pressure = p;
                this.Humidity = h;
            }
    }
}
