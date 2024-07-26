using CadEditor;
using System;
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
        public void addSubeditorButton(FormMain formMain)
        {
            this.formMain = formMain;
            var rm = new ResourceManager("PluginEditLayout.Icon", this.GetType().Assembly);
            var icon = (System.Drawing.Bitmap)rm.GetObject("icon_layout");
            var item = new ToolStripButton("Layout Editor", icon, btLayout_Click);
            item.DisplayStyle = ToolStripItemDisplayStyle.Image;
            formMain.addSubeditorButton(item);
        }

        public void addToolButton(FormMain formMain)
        {
        }

        public void loadFromConfig(object asm, object data)
        {
        }

        private void btLayout_Click(object sender, EventArgs e)
        {
            var f = new EditLayout();
            formMain.subeditorOpen(f, (ToolStripButton)sender, true);
        }

        FormMain formMain;
    }
}
