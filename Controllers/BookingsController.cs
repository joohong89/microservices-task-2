using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using microservices_task_2.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using microservices_task_2.Model;
using RestSharp;
using RestSharp.Serialization.Json;

namespace microservices_task_2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly BookingContext _context;
        private readonly string URL_PREFIX = "https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies/";
        private readonly string SLASH = "/";
        private readonly string JSON_FORMAT = ".json";
        private readonly string SGD = "sgd";


        public BookingsController(BookingContext context)
        {
            _context = context;
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBooking()
        {
             List<Booking> booking = await _context.Booking.ToListAsync();
            /*
             List<BookingDTO> bookingDtos = new List<BookingDTO>();
             foreach (var item in booking)
             {
                 BookingDTO newDto = convertBookingToDto(item);

                 bookingDtos.Add(newDto);
             }
            */
             return booking;
        }

       

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(long id)
        {
            var booking = await _context.Booking.FindAsync(id);


            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }

        // PUT: api/Bookings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(long id, BookingDTO dto)
        {
            if (id != dto.id)
            {
                return BadRequest();
            }

            // var booking = await _context.Booking.FindAsync(dto.id);
            var newBooking = convertDtoToBooking(dto);
            newBooking.paymentAmountinSgd = convertCurrencyToSgd(dto.paymentCurrency, dto.paymentAmount);

            _context.Entry(newBooking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Bookings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking(BookingDTO bookingDto)
        {
            Booking booking = convertDtoToBooking(bookingDto);


            // conversion of payment currency in sgd 
            float convertCurrency = convertCurrencyToSgd(bookingDto.paymentCurrency, bookingDto.paymentAmount);
            if (convertCurrency < 0)
            {
                return BadRequest();
            }

            booking.paymentAmountinSgd = convertCurrency;

            _context.Booking.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBooking", new { id = booking.id }, booking);
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(long id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookingExists(long id)
        {
            return _context.Booking.Any(e => e.id == id);
        }

        private BookingDTO convertBookingToDto(Booking item)
        {
            BookingDTO newDto = new BookingDTO();
            newDto.id = item.id;
            newDto.TransactionDateTime = item.TransactionDateTime;
            newDto.venue = item.venue;
            newDto.numberOfTicket = item.numberOfTicket;
            newDto.paymentCurrency = item.paymentCurrency;
            newDto.paymentAmount = item.paymentAmount;

            return newDto;
        }

        private Booking convertDtoToBooking(BookingDTO dto)
        {
            Booking item = new Booking();
            item.id = dto.id;
            item.TransactionDateTime = dto.TransactionDateTime;
            item.venue = dto.venue;
            item.numberOfTicket = dto.numberOfTicket;
            item.paymentCurrency = dto.paymentCurrency;
            item.paymentAmount = dto.paymentAmount;

            return item;
        }

        private float convertCurrencyToSgd(string paymentCurrency, float amount)
        {
            var client = new RestClient();
            var request = new RestRequest(URL_PREFIX + paymentCurrency + SLASH + SGD + JSON_FORMAT);
            var response = client.Execute(request);
            float results = -1;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                ConversionDTO obj = new JsonDeserializer().Deserialize<ConversionDTO>(response);
                results = amount * obj.sgd;
            }

            return results;
        }
    }
}
