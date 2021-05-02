$env:OSP_IDENTITYSERVICS_LOGDIR = "../../../../logs/identityservices/"
$env:OSP_CORESERVICS_LOGDIR = "../../../../logs/coreservices/"
$env:OSP_JOBSERVICS_LOGDIR = "../../../../logs/jobservices/"

$basedir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$logDir = "logFiles"
$jobs = New-Object System.Collections.ArrayList
function Start-Service($workingDirectory, $cmd, $logname, $cmdArguments, $jobName, $aspnetEnvironment = "Development")
{
    Write-Host "Starting $($jobName)"
    $arguments = @([System.IO.Path]::Combine($basedir, $workingDirectory), [System.IO.Path]::Combine($basedir, $logDir, $logname), $aspnetEnvironment)
    $arguments += $cmdArguments
    $job = Start-Job -ScriptBlock { Set-Location $args[0]; $env:ASPNETCORE_ENVIRONMENT = $args[2]; $localArgs = $args | Select-Object -Skip 3; & "$input" $localArgs 2>&1 >> $args[1] } -InputObject $cmd -ArgumentList $arguments -Name $jobName
    $jobs.Add($job) | OUT-NULL;
}

function Get-ServiceStatus()
{
    foreach ($job in $jobs) {
        Write-Host "Status job $($job.Name): $($job.State)"
        
    }    
}

function Delete-LogFile([string]$file)
{
    $file = [System.IO.Path]::Combine($basedir, $logDir, $file)
    if (Test-Path $file)
    {
        Remove-Item -Force $file
    }  
}

function Create-LogDirectory()
{
    $path = [System.IO.Path]::Combine($basedir, $logDir);
    If(!(test-path $path))
    {
        New-Item -ItemType Directory -Force -Path $path | Out-Null
    }
}

Create-LogDirectory
Delete-LogFile -file "redis-server.log"
Delete-LogFile -file "Identity.log"
Delete-LogFile -file "Policy.log"
Delete-LogFile -file "CoreServices.log"
Delete-LogFile -file "JobServices.log"
Delete-LogFile -file "Dashboard.log"

dotnet publish Osp\Backend\Ianitor.Osp.Backend.Identity\
dotnet publish Osp\Backend\Ianitor.Osp.Backend.Policy\
dotnet publish Osp\Backend\Ianitor.Osp.Backend.CoreServices\
dotnet publish Osp\Backend\Ianitor.Osp.Backend.JobServices\
#dotnet publish Osp\Backend\Ianitor.Osp.Backend.Dashboard\

Start-Service -cmd "redis-server" -logname "redis-server.log" -jobName "redis-server"
Start-Service -workingDirectory "bin/Debug/Identity/netcoreapp3.1/publish/" -cmd "dotnet" -logname "Identity.log" -cmdArguments @("Ianitor.Osp.Backend.Identity.dll", "--urls=https://*:5003/") -jobName "Identity"
Start-Service -workingDirectory "bin/Debug/Policy/netcoreapp3.1/publish/" -cmd "dotnet" -logname "Policy.log" -cmdArguments @("Ianitor.Osp.Backend.Policy.dll", "--urls=https://*:5011/") -jobName "Policy"
Start-Service -workingDirectory "bin/Debug/CoreServices/netcoreapp3.1/publish/" -cmd "dotnet" -logname "CoreServices.log" -cmdArguments @("Ianitor.Osp.Backend.CoreServices.dll", "--urls=""http://localhost:5000;https://localhost:5001""") -jobName "CoreServices"
Start-Service -workingDirectory "bin/Debug/JobServices/netcoreapp3.1/publish/" -cmd "dotnet" -logname "JobServices.log" -cmdArguments @("Ianitor.Osp.Backend.JobServices.dll", "--urls=https://localhost:5009") -jobName "JobServices"
Start-Service -workingDirectory "bin/Debug/Dashboard/netcoreapp3.1/publish/" -cmd "dotnet" -logname "Dashboard.log" -cmdArguments @("Ianitor.Osp.Backend.Dashboard.dll", "--urls=https://localhost:5005") -jobName "Dashboard" -aspnetEnvironment "Staging"


Get-ServiceStatus

Write-Host "Started. Press key to exit"

$wait = $true;
do
{
    # wait for a key to be available:
    if ([Console]::KeyAvailable)
    {
        # read the key, and consume it so it won't
        # be echoed to the console:
        [Console]::ReadKey($true) | Out-Null
        # exit loop
        break
    }
    
    foreach ($job in $jobs) {
        if ($job.State -ne "Running")
        {
            Write-Warning "Service $($job.Name) is in status $($job.State)"
            Receive-Job $job | Write-Output
            $wait = $false
            break;
        }
    }

    Start-Sleep -Seconds 2

} while ($wait)


Write-Host "Exiting jobs"
foreach ($job in $jobs) {
    Write-Host "Stopping job $($job.Name)"
    $job.StopJob()
}
Wait-Job $jobs | OUT-NULL
Write-Host "Jobs stopped"

Get-ServiceStatus

