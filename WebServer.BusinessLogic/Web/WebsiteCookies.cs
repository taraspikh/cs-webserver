using System.Collections.Generic;

namespace WebServer.BusinessLogic.Web
{
    using System;
    using System.Web;
    using System.Collections.ObjectModel;

    public class CookiesCollection : Collection<Cookie>
    {
        public CookiesCollection() : base(new List<Cookie>())
        {
        }

        public Cookie Find(Predicate<Cookie> match)
        {
            var items = (List<Cookie>) this.Items;
            return items.Find(match);
        }
    }

    public class WebsiteCookies
    {
        private CookiesCollection _cookiesList;

        public WebsiteCookies()
        {
            _cookiesList = new CookiesCollection();
        }

        public int Length
        {
            get { return _cookiesList.Count; }
        }

        /// <summary>
        /// Attaches cookies to the next Response. If cookie exists, it will be overwritten.
        /// </summary>
        /// <param name="key">Key of new cookie</param>
        /// <param name="value">Value of new cookie</param>
        public void Add(string key, string value)
        {
            var c = new Cookie(key, value);
            c.Expires = null;
            Add(key, value, null);
        }

        /// <summary>
        /// Attaches cookies to the next Response. If cookie exists, it will be overwritten.
        /// </summary>
        /// <param name="key">Key of new cookie</param>
        /// <param name="value">Value of new cookie</param>
        /// <param name="expires">Expiration date of new cookie</param>
        public void Add(string key, string value, DateTime? expires)
        {
            var c = new Cookie(key, value);
            c.Expires = expires;
            Add(c);
        }

        /// <summary>
        /// Adds cookie
        /// </summary>
        /// <param name="cookie"></param>
        public void Add(Cookie cookie)
        {
            var myCookie = GetCookie(cookie.Key);
            if (myCookie == null)
            {
                _cookiesList.Add(cookie);
            }else
            {
                myCookie.Expires = cookie.Expires;
                myCookie.Key = cookie.Key;
                myCookie.Value = cookie.Value;
                myCookie.Path = cookie.Path;
            }
        }

        public Cookie GetCookie(string key)
        {
            var cookie = _cookiesList.Find(c => c.Key == key);
            return cookie;
        }

        /// <summary>
        /// Attaches cookies removal query in the Request
        /// </summary>
        /// <param name="key">Key of the cookie to remove</param>
        public void RequestDelete(string key)
        {
            var myCookie = GetCookie(key);
            if (myCookie != null)
            {
                myCookie.Expires = DateTime.Now.AddDays(-10);
                myCookie.Value = "deleted";
            }
        }

        /// <summary>
        /// Just removes cookie from collection
        /// </summary>
        /// <param name="key">Key of the cookie to remove</param>
        public void Delete(string key)
        {
            var myCookie = GetCookie(key);
            if (myCookie != null)
            {
                _cookiesList.Remove(myCookie);
            }
        }

        public Collection<Cookie> Items
        {
            get { return _cookiesList; }
        }

    }

    public class Cookie
    {
        public Cookie(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
        public string Path { get; set; }
        public DateTime? Expires { get; set; }
    }
}