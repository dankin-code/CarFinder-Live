using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CarFinder.Models
{
    /// <summary>
    /// Definition of the myCar class with the different specifications for the vehicle searched in the database.
    /// </summary>
    public class myCar
    {
        public int id { get; set; }
        public string make { get; set; }
        public string model_name { get; set; }
        public string model_trim { get; set; }
        public string model_year { get; set; }
        public string body_style { get; set; }
        public string engine_cc { get; set; }
        public string engine_num_cyl { get; set; }
        public string engine_power_rps { get; set; }
        public string engine_torque_nm { get; set; }
        public string engine_compression { get; set; }
        public string drive_type { get; set; }
        public string transmission_type { get; set; }
        public string seats { get; set; }
        public string doors { get; set; }
        public string fuel_capacity_l { get; set; }
    }

    public class Recalls
    {
        int Count { get; set; }
        string Message { get; set; }
        public RecallItem[] Results { get; set; }
        public Recalls Result { get; set; }//added by IDE
    }

    public class RecallItem  //copy the list below as JSON string class
    {
        string Manufacturer { get; set; }
        string NHTSACampaignNumber { get; set; }
        string Component { get; set; }
        string Summary { get; set; }
        string Conequence { get; set; }
        string Remedy { get; set; }
    }

    public class CarData
    {
        public myCar myCar { get; set; }
        public Recalls recalls { get; set; }
        public string[] imageURLs { get; set; }


    public class CarsDB : System.Data.Entity.DbContext
        {

        public CarsDB() : base("AzureConnection")
            {

            }

    public static CarsDB Create()
        {
            return new CarsDB();
        }

     }
    }
}