namespace NeuralStyle.Core.Model
{
    public class JobSettings
    {
        public double StyleWeight { get; set; } = 50.0;
        public double TvWeight { get; set; } = 0.001;
        public double TemporalWeight { get; set; } = 200;
        public double ContentWeight { get; set; } = 1.0;
        public int ContentLossFunction { get; set; } = 1;
        public int Size { get; set; } = 700;
        public int Iterations { get; set; } = 500;
        public string Model { get; set; } = "imagenet-vgg-verydeep-19.mat";
    }
}