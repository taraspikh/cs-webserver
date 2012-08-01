using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.BusinessLogic.Helpers
{
    public interface IClock
    {
        DateTime Now { get; }
    }

    public class SystemClock : IClock
    {
        public DateTime Now { get { return DateTime.Now; } }
    }

    public class FakeClock : IClock
    {
        private readonly DateTime _fakeDateTime;
        public FakeClock(DateTime fakeDateTime)
        {
            _fakeDateTime = fakeDateTime;
        }
        public virtual DateTime Now
        {
            get
            {
                return _fakeDateTime;
            }
        }
    }
}
