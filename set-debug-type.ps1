$projectName = "Invio.Extensions.Reflection"

copy "src\${projectName}\${projectName}.csproj" "src\${projectName}\${projectName}.csproj.bak"

$project = New-Object XML
$project.Load("${pwd}\src\${projectName}\${projectName}.csproj")
$project.Project.PropertyGroup.DebugType = "full"
$project.Save("${pwd}\src\${projectName}\${projectName}.csproj")
