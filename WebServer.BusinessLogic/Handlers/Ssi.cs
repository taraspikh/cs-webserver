// -----------------------------------------------------------------------
// <copyright file="SSI.cs" company="SoftServe">
// SSI Parser
//Working with time and some others features
//Now not conected to project
// </copyright>
// -----------------------------------------------------------------------

namespace WebServer.BusinessLogic.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Ssi
    {
        private const string TimefmtShortDay = "<!--#config sizefmt=\"%a\"-->";
        private const string TimefmtFullDay = "<!--#config sizefmt=\"%A\"-->";
        private const string TimefmtShortMonth = "<!--#config sizefmt=\"%b\"-->";
        private const string TimefmtFullMonth = "<!--#config sizefmt=\"%B\"-->";
        private const string TimefmtDayOfMonth1 = "<!--#config sizefmt=\"%d\"-->";
        private const string TimefmtFormatDayMonthYear = "<!--#config sizefmt=\"%D\"-->";
        private const string TimefmtDayOfMonth2 = "<!--#config sizefmt=\"%e\"-->";
        private const string TimefmtHoursIn24Format = "<!--#config sizefmt=\"%H\"-->";
        private const string TimefmtHoursIn12Format = "<!--#config sizefmt=\"%I\"-->";
        private const string TimefmtDayOfYear = "<!--#config sizefmt=\"%j\"-->";
        private const string TimefmtNumberOfMonth = "<!--#config sizefmt=\"%m\"-->";
        private const string TimefmtMinutes = "<!--#config sizefmt=\"%M\"-->";
        private const string TimefmtAmPm = "<!--#config sizefmt=\"%p\"-->";
        private const string TimefmtFormatHourMinSecAp = "<!--#config sizefmt=\"%r\"-->";
        private const string TimefmtSeconds = "<!--#config sizefmt=\"%S\"-->";
        private const string TimefmtTimeInSeconds = "<!--#config sizefmt=\"%s\"-->";
        private const string TimefmtFormatHourMinSec = "<!--#config sizefmt=\"%T\"-->";
        private const string TimefmtWeekOfYear = "<!--#config sizefmt=\"%U\"-->";
        private const string TimefmtNumberDayOfYear = "<!--#config sizefmt=\"%w\"-->";
        private const string TimefmtShortYear = "<!--#config sizefmt=\"%y\"-->";
        private const string TimefmtFullYear = "<!--#config sizefmt=\"%Y\"-->";
        private const string TimefmtTimeZone = "<!--#config sizefmt=\"%a\"-->";

        private string line;

        public Ssi()
        {
            line = null;
        }
        public string ParseSsi(string text,Request req)
        {
            this.line = text;
            if (line.Contains("<!--#config sizefmt="))
            {
                DateTime now = DateTime.Now;
                line = line.Replace(TimefmtShortDay, now.ToString("ddd"));

                line = line.Replace(TimefmtFullDay, now.Day.ToString("dddd"));

                line = line.Replace(TimefmtShortMonth, now.ToString("MMM"));

                line = line.Replace(TimefmtFullMonth, now.ToString("MMMM"));

                line = line.Replace(TimefmtDayOfMonth1, now.ToString("dd"));

                line = line.Replace(TimefmtFormatDayMonthYear, now.ToString("HH/mm/ss"));

                line = line.Replace(TimefmtDayOfMonth2, now.ToString("dd"));

                line = line.Replace(TimefmtHoursIn24Format, now.ToString("HH"));

                line = line.Replace(TimefmtHoursIn12Format, now.ToString("hh"));

                line = line.Replace(TimefmtDayOfYear, now.DayOfYear.ToString());

                line = line.Replace(TimefmtNumberOfMonth, now.ToString("MM"));

                line = line.Replace(TimefmtMinutes, now.ToString("mm"));

                line = line.Replace(TimefmtAmPm, now.ToString("t"));

                line = line.Replace(TimefmtFormatHourMinSecAp, now.ToString("T"));

                line = line.Replace(TimefmtSeconds, now.ToString("ss"));

                //TODO make it correct

                line = line.Replace(TimefmtTimeInSeconds, now.ToString("ss"));

                line = line.Replace(TimefmtFormatHourMinSec, now.ToString("HH:mm:ss"));

                int week = Convert.ToInt32((Math.Abs(Convert.ToDouble(now.DayOfYear.ToString())/7.0)));
                line = line.Replace(TimefmtWeekOfYear, week.ToString());

                line = line.Replace(TimefmtNumberDayOfYear, now.DayOfYear.ToString());

                line = line.Replace(TimefmtShortYear, now.ToString("yy"));

                line = line.Replace(TimefmtFullYear, now.ToString("yyyy"));
                //TODO make it workable
                //line = line.Replace(Timefmt_ShortYear, now.ToString("tt", CultureInfo.CreateSpecificCulture("en-US")));
                //Console.WriteLine(now.ToString("K"))
            }
            //Some SSI Comands for AdminPage
            line = Regex.Replace(line, @"<!--#port -->", Configurator.Instance.Port.ToString());
            line = Regex.Replace(line, @"<!--#maxUsers -->", Configurator.Instance.MaxUsers.ToString());

            //now working with printenv
            if (line.Contains("<!--#printenv"))
            {
                line = Regex.Replace(line, "<!--#printenv DOCUMENT_ROOT -->",
                                     Configurator.Instance.RelativeWwwPath.ToString());
                line = Regex.Replace(line, "<!--#printenv USER_AGENT --> -->",req.UserAgent);
                line = Regex.Replace(line, "<!--#printenv HTTP_USER_AGENT -->",req.UserAgent);
                line = Regex.Replace(line, "<!--#printenv SERVER_ADDR -->",Configurator.Instance.Host.ToString());
                line = Regex.Replace(line, "<!--#printenv DOCUMENT_URI -->",req.HttpPath);
                line = Regex.Replace(line, "<!--#printenv REQUEST_URI -->",req.HttpPath);
                line = Regex.Replace(line, "<!--#printenv SERVER_SOFTWARE -->","071Server");
                line = Regex.Replace(line, "<!--#printenv SERVER_ADMIN -->","071cs@itacademy.lviv.ua");
                line = Regex.Replace(line, "<!--#printenv DATE_LOCAL -->",DateTime.Now.ToLocalTime().ToString());
                line = Regex.Replace(line, "<!--#printenv HTTP_COOKIE -->", req.CookiesRaw);
                
            }


            return line;
        }
    
    }
}
