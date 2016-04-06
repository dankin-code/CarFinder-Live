using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CarFinder.Models;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace CarFinder.Controllers
{

    public class CarsController : ApiController
    {
        //database connection

        CarData.CarsDB db = new CarData.CarsDB();

        /// <summary>
        /// This retrieves a list of available car model years
        /// </summary>
        /// <returns>IEnumerable list of years</returns>
        [HttpGet]
        [Route("api/years")]
        public async Task<IHttpActionResult> Years()
        {
            var retval = await db.Database.SqlQuery<string>("EXEC Years").ToListAsync();
            if (retval != null)
                return Ok(retval);
            else
                return NotFound();
        }

        /// <summary>
        /// Retrieves a list of available car makes
        /// </summary>
        /// <returns>IEnumerable list of makes</returns>

        [HttpGet]
        [Route("api/makes")] // declare 1 parameter: year
        public async Task<IHttpActionResult> Makes(string year)
        {
            var retval = await db.Database.SqlQuery<string>("EXEC MakesByYear @year", new SqlParameter("year", year)).ToListAsync();
            if (retval != null)
                return Ok(retval);
            else
                return NotFound();
        }

        /// <summary>
        /// Retrieves list of car models available
        /// </summary>
        /// <returns>IEnumerable list of models</returns>
        [HttpGet]
        [Route("api/models")] // declare 2 parameters: year and make
        public async Task<IHttpActionResult> Models(string year, string make)
        {
            var retval = await db.Database.SqlQuery<string>("EXEC ModelsByYearMake @year, @make", new SqlParameter("year", year), new SqlParameter("make", make)).ToListAsync();
            if (retval != null)
                return Ok(retval);
            else
                return NotFound();
        }

        /// <summary>
        /// Retrieves list of available car trims by selecting cars by Year, Make and Model
        /// </summary>
        /// <returns>IEnumerable list of trims</returns>

        [HttpGet]
        [Route("api/trims")] //declare 3 parameters year, make, and model of the car
        public async Task<IHttpActionResult> Trims(string year, string make, string model)
        {
            var retval = await db.Database.SqlQuery<string>("EXEC TrimByYearMakeAndModel  @year, @make, @model", new SqlParameter("year", year), new SqlParameter("make", make), new SqlParameter("model", model)).ToListAsync();
            if (retval != null)
                return Ok(retval);
            else
                return NotFound();
        }

        /// <summary>
        /// This API searches the database for the year, make, model, and model of the car searched and returns photos from Bing search and
        /// any recall data published on the NHTSA web API.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="make"></param>
        /// <param name="model"></param>
        /// <param name="trim"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/cars")] //declare 4 parameters year, make, model, and trim of the car
        public async Task<IHttpActionResult> GetCars(string year, string make, string model, string trim)
        {
            var carData = new CarData();
            carData.myCar = await db.Database.SqlQuery<myCar>("EXEC CarsByYearMakeModelTrim  @year, @make, @model, @trim", new SqlParameter("year", year), new SqlParameter("make", make), new SqlParameter("model", model), new SqlParameter("trim", trim)).FirstAsync();
            
            carData.recalls = Recalls(year, make, model);
            carData.imageURLs = GetImages(year, make, model, null);

            if (carData != null)
                return Ok(carData);
            else
                return NotFound();
        }


        /// <summary>
        /// This function collects recall data information from NHTSA database using their API
        /// http://www.nhtsa.gov/webapi/Default.aspx?Recalls/API/83
        /// recall data infromation from the NHTSA database and has three variables year, makes, and model
        /// </summary>
        /// <param name="year"></param>
        /// <param name="make"></param>
        /// <param name="model"></param>
        /// <returns></returns>

        private Recalls Recalls(string year, string make, string model)
        {
            HttpResponseMessage response;
            Recalls recalls;

            //HttpClient (and certain other web request objects) are not necessarily disposable
            //objects, unlike HttpResponseMessage, whic is. A best practice is to wrap such resources
            // within a using statement to allow early and determined cleanup.


            //string[] retval;

            using (var client = new HttpClient())
            {
                // 1) establish base client address
                client.BaseAddress = new System.Uri("http://www.nhtsa.gov/");

                try
                {
                    // 2) make request to specific URL on the client

                    //var query = webapi/api/Recalls/vehicle/modelyear

                    response = client.GetAsync("webapi/api/Recalls/vehicle/modelyear/" + year + "/make/" + make + "/model/" + model + "?format=json").Result;

                    // 3)construct a Recalls object from the resulting JSON datase
                    recalls = response.Content.ReadAsAsync<Recalls>().Result;
                }

                catch (Exception)
                {
                    //return InternalServerError();
                    return null;
                }
            }

            return recalls;
        }

        /// <summary>
        /// This section searches for the top 5 photos of the selected car from Bing using year, make, model and trim as the parameters
        /// We create a Bing container and pass the assigned key for credentials
        /// </summary>
        /// <param name="year"></param>
        /// <param name="make"></param>
        /// <param name="model"></param>
        /// <param name="trim"></param>
        /// <returns></returns>

        private string[] GetImages(string year, string make, string model, string trim)
        {
            // This is the query - or you could get it from args.
            string query = year + " " + make + " " + model + " " + trim;

            // Create a Bing container.
            string rootUri = "https://api.datamarket.azure.com/Bing/Search";
            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUri));

            // Replace this value with your account key.



            var accountKey = "i52CgTJWkEZb7wR4q0/XNKItRAQrkaUAFZuijkBao+k=";

            // Configure bingContainer to use your credentials.
            bingContainer.Credentials = new NetworkCredential("accountKey", accountKey);

            // Build the query.
            var imageQuery = bingContainer.Image(query, null, null, null, null, null, null).AddQueryOption("$top",5);
                //imageQuery = imageQuery.AddQueryOption("$top", 5);
            var imageResults = imageQuery.Execute();

            //extract the properties needed for the images
            List<string> images = new List<string>();
            foreach (var result in imageResults)
            {
                images.Add(result.MediaUrl);
            }
            return images.ToArray();

        }

    }

}
