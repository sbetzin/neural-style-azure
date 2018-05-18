using System.Collections.Generic;

namespace NeuralStyle.ConsoleClient.Model
{
    public class Job
    {
        public string Source { get; set; }
        public string Style { get; set; }
        public int Size { get; set; }
        public List<int> Iterations { get; set; }
    }
}