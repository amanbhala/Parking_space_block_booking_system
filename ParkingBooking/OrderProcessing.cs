using System.Text.RegularExpressions;

namespace ParkingBookingSystem
{
    public class OrderProcessing
    {
        private Random rng = new Random();

        private Order order;
        private double unitPrice;

        public delegate void OrderProcessedHandler(String id, String receiverId);

        //The price cut event, triggered when there is a price cut.
        public static event OrderProcessedHandler orderProcessed;

        public OrderProcessing(Order order, double unitPrice)
        {
            this.Order = order;
            this.UnitPrice = unitPrice;
        }

        public void ProcessOrder()
        {
            if (order != null)
            {
                String orderDetails = order.SenderId + "_" + order.ReceiverId;
                Console.WriteLine("Processing ({0}) Parking Agency Order {1} meant for Parking Structure ", Thread.CurrentThread.Name, order.SenderId, order.ReceiverId);

                // Check for a valid credit card number
                if (IsCreditCardValid(order.CardNumber))
                {
                    Console.WriteLine("({0}) - You have entered a valid credit card number", Thread.CurrentThread.Name);
                }
                else
                {
                    Console.WriteLine("({0}) - You have entered an invalid card number, Order will not be processed.", Thread.CurrentThread.Name);
                    return;
                }

                //Calculating tax between 8% and 12%
                double taxation = 1.0 + rng.Next(8, 12) / 100.0;
                //Calculating location charge between $2 and $8
                int locationCharge = rng.Next(2, 8);

                //final price = unitPrice*amount_of_parkings_spaces + Tax + location charge.
                double totalAmount = Math.Round(taxation * (unitPrice * order.Quantity) + locationCharge, 2);

                orderProcessed(order.SenderId, order.ReceiverId);
                Console.WriteLine("( Following order has been processd. {0} {1}. Total price: ${2} , Total quantity ${3}, each unit price ${4})",
                    Thread.CurrentThread.Name, orderDetails, totalAmount, order.Quantity, unitPrice);
            }
            else
            {
                Console.WriteLine("({0}) Order is empty.", Thread.CurrentThread.Name);
            }
        }

        //Check if the card no. is valid. For this simple usecase, it
        //only checks if the card num is between 5000 and 7000.
        private Boolean IsCreditCardValid(long cardNo)
        {
            // Makes sure that the credit-card number is a valid number 
            return (cardNo <= 9999 && cardNo >= 1000);
        }

        //Order accessor.
        public Order Order
        {
            get { return order; }
            private set { order = value; }
        }


        //Unit price accessor.
        public double UnitPrice
        {
            get { return unitPrice; }
            set { unitPrice = value; }
        }

    }
}
