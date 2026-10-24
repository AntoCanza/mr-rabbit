param([string]$Version='0.8.4',[string]$RootPath)
Add-Type -AssemblyName System.Drawing
$qaRoot = if([string]::IsNullOrWhiteSpace($RootPath)){Join-Path $PSScriptRoot ('..\Captures\Release-'+$Version)}else{$RootPath}
foreach ($qaSizeName in @('1340x800','1920x1200','2048x1536')) {
  $qaFolder = Join-Path $qaRoot $qaSizeName
  $qaPictures = @(Get-ChildItem -LiteralPath $qaFolder -Filter '*.png' | Sort-Object Name)
  for ($qaStart = 0; $qaStart -lt $qaPictures.Count; $qaStart += 16) {
    $qaBitmap = [System.Drawing.Bitmap]::new(1920,1320)
    $qaGraphics = [System.Drawing.Graphics]::FromImage($qaBitmap)
    $qaGraphics.Clear([System.Drawing.Color]::White)
    $qaFont = [System.Drawing.Font]::new('Arial',14)
    for ($qaIndex = $qaStart; $qaIndex -lt [Math]::Min($qaStart+16,$qaPictures.Count); $qaIndex++) {
      $qaSlot = $qaIndex-$qaStart
      $qaX = ($qaSlot % 4)*480
      $qaY = [Math]::Floor($qaSlot/4)*330
      $qaImage = [System.Drawing.Image]::FromFile($qaPictures[$qaIndex].FullName)
      $qaRatio = [Math]::Min(472/$qaImage.Width,300/$qaImage.Height)
      $qaWidth = [int]($qaImage.Width*$qaRatio)
      $qaHeight = [int]($qaImage.Height*$qaRatio)
      $qaGraphics.DrawString(($qaSizeName+' '+$qaPictures[$qaIndex].BaseName),$qaFont,[System.Drawing.Brushes]::Black,$qaX+4,$qaY+2)
      $qaGraphics.DrawImage($qaImage,($qaX+(480-$qaWidth)/2),($qaY+28),$qaWidth,$qaHeight)
      $qaImage.Dispose()
    }
    $qaBitmap.Save((Join-Path $qaRoot ($qaSizeName+'-sheet-'+([int]($qaStart/16)+1)+'.jpg')),[System.Drawing.Imaging.ImageFormat]::Jpeg)
    $qaFont.Dispose(); $qaGraphics.Dispose(); $qaBitmap.Dispose()
  }
}
