using CateringBackend.CrossTests.Client.Requests;
using CateringBackend.CrossTests.Meals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CateringBackend.CrossTests.Client
{
    public class Order
    {
        public Guid Id { get; set; }
        public DietDTO[] Diets { get; set; }
        public DeliveryDetails DeliveryDetails { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Price { get; set; }
        public string Status { get; set; }
        public Complaint[] Complaint { get; set; }

    }
    public class DietDTO
    {
        public Guid DietId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Meal[] Meals { get; set; }
        public int Price { get; set; }
        public bool Vegan { get; set; }
    }
    public class Complaint
    {
        public Guid ComplaintId { get; set; }
        public Guid OrderId { get; set; }
        public string Description { get; set; }
        public string Answer { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
    }
}
