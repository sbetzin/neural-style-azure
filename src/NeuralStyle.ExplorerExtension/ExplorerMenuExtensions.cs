using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NeuralStyle.Core;
using NeuralStyle.Core.Features.NeuralStyleTransfer;
using NeuralStyle.Core.Folders;
using SharpShell.Attributes;
using SharpShell.Diagnostics;
using SharpShell.SharpContextMenu;

namespace NeuralStyle.ExplorerExtension
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.Directory, ".jpg")]
    public class ExplorerMenuExtensions : SharpContextMenu
    {
        protected override bool CanShowMenu()
        {
            if (SelectedItemPaths.Count() > 1)
            {
                return false;
            }

            var path = SelectedItemPaths.First();

            var isInFolder = FolderCheck.IsInFolder(path, @"C:\Users\gensb\OneDrive\_nft\video");

            return isInFolder;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();

            menu.CreateMenuItem("Stylize", OnStylizeImage);

            return menu;
        }

        private void OnStylizeImage(object sender, EventArgs args)
        {
            Logger.NewLog += Logging.Log;
            Logger.Log($"Found {SelectedItemPaths.Count()} images to stylize");

            var inImages = SelectedItemPaths.ToArray();
            var allStyles = BasePathes.GetAllStyles();
            var settings = CreateSettings.GetDefaultSettings();


            CreateJobs.CreateNew(inImages, allStyles, settings, BasePathes.BasePath(), BasePathes.OutPath());

        }
    }
}