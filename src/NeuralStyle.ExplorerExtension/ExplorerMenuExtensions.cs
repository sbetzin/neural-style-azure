using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpShell.Attributes;
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
                Text = "Vergrössere Neurales Image"
            };

            itemCountLines.Click += OnEnlargeImage;

            //  Add the item to the context menu.
            menu.Items.Add(itemCountLines);

            //  Return the menu.
            return menu;
        }

        private void OnEnlargeImage(object sender, EventArgs args)
        {
            foreach (var path in SelectedItemPaths)
            {
                MessageBox.Show(path);
            }
        }
    }
}