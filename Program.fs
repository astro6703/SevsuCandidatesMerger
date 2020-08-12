module SevsuCandidatesMerger.Program

open System
open System.IO
open System.Text
open SevsuCandidatesMerger.Common

let private takeCourseCandidates (allCandidates: Candidate list)
                                 (takenCandidates: Candidate list)
                                 (threshold: int)
                                 : Candidate list =
    allCandidates
    |> List.except takenCandidates
    |> List.sortByDescending (fun x -> x.IsPrivileged, x.Score)
    |> List.take threshold

let private writeCourseToConsole (course: Course) (candidates: Candidate list): unit =
    printfn "%s:" course.Name

    candidates
    |> List.indexed
    |> List.iter (fun x ->
        let index, candidate = x

        printfn "%3i %50s %3i"
            (index + 1) candidate.Name candidate.Score
    )

let main: int =
    let courses = [
        { Name = "Информационные системы и технологии"; Priority = 1; Capacity = 49; CsvName = "is.csv" }
        { Name = "Прикладная информатика"; Priority = 2; Capacity = 22; CsvName = "pi.csv" }
        { Name = "Информатика и вычислительная техника"; Capacity = 49; Priority = 3; CsvName = "m.csv" }
        { Name = "Программная инженерия"; Priority = 4; Capacity = 22; CsvName = "progi.csv" }
        { Name = "Управление в технических системах"; Capacity = 40; Priority = 5; CsvName = "a.csv" }
    ]
    let excelFilePath = System.Environment.GetCommandLineArgs() |> Array.skip(1) |> Array.exactlyOne
    let excelFile = FileInfo excelFilePath
    let courses = Config.courses
    let allCandidatesByCourse = ExcelParser.getCandidatesByCourse excelFile courses

    let mutable takenCandidates = []
    let allCandidatesByCourse =
        courses
        |> List.map (fun x -> x, getCandidatesByCourse(x))
        |> Map.ofList

    let uniqueCandidatesByCourse =
        courses
        |> List.sortBy (fun x -> x.Priority)
        |> List.map (fun x ->
            let candidatesToTake = takeCourseCandidates allCandidatesByCourse.[x] takenCandidates x.Capacity
            takenCandidates <- takenCandidates @ candidatesToTake

            x, candidatesToTake
        )
        |> Map.ofList

    Console.OutputEncoding <- Encoding.UTF8
    courses
    |> List.iter (fun x ->
        printfn "%s" Environment.NewLine
        writeCourseToConsole x uniqueCandidatesByCourse.[x]
    )

    0
