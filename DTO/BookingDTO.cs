using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace microservices_task_2.Model
{
    public class BookingDTO
    {
        public long id { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public string venue { get; set; }
        public int numberOfTicket { get; set; }
        public string paymentCurrency { get; set; }
        public float paymentAmount { get; set; }
    }
}
