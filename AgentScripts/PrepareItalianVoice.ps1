param([string]$PythonExecutable)
$ErrorActionPreference = 'Stop'
# The approved voice is Elsa Neural. Never regenerate Windows desktop speech.
if ([string]::IsNullOrWhiteSpace($PythonExecutable)) {
    $rabbitBundledPython = Join-Path $env:USERPROFILE '.cache\codex-runtimes\codex-primary-runtime\dependencies\python\python.exe'
    if (Test-Path -LiteralPath $rabbitBundledPython) { $PythonExecutable = $rabbitBundledPython }
    else { throw 'Specify -PythonExecutable with a Python installation containing edge-tts and imageio-ffmpeg.' }
}
& $PythonExecutable (Join-Path $PSScriptRoot 'PrepareItalianVoice.py') --install
if ($LASTEXITCODE -ne 0) { throw 'Elsa Neural voice preparation failed; inspect the generation output.' }
