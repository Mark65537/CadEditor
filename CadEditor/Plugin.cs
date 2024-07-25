using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CadEditor
{
    public static class PluginLoader
    {
        public static T LoadPlugin<T>(string path)
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (appPath == null)
            {
                return default;
            }
            Assembly currentAssembly = Assembly.LoadFile(Path.Combine(appPath, path));
            foreach (Type type in currentAssembly.GetTypes())
            {
                if (type.GetInterfaces().Contains(typeof(T)))
                    return (T)Activator.CreateInstance(type);
            }
            return default;
        }
    }

    #region Interfaces
    /// <summary>
    /// Базовый интерфейс который должны наследовать все остальные интерфейсы плагинов
    /// </summary>
    public interface IPluginBase
    {
        string GetName();
    }
    public interface IPlugin : IPluginBase
    {
        void addSubeditorButton(FormMain formMain);
        void addToolButton(FormMain formMain);
        void loadFromConfig(object asm, object data); //asm is CSScriptLibrary.AsmHelper
    }
    /// <summary>
    /// Этот интерфейс должны наследовать все Видео Плагины
    /// </summary>
    public interface IVideoPlugin : IPluginBase
    {
        //TODO добавить необходимые методы, например GetPalette
    }
    public interface IVideoPluginNes : IVideoPlugin
    {
        void updateColorsFromConfig();

        Image[] makeBigBlocks(int videoNo, int bigBlockNo, int blockNo, int palleteNo, MapViewType smallObjectsViewType = MapViewType.Tiles,
            MapViewType curViewType = MapViewType.Tiles, int heirarchyLevel = 0);
        Image[] makeBigBlocks(int videoNo, int bigBlockNo, int blockNo, BigBlock[] bigBlockData, int palleteNo, MapViewType smallObjectsViewType = MapViewType.Tiles,
            MapViewType curViewType = MapViewType.Tiles, int heirarchyLevel = 0);

        Bitmap GetTile(int index, byte[] videoChunk, byte[] pallete, int subPalIndex, bool withAlpha = false);
        Bitmap makeImageStrip(byte[] videoChunk, byte[] pallete, int subPalIndex, bool withAlpha = false);
        Bitmap makeImageRectangle(byte[] videoChunk, byte[] pallete, int subPalIndex, bool withAlpha = false);

        Bitmap[] makeObjects(ObjRec[] objects, Bitmap[][] objStrips, MapViewType drawType, int constantSubpal = -1);
        Bitmap[] makeObjects(int videoPageId, int tilesId, int palId, MapViewType drawType, int constantSubpal = -1);
        Bitmap makeObject(int index, ObjRec[] objects, Bitmap[][] objStrips, MapViewType drawType, int constantSubpal = -1);
        Bitmap makeObjectsStrip(int videoPageId, int tilesId, int palId, MapViewType drawType, int constantSubpal = -1);

        Bitmap makeScreen(int scrNo, int levelNo, int videoNo, int bigBlockNo, int blockNo, int palleteNo, bool withBorders = true);

        Color[] DefaultNesColors { get; set; }
    }

    public interface IVideoPluginGb : IVideoPlugin
    {
        Image[] makeBigBlocks(byte[] ppuData, byte[] tiles, byte[] pallette, int count, MapViewType curViewType = MapViewType.Tiles);
        Color[] getPalette(byte[] pal);
        Bitmap getTile(byte[] ppuData, Color[] palette, int no);
        Bitmap getTilesRectangle(byte[] ppuData, Color[] palette);
    }

    public interface IVideoPluginSega : IVideoPlugin
    {
        Image[] makeBigBlocks(byte[] mapping, byte[] tiles, byte[] palette, int count, MapViewType curViewType = MapViewType.Tiles);
        Color[] getPalette(byte[] pal);
        Bitmap getTileFromArray(byte[] tiles, ref int position, Color[] palette, byte palIndex);
        Bitmap getTileFrom2ColorArray(byte[] tiles, ref int position);
        byte[] getArrayFrom2ColorTile(Bitmap tile);
        byte[] getArrayFrom2ColorBlock(Bitmap block);
        Bitmap getTile(byte[] tiles, ushort word, Color[] palette, byte palIndex, bool hf, bool vf);
    } 
    #endregion
}
