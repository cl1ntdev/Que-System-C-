using System;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Speech.Synthesis;

namespace QueSystem
{
    internal class Program
    {
        static string conn = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
        static int allQues = 0;
        static int currentServing = 0;
        static String servingSay = "";

        //activity 2

        static void Koisk()
        {
            do
            {
                Console.Clear();
                var insertQue = "";
                var counterQue = "select count(*) from _ques";
                int tickNumb = 0;
                using (var db = new MySqlConnection(conn))
                {
                    db.Open();
                    using (var countCmd = new MySqlCommand(counterQue, db))
                    {
                        int allQues = Convert.ToInt32(countCmd.ExecuteScalar());

                        if (allQues <= 0)
                        {
                            insertQue = "INSERT INTO _ques (status) VALUES ('serving')";
                        }
                        else
                        {
                            insertQue = "INSERT INTO _ques (status) VALUES ('not serving')";
                        }
                        tickNumb = allQues + 1;
                    }

                    using (var insCmd = new MySqlCommand(insertQue, db))
                    {
                        insCmd.ExecuteNonQuery();
                        
                    }

                    Console.WriteLine("Please wait in line, your ticket is: " + tickNumb);

                }
                Console.ReadLine();
            } while (true);
        }
        //activity 1
        static void Display()
        {
            do
            {
                Console.Clear();
                SpeechSynthesizer audio = new SpeechSynthesizer();
                using (var mydb = new MySqlConnection(conn))
                {
                    mydb.Open();
                    var query = "select * from _ques";

                    using (var readerCli = new MySqlCommand(query, mydb))
                    {
                        var reader = readerCli.ExecuteReader();
                        while (reader.Read())
                        {
                            if ((reader["status"]).ToString() == "serving")
                            {
                                Console.WriteLine("Now Serving Number: " + reader["id"] + " || Estimated time: 3 minutes");
                                servingSay = "Now Serving Number: " + reader["id"] + " || Estimated time: 3 minutes";
                                audio.Volume = 75;
                                audio.Rate = 2;
                                audio.Speak(servingSay);

                                currentServing = int.Parse(reader["id"].ToString());
                            }
                        }
                    }
                }
                Console.ReadLine();
            } while (true);
        }
        //activity 3

        static void Teller()
        {
            while(true)
            {
                Console.Clear();


                var queNotServe = "update _ques set status='not serving' where id=@currentServing";
                var queServe = "update _ques set status='serving' where id=@newServe";
                using (var db = new MySqlConnection(conn))
                {
                    db.Open();
                    var query = "select * from _ques";
                    var _noQuery = "select count(*) from _ques";


                    using (var readerCli = new MySqlCommand(query, db))
                    {

                        using (var reader = readerCli.ExecuteReader())
                        {
                            allQues = 0;
                            currentServing = 0;
                            while (reader.Read())
                            {
                                allQues++;

                                if ((reader["status"]).ToString() == "serving")
                                {
                                    currentServing = int.Parse(reader["id"].ToString());
                                }
                            }
                        }
                    }

                    var newServe = currentServing + 1;
                    
                    if (newServe >= allQues)
                    {
                        newServe--;
                        Console.WriteLine("No more Ques");
                        return;

                    }
                    else
                    {
                       
                        Console.WriteLine("We will now serve number: " + newServe);

                            using (var updateNoServe = new MySqlCommand(queNotServe, db))
                            {
                                updateNoServe.Parameters.AddWithValue("@currentServing", currentServing);
                                updateNoServe.ExecuteNonQuery();
                            }
                            using (var update = new MySqlCommand(queServe, db))
                            {
                                update.Parameters.AddWithValue("@newServe", newServe);
                                update.ExecuteNonQuery();
                            }
                        
                        var recalled = Console.ReadLine();
                        if (recalled.ToLower() == "recall")
                        {

                            SpeechSynthesizer synth = new SpeechSynthesizer();
                            synth.Volume = 75;
                            synth.Rate = 2;
                            synth.Speak("We will now serve number: " + newServe);


                        }

                    }
                }
               
            }
        }
        static void ExecuteChoice(int ch)
        {
            switch (ch)
            {
                case 1:

                    Console.WriteLine("Display");
                    Display();

                    break;
                case 2:
                    Console.WriteLine("Kiosk");
                    Koisk();
                    Console.ReadLine();
                    break;
                case 3:
                    Console.WriteLine("Teller");
                   
                     Teller();

                    Console.ReadLine();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please select 1, 2, or 3.");
                    break;
            }
        }

        static void Main(string[] args)
        {
            allQues = 0;
            
            if (args.Length > 0)
            {
                
                if (int.TryParse(args[0], out int ch))
                {
                    ExecuteChoice(ch);
                }
                else
                {
                    Console.WriteLine("Invalid argument. Please provide a number: 1 (Display), 2 (Kiosk), or 3 (Teller).");
                }
            }
            else
            {
                Console.WriteLine("1. Display || 2. Kiosk || 3. Teller");
                Console.Write("Choose an option: ");
                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    ExecuteChoice(choice);
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
        }
    }
}
