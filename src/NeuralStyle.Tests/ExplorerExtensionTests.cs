using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralStyle.Core;
using NeuralStyle.Core.Folders;
using NeuralStyle.ExplorerExtension;
using Xunit;

namespace NeuralStyle.Tests
{

    public class ExplorerExtensionTests
    {
        [Fact]
        public static void TestLargeImage()
        {
            var menu = new ExplorerMenu();

            var result = FolderCheck.IsInFolder(@"C:\Users\gensb\OneDrive\_nft\in\lofoten-8.jpg", BasePathes.InPath());

            result = FolderCheck.IsInFolder(@"C:\Users\gensb\OneDrive\_nft\out\kroatien_05-eugene_ivanov_gentleman_with_newspaper-1000px_cw_30000_sw_100000_tv_10_model_vgg19_opt_lbfgs_origcolor_0.jpg", BasePathes.InPath());
        }

    }
}
