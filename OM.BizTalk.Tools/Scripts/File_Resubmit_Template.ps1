# File_Resubmit_Template.ps1
# !!!PLEASE MAKE A COPY DO NOT MODIFY THIS COPY!!!
# Used to pace file resubmission

# Set these ...
$source = ""             #1 Specifiy path to Source files (i.e. C:\MyFolder\MyFailedFiles)
$writeTo = ""            #2 Specifiy the folderthat files will be MOVED to (i.e. C:\MyFolder\In)
$filter = "*"            #3 Specify file pattern (i.e. *.txt) 
$amountUntilDelay = 50   #4 Specify how many files get moved at a time 
$delayInSeconds = 5      #5 Specify a wait time in seconds until next group of files is moved          
######################################################

$amountCopied = 0
$totalCopied = 0

$allFiles = Get-ChildItem -Path $source -Filter $filter   

foreach ($f in $allFiles)
{
    Move-Item  $f.FullName -Destination $writeTo

    $amountCopied++
    $totalCopied++

    if($amountCopied -eq $amountUntilDelay)
    {
        $d = Get-Date
        Write-Host "$totalCopied copied as of $d."
        Start-Sleep -s $delayInSeconds
        $amountCopied = 0
    }
}

$d = Get-Date
Write-Host "Final: $totalCopied copied as of $d."