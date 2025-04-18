﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TEC_WMS_API.Models.RequestModel
{
    

    public class DeviceRequest
    {
        
        public int? DeviceId { get; set; }  // Nullable since it's auto-generated

        public string? UserName { get; set; }
        public string? DeviceSerialNo { get; set; }
        public string? CreatedBy { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }

    public class UpdateDeviceRequest
    {
        [JsonIgnore] // 🚨 Excludes DeviceId from the request body
        public int? DeviceId { get; set; }
        public string? UserName { get; set; }
        public string? DeviceSerialNo { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
    public class DevicedropdownRequest
    {
        public int? DeviceId { get; set; }
        public string? DeviceSerialNo { get; set; }
    }
}
