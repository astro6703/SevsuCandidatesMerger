module SevsuCandidatesMerger.ExcelParser

open System
open System.IO
open OfficeOpenXml
open SevsuCandidatesMerger.Common

let private nameColumnIndex = 2
let private scoreColumnIndex = 3
let private isPrivilegedColumnIndex = 9

let private courseNameToTableFirstRowOffset = 5;

let private getTableLastRow (worksheet: ExcelWorksheet)
                            (tableStartRow: int)
                            : int =
    let mutable row = tableStartRow
    let mutable rowText = worksheet.Cells.[row, 1].Text
    while rowText <> "Списки поступающих" || rowText = String.Empty do
        row <- row + 1
        rowText <- worksheet.Cells.[row, 1].Text

    row - 1

let private readCandidatesTable (worksheet: ExcelWorksheet)
                                (tableStartRow: int)
                                : Candidate list =
    let tableLastRow = getTableLastRow worksheet tableStartRow

    [tableStartRow .. tableLastRow]
    |> List.map (fun row ->
        let getCellTextByIndex (columnIndex: int): string =
            worksheet.Cells.[row, columnIndex].Text

        let candidateName = getCellTextByIndex nameColumnIndex
        let isCandidatePrivileged = getCellTextByIndex isPrivilegedColumnIndex <> String.Empty
        let candidateScore = (int) (getCellTextByIndex scoreColumnIndex)

        { Name = candidateName; Score = candidateScore; IsPrivileged = isCandidatePrivileged }
    )

let getCandidatesByCourse (excelFile: FileInfo)
                          (courses: Course list)
                          : Map<Course, Candidate list> =
    ExcelPackage.LicenseContext <- (Nullable<LicenseContext>) LicenseContext.NonCommercial
    use package = new ExcelPackage(excelFile)
    let worksheet = package.Workbook.Worksheets.[0]
    let fileStart = worksheet.Dimension.Start
    let fileEnd = worksheet.Dimension.End
    let courseNames = courses |> List.map (fun x -> x.Name)
    let mutable candidatesByCourse = Map.empty

    [ fileStart.Row .. fileEnd.Row ]
    |> List.iter (fun row ->
        let cellText = worksheet.Cells.[row, 1].Text

        if courseNames |> List.exists (fun name -> cellText.Contains name) then
            let matchedCourse = courses |> List.filter (fun x -> cellText.Contains x.Name) |> List.exactlyOne
            let matchedCourseCandidates = readCandidatesTable worksheet (row + courseNameToTableFirstRowOffset)

            candidatesByCourse <- candidatesByCourse.Add(matchedCourse, matchedCourseCandidates)
    )

    candidatesByCourse