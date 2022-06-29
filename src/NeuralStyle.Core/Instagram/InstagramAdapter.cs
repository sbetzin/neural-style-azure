using System.IO;
using System.Threading.Tasks;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Logger;

namespace NeuralStyle.Core.Instagram
{
    public static class InstagramAdapter
    {
        public static async Task NewPost(string file, string text)
        {
            var userSession = new UserSessionData
            {
                UserName = "info@artme.ai",
                Password = "artmeaipictures",
            };

            var device = new AndroidDevice();
            

            var api = InstaApiBuilder.CreateBuilder().SetUser(userSession).UseLogger(new DebugLogger(LogLevel.Exceptions)).Build();
            api.SetDevice(device);

            var loginResult = await api.LoginAsync();
            Logger.Log($"Login sucessful: {loginResult.Succeeded}");


            var bytes = File.ReadAllBytes(file);
            var img = new InstaImageUpload
            {
                ImageBytes = bytes,
                Uri = file
            };

            //img.UserTags.Add(new InstaUserTagUpload
            //{
            //    Username = "rmt4006",
            //    X = 0.5,
            //    Y = 0.5
            //});

            

            var up = await api.MediaProcessor.UploadPhotoAsync(img, text);
            
            Logger.Log($"Picture Post sucessful: {up.Succeeded}");
        }
    }
}