﻿using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using CadEditor;
using Newtonsoft.Json;

namespace PluginExportScreens
{
    public class PluginExportScreens : IPlugin
    {
        public string GetName()
        {
            return "Export screens";
        }
        public void addSubeditorButton(FormMain formMain)
        {
        }

        public void addToolButton(FormMain formMain)
        {
            this.formMain = formMain;
            var rm = new ResourceManager("PluginExportScreens.Icon", this.GetType().Assembly);

            var iconImport = (Bitmap)rm.GetObject("icon_import");
            var item = new ToolStripButton("Import", iconImport, btImport_Click);
            item.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            formMain.addToolButton(item);

            var iconExportPic = (Bitmap)rm.GetObject("icon_export");
            var exportMenu = new ToolStripSplitButton("Export", iconExportPic);
            formMain.addToolButton(exportMenu);

            var itemMenu = new ToolStripMenuItem("Export screens as png", iconExportPic, bttExportPic_Click);
            itemMenu.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            exportMenu.DropDownItems.Add(itemMenu);

            var iconExportJson = (Bitmap)rm.GetObject("icon_export");
            itemMenu = new ToolStripMenuItem("Export json", iconExportJson, bttExportJson_Click);
            itemMenu.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            exportMenu.DropDownItems.Add(itemMenu);

            var iconExport = (Bitmap)rm.GetObject("icon_export");
            itemMenu = new ToolStripMenuItem("Export binary", iconExport, btExport_Click);
            itemMenu.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            exportMenu.DropDownItems.Add(itemMenu);

            var iconExportTmx = (Bitmap)rm.GetObject("icon_export");
            itemMenu = new ToolStripMenuItem("Export TMX", iconExportTmx, btExportTmx_Click);
            itemMenu.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            exportMenu.DropDownItems.Add(itemMenu);
        }

        public void loadFromConfig(object asm, object data)
        {
        }


