namespace NeuralStyle.Core.Model
{
    public class Job
    {
        public string SourceName { get; set; }
        public string StyleName { get; set; }
        public string TargetName { get; set; }
        public double StyleWeight { get; set; } = 50.0;
        public double ContentWeight { get; set; } = 1.0;
        public int Size { get; set; } = 700;
        public int Iterations { get; set; } = 500;
        public string Model { get; set; } = "imagenet-vgg-verydeep-19.mat";
    }
}