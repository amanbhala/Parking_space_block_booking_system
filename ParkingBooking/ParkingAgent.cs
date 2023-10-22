using System.Text;

namespace ParkingBookingSystem
{
    public class ParkingAgent
    {
        private static bool agentsActive = true;
        private static Random random = new Random();
        private bool parkingNeeded = true;
        private double unitPrice;
        private string parkingId;

        public void ParkingAgentFunc()
        {
            // Continue thread until Parking Structures are no longer active
            while (agentsActive)
            {
                // Check if an order needs to be created
                if (parkingNeeded)
                {
                    createOrder(parkingId);
                }
                else
                {
                    // No orders are needed so sleep the thread for some time
                    Console.WriteLine("Parking Agency ({0}) is currently in wait state.", Thread.CurrentThread.Name);
                    Thread.Sleep(1000);

                    //next time - make an order.
                    parkingNeeded = true;
                }
            }

            Console.WriteLine("Parking agent is not active, shutting down the Thread ({0})", Thread.CurrentThread.Name);
        }

        //Attach a callback which is called when pricecut event is called
        public void following(ParkingStructure parkingStructure)
        {
            Console.WriteLine("Following price cut event", Thread.CurrentThread.Name);
            parkingStructure.PriceCut += orderDetails;
        }

        private void createOrder(string parkingId)
        {

            // Tell system no order is needed
            parkingNeeded = false;
            Order order = new Order();
            order.Quantity = random.Next(10, 50);
            order.CardNumber = random.Next(1000, 9999);
            order.SenderId = Thread.CurrentThread.Name;
            order.ReceiverId = parkingId;

            //Write an order to the MultiCellBuffer.
            ParkingBooking._buff.setOneCell(order);

            Console.WriteLine("({0}) Created order for #{1} units",
                Thread.CurrentThread.Name, order.Quantity);
        }

        public void orderDetails(String id, double price)
        {
            parkingId = id;
            unitPrice = price;
        }

        public void OrderProcessedConfirmation(String senderId, String receiverId)
        {

            if (Thread.CurrentThread.Name.Contains(senderId))
            {
                Console.WriteLine("({0}) received order processed confirmation", Thread.CurrentThread.Name);
            }
        }

        public static bool ParkingsActive
        {
            get { return ParkingAgent.agentsActive; }
            set { ParkingAgent.agentsActive = value; }
        }
    }
}
