module SevsuCandidatesMerger.Utils

let getValues (map: Map<'a, 'b>): 'b list =
    map
    |> Map.toList
    |> List.map (fun x -> snd x)

let collectMapValuesLists (map: Map<'a, 'b list>) =
    map
    |> getValues
    |> List.collect (fun x -> x)