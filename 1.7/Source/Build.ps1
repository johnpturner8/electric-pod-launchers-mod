param(
    [string]$RimWorldDir = (Join-Path $PSScriptRoot '../../../..'),
    [string]$HarmonyPath,
    [string]$CompilerPath
)

$ErrorActionPreference = 'Stop'
$RimWorldDir = (Resolve-Path -LiteralPath $RimWorldDir).Path
if (-not $HarmonyPath) {
    $HarmonyPath = Join-Path $RimWorldDir '../../workshop/content/294100/2009463077/1.7/Assemblies/0Harmony.dll'
}
$HarmonyPath = (Resolve-Path -LiteralPath $HarmonyPath).Path
if (-not (Test-Path -LiteralPath (Join-Path $RimWorldDir 'UnityStubs.dll'))) {
    throw 'RimWorldDir must point to a RimWorld 1.7 installation.'
}
if (-not $CompilerPath) {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
    if (Test-Path -LiteralPath $vswhere) {
        $CompilerPath = & $vswhere -latest -products '*' -find 'MSBuild\**\Bin\Roslyn\csc.exe' | Select-Object -First 1
    }
}
if (-not $CompilerPath -or -not (Test-Path -LiteralPath $CompilerPath)) {
    throw 'Install the Visual Studio C# build tools or supply -CompilerPath pointing to csc.exe. Alternatively build the csproj with the .NET 10 SDK.'
}

# Use the runtime assemblies shipped with the game; no SDK or NuGet restore needed.
$outputDir = Join-Path $PSScriptRoot '../Assemblies'
New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
$compilerArgs = @('/nologo', '/target:library', '/nostdlib+', '/optimize+', '/deterministic+',
    ('/out:' + (Join-Path $outputDir 'ElectricPodLauncher.dll')))
foreach ($dll in (Get-ChildItem -LiteralPath $RimWorldDir -Filter '*.dll' | Sort-Object Name)) {
    try {
        $null = [Reflection.AssemblyName]::GetAssemblyName($dll.FullName)
    } catch [System.BadImageFormatException] {
        continue # Skip native engine libraries.
    }
    $compilerArgs += '/reference:' + $dll.FullName
}
$compilerArgs += '/reference:' + $HarmonyPath
$compilerArgs += (Get-ChildItem -LiteralPath (Join-Path $PSScriptRoot 'ElectricPodLauncher/ElectricPodLauncher') -Filter '*.cs' -Recurse | Sort-Object FullName).FullName
& $CompilerPath @compilerArgs
if ($LASTEXITCODE -ne 0) { throw "C# compilation failed with exit code $LASTEXITCODE." }
Write-Output 'Built 1.7/Assemblies/ElectricPodLauncher.dll'
