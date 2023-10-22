using System.Collections;
using System.Text;

namespace ParkingBookingSystem
{
    public class ParkingStructure
    {
        private const int MAXIMUM_PRICE_CUT_EVENTS_ALLOWED = 10; // Constant for Maximum Price Cuts

        private int priceCutCount = 1; // Counter for the price cuts
        private double currentPrice = 0.0; // current parking price.
        private double previousPrice = 0.0; // previous parking price.
        private static Random random = new Random(); // Random number generator

        private ArrayList processingThreads = new ArrayList();

        public delegate void PriceCutHandler(String id, double price);

        //The price cut event, triggered when there is a price cut.
        public event PriceCutHandler PriceCut;

        //Emits the `PriceCut` event.
        private void PriceCutEvent()
        {
            // Make sure the event is subscribed to
            if (PriceCut != null)
            {
                // Fire the PriceCut event
                Console.WriteLine("Executing a Price Cut Event (#{0}) ({1})",
                    priceCutCount, Thread.CurrentThread.Name);
                priceCutCount++;
                PriceCut(Thread.CurrentThread.Name, currentPrice);
            }
            else
            {
                Console.WriteLine("No PriceCut event subscribers, Event not send to nodbody");
            }
        }

        //Entrypoint for the thread.
        public void ParkingStructureFunc()
        {
            // Continue until `` price cuts have been sent
            while (priceCutCount <= MAXIMUM_PRICE_CUT_EVENTS_ALLOWED)
            {
                // Calculate the Unit Price for rooms from the Pricing model
                pricingModel();


                Console.WriteLine("({0}) - Validating if it is a price cut event, old price to new price ({1} to {2})",
                    Thread.CurrentThread.Name, previousPrice, currentPrice);

                // Check if the previous price was more than the current price
                if (currentPrice < previousPrice)
                {
                    Console.WriteLine("({0}) - Price Cut event happened. Price has dropped from ({1} to {2}).",
                        Thread.CurrentThread.Name, previousPrice, currentPrice);
                    // PriceCut has been made, trigger the event
                    PriceCutEvent();
                }

                // Initialize the Previous Price to the Current Price
                previousPrice = currentPrice;

                // Retrieve and Process orders from the Multi-Cell buffer
                ProcessOrder(getOrder());
            }

            foreach (Thread item in processingThreads)
            {
                while (item.IsAlive) ;
            }

            Console.WriteLine("Desired Price cut events happened, terminating Parking Structure Thread ({0})",
                Thread.CurrentThread.Name);
        }

        //Pricing function to generating price between 10 and 40.
        public void pricingModel()
        {
            currentPrice = random.Next(10, 40);
        }


        //Gets an order from the MultiCellBuffer.
        private Order getOrder()
        {
            return ParkingBooking._buff.getOneCell();
        }

        //Send the pulled order to the OrderProcessing.
        private void ProcessOrder(Order order)
        { 
            // Check to verify that the order is meant for the current parkingStructure.
            // This will lead to duplication, which will lead to thread getting into permanant wait state.
            if (order.ReceiverId == Thread.CurrentThread.Name)
            {
                Console.WriteLine("Currently processing order for the Parking Structure ({0})", Thread.CurrentThread.Name);
                OrderProcessing processor = new OrderProcessing(order, currentPrice);
                Thread processingThread = new Thread(new ThreadStart(processor.ProcessOrder));
                processingThreads.Add(processingThread);
                processingThread.Name = "OrderProcessor " + Thread.CurrentThread.Name;
                processingThread.Start();
            }
            else
            {
                Console.WriteLine("Current Order is not meant for Parking Structure ({0}).", Thread.CurrentThread.Name);
            }
        }
    }
}
