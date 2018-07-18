using CadEditor;
using System;
using System.Drawing;

public class Data 
{ 
  public OffsetRec getScreensOffset()  { return new OffsetRec(0x0100BF, 1 , 256, 16, 16); }
  public string getBlocksFilename()    { return "argos_no_senshi_3.png"; }
  public int    getPictureBlocksWidth()   { return 16; }

  public bool isBigBlockEditorEnabled() { return false; }
  public bool isBlockEditorEnabled()    { return false; }
  public bool isEnemyEditorEnabled()    { return false; }

  public bool getScreenVertical()         { return true; }
}