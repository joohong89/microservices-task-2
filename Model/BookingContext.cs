using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using microservices_task_2.Model;

namespace microservices_task_2.Model
{
    public class BookingContext: DbContext
    {
        public BookingContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Booking> FindBookings;

        public DbSet<microservices_task_2.Model.Booking> Booking { get; set; }
    }
}
