﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using CadEditor;
using CSScriptLibrary;

namespace PluginLevelParamsCad
{
    public class PluginLevelParams : IPlugin
    {
        public string GetName()
        {
            return "Chip and Dale Level Parameters Editor";
        }
        public void addSubeditorButton(MainForm formMain)
        {
            this.formMain = formMain;
            //var rm = new ResourceManager("PluginMapEditor.Icon", this.GetType().Assembly);
            //var iconMap = (System.Drawing.Bitmap)rm.GetObject("icon_map");
            var item = new ToolStripButton("Level params", null, btLevelParams_Click);
            item.DisplayStyle = ToolStripItemDisplayStyle.Image;
            formMain.addSubeditorButton(item);
        }

        public void addToolButton(MainForm formMain)
        {
        }

        public void loadFromConfig(object asmObj, object data)
        {
            AsmHelper asm = (AsmHelper)asmObj;
            GlobalsCad.boxesBackOffset = (OffsetRec)asm.InvokeInst(data, "*.getBoxesBackOffset");
            GlobalsCad.levelRecBaseOffset = (int)asm.InvokeInst(data, "*.getLevelRecBaseOffset");
            GlobalsCad.levelRecDirOffset = (int)asm.InvokeInst(data, "*.getLevelRecDirOffset");
            GlobalsCad.layoutPtrAdd = (int)asm.InvokeInst(data, "*.getLayoutPtrAdd");
            GlobalsCad.scrollPtrAdd = (int)asm.InvokeInst(data, "*.getScrollPtrAdd");
            GlobalsCad.dirPtrAdd = (int)asm.InvokeInst(data, "*.getDirPtrAdd");
            GlobalsCad.doorRecBaseOffset = (int)asm.InvokeInst(data, "*.getDoorRecBaseOffset");
        }

        private void btLevelParams_Click(object sender, EventArgs e)
        {
            var f = new EditLevelData();
            formMain.subeditorOpen(f, (ToolStripButton)sender, true);
        }

        MainForm formMain;
    }
}
