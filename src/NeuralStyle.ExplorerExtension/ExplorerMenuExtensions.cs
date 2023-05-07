using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NeuralStyle.Core;
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

            var itemCountLines = new ToolStripMenuItem
            {
                Text = "Create Masked Images"
            };

            itemCountLines.Click += OnEnlargeImage;

            menu.Items.Add(itemCountLines);

            return menu;
        }

        private void OnEnlargeImage(object sender, EventArgs args)
        {
            Logger.NewLog += Logging.Log;

            Logger.Log($"Found {SelectedItemPaths.Count()} images to enlarge");
            foreach (var path in SelectedItemPaths)
            {
                //CreateEnlargeJob.CreateLargeImageJob(path);
            }
        }
    }
}