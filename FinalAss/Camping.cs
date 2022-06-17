using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAss
{
    internal class Camping
    {
        public List<Reservation> reservations;
        public Camping()
        {
            reservations = new List<Reservation>();
        }
        public double getEarnings()
        {
            double totalEarnings = 0;
            foreach (Reservation addedReservation in reservations)
            {
                totalEarnings += addedReservation.getPrice();
            }
            return totalEarnings;

        }
        public double getAverageDaysRented()
        {
            int i = 0;
            int totalLengthOfReservations = 0;
            foreach (Reservation addedReservation in reservations)
            {
                totalLengthOfReservations += addedReservation.getRentLengthDays();
                i++;
            }
            if (totalLengthOfReservations > 0)
            {
                return totalLengthOfReservations / i;
            }
            else
            {
                return 0;
            }
        }
        public int getTotalAmountOfPeople()
        {
            int totalAmountOfPeople = 0;
            foreach(Reservation addedReservation in reservations)
            {
                totalAmountOfPeople += addedReservation.getNumberOfPeople();
            }
            return totalAmountOfPeople;
        }

    }
}
