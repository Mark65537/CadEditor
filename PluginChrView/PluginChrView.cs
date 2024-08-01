using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CadEditor;

namespace PluginPrgView
{
    using System;
    using System.Windows.Forms;
    using System.Resources;

    using CadEditor;

    namespace PluginChrView
    {
        public class PluginChrView : IPlugin
        {
            public string GetName()
            {
                return "Chr-bank viewer";
            }
            public void addSubeditorButton(MainForm formMain)
            {
                this.formMain = formMain;
                var rm = new ResourceManager("PluginChrView.Icon", this.GetType().Assembly);
                var icon = (Bitmap)rm.GetObject("icon_video");
                var item = new ToolStripButton("View video", icon, btHex_Click)
                {
                    DisplayStyle = ToolStripItemDisplayStyle.Image
                };
                formMain.addSubeditorButton(item);
            }

            public void addToolButton(MainForm formMain)
            {
            }

            public void loadFromConfig(object asm, object data)
            {
            }

            private void btHex_Click(object sender, EventArgs e)
            {
                var f = new EditVideo();
                formMain.subeditorOpen(f, (ToolStripButton)sender, false);
            }

            MainForm formMain;
        }
    }

}
