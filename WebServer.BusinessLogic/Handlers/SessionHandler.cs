// -----------------------------------------------------------------------
// <copyright file="SessionHandler.cs" company="">
// Перевіряє чи правильний логін і пароль....не докінця коректно паше але наразі нормально.Поправлю щоб точне логування було
// після логінення кидає на сторінку з помилкою або на сторінку де пише шо ти залогінився
// </copyright>
// -----------------------------------------------------------------------

namespace WebServer.BusinessLogic.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using WebServer.BusinessLogic.Web;

    /// <summary>
    /// Handles Session cookieless requests.
    /// For example, path of the file in GET method ca be: /{12345}/
    /// </summary>
    public class SessionHandler : Handler
    {
        public override Response Handle(Request request)
        {
            //create empty Session to fill it according to IsCookieless value
            Session session = null;

            //check for {12345} in path
            var path = System.Web.HttpUtility.UrlDecode(request.HttpPath);
            if (path.StartsWith("/[") && SessionManager.Instance.IsCookieless)
            {
                //remove first 2 chars
                var sessionValue = path.Substring(2);
                
                //find ending
                int idxEnd = sessionValue.IndexOf("]");
                if (idxEnd > 0)
                {
                    //save new path without session
                    string newPath = sessionValue.Substring(idxEnd + 1);

                    //clean request's httpPath for next handlers
                    request.HttpPath = newPath;
                    
                    //get real sessionValue
                    sessionValue = sessionValue.Remove(idxEnd);
                    
                    //see if session with this value is valid
                    session = SessionManager.Instance.GetSessionByKey(sessionValue, request);
                }
            }

            //check for cookie
            if (!SessionManager.Instance.IsCookieless)
            {
                session = SessionManager.Instance.GetCurrentSession(request);
            }

            //got session
            if (session != null)
            {
                request.SessionKey = session.SessionKey;
            }

            //pass it to the next handlers
            return this.NextHandler.Handle(request);
        }

    }
}
