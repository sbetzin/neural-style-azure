using System.Collections.Generic;

namespace NeuralStyle.ConsoleClient.Model
{
    public class Job
    {
        public string Source { get; set; }
        public string Style { get; set; }
        public string TargetName { get; set; }
        public double StyleWeight { get; set; } = 5.0;
        public double StyleScale { get; set; } = 1.0;
        public int Size { get; set; } = 700;
        public int Iterations { get; set; } = 500;
        public int TileSize { get; set; } = 1500;
        public int TileOverlap { get; set; } = 100;
        public bool UseOriginalColors { get; set; } = true;
    }
}