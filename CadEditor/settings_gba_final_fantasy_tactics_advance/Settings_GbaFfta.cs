using CadEditor;
using System;
using System.Drawing;

public class Data 
{ 
  public OffsetRec getScreensOffset()  { return new OffsetRec(4096, 1 , 0x20*0x40, 0x20, 0x40);  }
  public OffsetRec getScreensOffset2() { return new OffsetRec(0   , 1 , 0x20*0x40, 0x20, 0x40);  }
  public int getScreenWidth()          { return 0x20; }
  public int getScreenHeight()         { return 0x40; }
  public int getWordLen()              { return 2;}
  public int getLayersCount()          { return 2;}
  public bool isLittleEndian()         { return true; }
  public int    getPictureBlocksWidth(){ return 64; }
  public int getBigBlocksCount()       { return 542; }
  public string getBlocksFilename()    { return "gba_ffta_1.png"; }
  
  public bool isBigBlockEditorEnabled() { return false; }
  public bool isBlockEditorEnabled()    { return false; }
  public bool isEnemyEditorEnabled()    { return false; }
  
  public LoadScreensFunc loadScreensFunc() { return loadScreens; }
  public SaveScreensFunc saveScreensFunc() { return saveScreens; }
  
  public Screen[] loadScreens()
  {
      var screen1 = Globals.getScreen(getScreensOffset(), 0);
      var screen2 = Globals.getScreen(getScreensOffset2(), 0);
      //add two layers in one screen
      screen1.layers = new BlockLayer[] { screen1.layers[0], screen2.layers[0] };
      return new Screen[] { screen1 };
      
  }
  
  public void saveScreens(Screen[] screens)
  {
      Utils.saveScreensToOffset(getScreensOffset() , screens, 0, 0, 0);
      Utils.saveScreensToOffset(getScreensOffset2(), screens, 0, 0, 1);
  }
}