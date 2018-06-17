function CreateList($ListProps) {
  $NewList = Get-PnPList -Identity $ListProps.Title -Includes Fields
  if ($NewList -eq $null) {
    Write-host Creating list $ListProps.Title
    $NewList = New-PnPList -Title $ListProps.Title -Template GenericList -Url "Lists/$($ListProps.Name)"
    $NewList = Get-PnPList -Identity $ListProps.Name -Includes Fields
  }
  return $NewList
}

function CreateField($NewFieldProps) {
  $NewField = ($NewFieldProps.List.Fields) | Where-Object {$_.InternalName -eq $NewFieldProps.Name}
  if ($NewField -eq $null) {
    Write-host Adding field $NewFieldProps.Name
    if ($NewFieldProps.Type -eq "Choice") {
      $NewField = Add-PnPField -List $ListProps.Title -DisplayName $NewFieldProps.Title -InternalName $NewFieldProps.Name -Type $NewFieldProps.Type -Choices $NewFieldProps.Choices -AddToDefaultView
    }
    else {
      $NewField = Add-PnPField -List $ListProps.Title -DisplayName $NewFieldProps.Title -InternalName $NewFieldProps.Name -Type $NewFieldProps.Type -AddToDefaultView
    }

    $NewField = ($NewFieldProps.List.Fields) | Where-Object {$_.InternalName -eq $NewFieldProps.Name}
  }
}

$ListProps = @{Name = "Courses"; Title = "Courses"}
$NewList = CreateList $ListProps
CreateField @{List = $NewList; Name = "Description"; Title = "Description"; Type = "Note"}
CreateField @{List = $NewList; Name = "CourseType"; Title = "Course Type"; Type = "Choice"; Choices = "Webinar", "Seminar", "Other"}
CreateField @{List = $NewList; Name = "Price"; Title = "Price"; Type = "Text"; }
CreateField @{List = $NewList; Name = "Level"; Title = "Level"; Type = "Text"; }
CreateField @{List = $NewList; Name = "Active"; Title = "Active"; Type = "Boolean"; }