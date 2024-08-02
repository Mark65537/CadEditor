using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CadEditor
{
    public partial class EditLayout : Form
    {
        public EditLayout()
        {
            InitializeComponent();
        }

        private void EditForm_Load(object sender, EventArgs e)
        {
            makeScreens();
            LoadSpritesFromResources();
            InitializePanels();
            InitializeComboBoxes();
            InitializeLayoutComboBox();
        }

        private void LoadSpritesFromResources()
        {
            try
            {
                //TODO не может найти ресурсы
                LoadImageListFromResources(scrollSprites, PluginEditLayout.EditLayout.scrolls);
                LoadImageListFromResources(doorSprites, PluginEditLayout.EditLayout.doors);
                LoadImageListFromResources(dirSprites, PluginEditLayout.EditLayout.dirs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error in PluginEditLayout : {ex.Message}");
            }
        }

        private void LoadImageListFromResources(ImageList imageList, Bitmap resource)
        {
            imageList.Images.Clear();
            imageList.Images.AddStrip(resource);
        }

        private void InitializePanels()
        {
            InitializePanel(objPanel, scrollSprites, buttonScrollClick);
            InitializePanel(doorsPanel, doorSprites, buttonDoorClick);
            InitializeBlocksPanel();
            UtilsGui.setCbItemsCount(cbVideoNo, ConfigScript.videoOffset.recCount);
            UtilsGui.setCbItemsCount(cbBigBlockNo, ConfigScript.bigBlocksOffsets[0].recCount);
            UtilsGui.setCbItemsCount(cbBlockNo, ConfigScript.blocksOffset.recCount);
            UtilsGui.setCbItemsCount(cbPaletteNo, ConfigScript.palOffset.recCount);
            cbVideoNo.SelectedIndex = 0;
            cbBigBlockNo.SelectedIndex = 0;
            cbBlockNo.SelectedIndex = 0;
            cbPaletteNo.SelectedIndex = 0;
        }

        private void InitializePanel(Panel panel, ImageList imageList, EventHandler clickHandler)
        {
            panel.Controls.Clear();
            panel.SuspendLayout();

            for (int i = 0; i < imageList.Images.Count; i++)
            {
                var button = new Button
                {
                    Size = new Size(32, 32),
                    ImageList = imageList,
                    ImageIndex = i
                };
                button.Click += clickHandler;
                panel.Controls.Add(button);
            }

            panel.ResumeLayout();
        }

        private void InitializeBlocksPanel()
        {
            blocksPanel.Controls.Clear();
            blocksPanel.SuspendLayout();

            for (int i = 0; i < ConfigScript.screensOffset[scrLevelNo].recCount; i++)
            {
                var button = new Button
                {
                    Size = new Size(64, 64),
                    ImageList = screenImages,
                    ImageIndex = i
                };
                button.Click += buttonBlockClick;
                blocksPanel.Controls.Add(button);
            }

            blocksPanel.ResumeLayout();
        }

        private void InitializeComboBoxes()
        {
            cbLayoutNo.Items.Clear();
            foreach (var lr in ConfigScript.getLevelRecs())
            {
                cbLayoutNo.Items.Add(string.Format("0x{0:X} ({1}x{2})", lr.layoutAddr, lr.width, lr.height));
            }
            cbLayoutNo.SelectedIndex = 0;
            cbShowScrolls.Visible = ConfigScript.isShowScrollsInLayout();
            btExport.Visible = pnParamGeneric.Visible = true;
        }

        private void InitializeLayoutComboBox()
        {
            cbLayoutNo.Items.Clear();
            foreach (var lr in ConfigScript.getLevelRecs())
            {
                cbLayoutNo.Items.Add(string.Format("0x{0:X} ({1}x{2})", lr.layoutAddr, lr.width, lr.height));
            }
            cbLayoutNo.SelectedIndex = 0;
        }

        private void reloadLevelLayer()
        {
            curLevelLayerData = ConfigScript.getLayout(curActiveLayout);
            curActiveBlock = 0;
            pbMap.Invalidate();
        }

        private void makeScreens()
        {
            screenImages.Images.Clear();
            for (int scrNo = 0; scrNo <= 256/*ConfigScript.screensOffset[scrLevelNo].recCount*/; scrNo++)
                screenImages.Images.Add(CreateLabeledBitmap(64, 64, scrNo));
        }

        /// <summary>
        /// Creates a bitmap image with specified width and height, fills it with black color, draws a green border, and optionally adds a hexadecimal number.
        /// </summary>
        /// <param name="w">The width of the bitmap.</param>
        /// <param name="h">The height of the bitmap.</param>
        /// <param name="no">The number to be converted to hexadecimal and drawn on the bitmap if the condition is met.</param>
        /// <returns>A Bitmap object with the specified modifications.</returns>
        /// <example>
        /// <code>
        /// Bitmap image = CreateLabeledBitmap(200, 100, 1234);
        /// </code>
        /// </example>
        private Image CreateLabeledBitmap(int w, int h, int no)
        {
            var b = new Bitmap(w, h);
            using (var g = Graphics.FromImage(b))
            {
                g.FillRectangle(Brushes.Black, new Rectangle(0, 0, w, h));
                g.DrawRectangle(new Pen(Color.Green, w/32.0f), new Rectangle(0, 0, w, h));
                if (no % 256 != 0)
                  g.DrawString(string.Format("{0:X}", no), new Font("Arial", w/8.0f), Brushes.White, new Point(0, 0));
            }
            return b;
        }

        private void pb_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            int w = curWidth;
            int h = curHeight;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int index = curLevelLayerData.layer[y * w + x];
                    int scroll = curLevelLayerData.scroll != null ? curLevelLayerData.scroll[y * w + x] : 0;
                    g.DrawImage(screenImages.Images[index % 256], new Rectangle(x*64, y*64, 64, 64));
                    if (showScrolls)
                      g.DrawString(string.Format("{0:X}", scroll), new Font("Arial", 8), new SolidBrush(Color.Red), new Rectangle(x * 64 + 24, y * 64 + 24, 32, 16));
                }
            }
        }

        private void changeScroll(int index)
        {
            if (curLevelLayerData.scroll != null)
            {
                var scrollByteArray = ConfigScript.getScrollByteArray();
                if (curActiveBlock < scrollByteArray.Length)
                {
                    curLevelLayerData.scroll[index] = scrollByteArray[curActiveBlock];
                }
            }
        }

        private void pb_MouseUp(object sender, MouseEventArgs e)
        {
            int dx = e.X / 64;
            int dy = e.Y / 64;

            if (dx >= curLevelLayerData.width || dy >= curLevelLayerData.height)
                return;

            dirty = true;
            int index = dy * curLevelLayerData.width + dx;

            if (drawMode == MapDrawMode.Screens)
                curLevelLayerData.layer[index] = curActiveBlock & 0xFF;
            else if (drawMode == MapDrawMode.Scrolls)
                changeScroll(index);
            else if (drawMode == MapDrawMode.Doors)
            {
                if (curLevelLayerData.scroll != null)
                {
                    curLevelLayerData.scroll[index] = (curActiveBlock & 0x1F) | (curLevelLayerData.scroll[index] & 0xE0);
                }
               
            }
            pbMap.Invalidate();
        }

        private int curActiveBlock;
        private MapDrawMode drawMode = MapDrawMode.Screens;
        private bool dirty;
        private bool showScrolls;
        private LevelLayerData curLevelLayerData;

        private int curActiveLayout;

        //for export params
        private int curVideoNo;
        private int curBigBlockNo;
        private int curBlockNo;
        private int curPalleteNo;

        private int curWidth = 1;
        private int curHeight = 1;

        //
        private int scrLevelNo = 0;
        

        private void cbLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!UtilsGui.askToSave(ref dirty, saveToFile, returnCbLevelIndex))
                return;
            if (cbLayoutNo.SelectedIndex == -1)
                return;

            curActiveLayout = cbLayoutNo.SelectedIndex;
            curWidth = ConfigScript.getLevelWidth(curActiveLayout);
            curHeight = ConfigScript.getLevelHeight(curActiveLayout);

            drawMode = MapDrawMode.Screens;
            curActiveBlock = 0;
            activeBlock.Image = screenImages.Images[0];

            updatePanelsVisibility();
            cbLayoutNo.Items.Clear();
            foreach (var lr in ConfigScript.getLevelRecs())
                cbLayoutNo.Items.Add(String.Format("0x{0:X} ({1}x{2})", lr.layoutAddr, lr.width, lr.height));
            UtilsGui.setCbIndexWithoutUpdateLevel(cbLayoutNo, cbLevel_SelectedIndexChanged, curActiveLayout);
            reloadLevelLayer();
        }

        private void updatePanelsVisibility()
        {
            bool showScroll = ConfigScript.isShowScrollsInLayout();
            pnDoors.Visible = showScroll;
            pnSelectScroll.Visible = showScroll;
            pnGeneric.Visible = true;
        }

        private void buttonBlockClick(Object button, EventArgs e)
        {
            int index = ((Button)button).ImageIndex;
            activeBlock.Image = screenImages.Images[index];
            curActiveBlock = index;
            drawMode = MapDrawMode.Screens;
        }

        private void buttonScrollClick(Object button, EventArgs e)
        {
            int index = ((Button)button).ImageIndex;
            activeBlock.Image = scrollSprites.Images[index];
            curActiveBlock = index;
            drawMode = MapDrawMode.Scrolls;
        }

        private void buttonDoorClick(Object button, EventArgs e)
        {
            int index = ((Button)button).ImageIndex;
            activeBlock.Image = doorSprites.Images[index];
            curActiveBlock = index;
            drawMode = MapDrawMode.Doors;
        }

        private void EditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dirty)
            {
                DialogResult dr = MessageBox.Show("Level was changed. Do you want to save current level?", "Save", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                    saveToFile();
            }
        }

        private bool saveToFile()
        {
            if (!ConfigScript.setLayout(curLevelLayerData, curActiveLayout))
            {
                return false;
            }

            dirty = !Globals.flushToFile();
            return !dirty; 
        }

        private void returnCbLevelIndex()
        {
            UtilsGui.setCbIndexWithoutUpdateLevel(cbLayoutNo, cbLevel_SelectedIndexChanged, curActiveLayout);
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            saveToFile();
        }

        private void cbShowScrolls_CheckedChanged(object sender, EventArgs e)
        {
            showScrolls = cbShowScrolls.Checked;
            pbMap.Invalidate();
        }

        private Bitmap makeLevelImage()
        {
            var answer = new Bitmap(curWidth*512, curHeight*512);
            using (var g = Graphics.FromImage(answer))
            {
                for (int w = 0; w < curWidth; w++)
                {
                    for (int h = 0; h < curHeight; h++)
                    {
                        int scrNo = curLevelLayerData.layer[h*curWidth + w] - 1;
                        Bitmap scr = scrNo >= 0 ? ConfigScript.videoNes.makeScreen(scrNo, scrLevelNo, curVideoNo, curBigBlockNo, curBlockNo, curPalleteNo) : VideoHelper.emptyScreen(512,512,false);
                        g.DrawImage(scr, new Point(w*512,h*512));
                    }
                }
            }
            return answer;
        }

        private void btExport_Click(object sender, EventArgs e)
        {
            var f = new SelectFile();
            f.filename = "level.png";
            f.ShowDialog();
            if (!f.result)
                return;
            var fn = f.filename;
            Bitmap levelImage = makeLevelImage();
            levelImage.Save(fn);
        }

        private void cbVideoNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            curVideoNo = cbVideoNo.SelectedIndex + 0x90;
            curBigBlockNo = cbBigBlockNo.SelectedIndex;
            curBlockNo = cbBlockNo.SelectedIndex;
            curPalleteNo = cbPaletteNo.SelectedIndex;
        }
    }        
    enum MapDrawMode
    {
        Screens,
        Scrolls,
        Doors
    };
}
