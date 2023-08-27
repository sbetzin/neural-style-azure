using System.Collections.Generic;

namespace NeuralStyle.Console.MovePathes
{
    public static class CreatePathes
    {
        public static void AddZoomMovement(this Dictionary<string, object> settings)
        {
            settings["result_name"] = settings["result_name"].ToString().Replace("#movename#", "zoom");

            settings["coordinates"] = new[]
            {
                new[] { 0.0, 0.0, 0.0 },
                new[] { 0.0, 0.0, -0.1 },
                new[] { 0.01, 0, -0.09 },
                new[] { 0, 0.01, -0.08 },
                new[] { -0.01, 0, -0.07 },
                new[] { 0, -0.01, -0.06 },
                new[] { 0.01, 0, -0.05 },
                new[] { 0, 0.01, -0.04 },
                new[] { -0.01, 0, -0.03 },
                new[] { 0.0, 0.0, 0.0 }
            };
        }

        public static void AddSlideMovement(this Dictionary<string, object> settings)
        {
            settings["result_name"] = settings["result_name"].ToString().Replace("#movename#", "slide_move");
            settings["coordinates"] = new[]

            {
                new[] { 0.0, 0.0, 0.0 },
                new[] { -0.02, -0.02, 0 },
                new[] { 0, -0.01, 0.01 },
                new[] { 0.01, 0, 0.015 },
                new[] { 0.02, 0.01, 0.02 },
                new[] { 0.0, 0.0, 0.0 }
            };
        }
    }
}