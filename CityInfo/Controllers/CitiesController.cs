using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CityInfo.Services;
using CityInfo.Models;
using System.Diagnostics;

namespace CityInfo.Controllers
{
    [Route("api/cities")]
    public class CitiesController : Controller
    {
        private ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet()]
        public IActionResult GetCities()
        {
            //  return Ok(CitiesDataStore.Current.Cities);
            var cityEntities = _cityInfoRepository.GetCities();
            var results = AutoMapper.Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);
            //var results = new List<CityWithoutPointsOfInterestDto>();

            //foreach (var cityEntity in cityEntities)
            //{
            //    results.Add(new CityWithoutPointsOfInterestDto {
            //        Id = cityEntity.Id,
            //        Description = cityEntity.Description,
            //        Name = cityEntity.Name
            //    });
            //}

            return Ok(results);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointsOfInterest = false, string testQuery = "Nothing")
        {
            var city = _cityInfoRepository.GetCity(id, includePointsOfInterest);
            if(city == null)
            {
                return NotFound();
            }

            if(includePointsOfInterest)
            {
                var cityResult = AutoMapper.Mapper.Map<CityDto>(city);
                return Ok(cityResult);
            }

            var cityWithoutPointsOfInterest = AutoMapper.Mapper.Map<CityWithoutPointsOfInterestDto>(city);

            return Ok(cityWithoutPointsOfInterest);
        }


        //[HttpGet("{id}")]
        //public IActionResult GetCity(int id)
        //{
        //    var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
        //    if (cityToReturn == null)
        //        return NotFound();
        //    return Ok(cityToReturn);
        //}

    }
}
