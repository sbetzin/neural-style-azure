using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NeuralStyle.Core;
using SharpShell.Attributes;
using SharpShell.Diagnostics;
using SharpShell.SharpContextMenu;

namespace NeuralStyle.ExplorerExtension
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".jpg")]
    public class ExplorerMenuExtensions : SharpContextMenu
    {
        protected override bool CanShowMenu()
        {
            return true;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();

            var itemCountLines = new ToolStripMenuItem
            {
                Text = "Erstelle Webpage"
            };

            itemCountLines.Click += OnEnlargeImage;

            //  Add the item to the context menu.
            menu.Items.Add(itemCountLines);

            //  Return the menu.
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