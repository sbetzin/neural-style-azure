using System.IO;
using System.Threading.Tasks;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Logger;

namespace NeuralStyle.Core.Instagram
{
    public static class InstagramAdapter
    {
        public static async Task Test(string file)
        {
            var userSession = new UserSessionData
            {
                UserName = "info@artme.ai",
                Password = "artmeaipictures"
            };

            var api = InstaApiBuilder.CreateBuilder().SetUser(userSession).UseLogger(new DebugLogger(LogLevel.Exceptions)).Build();

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

            var caption = @"Another pic of the day!
#web3 #xrp #xrpnft #xrpnfts #sologenic #digitalart # #nftcommunity #nft #nftcollector #nftcollectors #nftcollectibles #nftart #nft #crypto 

Want your own picture transferred into digital painting? DM us or mail to info@artme.ai";

            var up = await api.MediaProcessor.UploadPhotoAsync(img, caption);
            
            Logger.Log($"Picture Post sucessful: {up.Succeeded}");
        }
    }
}