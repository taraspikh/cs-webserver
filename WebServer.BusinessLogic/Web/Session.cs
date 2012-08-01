namespace WebServer.BusinessLogic.Web
{
    using System;
    using System.Web;

    public class Session
    {
        /// <summary>
        /// Date when the session was created
        /// </summary>
        public DateTime DateCreated { get; private set; }

        /// <summary>
        /// Unique key of the session
        /// </summary>
        public string SessionKey { get; private set; }

        /// <summary>
        /// Indicates that logged into the session user has Administrator privileges
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Last time of Session access
        /// </summary>
        public DateTime LastAccessed { get; set; }

        /// <summary>
        /// Unique client validation. Consists of UserAgent + IP
        /// </summary>
        public string UniqueValidation { get; private set; }

        /// <summary>
        /// Creates session key based on Guid
        /// </summary>
        /// <returns></returns>
        private string CreateSessionKey()
        {
            return Guid.NewGuid().ToString();
        }

        public Session(Request request)
        {
            this.SessionKey = CreateSessionKey();
            this.DateCreated = DateTime.Now;
            this.LastAccessed = DateCreated;
            this.UniqueValidation = CreateUniqueValidation(request);
            IsAdmin = false;
        }

        /// <summary>
        /// Saves UserAgent+IP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string CreateUniqueValidation(Request request)
        {
            //validate request before using it
            if (request == null)
            {
                return String.Empty;
            }
            string sValidation = string.Format("{0}+{1}", request.UserAgent, request.UserIpAddress);
            return sValidation;
        }
    }
}