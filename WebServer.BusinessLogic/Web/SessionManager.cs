namespace WebServer.BusinessLogic.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.ObjectModel;

    public class SessionManager
    {
        private static SessionManager _instance;

        /// <summary>
        /// Desired name of the cookie
        /// </summary>
        private const string SESSION_COOKIE_NAME = "sessionKey";

        /// <summary>
        /// Count of connected to the server's session users
        /// </summary>
        public int ConnectedUsersCount { get; set; }

        /// <summary>
        /// Session singleton instance
        /// </summary>
        public static SessionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SessionManager();
                    _instance.SessionList = new Collection<Session>();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Date of session creation
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// List of all sessions of webserver
        /// </summary>
        public Collection<Session> SessionList { get; private set; }

        /// <summary>
        /// Cookieless session stores session data in URL
        /// </summary>
        public bool IsCookieless
        {
            get { return Configurator.Instance.IsCookielessSession; }
        }

        /// <summary>
        /// Creates session and returns Session
        /// </summary>
        /// <returns>Session object with session key = SessionKey</returns>
        public Session CreateSession(Request request, Response response)
        {
            //try getting Session from cookie
            Session s = GetSessionFromCookie(request);

            if (s != null)
            {
                s.LastAccessed = DateTime.Now;
            }
            else
            {
                //session was not found in cookie, create new one
                s = new Session(request);
            }

            //check if session was created before
            if (GetSessionByKey(s.SessionKey, request) == null)
            {
                //add new session
                SessionList.Add(s);

                if (!IsCookieless)
                {
                    //add cookie with session in it
                    var c = new Cookie(SESSION_COOKIE_NAME, s.SessionKey);
                    
                    //set session for whole website
                    c.Path = "/";
                    c.Expires = DateTime.Now.AddDays(10);

                    //add session request to response
                    if (response != null)
                    {
                        response.Cookies.Add(c);
                    }
                }

                //return SessionKey
                return s;
            }
            

            return null;
        }

        private Session GetSessionFromCookie(Request request)
        {
            if (!IsCookieless)
            {
                if(request.Cookies == null)
                {
                    return null;
                }
                Cookie value = request.Cookies.GetCookie(SESSION_COOKIE_NAME);
                if (value != null)
                {
                    //got session value. check if server remembers it
                    var session = GetSessionByKey(value.Value, request);
                    if (session != null)
                    {
                        return session;
                    }
                }
            }

            return null;
        }

        public Session GetSessionByKey(string sessionKey, Request request)
        {
            //create validation for current user
            var currentUserValidation = Session.CreateUniqueValidation(request);

            // only compare current sessions by SessionKey and validation
            var item = SessionList.Where(i => i.SessionKey == sessionKey && i.UniqueValidation == currentUserValidation).SingleOrDefault();
            return item;
        }

        public bool DeleteSession(string sessionKey, Request request, Response response)
        {
            var session = GetSessionByKey(sessionKey, request);
            if (session != null)
            {
                //remove session from list
                SessionList.Remove(session);

                if (!IsCookieless)
                {
                    //remove cookie also
                    response.Cookies.RequestDelete(SESSION_COOKIE_NAME);
                }

                //return success
                return true;
            }
            return false;
        }

        public Session GetCurrentSession(Request request)
        {
            if (IsCookieless && request.SessionKey != null)
            {
                return GetSessionByKey(request.SessionKey, request);
            }
            return GetSessionFromCookie(request);
        }

        

        public void UpdateSession(Request request, Session s)
        {
            var session = GetSessionByKey(s.SessionKey, request);
            session.LastAccessed = DateTime.Now;
            session.IsAdmin = s.IsAdmin;
        }
    }
}