using System;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Text;
using System.Drawing.Drawing2D;


namespace locationserver
{

    /*  private void WriteDictToFile(Dictionary<string, string> someDict, string path)
      {
          using (StreamWriter fileWriter = new StreamWriter("")
          {
          // You can modify <string, string> notation by placing your types.
          foreach (KeyValuePair<string, string> kvPair in someDict)
                  {
                      fileWriter.WriteLine("{0}: {1}", kvPair.Key, kvPair.Value);
                  }
          fileWriter.Close();
      }
  }

          var dictionary = new Dictionary<string, string>();
                  dictionary["perls"] = "dot";
                  dictionary["net"] = "perls";
                  dictionary["dot"] = "net";
                  Write(dictionary, "C:\\dictionary.bin");
              }
              else if (value == "2")
              {
                  var dictionary = Read("C:\\dictionary.bin");
                  foreach (var pair in dictionary)


          dictionary.txt

  */

    class Program
    {
        //public class dictionread : Program
        //{
        //    private void WriteDictToFile(Dictionary<string, string> TheLocations, string path)
        //    {
        //        using (StreamWriter fileWriter = new StreamWriter(""))
        //        {
        //            //G:\NetwrkingAswSourceControl\locationserver\locationserver\Readdictionary.txt
        //            // You can modify <string, string> notation by placing your types.
        //            foreach (KeyValuePair<string, string> kvPair in TheLocations)
        //            {
        //                fileWriter.WriteLine("{0}: {1}", kvPair.Value, kvPair.Value);
        //            }
        //            fileWriter.Close();
        //        }
        //    }



            static Dictionary<string, string> TheLocations = new Dictionary<String, String>();


            static int PortNumber = 43;
            static String Username = null;
            static String location = null;
            static String Status = "OK";
                //

            /// <summary>
            /// 
            /// </summary>
            /// <param name="args"></param>
            ///

            public static Logging Log;
            static void Main(string[] args)
            {
                String filename = null;
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-l":
                            filename = args[++i];
                            break;

                    case "-W":
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new locationserverwindowsform());

