using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsageDataAnonymiser.Models;

namespace UsageDataAnonymiser
{
    class Program
    {
        static void Main(string[] args)
        {
            //read file into collection of UsageEvents
            Console.WriteLine("Reading source file");
            var sourceFilePath = Environment.CurrentDirectory + "\\usage.csv";
            var sourceFile = new CsvReader(File.OpenText(sourceFilePath));
            var usageEvents = sourceFile.GetRecords<UsageEvent>().ToList();

            //get distinct list of user IDs
            var userIds = usageEvents.Select(o => o.UserId).Distinct();

            // Create a copy of the usage event
            Console.WriteLine("Anonymising user ids");
            var anonymisedUsageEvents = usageEvents.ToList();
            var spinner = new ConsoleSpiner();
            var spinning = true;
            while (spinning)
            {
                // Create a test UserID dictonary 
                var userIdDictionary = new Dictionary<string, string>();

                //Loop through each event
                foreach (var usageEvent in anonymisedUsageEvents)
                {
                    // Chack if I have seen this ID Before.
                    string value;
                    userIdDictionary.TryGetValue(usageEvent.UserId, out value);
                    if (value == null)
                    {
                        // Haven't seen it so add to the dictionary and set the userId to the new value
                        var newValue = Guid.NewGuid().ToString();
                        userIdDictionary.Add(usageEvent.UserId, newValue);
                        usageEvent.UserId = newValue;
                    }
                    else
                    {
                        // I have this in the dictionary change the value to the anon version
                        usageEvent.UserId = value;
                    }

                    spinner.Turn();
                }
                spinning = false;
            }

            //write destination file
            Console.WriteLine("Writing new file");
            var targetFilePath = Environment.CurrentDirectory + "\\usage-anonymised.csv";
            using (TextWriter writer = new StreamWriter(targetFilePath))
            {
                var csv = new CsvWriter(writer);
                csv.Configuration.Encoding = Encoding.UTF8;
                csv.WriteRecords(anonymisedUsageEvents);
            }

            Console.WriteLine("DONE");
            Console.WriteLine($"Source file had {usageEvents.Count()} rows and target file has {anonymisedUsageEvents.Count()} rows");
            Console.WriteLine($"{userIds.Count()} user IDs were anonymised");
            Console.ReadKey();
        }
    }

    class ConsoleSpiner
    {
        int counter;
        public ConsoleSpiner()
        {
            counter = 0;
        }
        public void Turn()
        {
            counter++;
            switch (counter % 4)
            {
                case 0: Console.Write("/"); break;
                case 1: Console.Write("-"); break;
                case 2: Console.Write("\\"); break;
                case 3: Console.Write("|"); break;
            }
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        }
    }
}
