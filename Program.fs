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

let private writeCourseToConsole (course: Course)
                                 (candidates: Candidate list)
                                 : unit =
    printfn "%s%s:" Environment.NewLine course.Name
    printfn "%3s%50s%8s%15s" "№" "ФИО" "Баллы" "Особое право"

    candidates
    |> List.indexed
    |> List.iter (fun x ->
        let index, candidate = x

        let isPrivilegedClause = (if candidate.IsPrivileged then "*".PadRight(6) else String.Empty)

        printfn "%3i%50s%8i%15s"
            (index + 1) candidate.Name candidate.Score isPrivilegedClause
    )

let main: int =
    let excelFilePath = System.Environment.GetCommandLineArgs() |> Array.skip(1) |> Array.exactlyOne
    let excelFile = FileInfo excelFilePath
    let courses = Config.courses
    let allCandidatesByCourse = ExcelParser.getCandidatesByCourse excelFile courses

    let mutable takenCandidates = []

    let uniqueCandidatesByCourse =
        courses
        |> List.sortBy (fun x -> x.Priority)
        |> List.map (fun x ->
            let candidatesToTake = takeCourseCandidates allCandidatesByCourse.[x] takenCandidates x.Capacity
            takenCandidates <- takenCandidates @ candidatesToTake

            x, candidatesToTake
        )
        |> Map.ofList

    let notTakenCandidates =
        allCandidatesByCourse
        |> Utils.collectMapValuesLists
        |> List.except (uniqueCandidatesByCourse |> Utils.collectMapValuesLists)
        |> List.sortByDescending (fun x -> x.Score)

    Console.OutputEncoding <- Encoding.UTF8
    courses
    |> List.iter (fun course ->
        writeCourseToConsole course uniqueCandidatesByCourse.[course]
    )
    writeCourseToConsole { Name = "Не поступившие"; Priority = 100; Capacity = 10000; } notTakenCandidates

    0
