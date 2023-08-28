using NeuralStyle.Core.Model;

namespace NeuralStyle.Core.Features.NeuralStyleTransfer
{
    public static class CreateSettings
    {
        public static JobSettings GetDefaultSettings()
        {
            var settings = new JobSettings
            {
                Size = 1000,
                StyleWeight = 1e5,
                ContentWeight = 3e4,
                TvWeight = 1e1,
                Model = "vgg19",
                Optimizer = "lbfgs",
                Iterations = 500,
                Init = "content"
            };

            return settings;
        }
    }
}