using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using RestSharp;

namespace Services.MapBox;
public class DistanceCalculation
{
    public async Task<List<LocationDistance>> GetDistancesAsync(double latitude, double longtitude, List<Court> destinations)
    {

        var locationList = destinations.ToList();

        //Tao Url de ket noi den MapBox
        string apiKey = "pk.eyJ1IjoiYmluaG5kMTYwNDE3IiwiYSI6ImNseGFpdG02MDJyYWIyeHNkbGlnMHp6emwifQ.J0mzr3tnVm0z8oybTD76ZA";
        var client = new RestClient("https://api.mapbox.com");
        var request = new RestRequest($"/directions-matrix/v1/mapbox/driving/{longtitude},{latitude};{string.Join(";", destinations.ToList().Select(d => $"{d.Longitude},{d.Latitude}"))}?access_token={apiKey}&annotations=distance");

        //Send Url
        var response = await client.GetAsync(request);

        var distances = new List<LocationDistance>();

        if (response.IsSuccessful)
        {
            //Tu response tra ve, lay ra cac element result ket qua tinh khoang cach
            var jsonResponse = JsonDocument.Parse(response.Content);
            var distancesArray = jsonResponse.RootElement.GetProperty("distances").EnumerateArray().First().EnumerateArray();

            int index = 0;

            foreach (var distanceElement in distancesArray)
            {
                // Kiểm tra xem khoảng cách có hợp lệ không
                if (distanceElement.ValueKind == JsonValueKind.Null)
                    continue;
                var distanceMeters = distanceElement.GetDouble();

                //Khong hieu sao luon co 1 element bi thua, nen loai bo
                if (distanceMeters == 0)
                {
                    continue;
                }

                distances.Add(new LocationDistance
                {
                    Location = locationList.ElementAt(index),
                    DistanceInKm = Math.Round(distanceMeters / 1000.0, 2) // Chuyen meters to kilometers, lam tron sau dau phay 2 chu so 
                });
                index++;
            }
        }

        return distances;
    }

}

public class LocationDistance
{
    public Court Location { get; set; }
    public double DistanceInKm { get; set; }
}

