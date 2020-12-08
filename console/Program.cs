using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Math;


namespace TelepromterConsole
{

    internal class TelePromterConfig 
    {
        public int DelayInMilliseconds {get; private set;} = 200;
        public void UpdateDelay(int increment)
        {
            var newDelay = Min(DelayInMilliseconds + increment, 1000);
            newDelay = Max(newDelay, 20);
            DelayInMilliseconds = newDelay;
        }

        public bool Done { get; private set;}

        public void SetDone()
        {
            Done = true;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            
            RunTelepromter().Wait();
            
        }

        private static async Task RunTelepromter()
        {
            var config = new TelePromterConfig();
            var displayTask = ShowTeleprompter(config);
            var speedTask = GetInput(config);
            await Task.WhenAny(displayTask, speedTask);
        }

        private static async Task ShowTeleprompter(TelePromterConfig config) 
        {
            var words = ReadFrom("sampleQuotes.txt");
            foreach (var word in words)
            {
                Console.Write(word);
                if (!string.IsNullOrWhiteSpace(word))
                {
                    await Task.Delay(200);
                }
            }
            config.SetDone();
        }

        private static async Task GetInput(TelePromterConfig config)
        {
            var delay = 200;
            Action work = () => 
            {
                do {
                    var key = Console.ReadKey(true);
                    if (key.KeyChar == '>')
                        config.UpdateDelay(-10);
                    else if (key.KeyChar == '<')
                        config.UpdateDelay(+10);
                    else if (key.KeyChar == 'x' || key.KeyChar == 'X')
                        config.SetDone();
                } while (!config.Done);
            };
            await Task.Run(work);
        }

        static IEnumerable<string> ReadFrom(string file)
        {
            string line;
            using (var reader = File.OpenText(file))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    var words = line.Split(' ');
                    foreach (var word in words)
                    {
                        yield return word + " ";
                    }
                    yield return Environment.NewLine;
                }
            }
        }
    }
}
