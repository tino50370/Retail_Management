using Geolocation;
using Newtonsoft.Json;
using RetailKing.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;

namespace RetailKing
{
    public class Geolocator
    {
        RetailKingEntities db = new RetailKingEntities();
        public IpInfo GetUserLocationDetailsByIp(string ip)
        {
            var ipInfo = new IpInfo();
            try
            {
                var info = new WebClient().DownloadString("http://freegeoip.net/json/" + ip);
                ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
            }
            catch (Exception ex)
            {
                //Exception Handling
            }

            return ipInfo;
        }

        public IpInfo GetBounderies(double  minLatitude, double minLongitude, int radius)
        {
            var ipInfo = new IpInfo();
            CoordinateBoundaries boundaries = new CoordinateBoundaries(minLatitude, minLongitude, radius, DistanceUnit.Kilometers);     
            double maxLatitude = boundaries.MaxLatitude;
            double maxLongitude = boundaries.MaxLongitude;
            ipInfo.Latitude = maxLatitude.ToString();
            ipInfo.Longitude = maxLongitude.ToString();
            return ipInfo;
        }

        public List<Advert> GetAdvert(IpInfo origin)
        {
            Random xskip = new Random();
            var dd = DateTime.Now.AddDays(-1);
            double Latitude = double.Parse(origin.Latitude);
            double Longitude = double.Parse(origin.Longitude);

            var results = db.Adverts
            .Where(x => x.minLatitude != null && (x.minLatitude <= Latitude && x.maxLatitude >= Latitude))
            .Where(x => x.maxLongitude != null && (x.minLongitude <= Longitude && x.maxLongitude >= Longitude))
            .Where(x => x.ExpireryDate > dd)
            .ToList();
            if (results.Count > 10)
            {
                var s = xskip.Next(0, results.Count - 5);
                results = results.Skip(s).Take(10).ToList();
            } 
            
            return results;
        }
   
    }
}