                        break;
                    default:
                            Console.WriteLine("Unknow option" + args[i]);
                            break;
                    }
                }
                Log = new Logging(filename);

            
                RunServer();
            }

            static void RunServer()

            {
                TcpListener listener;
                Socket connection;
                Handler RequestHandler;



                listener = new TcpListener(IPAddress.Any, PortNumber);
                while (true)
                {
                    try
                    {
                        listener.Start();
                        Console.WriteLine("Server has started Listening for connection");
                        connection = listener.AcceptSocket();
                        RequestHandler = new Handler();
                        Thread t = new Thread(() => RequestHandler.DoRequest(connection, Log));
                        t.Start();


                    }
                    catch
                  (Exception e)

                    {
                        Console.WriteLine("Check Connection settings" + e.ToString());

                    }

                }

            }
            /// <summary>
            /// 
            /// </summary>
            class Handler
            {
                // public void DoRequest(Socket connection , Logging log )
                public void DoRequest(Socket connection, Logging Log)

                {
                    string Host = ((IPEndPoint)connection.RemoteEndPoint).Address.ToString();

                    NetworkStream socketStream;
                    socketStream = new NetworkStream(connection);
                    Console.WriteLine("Conection Received");

                    Status = "OK";
                    String line = null;
                    try
                    {

                        StreamWriter sw = new StreamWriter(socketStream); //this reads
                        StreamReader sr = new StreamReader(socketStream);

                        socketStream.WriteTimeout = 1000;
                        socketStream.ReadTimeout = 1000;


                        line = sr.ReadLine();

                        Console.WriteLine("Response Received: " + line);




                        if (line.StartsWith("GET /?name=") && (line.EndsWith(" HTTP/1.1")))

                        {


                            string[] HTTPLINES = line.Split(new char[] { ' ' });

                            Console.WriteLine("HTTP 1.0 DONE SUCCESSFULLY");
                            string GetuserNameLine;
                            GetuserNameLine = HTTPLINES[1].TrimStart('/', '?', '=');
                            Username = GetuserNameLine.Remove(0, 5);


                            if (TheLocations.ContainsKey(Username))
                            {
                                // the location can be found in the dictionary
                                sw.WriteLine("HTTP/1.1 200 OK");
                                sw.WriteLine("Content-Type: text/plain\r\n");
                                sw.WriteLine(TheLocations[Username]);
                                Status = "OK";
                                sw.Flush();
                                return;


                            }

                            else
                            {

                                sw.WriteLine("HTTP/1.1 404 Not Found\r\n");
                                sw.WriteLine("Content-Type: text/plain\r\n\r\n");
                                sw.WriteLine();
                                Status = "UNKNOWN";
                                sw.Flush();


                                return;

                            }


                        }

                        else if (line.Equals("POST / HTTP/1.1") && (sr.Peek() >= 0))
                        {


                            string ReadUsername_And_LocationLine = sr.ReadLine(); // HOST:
                            ReadUsername_And_LocationLine = sr.ReadLine(); // Content-Length:
                            int content_length = int.Parse(ReadUsername_And_LocationLine.Split(' ')[1]);
                            sr.ReadLine();

                            ReadUsername_And_LocationLine = "";
                            try
                            {
                                for (int i = 0; i < content_length; i++)
                                {
                                    ReadUsername_And_LocationLine += (char)sr.Read();
                                }
                            }
                            catch
                            {
                                // ignore the fault
                            }


                            string[] GetLocationAndUsername = ReadUsername_And_LocationLine.Split(new char[] { '&', '=' });
                            Username = GetLocationAndUsername[1];
                            location = GetLocationAndUsername[3];

                            TheLocations[Username] = location;
                            sw.WriteLine("HTTP/1.1 200 OK\r\n");
                            sw.WriteLine("Content-Type: text/plain\r\n\r\n");
                            sw.WriteLine();
                            Status = "OK";




                            sw.Flush();

                            return;
                        }


                        else if ((line.StartsWith("GET /?")) && (line.EndsWith(" HTTP/1.0")))

                        {
                            string[] HTTPLINES = line.Split(new char[] { ' ' });

                            Console.WriteLine("HTTP 1.0 DONE SUCCESSFULLY");
                            Username = HTTPLINES[1].TrimStart('/', '?');

                            if (TheLocations.ContainsKey(Username))
                            {
                                sw.WriteLine("HTTP/1.0 200 OK" + "\r\n");
                                sw.WriteLine("Content-Type: text/plain" + "\r\n\r\n");
                                sw.WriteLine();
                                sw.WriteLine(TheLocations[Username]);
                                Status = "OK";
                                sw.Flush();
                                return;
                            }

                            else
                            {
                                sw.WriteLine("HTTP/1.0 404 Not Found\r\n");
                                sw.WriteLine("Content-Type: text/plain\r\n");
                                sw.WriteLine();
                                Status = "UNKNOWN";
                                sw.Flush();
                                return;
                            }

                        }
                        else if ((line.StartsWith("POST")) && (line.EndsWith(" HTTP/1.0")) && (sr.Peek() >= 0))
                        {
                            string[] HTTPLINES = line.Split(new char[] { ' ' });

                            Username = HTTPLINES[1].TrimStart('/');
                            string ContentLenght;
                            ContentLenght = sr.ReadLine();
                            string OptionalHeader;
                            OptionalHeader = sr.ReadLine();
                            location = sr.ReadLine();

                            TheLocations[Username] = location;

                            sw.WriteLine("HTTP/1.0 200 OK\r\n");
                            sw.WriteLine("Content-Type: text/plain\r\n");
                            sw.WriteLine();
                            Status = "OK";
                            sw.Flush();


                        }

                        else if (line.StartsWith("GET /"))
                        {
                            string[] HTTPLINES = line.Split(new char[] { ' ' });
                            Username = HTTPLINES[1].TrimStart('/');
                            Console.WriteLine("HTTP0.9 DONE SUCCESSFULLY");
                            if (TheLocations.ContainsKey(Username))
                            {

                                sw.WriteLine("HTTP/0.9 200 OK" + "\r\n");
                                sw.WriteLine("Content-Type: text/plain" + "\r\n\r\n");
                                sw.WriteLine(TheLocations[Username]);
                                // the location can be found in the dictionary
                                sw.Flush();
                                return;
                            }

                            else
                            {

                                sw.WriteLine("HTTP/0.9 404 Not Found\r\n");
                                sw.WriteLine("Content-Type: text/plain\r\n\r\n");
                                Status = "UNKNOWN";
                                sw.WriteLine();
                                sw.Flush();
                                return;


                            }


                        }

                        else if (line.StartsWith("PUT /") && (sr.Peek() >= 0))
                        {
                            string[] HTTPLINES = line.Split(new char[] { ' ' });
                            Username = HTTPLINES[1].Trim('/');
                            string SecondLine;
                            SecondLine = sr.ReadLine();

                            string Read_ThirdLine;
                            Read_ThirdLine = sr.ReadLine();
                            location = Read_ThirdLine.Trim();


                            TheLocations[Username] = location;
                            sw.WriteLine("HTTP/0.9 200 OK");
                            sw.WriteLine("Content-Type: text/plain\r\n");
                            Status = "OK";
                            sw.Flush();



                        }

                        else
                        {
                            string[] Words = line.Split(new char[] { ' ' }, 2);



                            if (Words.Length < 1)
                            {
                                Console.WriteLine("Please enter words more than one ");
                                Status = "ENTER MORE ARGS";
                            }


                            else if (Words.Length == 1)
                            {


                                Username = Words[0];

                                if (TheLocations.ContainsKey(Username))
                                {

                                    sw.WriteLine(TheLocations[Username]);

                                }

                                else
                                {

                                    sw.WriteLine("ERROR: no entries found");

                                }
                                //sw.WriteLine("OK");
                                sw.Flush();
                                Console.WriteLine(Words[0]);
                                return;

                            }

                            else if (Words.Length == 2)
                            {
                                // We have an update
                                Username = Words[0];
                                location = Words[1].Trim();
                                if (TheLocations.ContainsKey(Username))
                                {
                                    TheLocations[Username] = location;

                                }
                                else
                                {
                                    TheLocations.Add(Username, location);

                                }

                                sw.WriteLine("OK\r\n");
                                Status = "OK";
                                sw.Flush();
                                Console.WriteLine(Words[0] + " " + Words[1]);


                            }

                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Status = "EXCEPTION";
                    }
                    finally
                    {
                        socketStream.Close();
                        connection.Close();
                        Log.WriteToLog(Host, line, Status);
                    }
                }
            }
        }


    }
    /// <summary>
    /// Logging example from https://stackoverflow.com/questions/2954900/simple-multithread-safe-log-class
    /// </summary>

    public class Logging
    {
        public static String Logfile = null;
        public Logging(String filename)
        {
            Logfile = filename;
        }

        private static readonly object locker = new object();
        public void WriteToLog(String hostname, String message, String Status)
        {
            String line = hostname + " - - " + DateTime.Now.ToString("'['dd'/'yyyy':'HH':'mm':'ss zz00']'") + "\"" + message + "\" " + Status;
            lock (locker)
            {
                Console.WriteLine(message);
                if (Logfile == null)
                    return;
                try
                {

                    StreamWriter sw;
                    sw = File.AppendText(Logfile);
                    sw.WriteLine(line);
                    sw.Close();

                }
                catch
                {
                    Console.WriteLine("Unable to write logfile" + Logfile);
                }

            }

        }
    }
