// -----------------------------------------------------------------------
// <copyright file="PostHandler.cs" company="">
// TODO: Update copyright text.
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
    using WebServer.BusinessLogic.Logs;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AdminHandler:Handler
    {
        public override Response Handle(Request request)
        {

            if (request.HttpMethod==EnumHttpMethod.POST)
            {
                var a = new StringBuilder("login=");
                a.Append(Configurator.Instance.AdminLogin.ToString());
                a.Append("&");
                string tempLogin=a.ToString();
                //string temp_login = String.Join("login=", Configurator.Instance.AdminLogin.ToString(),"&");
                var b=new StringBuilder("password=");
                b.Append(Configurator.Instance.AdminPassword.ToString());
                b.Append("\0");
                string tempPassword = b.ToString();
                
                
                if (request.RawData.Contains("login=") && request.RawData.Contains("password=") && request.HttpPath.StartsWith("/admin/index.html"))
                {
                    if (request.RawData.Contains(tempLogin) && request.RawData.Contains(tempPassword))
                    {
                        string filePath = Configurator.Instance.RelativeWwwPath + "\\admin\\settings.html";
                        string text = File.ReadAllText(filePath);
                        Byte[] data = this.TextToByteArray(text, Encoding.ASCII);
                        string MIMEType = "text/html";
                        string version = request.HttpVersion;

                        // remember session
                        var resp = new Response(MIMEType, version, data);
                        var s = SessionManager.Instance.CreateSession(request, resp);
                        s.IsAdmin = true;
                        SessionManager.Instance.UpdateSession(request, s);

                        // log successful attempt
                        Logger.Instance.Log(string.Format("-> Admin logged in with IP {0}. Session key is {1}",
                                                          request.UserIpAddress, s.SessionKey));

                        // move user to sessioned page if IsCookieless
                        if (SessionManager.Instance.IsCookieless)
                        {
                            //create temporary redirect, according to HTTP/1.0 specification
                            resp.HttpResponseCode = 302;
                            resp.HttpResponseCodeDescription = "Found";

                            resp.Location = string.Format("/[{0}]/{1}", s.SessionKey,
                                                          request.HttpPath.TrimStart(new[] {'/'}));
                        }

                        return resp;
                    }
                    else
                    {
                        string filePath = Configurator.Instance.RelativeWwwPath + "\\admin\\error.html";
                        string text = File.ReadAllText(filePath);
                        Byte[] data = this.TextToByteArray(text, Encoding.ASCII);
                        string MIMEType = "text/html";
                        string version = request.HttpVersion;
                        return new Response(MIMEType, version, data);
                    }
                }
                else if (request.HttpPath.StartsWith("/admin/index.html")&&(SessionManager.Instance.GetCurrentSession(request).IsAdmin==true))
                {
                        string portLine = request.RawData.Substring(request.RawData.IndexOf("port="));
                        portLine = portLine.Substring(5, portLine.IndexOf("&")-5);
                        
                        int port = 8080;
                        try
                        {
                            port = Convert.ToInt32(portLine);
                        }
                        catch (Exception)
                        {
                            port = Configurator.Instance.Port;

                        }
                        finally
                        {
                            Configurator.Instance.Port = port;
                        }         
                  
                        string usersLine = request.RawData.Substring(request.RawData.IndexOf("maxusers="));
                        usersLine = usersLine.Substring(5, usersLine.IndexOf("&")-5);
                        
                        int users = 8080;
                        try
                        {
                            users = Convert.ToInt32(usersLine);
                        }
                        catch (Exception)
                        {
                            users = Configurator.Instance.MaxUsers;

                        }
                        finally
                        {
                            Configurator.Instance.MaxUsers = users;
                        }
                    
                    if (request.RawData.Contains("mode=rad1"))
                    {
                        Configurator.Instance.ServConfig = ServerConfiguration.Normal;
                    }
                    else
                        if (request.RawData.Contains("mode=rad2"))
                        {
                            Configurator.Instance.ServConfig = ServerConfiguration.Redirect;
                        }
                        else
                            if (request.RawData.Contains("mode=rad3"))
                            {
                                Configurator.Instance.ServConfig = ServerConfiguration.LoadBalancer;
                            }

                    string filePath = Configurator.Instance.RelativeWwwPath + "\\admin\\Loggined.html";
                    string text = File.ReadAllText(filePath);
                    Byte[] data = this.TextToByteArray(text, Encoding.ASCII);
                    string MIMEType = "text/html";
                    string version = request.HttpVersion;
                    return new Response(MIMEType, version, data);
                }
                else
                {
                    string filePath = Configurator.Instance.RelativeWwwPath + "\\admin\\error.html";
                    string text = File.ReadAllText(filePath);
                    Byte[] data = this.TextToByteArray(text, Encoding.ASCII);
                    string MIMEType = "text/html";
                    string version = request.HttpVersion;
                    return new Response(MIMEType, version, data);
                }
            }

            if (request.HttpMethod == EnumHttpMethod.GET)
            {
                if (request.HttpPath.StartsWith("/admin/Logout.htm"))
                {
                    var session = SessionManager.Instance.GetCurrentSession(request);

                    // log logging out process
                    Logger.Instance.Log(string.Format("<- Admin logged out with IP {0}", request.UserIpAddress));

                    var resp = new Response();
                    resp.Data = ASCIIEncoding.ASCII.GetBytes("You have been logged out");
                    SessionManager.Instance.DeleteSession(session.SessionKey, request, resp);
                    return resp;
                }
            }
            
            return this.NextHandler.Handle(request);
            
            
        }

    }
}
