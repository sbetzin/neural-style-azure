namespace NeuralStyle.Core.Model
{
    public class JobSettings
    {
        public double StyleWeight { get; set; } = 50.0;
        public double TvWeight { get; set; } = 0.001;
        public double ContentWeight { get; set; } = 1.0;
        public int Size { get; set; } = 700;
        public int Iterations { get; set; } = 500;
        public string Model { get; set; } = "vgg19";
        public string Optimizer { get; set; } = "lbfgs";
        public string Init { get; set; } = "content";
        public string TargetName { get; set; } = string.Empty;
    }
}