using CadEditor;
using System;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
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
            var assembly = Assembly.GetExecutingAssembly();
            var imgPath = "PluginEditLayout.icon.png";
            using (var imageStream = assembly.GetManifestResourceStream(imgPath))
            {
                // Проверка, что поток не пустой
                if (imageStream != null)
                {
                    // Создание объекта Bitmap из потока
                    Bitmap icon = new Bitmap(imageStream);
                    var item = new ToolStripButton("Layout Editor", icon, btLayout_Click);
                    item.DisplayStyle = ToolStripItemDisplayStyle.Image;
                    formMain.addSubeditorButton(item);
                }
                else
                {
                    // Обработка ошибки, если ресурс не найден
                    MessageBox.Show("Resource not found.");
                }
            }
            //var icon = new Bitmap("icon.png");
            
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
