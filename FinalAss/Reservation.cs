using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAss
{
    internal class Reservation
    {
        public int numberOfPeople;
        public DateTime startDate;
        public DateTime endDate;
        public bool carPresent;
        public Site site;
        

        public Reservation(int cNumberOfPeople, DateTime cStartDate, DateTime cEndDate, bool cCarPresent, Site cSite)
        {
            numberOfPeople = cNumberOfPeople;
            startDate = cStartDate;
            endDate = cEndDate;
            carPresent = cCarPresent;
            site = cSite;
        }
        public int getRentLengthDays()
        {
            return (endDate - startDate).Days;
        }
        public int getNumberOfPeople()
        {
            return numberOfPeople;
        }
        public double getPrice()
        {
            int extraCarPayment = 0;
            if (carPresent)
            {
                extraCarPayment = 3;
            }
            double payment = (15 + 2.5 * numberOfPeople + extraCarPayment) * (endDate - startDate).Days;
            
            if (DateTime.IsLeapYear(startDate.Year))
            {
                if(startDate.DayOfYear >= 193 && startDate.DayOfYear <= 228)
                {
                    return payment * 1.25;
                }
                else
                {
                    return payment;
                }
            }
            else
            {
                if(startDate.DayOfYear >= 192 && startDate.DayOfYear <= 227)
                {
                    return payment * 1.25;
                }
                else
                {
                    return payment;
                }
            }
        }
    }
}
