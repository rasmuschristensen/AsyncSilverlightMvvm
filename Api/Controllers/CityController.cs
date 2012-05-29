using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Api.Controllers
{
    public class CityController : ApiController
    {           
        /// <summary>
        /// usage: GET /api/cities
        /// </summary>
        /// <returns></returns>
        public IQueryable<City> Get()
        {
            return
                new List<City>
                    {
                        new City {Name = "Copenhagen"},
                        new City {Name = "Washington"},
                        new City {Name = "Paris"},
                        new City {Name = "Rome"}
                    }.AsQueryable();
        }
    }

    public class City
    {
        public string Name { get; set; }
    }
}
