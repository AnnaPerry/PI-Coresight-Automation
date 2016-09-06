param([string]$nugetSourceUrl, [string]$nugetSourceApiKey, [string]$versionSuffix = "", [string]$solutionDir = "")

##
## A script to package the solution's three libraries as NuGet packages.
##
## IMPORTANT: before running this script, update all references to the current version number in this script and in each project.json. The messy reality with current netcore tooling.
##

$versionRoot = "1.0.2" # Also specified in every project.json. Update all instances before running this script.
$version = If ($versionSuffix -ne "") { "{0}-{1}" -f $versionRoot, $versionSuffix } else { $versionRoot } 

# If solution directory is not specified, default to paths relative to the script's location within the repository
$scriptDir = If ($solutionDir -eq "") { Split-Path -Parent $MyInvocation.MyCommand.Path } else { Join-Path -Path $solutionDir -ChildPath \CoresightAutomation.NuGetPublish }
if (!(Test-Path $scriptDir)) { throw [System.IO.FileNotFoundException] "$scriptDir is not a valid directory." }
$solutionDir = If ($solutionDir -eq "") { Split-Path -Parent $scriptDir } else { $solutionDir }

# Ensure that dotnet core is installed and in the path
$nugetExeName = 'dotnet'
try {Get-Command dotnet} catch [CommandNotFoundException] { throw [InvalidOperationException]  "dotnet is not in the PATH" }

# Ensure that dotnet core is installed and in the path
$nugetExeName = 'nuget'
try {Get-Command nuget} catch [CommandNotFoundException] { throw [InvalidOperationException]  "nuget is not in the PATH" }

# Create the output directory if it doesn't already exist
$outputDir  = Join-Path -Path $solutionDir -ChildPath \CoresightAutomation.NuGetPublish\pack
New-Item -ItemType Directory -Force -Path $outputDir

# Clear any existing nupkgs in the output directory
Join-Path -Path $outputDir -ChildPath "*.nupkg" | Remove-Item

# Package multiple projects
$xProjectFolders = "CoresightAutomation", "CoresightAutomation.PIWebAPI"
$projectFolders | ForEach-Object {
	$projectFile = Join-Path $solutionDir -ChildPath $_ | Join-Path -ChildPath "project.json"
	
	If ($versionSuffixParameter -ne "")
	{
		dotnet pack --output $outputDir --version-suffix `"$versionSuffix`" $projectFile
	}
	else
	{
		dotnet pack --output $outputDir $projectFile
	}
}

$csProjectFolders = "CoresightAutomation.AFSDK"
$csProjectFolders | ForEach-Object {
	$nuspecFilename = '{0}.nuspec' -f $_
	$nuspecFile = Join-Path $solutionDir -ChildPath $_ | Join-Path -ChildPath $nuspecFilename
	nuget pack $nuspecFile -OutputDirectory $outputDir -Version $version
}

# Push to source
Join-Path $outputDir -ChildPath "*.nupkg" | Get-ChildItem -Exclude *.symbols.nupkg | Foreach-Object {
	$filename = $_.FullName
	& nuget push "$filename" $nugetSourceApiKey -Source $nugetSourceUrl
}