        private void bttExportPic_Click(object sender, EventArgs e)
        {
            SaveScreensCount.exportMode = true;
            SaveScreensCount.filename = "exportedScreens.png";
            var f = new SaveScreensCount();
            f.Text = "Export picture";

            formMain.subeditorOpen(f, (ToolStripItem)sender, true);

            if (SaveScreensCount.result)
            {
                if (SaveScreensCount.count <= 0)
                {
                    MessageBox.Show("Screens count value must be greater than 0", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                int saveLastIndex = SaveScreensCount.first + SaveScreensCount.count;
                var screens = ConfigScript.loadScreens();
                int screensCount = screens.Length;
                if (saveLastIndex > screensCount)
                {
                    MessageBox.Show(
                        string.Format(
                            "First screen + Screens Count value ({0}) must be less than Total Screen Count in the game ({1}",
                            saveLastIndex, screensCount), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int first = SaveScreensCount.first;
                int firstW = screens[0].width;
                int firstH = screens[0].height;
                float curScale = formMain.curScale;
                //only for screens with same sizes

                var renderParams = new MapEditor.RenderParams
                {
                    bigBlocks = formMain.bigBlocks,
                    curScale = curScale,
                    showBorder = false,
                    width = firstW,
                    height = firstH
                };

                var probeIm = MapEditor.screenToImage(formMain.screens, formMain.screenNo, renderParams);
                int screenCount = SaveScreensCount.count;
                var resultImage = new Bitmap(probeIm.Width * screenCount, probeIm.Height);
                using (var g = Graphics.FromImage(resultImage))
                {
                    for (int i = 0; i < screenCount; i++)
                    {
                        int width = screens[i].width;
                        int height = screens[i].height;
                        var im = MapEditor.screenToImage(formMain.screens, first + i, renderParams);
                        g.DrawImage(im, new Point(i * im.Width, 0));
                    }
                }
                resultImage.Save(SaveScreensCount.filename);
            }
        }

        private void bttExportJson_Click(object sender, EventArgs e)
        {
            var screens = ConfigScript.loadScreens();

            SaveScreensCount.exportMode = true;
            SaveScreensCount.filename = "exportedScreens.json";
            var f = new SaveScreensCount();
            f.Text = "Export json";

            formMain.subeditorOpen(f, (ToolStripItem)sender, true);

            if (SaveScreensCount.result)
            {
                if (SaveScreensCount.count <= 0)
                {
                    MessageBox.Show("Screens count value must be greater than 0", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                int saveLastIndex = SaveScreensCount.first + SaveScreensCount.count;
                int screensCount = screens.Length;
                if (saveLastIndex > screensCount)
                {
                    MessageBox.Show(string.Format("First screen + Screens Count value ({0}) must be less than Total Screen Count in the game ({1}", saveLastIndex, screensCount), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int first = SaveScreensCount.first;
                int screenCount = SaveScreensCount.count;
                var screenParams = new { Screens = new CadEditor.Screen[screenCount] };
                using (TextWriter tw = new StreamWriter(SaveScreensCount.filename))
                {
                    for (int i = 0; i < screenCount; i++)
                    {
                        screenParams.Screens[i] = screens[first + i];
                    }
                    tw.WriteLine(JsonConvert.SerializeObject(screenParams));
                }
            }
        }

        private void btExportTmx_Click(object sender, EventArgs e)
        {
            var f = new ExportTMX();
            f.setFormMain(formMain);
            formMain.subeditorOpen(f, (ToolStripItem)sender, true);
        }

        private void btImport_Click(object sender, EventArgs e)
        {
            var screens = ConfigScript.loadScreens();
            SaveScreensCount.exportMode = false;
            SaveScreensCount.filename = "exportedScreens.bin";
            var f = new SaveScreensCount();
            f.Text = "Import";
            formMain.subeditorOpen(f, (ToolStripItem)sender, true);
            if (SaveScreensCount.result)
            {
                int saveLastIndex = SaveScreensCount.first;
                if (saveLastIndex > screens.Length)
                {
                    MessageBox.Show(string.Format("First screen ({0}) must be less than Total Screen Count in the game ({1}", saveLastIndex, screens.Length), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!File.Exists(SaveScreensCount.filename))
                {
                    MessageBox.Show(string.Format("File ({0}) not exists", SaveScreensCount.filename), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int screenSize = ConfigScript.screensOffset[0].recSize; //TODO: only for games with only one screensOffset
                int first = SaveScreensCount.first;
                var data = Utils.loadDataFromFile(SaveScreensCount.filename);
                int screenCount = data.Length / screenSize;
                for (int i = 0; i < screenCount; i++)
                {
                    Array.Copy(data, i * screenSize, screens[first + i].layers[0].data, 0, screenSize);
                }
            }
            formMain.setScreens(screens);
            formMain.setDirty();
            formMain.reloadLevel(false);
        }

        private void btExport_Click(object sender, EventArgs e)
        {
            CadEditor.Screen[] screens = ConfigScript.loadScreens();
            SaveScreensCount.exportMode = true;
            SaveScreensCount.filename = "exportedScreens.bin";
            var f = new SaveScreensCount();
            f.Text = "Export";
            formMain.subeditorOpen(f, (ToolStripItem)sender, true);
            if (SaveScreensCount.result)
            {
                if (SaveScreensCount.count <= 0)
                {
                    MessageBox.Show("Screens count value must be greater than 0", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                int saveLastIndex = SaveScreensCount.first + SaveScreensCount.count;
                if (saveLastIndex > screens.Length)
                {
                    MessageBox.Show(string.Format("First screen + Screens Count value ({0}) must be less than Total Screen Count in the game ({1}", saveLastIndex, screens.Length), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int screenSize = ConfigScript.screensOffset[0].recSize; //TODO: only for games with only one screensOffset
                int screenCount = SaveScreensCount.count;
                int first = SaveScreensCount.first;
                var data = new byte[screenSize * screenCount];

                for (int i = 0; i < screenCount; i++)
                {
                    //byte[] byteScreen = new byte[screens[i + first].layers[0].data.Length];
                    //all ints will be truncated to byte. it's ok for NES games, but may not for other platforms
                    var byteScreen = Array.ConvertAll(screens[i + first].layers[0].data, (int x)=>(byte)x);
                    Array.Copy(byteScreen, 0, data, screenSize * i, screenSize);
                }
                Utils.saveDataToFile(SaveScreensCount.filename, data);
            }
        }

        FormMain formMain;
    }
}
