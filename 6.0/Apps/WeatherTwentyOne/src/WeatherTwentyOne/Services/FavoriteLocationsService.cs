using Microsoft.Maui.Storage;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherTwentyOne.Models;

namespace WeatherTwentyOne.Services
{
    public class FavoriteLocationsService
    {
        private const string PreferencesKey = "FavoriteLocations";
        private List<FavoriteLocation> _favoriteLocations;

        public FavoriteLocationsService()
        {
            _favoriteLocations = new List<FavoriteLocation>();
            _favoriteLocations = GetFavoriteLocations();
        }


        public List<FavoriteLocation> GetFavoriteLocations()
        {
            string json = Preferences.Get(PreferencesKey, string.Empty);

            if (string.IsNullOrEmpty(json))
            {
                return new List<FavoriteLocation>();
            }

            return JsonSerializer.Deserialize<List<FavoriteLocation>>(json);
        }
        public bool LocationExists(FavoriteLocation locationToCheck)
        {
            return _favoriteLocations.Any(existingLocation => existingLocation.City.Equals(locationToCheck.City) && existingLocation.State.Equals(locationToCheck.State));
        }

        public void AddFavoriteLocation(FavoriteLocation location)
        {
            if (!LocationExists(location))
            {
                _favoriteLocations.Add(location);
                SaveFavoriteLocations(_favoriteLocations);
            }
        }

        public void RemoveFavoriteLocation(FavoriteLocation location)
        {
            _favoriteLocations.RemoveAll(l => l.City == location.City && l.State == location.State);
            SaveFavoriteLocations(_favoriteLocations);
        }

        private void SaveFavoriteLocations(List<FavoriteLocation> locations)
        {
            string json = JsonSerializer.Serialize(locations);
            Preferences.Set(PreferencesKey, json);
        }
    }
}
