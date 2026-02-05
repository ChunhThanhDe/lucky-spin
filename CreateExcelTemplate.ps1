# Script PowerShell để tạo file Excel mẫu từ CSV
$csvPath = "e:\Lucky Spin\Lucky Spin\LuckySpin\LuckySpin\bin\Lucky Wheel\Config\INPUT_TEMPLATE.csv"
$excelPath = "e:\Lucky Spin\Lucky Spin\LuckySpin\LuckySpin\bin\Lucky Wheel\Config\INPUT.xlsx"

# Tạo Excel Application
$excel = New-Object -ComObject Excel.Application
$excel.Visible = $false
$excel.DisplayAlerts = $false

# Mở file CSV
$workbook = $excel.Workbooks.Open($csvPath)

# Save as Excel format
$workbook.SaveAs($excelPath, 51) # 51 = xlOpenXMLWorkbook (.xlsx)

# Đóng và giải phóng
$workbook.Close()
$excel.Quit()

# Giải phóng COM objects
[System.Runtime.Interopservices.Marshal]::ReleaseComObject($workbook) | Out-Null
[System.Runtime.Interopservices.Marshal]::ReleaseComObject($excel) | Out-Null
[System.GC]::Collect()
[System.GC]::WaitForPendingFinalizers()

Write-Host "File Excel mẫu đã được tạo tại: $excelPath"
