using CadEditor;
using System;
using System.Drawing;

public class Data 
{ 
  public OffsetRec getScreensOffset()  { return new OffsetRec(0, 19 , 16*16, 16, 16);   }
  public string getBlocksFilename()    { return "sega_quackshot_1.png"; }
  
  public bool isBigBlockEditorEnabled() { return false; }
  public bool isBlockEditorEnabled()    { return false; }
  public bool isEnemyEditorEnabled()    { return false; }
}