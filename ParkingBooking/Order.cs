using System.Globalization;

namespace ParkingBookingSystem
{
    public class Order
    {
        // ParkingAgencyId which placed the order
        public string SenderId { get; set; }

        // ParkingStructureId which will receive the order
        public string ReceiverId { get; set; }

        // Credit Card number
        public long CardNumber { get; set; }

        // Number of parking spaces to order
        public int Quantity { get; set; }

        // Unit price of each parking space.
        public double UnitPrice { get; set; }

        // Current date and time
        public DateTime OrderDate { get; set; }
    }
}

