using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clockwork.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Clockwork.API.Controllers
{
    [Route("[controller]")]
    public class ClockController : Controller
    {
        private readonly int _batchSize = 10;
        private readonly ClockworkContext _clockworkContext;

        public ClockController(ClockworkContext context)
        {
            _clockworkContext = context;
        }

        [HttpGet("history")]
        public IActionResult History()
        {
            return Get(0);
        }

        // GET <controller>/{index}
        [HttpGet("history/{index}")]
        public IActionResult Get(int index)
        {
            var returnVal = new
            {
                pastTicks = _clockworkContext.CurrentTimeQueries.Where(obj => obj.CurrentTimeQueryId <= index + _batchSize && obj.CurrentTimeQueryId >= index).ToList(),   // Hardcode to have paging of 10 items...
                total = _clockworkContext.CurrentTimeQueries.Count()
            };

            return Json(returnVal);
        }

        // GET api/currenttime
        [HttpGet("time")]
        public IActionResult Time()
        {
            return Get(string.Empty);
        }

        // GET api/currenttime/timezone
        [HttpGet("time/{timezone}")]
        public IActionResult Get(string timezone)
        {
            var utcTime = DateTime.UtcNow;
            var serverTime = DateTime.Now;
            var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();

            int offset = OffsetFromTimezone(timezone);
            if(offset == 0)
            {
                timezone = string.Empty;
            }
            else
            {
                serverTime = DateTime.UtcNow.AddHours(offset);
            }

            var returnVal = new CurrentTimeQuery
            {
                UTCTime = utcTime,
                ClientIp = ip,
                Time = serverTime,
                RequestedTimezone = timezone
            };

            _clockworkContext.CurrentTimeQueries.Add(returnVal);
            if(_clockworkContext.SaveChanges() > 0)
            {
                return Ok(returnVal);
            }
            else
            {
                return Json(new { status = "error", message = "Error saving to the DB!" });
            }
        }

        private int OffsetFromTimezone(string timezone)
        {
            //
            //  Ideally these values would be stored in a DB somewhere...
            //  https://www.timetemperature.com/abbreviations/united_states_time_zone_abbreviations.shtml
            //
            if (timezone.Equals("AST", StringComparison.InvariantCultureIgnoreCase) ||
                timezone.Equals("EDT", StringComparison.InvariantCultureIgnoreCase))
            {
                return -4;
            }
            else if (timezone.Equals("EST", StringComparison.InvariantCultureIgnoreCase) ||
                     timezone.Equals("CDT", StringComparison.InvariantCultureIgnoreCase))
            {
                return -5;
            }
            else if (timezone.Equals("CST", StringComparison.InvariantCultureIgnoreCase) ||
                     timezone.Equals("MDT", StringComparison.InvariantCultureIgnoreCase))
            {
                return -6;
            }
            else if (timezone.Equals("MST", StringComparison.InvariantCultureIgnoreCase) ||
                     timezone.Equals("PDT", StringComparison.InvariantCultureIgnoreCase))
            {
                return -7;
            }
            else if (timezone.Equals("PST", StringComparison.InvariantCultureIgnoreCase) ||
                     timezone.Equals("AKDT", StringComparison.InvariantCultureIgnoreCase))
            {
                return -8;
            }
            else if (timezone.Equals("HADT", StringComparison.InvariantCultureIgnoreCase) ||
                     timezone.Equals("AKST", StringComparison.InvariantCultureIgnoreCase))
            {
                return -9;
            }
            else if (timezone.Equals("HST", StringComparison.InvariantCultureIgnoreCase)  ||
                     timezone.Equals("HAST", StringComparison.InvariantCultureIgnoreCase) ||
                     timezone.Equals("SDT", StringComparison.InvariantCultureIgnoreCase))
            {
                return -10;
            }
            else if (timezone.Equals("SST", StringComparison.InvariantCultureIgnoreCase))
            {
                return -11;
            }
            else if (timezone.Equals("CHST", StringComparison.InvariantCultureIgnoreCase))
            {
                return 10;
            }
            return 0;
        }
    }
}
