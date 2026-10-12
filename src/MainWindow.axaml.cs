using Avalonia.Controls;
using System;
using System.IO;

namespace SpaceTraffic
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes new MainWindow without any specified scenario
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes new MainWindow with specified scenario
        /// </summary>
        /// <param name="scenario">name of the scenario</param>
        public MainWindow(string scenario) : this()
        {
            //TODO: load scenario, or throw an exception if it does not exist
            if(!scenario)
            {
                throw new Exception("Scenář neexistuje");
                System.exit(0);
            }

            string[] lines = File.ReadAllLines(scenario);
            foreach (string line in lines)
            {
                line = line.Trim();
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    continue
                }
                if (line.StartsWith("#"))
                {
                    continue
                }
                Console.WriteLine(line);
                    
            }
            
        }
    }
}