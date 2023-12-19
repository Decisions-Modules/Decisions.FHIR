# The purpose of this script is to build the Decisions Module without having the Decisions Code Base checked out.
# It has an optional parameter which when set to true will deploy the module to your local Decisions instance and restart the service.

#Requires -RunAsAdministrator

param 
(
    [Parameter(Mandatory=$false)]
	[switch] $deployModuleLocally
)

function FindModuleName($buildProj)
{
    [xml]$local:XmlDocument = Get-Content -Path $buildProj

    foreach($local:target in $local:XmlDocument.Project.Target){
        if($local:target.Name -eq "build_module")
        {
            $local:cmdline = $local:target.Exec.Command
            break
        }
    }

    $local:cmdline = $local:cmdline -split ' '
    $local:index = [array]::indexof($local:cmdline,"-buildmodule")
    $local:index++

    return $local:cmdline[$local:index]
}

function GetCompileTarget($basePath) {
    $guess = Join-Path -Path $basePath -ChildPath "build.proj"
    if (Test-Path -PathType leaf -LiteralPath $guess ) {
        return $guess
    }
    throw "Could not find a build.proj file, please create one."
}

function StopDecisionsServer {
    $local:service = (get-service "DecisionsServer");

    if ($local:service.status -eq "Running") {
        $local:service.Stop()
    }

    Write-Output "stopping Decisions Server..."
    do { $local:service.refresh(); sleep 1; } until ($local:service.status -eq "Stopped")
}

function CopyModule($basePath, $moduleName)
{
    $local:fullModuleName = "$basePath\$moduleName.zip"
    $local:destination  = "C:\Program Files\Decisions\Decisions Server\CustomModules\$local:moduleName.zip"

    Write-Output "Copying module..."
    Copy-Item $local:fullModuleName $local:destination
}

function StartDecisionsServer {
    $local:service = (get-service "DecisionsServer");
    Write-Output "starting Decisions Server..."
    $local:service.Start()

    do { $local:service.refresh(); sleep 1; } until ($local:service.status -eq "Running")
    Write-Output "SHM Started."
}

$basePath = (Get-Location).Path
Write-Output "Using basePath - $basePath"

$moduleName = FindModuleName("$basePath\build.proj")

# Build the Module.
Write-Output "Building $moduleName"

Write-Output "Compiling Project by build.proj, or by .sln file."
$compileTarget = GetCompileTarget($basepath)

Write-Output "Found Compile Target - $compileTarget"

dotnet build $compileTarget

# If the deployModuleLocally switch is present, do the needful
if($deployModuleLocally.IsPresent) 
{
	Write-Output "Deploying $moduleName to the Local Decisions Instance"
	StopDecisionsServer
	CopyModule $basePath $moduleName
	StartDecisionsServer
}