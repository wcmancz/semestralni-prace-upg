using Avalonia.Controls;
using Avalonia.Metadata;
using DefaultNamespace;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SpaceTraffic
{
    public partial class MainWindow : Window
    {

        private readonly Dictionary<string, SpaceBodies> bodies = new();
        private readonly List<Destination> dests = new();
        private readonly List<Ship> ships = new();

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
            string path = Path.Combine(AppContext.BaseDirectory, "data", scenario);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Scénář '{scenario}' nebylo nalezeno");
            }

            LoadScenario(path);
            View.SetData(bodies.Values, ships, dests);
            View.Start();
        }

        private void LoadScenario(string path)
        {
            var ci = CultureInfo.InvariantCulture;
            var parents = new Dictionary<string, string>();
            var destRows = new List<string[]>();
            var shipRows = new List<string[]>();
            string? section = null;

            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                string fixedLine = line.Trim();

                if (fixedLine.Length == 0 || fixedLine.StartsWith("#"))
                {
                    continue;
                }

                if (fixedLine.StartsWith("[") && fixedLine.EndsWith("]"))
                {
                    section = fixedLine.Substring(1, fixedLine.Length - 2);
                    continue;
                }

                string[] item = fixedLine.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

                switch (section)
                {
                    case "BODIES":
                        var body = new SpaceBodies(
                            item[0],
                            item[1],
                            double.Parse(item[2], ci),
                            double.Parse(item[4], ci),
                            double.Parse(item[5], ci),
                            double.Parse(item[6], ci),
                            int.Parse(item[7], ci),
                            int.Parse(item[8], ci),
                            int.Parse(item[9], ci));
                        bodies[body.GetName()] = body;
                        parents[body.GetName()] = item[3];
                        break;
                    case "DESTINATIONS":
                        destRows.Add(item);
                        break;
                    case "SHIPS":
                        shipRows.Add(item);
                        break;
              
                }
            }
            foreach (var (name, parentName) in parents)
                if (parentName != "-")
                    bodies[name].SetParent(bodies[parentName]);

            foreach (var item in destRows)
                dests.Add(new Destination(bodies[item[0]], bodies[item[1]], int.Parse(item[2], ci)));

            foreach (var item in shipRows)
                ships.Add(new Ship(item[0], double.Parse(item[1], ci), bodies[item[2]],
                    int.Parse(item[3], ci), int.Parse(item[4], ci), Ship.ParseStrategy(item[5]), int.Parse(item[6], ci)));
        }

    }
}