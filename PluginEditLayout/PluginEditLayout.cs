using CadEditor;
using System;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace PluginEditLayout
{
    public class PluginEditLayout : IPlugin
    {
        public string GetName()
        {
            return "Layout Editor";
        }
        public void addSubeditorButton(MainForm formMain)
        {
            this.formMain = formMain;
            var icon = new Bitmap("icon.png");
            var item = new ToolStripButton("Layout Editor", icon, btLayout_Click);
            item.DisplayStyle = ToolStripItemDisplayStyle.Image;
            formMain.addSubeditorButton(item);
        }

        public void addToolButton(MainForm formMain)
        {
        }

        public void loadFromConfig(object asm, object data)
        {
        }

        private void btLayout_Click(object sender, EventArgs e)
        {
            var f = new CadEditor.EditLayout();
            formMain.subeditorOpen(f, (ToolStripButton)sender, true);
        }

        MainForm formMain;
    }
}
