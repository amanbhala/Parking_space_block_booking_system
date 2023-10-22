namespace ParkingBookingSystem
{
    public class ParkingBooking
    {
        // Number of Parking Structure threads.
        private const int K = 2;

        // Number of Parking Agency threads.
        private const int N = 10;

        private static Thread[] parkingStrThreads = new Thread[K];
        private static Thread[] agencyThreads = new Thread[N];
        private static ParkingStructure[] parkingStructures = new ParkingStructure[K];

        public static MultiCellBuffer _buff = new MultiCellBuffer();

        static void Main(string[] args)
        {
            // Create `K` parking structure threads.
            for (int i = 0; i < K; ++i)
            {
                ParkingStructure parkingStructure = new ParkingStructure();
                parkingStructures[i] = parkingStructure;
                parkingStrThreads[i] = new Thread(parkingStructure.ParkingStructureFunc);
                parkingStrThreads[i].Name = "ParkingStructure" + i;
                parkingStrThreads[i].Start();

                //wait till the current thread comes to life.
                while (!parkingStrThreads[i].IsAlive) ;
            }

            // Create `N` parking agent threads.
            for (int i = 0; i < N; ++i)
            {
                ParkingAgent parkingAgency = new ParkingAgent();

                // Loop through the Parking Structures and Subscribe to the Price Cut event
                for (int j = 0; j < K; ++j)
                {
                    parkingAgency.following(parkingStructures[j]);
                }
                OrderProcessing.orderProcessed += parkingAgency.OrderProcessedConfirmation;
                agencyThreads[i] = new Thread(parkingAgency.ParkingAgentFunc);
                agencyThreads[i].Name = "ParkingAgency" + i;
                agencyThreads[i].Start();

                //wait till the current thread comes to life.
                while (!agencyThreads[i].IsAlive) ;
            }

            // Wait for the parking structure threads to perform price cuts
            for (int i = 0; i < K; ++i)
            {
                while (parkingStrThreads[i].IsAlive) ;
            }

            //As a safe check, set parking agency active flag to
            //false for each parking agency when shutting down.
            for (int i = 0; i < N; ++i)
            {
                ParkingAgent.ParkingsActive = false;
            }

            // Wait for the Parking Agency threads to shut down.
            for (int i = 0; i < N; ++i)
            {
                while (agencyThreads[i].IsAlive) ;
            }

            // Quit
            Console.WriteLine("\n\nMain thread complete, press any button to quit");
            Console.ReadLine();
        }
    }
}
