﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AsyncLearning
{
    internal class Methods
    {
        public async Task<double> GetTaskExecTimeAsync(Func<Task> task, ConsoleColor color = ConsoleColor.Green)
        {
            Stopwatch sw1 = new Stopwatch();

            sw1.Start();
            await task.Invoke();
            sw1.Stop();

            return (double)sw1.ElapsedMilliseconds / 1000;
        }

        public async Task TimeTaskExecAsync(double delay, string Idenifier)
        {
            CWriteLine($"{Idenifier} has started sleeping for {delay.ToString("#.000")} seconds...", ConsoleColor.Yellow);
            
            double execSeconds = await GetTaskExecTimeAsync(async () =>
            {
                await Task.Delay((int)(delay * 1000));
            });

            CWriteLine($"{Idenifier} has finished after {execSeconds} seconds!", ConsoleColor.Green);
        }

        public void CWriteLine(string msg, ConsoleColor color)
        {
            ConsoleColor prevForeColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = prevForeColor;
        }

        public void CWrite(string msg, ConsoleColor color)
        {
            ConsoleColor prevForeColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(msg);
            Console.ForegroundColor = prevForeColor;
        }

        public void Pause(string pauseMsg, ConsoleColor color = ConsoleColor.Gray)
        {
            CWriteLine(pauseMsg, color);
            Console.ReadKey(true);
        }

        public IEnumerable<string> IterString()
        {
            string[] array1 = new string[] { "a", "b", "c", "d", "e", "f", "g", "h" };
            foreach (string v in array1)
                yield return v;
        }
    }
}
