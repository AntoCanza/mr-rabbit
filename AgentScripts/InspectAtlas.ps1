param([Parameter(Mandatory=$true)][string]$ImagePath)
Add-Type -AssemblyName System.Drawing
Add-Type -ReferencedAssemblies System.Drawing.Common,System.Drawing.Primitives,System.Private.Windows.GdiPlus,System.Private.Windows.Core -TypeDefinition @'
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
public static class AtlasInspection {
 public static string Read(string path) {
  using(var bitmap=new Bitmap(path)) {
   int w=bitmap.Width,h=bitmap.Height;
   var data=bitmap.LockBits(new Rectangle(0,0,w,h),ImageLockMode.ReadOnly,PixelFormat.Format32bppArgb);
   var bytes=new byte[data.Stride*h];Marshal.Copy(data.Scan0,bytes,0,bytes.Length);int stride=data.Stride;bitmap.UnlockBits(data);
   var visited=new bool[w*h];var queue=new int[w*h];var boxes=new int[16,4];
   for(int c=0;c<16;c++){boxes[c,0]=w;boxes[c,1]=h;}
   for(int start=0;start<w*h;start++) {
    if(visited[start]||bytes[(start/w)*stride+(start%w)*4+3]<128)continue;
    int head=0,tail=1,minX=w,minY=h,maxX=0,maxY=0;queue[0]=start;visited[start]=true;
    while(head<tail) {
     int pos=queue[head++],x=pos%w,y=pos/w;minX=Math.Min(minX,x);maxX=Math.Max(maxX,x);minY=Math.Min(minY,y);maxY=Math.Max(maxY,y);
     for(int d=0;d<4;d++) {
      int nx=x+(d==0?-1:d==1?1:0),ny=y+(d==2?-1:d==3?1:0);
      if(nx<0||nx>=w||ny<0||ny>=h)continue;int n=ny*w+nx;
      if(!visited[n]&&bytes[ny*stride+nx*4+3]>=128){visited[n]=true;queue[tail++]=n;}
     }
    }
    if(tail<25)continue;
    int cell=Math.Min(3,((minX+maxX)*2)/w)+4*Math.Min(3,((minY+maxY)*2)/h);
    boxes[cell,0]=Math.Min(boxes[cell,0],minX);boxes[cell,1]=Math.Min(boxes[cell,1],minY);
    boxes[cell,2]=Math.Max(boxes[cell,2],maxX);boxes[cell,3]=Math.Max(boxes[cell,3],maxY);
   }
   var result=new System.Text.StringBuilder();result.AppendLine("Dimensions: "+w+"x"+h);
   for(int c=0;c<16;c++) {
    int x=Math.Max(0,boxes[c,0]-4),y=Math.Max(0,boxes[c,1]-4),right=Math.Min(w,boxes[c,2]+5),bottom=Math.Min(h,boxes[c,3]+5);
    result.AppendLine("new Rect("+x+","+y+","+(right-x)+","+(bottom-y)+"),");
   }
   return result.ToString();
  }
 }
}
'@
[AtlasInspection]::Read((Resolve-Path -LiteralPath $ImagePath).Path)
