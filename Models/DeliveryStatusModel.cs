using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delivio.Models
{
    public class DeliveryStatusModel
    {
        public string UserId { get; set; }
        public string SessionId { get; set; }
        public string Zoom { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string RecipientName { get; set; }
        public string ItemName { get; set; }
        public List<object> PhotoIds { get; set; }
        public List<string> PhotoUrls { get; set; }
        public bool IsDeliveryCompleted { get; set; }
        public string DriverFirstName { get; set; }
        public string DriverLastName { get; set; }
        public string DriverPhoneNumber { get; set; }
        public string DriverVehicle { get; set; }
        public string VehiclePlateNumber { get; set; }
        public string RestaurantName { get; set; }
        public DateTime DeliveryPlanningTime { get; set; }
        public DateTime DeliveryStartTime { get; set; }
        public DateTime DeliveryEndTime { get; set; }
        public string VehiclePhotoId { get; set; }
        public string VehiclePhotoUrl { get; set; }
        public string UserPhotoId { get; set; }
        public string UserPhotoUrl { get; set; }
        public string ProofPhotoId { get; set; }
        public string ProofPhotoUrl { get; set; }
        public DateTime EstimatedArrivalTime { get; set; }
        public string TargetLatitude { get; set; }
        public string TargetLongitude { get; set; }
        public string DeliveryMethod { get; set; }
        public bool IsDeliveryCancelled { get; set; }
        public string CancellationReason { get; set; }
        public DateTime CancelledOn { get; set; }
        public string ImagesUrl { get; set; }
        public string GoogleMapApiSrc { get; set; }
    }
}
