using System.Diagnostics;

namespace WebServer.BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Net.Sockets;
    using System.Threading;
    using WebServer.BusinessLogic.Handlers;
    using WebServer.BusinessLogic.Logs;
    

    public class Communicator: INotifyPropertyChanged
    {
        /// <summary>
        /// Create static instance, Singleton, so it could be created only once
        /// </summary>
        private static Communicator _instance;

        /// <summary>
        /// Bool for starting listening
        /// </summary>
        public bool IsActive { get; set; }
        
        public static Communicator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Communicator();
                }
                return _instance;
            }
        }

        private int _activeConnections;

        public int ActiveConnections
        {
            get
            {
                return _activeConnections;
            }
            private set
            {
                _activeConnections = value;
                this.OnPropertyChanged("ActiveConnections");
            }
        }

        /// <summary>
        /// Starts listening.
        /// </summary>
        public void StartListening()
        {
            // Set thread max threads.
            int maxThreads = Environment.ProcessorCount * 4;
            ThreadPool.SetMaxThreads(maxThreads, maxThreads);

            //set the active state
            IsActive = true;

            //run thread of webserver
            Thread thread = new Thread(StartListeningThread);
            //ThreadPool.QueueUserWorkItem();
            thread.Start();
        }

        /// <summary>
        /// Stops listening in the while loop
        /// </summary>
        public void StopListening()
        {
            IsActive = false;

            if (_tcpListener != null)
            {
                _tcpListener.Server.Close();
            }
        }

        public bool TryToFreePortForWebserver()
        {
            _tcpListener = new TcpListener(Configurator.Instance.Host, Configurator.Instance.Port);
            try
            {
                _tcpListener.Start();
                return true;
            }
            catch (SocketException)
            {
                Logger.Instance.Log(string.Format("Error. Server could not restart itself on address: {0}:{1}", Configurator.Instance.Host, Configurator.Instance.Port));
            }
            finally
            {
                _tcpListener.Server.Close();

            }
            return false;
        }

        private TcpListener _tcpListener;
        

        /// <summary>
        /// Starts listening thread
        /// </summary>
        private void StartListeningThread()
        {
            for (int i = Configurator.Instance.Port; i < 65000; i++)
            {
                if (TryToFreePortForWebserver())
                {
                    //port free'd
                    break;
                }

                //try to change port
                Configurator.Instance.Port++;
            }
            

            _tcpListener = new TcpListener(Configurator.Instance.Host, Configurator.Instance.Port);
            _tcpListener.Start();


            while (IsActive)
            {
                try
                {
                    Socket tcpSocket = _tcpListener.AcceptSocket();
                    if (tcpSocket.Connected)
                    {
                        ActiveConnections++;
                        ThreadPool.QueueUserWorkItem(this.ReturnResponseThreadProc, tcpSocket);
                    }
                }
                catch (SocketException e)
                {
                    Logger.Instance.Log("Error while AcceptSocket: "+e.Message);
                }
            }
            

        }


        private void ReturnResponseThreadProc(Object tcpSocket)
        {
            this.ReturnResponse((Socket)tcpSocket);
        }

        /// <summary>
        /// Handles request and sends response to browser.
        /// </summary>
        /// <param name="tcpSocket"></param>
        private void ReturnResponse(Socket tcpSocket)
        {
            try
            {
                Logger.Instance.Log(string.Format("Connected {0}:{1}", (tcpSocket.RemoteEndPoint as IPEndPoint).Address, (tcpSocket.RemoteEndPoint as IPEndPoint).Port));

                // keep connection alive and process all the requests from client
                while (IsActive)
                {
                    
                    // create container for received data
                    var temp = new byte[tcpSocket.ReceiveBufferSize];
                    
                    // get data from client and return count of read bytes
                    int res = tcpSocket.Receive(temp);
                    if (res == 0)
                    {
                        // no more data from client, he has disconnected from server
                        break;
                    }

                    // create request
                    Request req = new Request();
                    req.ParseRequest(temp);

                    // get IP
                    IPEndPoint ipEndPoint = tcpSocket.RemoteEndPoint as IPEndPoint;
                    if (ipEndPoint != null)
                    {
                        req.UserIpAddress = ipEndPoint.Address.ToString();
                    }

                    // log request
                    Logger.Instance.Log(String.Format("{0} {1} from {2}; UserAgent: {3}", req.HttpMethod, req.HttpPath, req.UserIpAddress, req.UserAgent));

                    Response response = null;

                    //handle only if correct format of request
                    if (req.IsCorrect)
                    {
                        //get Response
                        response = this.HandleRequest(req);
                    }
                    else
                    {
                        // respond with error message
                        response = new Response();
                        response.MimeType = "text/html";
                        response.Data =
                            Encoding.UTF8.GetBytes("Request was not in correct format, please fix your browser!");
                    }

                    //TODO: check if Response.IsCorrect?

                    //form Response
                    var responseBytes = this.GetResponseBytes(response);

                    //send Response to the web browser
                    this.SendToBrowser(responseBytes, ref tcpSocket);
                }

            }catch(SocketException)
            {
                Logger.Instance.Log("SocketException happened in Communicator");
            }
            finally
            {
                ActiveConnections--;
                //Interlocked.Decrement(ref _activeConnections);


                var ipEndPoint = tcpSocket.RemoteEndPoint as IPEndPoint;
                if (ipEndPoint != null)
                {
                    Logger.Instance.Log(string.Format("Disconnecting {0}:{1}", ipEndPoint.Address, ipEndPoint.Port));
                }

                //try closing socket
                TryClosingSocket(tcpSocket);
            }

            
        }

        private void TryClosingSocket(Socket tcpSocket)
        {
            try
            {
                if (tcpSocket.Connected)
                {
                    tcpSocket.Shutdown(SocketShutdown.Both);
                    tcpSocket.Close();
                }
            }
            catch (SocketException)
            {
                Trace.WriteLine("forseclosed");
            }
        }

        /// <summary>
        /// Sends data to browser.
        /// </summary>
        /// <param name="responseBytes"></param>
        /// <param name="tcpSocket"></param>
        private void SendToBrowser(byte[] responseBytes, ref Socket tcpSocket)
        {
            //send data back to opened socket
            tcpSocket.Send(responseBytes);
        }

        /// <summary>
        /// Converts response to bytes.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private byte[] GetResponseBytes(Response response)
        {
            var responseBuilder = new ResponseBuilder(response);
            var responseBytes = responseBuilder.GetResponseBytes();
            return responseBytes;
        }

        /// <summary>
        /// Sends formed request object to the handlers
        /// </summary>
        /// <param name="request">
        /// </param>
        /// <returns>
        /// Responce
        /// </returns>
        private Response HandleRequest(Request request)
        {
            // create Handlers
            var handler1 = new SessionHandler();
            var handler2 = new DirectoryHandler();
            var handler3 = new AdminHandler();
            var handler4 = new TextHandler();
            var handler5 = new BinaryHandler();
            var handler6 = new ErrorHandler();

            // set Handler sequence of execution
            handler1.SetNext(handler2);
            handler2.SetNext(handler3);
            handler3.SetNext(handler4);
            handler4.SetNext(handler5);
            handler5.SetNext(handler6);

            // handle request
            return handler1.Handle(request);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
