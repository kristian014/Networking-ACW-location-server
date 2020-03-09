using System;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Threading;

namespace locationserver
{
    class Program
    {
        static Dictionary<string, string> TheLocations = new Dictionary<String, String>();
        static int PortNumber = 43;
        static String Username = null;
        static String location = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
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
                    Thread t = new Thread(() => RequestHandler.DoRequest(connection));
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
                 public void DoRequest(Socket connection)

            {
                string Host = ((IPEndPoint)connection.RemoteEndPoint).Address.ToString();

                NetworkStream socketStream;
                socketStream = new NetworkStream(connection);
                Console.WriteLine("Conection Received");
                //String line = null;
                try
                {
                  
                    StreamWriter sw = new StreamWriter(socketStream); //this reads
                    StreamReader sr = new StreamReader(socketStream);

                    socketStream.WriteTimeout = 1000;
                    socketStream.ReadTimeout = 1000;


                    string line = sr.ReadLine();

                    Console.WriteLine("Response Received: " + line);




                    if (line.StartsWith("GET /?name=") && (line.EndsWith(" HTTP/1.1")))

                    {

                        Console.WriteLine("Im in the 1.1 get hello");
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
                            sw.Flush();
                            return;


                        }

                        else
                        {

                            sw.WriteLine("HTTP/1.1 404 Not Found\r\n");
                            sw.WriteLine("Content-Type: text/plain\r\n\r\n");
                            sw.WriteLine();
                            sw.Flush();
                            Console.WriteLine("HTTP/1.1 404 Not Found");
                            Console.WriteLine("Content - Type: text/plain");


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
                        sw.Flush();
                        Console.WriteLine("HTTP/1.1 200 OK\r\n");
                        Console.WriteLine("Content-Type: text/plain\r\n");

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
                            sw.Flush();
                            Console.WriteLine("HTTP/1.0 200 OK");
                            Console.WriteLine("Content-Type: text/plain");
                        }

                        else
                        {
                            sw.WriteLine("HTTP/1.0 404 Not Found\r\n");
                            sw.WriteLine("Content-Type: text/plain\r\n");
                            sw.WriteLine();
                            sw.Flush();

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
                        sw.Flush();
                        Console.WriteLine("HTTP/1.0 200 OK\r\n");
                        Console.WriteLine("Content-Type: text/plain\r\n");

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
                            sw.WriteLine("Content-Type: text/plain\r\n");


                            Console.WriteLine("HTTP/0.9 404 Not Found\r\n");
                            Console.WriteLine("Content-Type: text/plain\r\n");
                            Console.WriteLine();
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

                        sw.Flush();
                        Console.WriteLine("HTTP/0.9 200 OK\r\n");
                        Console.WriteLine("Content-Type: text/plain\r\n");


                    }

                    else
                    {
                        string[] Words = line.Split(new char[] { ' ' }, 2);



                        if (Words.Length < 1)
                        {
                            Console.WriteLine("Please enter words more than one ");
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
                            sw.Flush();
                            Console.WriteLine(Words[0] + " " + Words[1]);


                        }

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                    finally
                {
                    socketStream.Close();
                    connection.Close();
                   // log.WriteToLog(Host, line, status);
                }
            }
        }
    }


}
/*

public class Logging 
{
    public static String logfile = null;

    // https://stackoverflow.com/questions/2954900/simple-multithread-safe-log-class
    public Logging(String filename) 
    {
        logfile = filename;
    }

    private static readonly object locker = new object();



    public void WriteToLog(String hostname, String message, String Status) 
    {

        String line = hostname + " - - " + DateTime.Now.ToString("'['dd'/'MM'/'yyyy':'HH':'ss zz00']'") + "\"" + message + 
            "\" " + Status;
        lock(locker) 
        {
            Console.WriteLine(line);
            if (logfile == null) return;
                    try
            {
                StreamWriter sw;
                sw = File.AppendText("logfile");
                sw.WriteLine(line);
                sw.Close();
            }
            catch
            {
                Console.WriteLine("Cannot write file" + logfile);
            }
           
        } 
    } 
} 
*/