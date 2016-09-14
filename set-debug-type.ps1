copy 'src\Invio.Extensions.Reflection\project.json' 'src\Invio.Extensions.Reflection\project.json.bak'
$project = Get-Content 'src\Invio.Extensions.Reflection\project.json.bak' -raw | ConvertFrom-Json
$project.buildOptions.debugType = "full"
$project | ConvertTo-Json  | set-content 'src\Invio.Extensions.Reflection\project.json'
