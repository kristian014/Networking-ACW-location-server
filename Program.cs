using System;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Collections.Generic;

namespace locationserver
{
    class Program
    {
        static Dictionary<string, string> TheLocations = new Dictionary<String, String>();
        static void Main(string[] args)
        {
            RunServer();
        }
        static void RunServer()

        {


            TcpListener listener;
            Socket connection;
            NetworkStream socketStream;

            listener = new TcpListener(IPAddress.Any, 43);
            while (true)
            {
                try
                {
                    listener.Start();
                    Console.WriteLine("Server has started Listening for connection");
                    connection = listener.AcceptSocket();

                    socketStream = new NetworkStream(connection);

                    Console.WriteLine("Conection Received");

                    DoRequest(socketStream);

                    connection.Close();
                    socketStream.Close();

                }
                catch
              (Exception e)

                {
                    Console.WriteLine("Check Connection settings" + e.ToString());

                }

            }

        }

        static void DoRequest(NetworkStream socketStream)

        {
            try
            {

                StreamWriter sw = new StreamWriter(socketStream); //this reads
                StreamReader sr = new StreamReader(socketStream);

                socketStream.WriteTimeout = 1000;
                socketStream.ReadTimeout = 1000;

                String line = sr.ReadLine();
                Console.WriteLine("Response Received" + line);
                String[] Words = line.Split(new char[] { ' ' }, 2);
                String username = null, location = null;




                if (Words.Length < 1)
                {
                    Console.WriteLine("Please enter words more than one ");
                }

                else if (Words.Length == 1)
                {
                    // We have a lookup
                    username = Words[0];

                    if (TheLocations.ContainsKey(username))
                    {
                        // the location can be found in the dictionary
                        sw.WriteLine(TheLocations[username]);

                    }
                    else
                    {
                        // Oh no! This user is not known
                        //location = "ERROR: no entries found";
                        sw.WriteLine("ERROR: no entries found");
                        Console.WriteLine("ERROR: no entries found");
                    }
                    //sw.WriteLine("OK");
                    sw.Flush();
                    Console.WriteLine(Words[0]);


                }

                else
                {
                    // We have an update
                    username = Words[0];
                    location = Words[1].Trim();
                    if (TheLocations.ContainsKey(username))
                    {
                        TheLocations[username] = location;
                        Console.WriteLine(location + "\r\n");
                        //sw.WriteLine(TheLocations[username] + " " + location );
                    }
                    else
                    {
                        TheLocations.Add(username, location);
                        //location = "ERROR: no entries found";
                        // username = "Error: no entries found";
                    }
                    //sw.WriteLine(Words[0] + " " + Words[1] );
                    // location = TheLocations[username];
                    sw.WriteLine("OK");
                    sw.Flush();
                    Console.WriteLine(Words[0] + " " + Words[1]);


                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Timeout");
            }



        }
    }
}



