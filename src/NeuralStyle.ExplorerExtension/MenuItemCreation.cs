using System;
using System.Windows.Forms;

namespace NeuralStyle.ExplorerExtension
{
    public static class MenuItemCreation
    {
        public static void CreateMenuItem(this ContextMenuStrip menu, string text, EventHandler targetFunction)
        {
            var menuItem = new ToolStripMenuItem { Text = text };
            menuItem.Click += targetFunction;

            menu.Items.Add(menuItem);
        }
    }
}