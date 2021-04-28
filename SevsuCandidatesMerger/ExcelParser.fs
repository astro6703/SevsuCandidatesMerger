module SevsuCandidatesMerger.ExcelParser

open OfficeOpenXml
open SevsuCandidatesMerger.Common
open System
open System.IO

let private nameColumnIndex = 2
let private scoreColumnIndex = 3
let private isPrivilegedColumnIndex = 9

let private iterateThroughRowsUntil (exitCondition: string -> bool)
                                    (startRow: int)
                                    (worksheet: ExcelWorksheet)
                                    : int =
    let mutable row = startRow
    let mutable rowText = worksheet.Cells.[row, 1].Text
    while not <| exitCondition rowText do
        row <- row + 1
        rowText <- worksheet.Cells.[row, 1].Text

    row - 1

let private getTableFirstRow (tableCourseNameRow: int)
                             (worksheet: ExcelWorksheet)
                             : int =
    iterateThroughRowsUntil (fun x -> x = "2") tableCourseNameRow worksheet

let private getTableLastRow (tableStartRow: int)
                            (worksheet: ExcelWorksheet)
                            : int =
    iterateThroughRowsUntil (fun x -> x = "Списки поступающих" || x = String.Empty) tableStartRow worksheet

let private readCandidatesTable (tableFirstRow: int)
                                (tableLastRow: int)
                                (worksheet: ExcelWorksheet)
                                : Candidate list =
    [tableFirstRow .. tableLastRow]
    |> List.map (fun row ->
        let getCellTextByIndex (columnIndex: int): string =
            worksheet.Cells.[row, columnIndex].Text

        let candidateName = getCellTextByIndex nameColumnIndex
        let isCandidatePrivileged = getCellTextByIndex isPrivilegedColumnIndex <> String.Empty
        let candidateScore = int (getCellTextByIndex scoreColumnIndex)

        { Name = candidateName; Score = candidateScore; IsPrivileged = isCandidatePrivileged }
    )

let getCandidatesByCourse (excelFile: FileInfo)
                          (courses: Course list)
                          : Map<Course, Candidate list> =
    ExcelPackage.LicenseContext <- Nullable<LicenseContext> LicenseContext.NonCommercial
    use package = new ExcelPackage(excelFile)
    let worksheet = package.Workbook.Worksheets.[0]
    let fileStart = worksheet.Dimension.Start
    let fileEnd = worksheet.Dimension.End
    let courseNames = courses |> List.map (fun x -> x.Name)
    let mutable candidatesByCourse = Map.empty

    [ fileStart.Row .. fileEnd.Row ]
    |> List.iter (fun row ->
        let cellText = worksheet.Cells.[row, 1].Text

        if courseNames |> List.exists cellText.Contains then
            let matchedCourse = courses |> List.filter (fun x -> cellText.Contains x.Name) |> List.exactlyOne
            let tableFirstRow = getTableFirstRow row worksheet
            let tableLastRow = getTableLastRow tableFirstRow worksheet
            let matchedCourseCandidates = readCandidatesTable tableFirstRow tableLastRow worksheet

            candidatesByCourse <- candidatesByCourse.Add(matchedCourse, matchedCourseCandidates)
    )

    candidatesByCourse