using System;
using Portfolio.Api.Models;

namespace Portfolio.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("beginning program...");
            Console.WriteLine(Trade.foo());

            bool running = true;

            Portfolio.Api.Models.Portfolio dummyPortfolio = new Portfolio.Api.Models.Portfolio("123");
            dummyPortfolio.PopulateDummyPortfolio();
            dummyPortfolio.Accounts[0].Load();

            while (running)
            {
                Console.WriteLine("\nEnter a command (p = print portfolio, e = exit): ");
                var input = Console.ReadKey(intercept: true).KeyChar;

                if (input == 'e' || input == 'E')
                {
                    running = false; // exit loop
                }
                else if (input == 'p' || input == 'P')
                {
                    Console.WriteLine("foo!");
                }
                else
                {
                    Console.WriteLine($"\nUnrecognized input: {input}");
                }
            }

            Console.WriteLine("Exiting ConsoleApp...");
        }
    }
}