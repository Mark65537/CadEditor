﻿using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CadEditor
{
    public class Globals
    {
        public static bool LoadData(string filename, string dumpfile, string configFilename)
        {
            try
            {
                int size = (int)new FileInfo(filename).Length;
                using (FileStream f = File.OpenRead(filename))
                {
                    romdata = new byte[size];
                    f.Read(romdata, 0, size);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load rom error");
                return false;
            }

            try
            {
                if (dumpfile != "")
                {
                    int size = (int)new FileInfo(dumpfile).Length;
                    using (FileStream f = File.OpenRead(dumpfile))
                    {
                        dumpdata = new byte[size];
                        f.Read(dumpdata, 0, size);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load dump error");
                return false;
            }
            
            try
            {
                ConfigScript.LoadFromFile(configFilename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load config error");
                return false;
            }

            return true;
        }

        public static bool flushToFile()
        {
            if (OpenFile.dumpName != "")
            {
                try
                {
                    using (FileStream f = File.OpenWrite(OpenFile.dumpName))
                    {
                        f.Write(dumpdata, 0, dumpdata.Length);
                        f.Seek(0, SeekOrigin.Begin);
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
            try
            {
                using (FileStream f = File.OpenWrite(OpenFile.fileName))
                {
                    f.Write(Globals.romdata, 0, Globals.romdata.Length);
                    f.Seek(0, SeekOrigin.Begin);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        public static int readBlockIndexFromMap(byte[] arrayWithData, int romAddr, int index)
        {
            int wordLen = ConfigScript.getWordLen();
            bool littleEndian = ConfigScript.isLittleEndian();
            int dataStride = ConfigScript.getScreenDataStride();
            if (wordLen == 1)
            {
                return ConfigScript.ConvertScreenTile(arrayWithData[romAddr + index * dataStride]);
            }
            else if (wordLen == 2)
            {
                if (littleEndian)
                {
                    return ConfigScript.ConvertScreenTile(Utils.readWordLE(arrayWithData, romAddr + index * (dataStride * wordLen)));
                }
                else
                {
                    return ConfigScript.ConvertScreenTile(Utils.readWord(arrayWithData, romAddr + index * (dataStride * wordLen)));
                }
            }
            return -1;
        }

        public static Screen getScreen(OffsetRec screenOffset,  int screenIndex)
        {
            var result = new int[Math.Max(64, screenOffset.recSize)];
            var arrayWithData = dumpdata ?? romdata;
            int dataStride = ConfigScript.getScreenDataStride();
            int wordLen = ConfigScript.getWordLen();
            //bool littleEndian = ConfigScript.isLittleEndian();
            int beginAddr = screenOffset.beginAddr + screenIndex * screenOffset.recSize * dataStride * wordLen;
            for (int i = 0; i < screenOffset.recSize; i++)
                result[i] = readBlockIndexFromMap(arrayWithData, beginAddr, i);
            //TODO: read layer2

            int w = screenOffset.width;
            int h = screenOffset.height;
            if (ConfigScript.getScreenVertical())
            {
                Utils.swap(ref w, ref h);
                result = Utils.transpose(result, w, h);
            }

            return new Screen(new BlockLayer(result), w, h);
        }

        public static Image[] makeSegaBigBlocks(int curActiveVideoNo, int curActiveBigBlockNo, int curActivePalleteNo, MapViewType curViewType)
        {
            byte[] mapping = ConfigScript.getSegaMapping(curActiveBigBlockNo);
            byte[] videoTiles = ConfigScript.getVideoChunk(curActiveVideoNo);
            byte[] pal = ConfigScript.getPal(curActivePalleteNo);
            int count = ConfigScript.getBigBlocksCount(ConfigScript.getbigBlocksHierarchyCount() - 1, curActiveBigBlockNo);
            return ConfigScript.videoSega.makeBigBlocks(mapping, videoTiles, pal, count, curViewType);
        }

        public static Image[] makeGbBigBlocks(int curActiveVideoNo, int curActiveBigBlockNo, int curActivePalleteNo, MapViewType curViewType)
        {
            byte[] videoTiles = ConfigScript.getVideoChunk(curActiveVideoNo);
            ObjRec[] blocks = ConfigScript.getBlocksFunc(curActiveBigBlockNo);
            var blocksData = new byte[blocks.Length * 4]; //hardcode small blocks sizes
            Utils.writeBlocksLinear(blocks, blocksData, 0, 256, false, false);
            byte[] pal = ConfigScript.getPal(curActivePalleteNo);
            int count = ConfigScript.getBigBlocksCount(ConfigScript.getbigBlocksHierarchyCount() - 1, curActiveBigBlockNo);
            return ConfigScript.videoGb.makeBigBlocks(videoTiles, blocksData, pal, count, curViewType);
        }

        public static byte[] romdata { get; set; }
        public static byte[] dumpdata;
        public static int chunksCount = 256;
        public static int videoPageSize = 4096;
        public static int palLen = 16;
        public static int segaPalLen = 128;
    }
}