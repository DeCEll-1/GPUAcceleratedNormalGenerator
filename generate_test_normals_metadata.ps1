# Define base relative directories
$origDir = "out/original"
$manifestPath = "manifest.psv"

# Initialize/Clear the manifest file
Clear-Content -Path $manifestPath

Write-Host "Scanning '$origDir' for .png files (including subdirectories)..."

if (Test-Path $origDir) {
    # Find all .png files recursively in the original directory
    $pngFiles = Get-ChildItem -Path $origDir -Filter "*.png" -File -Recurse
    $absoluteOrigDir = (Resolve-Path $origDir).Path
    
    # Get absolute base paths for the other output directories
    $absHeightBase = Join-Path $PWD "out/heightmap"
    $absBlurBase = Join-Path $PWD "out/blurred"
    $absNormalBase = Join-Path $PWD "out/normalmap"
    
    $count = 0
    foreach ($file in $pngFiles) {
        # Use C# .NET Path.GetRelativePath method to get the exact relative subpath structure
        $subPath = [System.IO.Path]::GetRelativePath($absoluteOrigDir, $file.FullName)
        
        # Construct absolute paths for all four columns, respecting subfolders
        $absOriginal = $file.FullName
        $absHeight = Join-Path $absHeightBase $subPath
        $absBlur = Join-Path $absBlurBase $subPath
        $absNormal = Join-Path $absNormalBase $subPath
        
        # Write the pipe-separated absolute paths line to the manifest file
        "$absOriginal|$absHeight|$absBlur|$absNormal" | Out-File -FilePath $manifestPath -Append -Encoding utf8
        
        $count++
        if ($count % 500 -eq 0) {
            Write-Host "Processed $count files..."
        }
    }
    
    Write-Host "Done! Manifest successfully created with $count absolute entries at '$manifestPath'."
} else {
    Write-Host "Error: The directory '$origDir' does not exist." -ForegroundColor Red
}