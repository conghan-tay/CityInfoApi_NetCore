using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using CityInfo.Services;
namespace CityInfo.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        private ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService mailService,
            ICityInfoRepository cityInfoRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository; 
           // other way to get logger
           // HttpContext.RequestServices.GetService();
        }

        [HttpGet("{cityId}/pointsofInterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                if(!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"City with id {cityId} not found when accessing points of interes");
                    return NotFound();
                }

                var pointsOfInterestForCity = _cityInfoRepository.GetPointOfInterestsForCity(cityId);
                var pointsOfInterestsResults = AutoMapper.Mapper.
                    Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity);

                return Ok(pointsOfInterestsResults);
               // throw new Exception("Exception Test!!");
                //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
                //if (city == null)
                //{
                //    _logger.LogInformation($"City with {cityId} not found");
                //    return NotFound();
                //}

                //return Ok(city.PointsOfInterest);
            }
            catch(Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}", ex);
                return StatusCode(500, "A problem happened while handling you request");
            }
        }

        [HttpGet("{cityId}/pointsofInterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointsOfInterest(int cityId, int id)
        {

            if (!_cityInfoRepository.CityExists(cityId))
            {
                _logger.LogInformation($"City with id {cityId} not found when accessing points of interes");
                return NotFound();
            }

            var pointOfInterest = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterest == null)
                return NotFound();

            var pointOfInterestResult = AutoMapper.Mapper.
                    Map<PointOfInterestDto>(pointOfInterest);

            return Ok(pointOfInterestResult);
        }

        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            // if request format not correct
            if (pointOfInterest == null)
                return BadRequest();

            if (pointOfInterest.Description == pointOfInterest.Name)
                ModelState.AddModelError("Description", "The provided description should be different from the name");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            //var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);
            var finalPointOfInterest = AutoMapper.Mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            _cityInfoRepository.AddPointOfInterestForCity(cityId, finalPointOfInterest);

            if (!_cityInfoRepository.Save())
                return StatusCode(500, "A problem happened while handling your request!");

            var createdPOI = AutoMapper.Mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);

            // Resp with location header
            // 2nd argument is for the route [HttpGet("{cityId}/pointsofInterest/{id}", Name = "GetPointOfInterest")]
            // 3rd argument for the resp body the inserted Point of interest
            return CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, id = createdPOI.Id }, createdPOI);
        }

        [HttpPut("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id,
            [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            if (pointOfInterest == null)
                return BadRequest();

            if (pointOfInterest.Description == pointOfInterest.Name)
                ModelState.AddModelError("Description", "The provided description should be different from the name");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var poiEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (poiEntity == null)
                return NotFound();

            AutoMapper.Mapper.Map(pointOfInterest, poiEntity);

            if (!_cityInfoRepository.Save())
                return StatusCode(500, "A problem happened while handling your request!");


            // Successful but no content to return
            return NoContent();
        }

        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var poiEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (poiEntity == null)
                return NotFound();

            var pointOfInterestToPatch = AutoMapper.Mapper.Map<PointOfInterestForUpdateDto>(poiEntity);

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
                ModelState.AddModelError("Description", "The provided description should be different from the name");

            TryValidateModel(pointOfInterestToPatch);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AutoMapper.Mapper.Map(pointOfInterestToPatch, poiEntity);

            if (!_cityInfoRepository.Save())
                return StatusCode(500, "A problem happened while handling your request!");

            return NoContent();

        }

        [HttpDelete("{cityId}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var poiEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (poiEntity == null)
                return NotFound();

            _cityInfoRepository.DeletePointOfInterest(poiEntity);

            if (!_cityInfoRepository.Save())
                return StatusCode(500, "A problem happened while handling your request!");

            _mailService.Send("Point of Interest Deleted",
                $"Point of interest {poiEntity.Name} " +
                $"with id {poiEntity.Id} was deleted");

            return NoContent();

        }


    }
}
