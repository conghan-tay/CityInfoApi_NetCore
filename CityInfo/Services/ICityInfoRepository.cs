using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.Entities;

namespace CityInfo.Services
{
     public interface ICityInfoRepository
    {
        bool CityExists(int cityId);
        IEnumerable<City> GetCities();

        City GetCity(int cityId, bool includePointsOfInterest);

        IEnumerable<PointOfInterest> GetPointOfInterestsForCity(int cityId);

        PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterest);

        void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest);

        void DeletePointOfInterest(PointOfInterest pointOfInterest);

        bool Save();
    }
}
