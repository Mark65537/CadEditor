﻿using CadEditor;
using CadEnemyEditor;
using CSScriptLibrary;
using System;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace PluginAnimEditor
{
    class PluginAnimEditor : IPlugin
    {
        public string GetName()
        {
            return "Anim Editor (Capcom)";
        }
        public void addSubeditorButton(FormMain formMain)
        {
            this.formMain = formMain;
            var rm = new ResourceManager("PluginAnimEditor.Icon", this.GetType().Assembly);
            var iconAnim = (Bitmap)rm.GetObject("icon_anim");
            var item = new ToolStripButton("Anim Editor", iconAnim, btAnim_Click);
            item.DisplayStyle = ToolStripItemDisplayStyle.Image;
            formMain.addSubeditorButton(item);
        }

        public void addToolButton(FormMain formMain)
        {
        }

        public void loadFromConfig(object asmObj, object data)
        {
            AsmHelper asm = (AsmHelper)asmObj;
            AnimConfig.animCount = (int)asm.InvokeInst(data, "*.getAnimCount");
            AnimConfig.animAddrHi = (int)asm.InvokeInst(data, "*.getAnimAddrHi");
            AnimConfig.animAddrLo = (int)asm.InvokeInst(data, "*.getAnimAddrLo");
            AnimConfig.frameCount = (int)asm.InvokeInst(data, "*.getFrameCount");
            AnimConfig.frameAddrHi = (int)asm.InvokeInst(data, "*.getFrameAddrHi");
            AnimConfig.frameAddrLo = (int)asm.InvokeInst(data, "*.getFrameAddrLo");
            AnimConfig.coordCount = (int)asm.InvokeInst(data, "*.getCoordCount");
            AnimConfig.coordAddrHi = (int)asm.InvokeInst(data, "*.getCoordAddrHi");
            AnimConfig.coordAddrLo = (int)asm.InvokeInst(data, "*.getCoordAddrLo");
            AnimConfig.pal = (byte[])asm.InvokeInst(data, "*.getAnimPal");
            AnimConfig.animBankNo = (int)asm.InvokeInst(data, "*.getAnimBankNo");
        }

        private void btAnim_Click(object sender, EventArgs e)
        {
            var f = new AnimEditor();
            formMain.subeditorOpen(f, (ToolStripButton)sender, true);
        }

        FormMain formMain;
    }

    public static class AnimConfig
    {
        public static int animCount;
        public static int animAddrHi;
        public static int animAddrLo;

        public static int frameCount;
        public static int frameAddrHi;
        public static int frameAddrLo;

        public static int coordCount;
        public static int coordAddrHi;
        public static int coordAddrLo;

        public static byte[] pal;

        public static int animBankNo;
    }
}
