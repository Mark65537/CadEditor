﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using CadEditor;

namespace PluginSegaBackEditor
{
    public class PluginSegaBackEditor : IPlugin
    {
        public string GetName()
        {
            return "Sega Back Editor";
        }
        public void addSubeditorButton(MainForm formMain)
        {
            this.formMain = formMain;
            var rm = new ResourceManager("PluginSegaBackEditor.Icon", this.GetType().Assembly);
            var iconMap = (System.Drawing.Bitmap)rm.GetObject("icon_map");
            var item = new ToolStripButton("Back Editor", iconMap, btMap_Click)
            {
                DisplayStyle = ToolStripItemDisplayStyle.Image
            };
            formMain.addSubeditorButton(item);
        }

        public void addToolButton(MainForm formMain)
        {
        }

        private void btMap_Click(object sender, EventArgs e)
        {
            var f = new SegaBlockEdit();
            f.changeModeToBackEdit();
            formMain.subeditorOpen(f, (ToolStripButton)sender);
        }

        public void loadFromConfig(object asm, object data)
        {
        }

        MainForm formMain;
    }
}
