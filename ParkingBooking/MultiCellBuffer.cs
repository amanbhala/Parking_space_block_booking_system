using ParkingBookingSystem;

namespace ParkingBookingSystem
{
    public class MultiCellBuffer
    {
        // Size of the Multi-Cell Buffer
        private const int bufferSize = 3;

        // Pointers to keep track of buffer position
        int startIndex = 0;
        int endIndex = 0;
        int elementCount = 0;

        // Buffer for thread communication
        Order[] buffer = new Order[bufferSize];

        // Semaphores to control read/write access
        Semaphore writesemaphore = new Semaphore(bufferSize, bufferSize);
        Semaphore readsemaphore = new Semaphore(bufferSize, bufferSize);

        //Write an order to a buffer cell.
        public void setOneCell(Order order)
        {
            writesemaphore.WaitOne();
            lock (this)
            {
                // Buffer full, wait till the thread gets some space to write.
                while (elementCount == bufferSize)
                {
                    Console.WriteLine("Buffer is full right now. {0} is waiting to be able to write to a cell", Thread.CurrentThread.Name);
                    Monitor.Wait(this);
                }

                // This is done to make sure that all cells in the buffer get processing
                buffer[endIndex] = order;
                endIndex = (endIndex + 1) % bufferSize; //circle back to the array start.

                elementCount++; // Increment the number of elements
                Console.WriteLine("({0}) Following order has been sent to Buffer {4} - {1} {2}, Elements: {3}",
                    Thread.CurrentThread.Name,
                    order,
                    DateTime.Now,
                    elementCount,
                    endIndex
                );
                writesemaphore.Release();
                Monitor.Pulse(this);
            }
        }

        //Reading an order from the buffer.
        public Order getOneCell()
        {
            readsemaphore.WaitOne();
            lock (this)
            {
                Order element;

                // Buffer empty. Wait till some item is available to be read.
                while (elementCount == 0)
                {
                    Console.WriteLine("Buffer is currently empty. {0} waiting to be able to read from a cell", Thread.CurrentThread.Name);
                    Monitor.Wait(this);
                }

                element = buffer[startIndex];


                // Make sure the ReceiverId matches the parkingStructure
                if (element.ReceiverId == null || Thread.CurrentThread.Name==element.ReceiverId)
                {
                    // Order is for parkingStructure, Extract order to process
                    startIndex = (startIndex + 1) % bufferSize;
                    elementCount--;
                    Console.WriteLine("({0}) reading an order from Buffer {1} at time: {2}, Element number: {3}",
                        Thread.CurrentThread.Name,
                        element,
                        DateTime.Now,
                        elementCount
                    );
                }
                else
                {
                    // ReceiverId does not match the parkingStructure so do not extract order
                    Console.WriteLine("Order is not meant for Parking Structure ({0})", Thread.CurrentThread.Name);
                }

                readsemaphore.Release();
                Monitor.Pulse(this);
                return element;
            }
        }
    }
}
