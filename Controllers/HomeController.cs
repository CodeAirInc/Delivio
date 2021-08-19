using Delivio.Models;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Delivio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Displays delivery status
        /// </summary>
        public async Task<ActionResult> DeliveryStatus(string userId, string sessionId)
        {
            var model = new Delivio.Models.DeliveryStatusModel
            {
                UserId = userId,
                SessionId = sessionId,
                Zoom = "15"
            };

            string path = AppDomain.CurrentDomain.BaseDirectory + @"deliviotest.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            FirestoreDb db = FirestoreDb.Create("deliviotest");
            Query query = db.Collection("SessionLocations").Document(sessionId).Collection("Locations").OrderByDescending("date");
            QuerySnapshot snaps = await query.GetSnapshotAsync();
            DocumentSnapshot snap = snaps.FirstOrDefault();

            if (snap != null && snap.Exists)
            {
                Dictionary<string, object> currentLocation = snap.ToDictionary();
                model.Latitude = currentLocation.Where(l => l.Key == "latitude").SingleOrDefault().Value.ToString();
                model.Longitude = currentLocation.Where(l => l.Key == "longitude").SingleOrDefault().Value.ToString();
            }

            var query2 = db.Collection("DeliveryByUserId").Document(userId).Collection("DeliverySessions").WhereEqualTo("guid", sessionId);
            QuerySnapshot snaps2 = await query2.GetSnapshotAsync();
            DocumentSnapshot snap2 = snaps2.FirstOrDefault();

            if (snap2 != null && snap2.Exists)
            {
                Dictionary<string, object> currentStatusData = snap2.ToDictionary();
                model.RecipientName = currentStatusData.Where(l => l.Key == "recipientName").SingleOrDefault().Value.ToString();
                model.ItemName = currentStatusData.Where(l => l.Key == "itemName").SingleOrDefault().Value.ToString();
                model.PhotoIds = (List<object>)currentStatusData.Where(l => l.Key == "photoIds").SingleOrDefault().Value;
                var photoUrls = new List<string>();
                foreach (var photoId in model.PhotoIds)
                {
                    var photoUrl = "https://firebasestorage.googleapis.com/v0/b/deliviotest.appspot.com/o/images%2F"
                        + photoId + ".png?alt=media";
                    photoUrls.Add(photoUrl);
                }
                model.PhotoUrls = photoUrls;
                model.IsDeliveryCompleted = false;
                //model.IsDeliveryCompleted = currentStatusData.ContainsKey("isDeliveryCompleted") == true &&
                //        bool.Parse(currentStatusData.Where(l => l.Key == "isDeliveryCompleted").SingleOrDefault().Value.ToString());
                model.DeliveryPlanningTime = DateTime.Parse(currentStatusData.Where(l => l.Key == "createdOn").SingleOrDefault().Value.ToString());
                model.DeliveryStartTime = currentStatusData.ContainsKey("startedOn") == true ?
                    DateTime.Parse(currentStatusData.Where(l => l.Key == "startedOn").SingleOrDefault().Value.ToString()) :
                    DateTime.MinValue;
                model.DeliveryEndTime = DateTime.Parse(currentStatusData.Where(l => l.Key == "endedOn").SingleOrDefault().Value.ToString());
                model.TargetLatitude = currentStatusData.Where(l => l.Key == "targetLatitude").SingleOrDefault().Value.ToString();
                model.TargetLongitude = currentStatusData.Where(l => l.Key == "targetLongitude").SingleOrDefault().Value.ToString();
                model.DeliveryMethod = currentStatusData.ContainsKey("deliverBy") == true ?
                    currentStatusData.Where(l => l.Key == "deliverBy").SingleOrDefault().Value.ToString() : "driving";
                model.IsDeliveryCancelled = currentStatusData.ContainsKey("isDeliveryCancelled") == true ?
                    bool.Parse(currentStatusData.Where(l => l.Key == "isDeliveryCancelled").SingleOrDefault().Value.ToString()) : false;
                model.CancellationReason = currentStatusData.ContainsKey("cancellationReason") == true ?
                        currentStatusData.Where(l => l.Key == "cancellationReason").SingleOrDefault().Value.ToString() : string.Empty;
                model.CancelledOn = currentStatusData.ContainsKey("cancelledOn") == true && !string.IsNullOrWhiteSpace(currentStatusData.Where(l => l.Key == "cancelledOn").SingleOrDefault().Value.ToString()) ?
                    DateTime.Parse(currentStatusData.Where(l => l.Key == "cancelledOn").SingleOrDefault().Value.ToString()) : DateTime.MinValue;
            }
            else
            {
                query2 = db.Collection("DeliveryByUserId").Document(userId).Collection("CompletedSessions").WhereEqualTo("guid", sessionId);
                snaps2 = await query2.GetSnapshotAsync();
                snap2 = snaps2.FirstOrDefault();

                if (snap2 != null && snap2.Exists)
                {
                    Dictionary<string, object> currentStatusData = snap2.ToDictionary();
                    model.RecipientName = currentStatusData.Where(l => l.Key == "recipientName").SingleOrDefault().Value.ToString();
                    model.ItemName = currentStatusData.Where(l => l.Key == "itemName").SingleOrDefault().Value.ToString();
                    model.PhotoIds = (List<object>)currentStatusData.Where(l => l.Key == "photoIds").SingleOrDefault().Value;
                    var photoUrls = new List<string>();
                    foreach (var photoId in model.PhotoIds)
                    {
                        var photoUrl = "https://firebasestorage.googleapis.com/v0/b/deliviotest.appspot.com/o/images%2F"
                            + photoId + ".png?alt=media";
                        photoUrls.Add(photoUrl);
                    }
                    model.PhotoUrls = photoUrls;
                    model.IsDeliveryCompleted = true;
                    model.DeliveryPlanningTime = DateTime.Parse(currentStatusData.Where(l => l.Key == "createdOn").SingleOrDefault().Value.ToString());
                    model.DeliveryStartTime = DateTime.Parse(currentStatusData.Where(l => l.Key == "startedOn").SingleOrDefault().Value.ToString());
                    model.DeliveryEndTime = DateTime.Parse(currentStatusData.Where(l => l.Key == "endedOn").SingleOrDefault().Value.ToString());
                    model.ProofPhotoId = currentStatusData.ContainsKey("proofPhotoId") == true ?
                        currentStatusData.Where(l => l.Key == "proofPhotoId").SingleOrDefault().Value.ToString() : string.Empty;
                    model.ProofPhotoUrl = !string.IsNullOrWhiteSpace(model.ProofPhotoId) ?
                        "https://firebasestorage.googleapis.com/v0/b/deliviotest.appspot.com/o/images%2F" + model.ProofPhotoId + ".png?alt=media" :
                        string.Empty;
                    model.DeliveryMethod = currentStatusData.ContainsKey("deliverBy") == true ?
                        currentStatusData.Where(l => l.Key == "deliverBy").SingleOrDefault().Value.ToString() : "driving";
                    model.IsDeliveryCancelled = currentStatusData.ContainsKey("isDeliveryCancelled") == true ?
                        bool.Parse(currentStatusData.Where(l => l.Key == "isDeliveryCancelled").SingleOrDefault().Value.ToString()) : false;
                    model.CancellationReason = currentStatusData.ContainsKey("cancellationReason") == true ?
                        currentStatusData.Where(l => l.Key == "cancellationReason").SingleOrDefault().Value.ToString() : string.Empty;
                    model.CancelledOn = currentStatusData.ContainsKey("cancelledOn") == true && !string.IsNullOrWhiteSpace(currentStatusData.Where(l => l.Key == "cancelledOn").SingleOrDefault().Value.ToString()) ?
                        DateTime.Parse(currentStatusData.Where(l => l.Key == "cancelledOn").SingleOrDefault().Value.ToString()) : DateTime.MinValue;
                }
            }

            var query3 = db.Collection("DelivioUsers").WhereEqualTo("email", userId);
            QuerySnapshot snaps3 = await query3.GetSnapshotAsync();
            DocumentSnapshot snap3 = snaps3.FirstOrDefault();

            if (snap3 != null && snap3.Exists)
            {
                Dictionary<string, object> currentUserData = snap3.ToDictionary();
                model.DriverFirstName = currentUserData.Where(l => l.Key == "firstName").SingleOrDefault().Value.ToString();
                model.DriverLastName = currentUserData.Where(l => l.Key == "lastName").SingleOrDefault().Value.ToString();
                model.DriverPhoneNumber = currentUserData.Where(l => l.Key == "phonenumber").SingleOrDefault().Value.ToString();
                model.RestaurantName = currentUserData.Where(l => l.Key == "restaurantName").SingleOrDefault().Value.ToString();
                var carMake = currentUserData.ContainsKey("carMake") == true ?
                    currentUserData.Where(l => l.Key == "carMake").SingleOrDefault().Value.ToString() : string.Empty;
                var carModel = currentUserData.ContainsKey("carModel") == true ?
                    currentUserData.Where(l => l.Key == "carModel").SingleOrDefault().Value.ToString() : string.Empty;
                model.DriverVehicle = !string.IsNullOrWhiteSpace(carMake) ?
                    carMake + " " + carModel : string.Empty;
                model.VehiclePlateNumber = currentUserData.ContainsKey("plateNumber") == true ?
                    currentUserData.Where(l => l.Key == "plateNumber").SingleOrDefault().Value.ToString() : string.Empty;
                model.VehiclePhotoId = currentUserData.Where(l => l.Key == "vehiclePhotoId").SingleOrDefault().Value.ToString();
                model.VehiclePhotoUrl = "https://firebasestorage.googleapis.com/v0/b/deliviotest.appspot.com/o/images%2F"
                    + model.VehiclePhotoId + ".png?alt=media";
                model.UserPhotoId = currentUserData.Where(l => l.Key == "userPhotoId").SingleOrDefault().Value.ToString();
                model.UserPhotoUrl = "https://firebasestorage.googleapis.com/v0/b/deliviotest.appspot.com/o/images%2F"
                    + model.UserPhotoId + ".png?alt=media";
            }

            model.ImagesUrl = _config.GetValue<string>("DelivioUrl") + "/images";
            model.GoogleMapApiSrc = _config.GetValue<string>("GoogleMapApiSrc");

            return View(model);
        }
    }
}
