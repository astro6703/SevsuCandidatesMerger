module SevsuCandidatesMerger.Utils

let getValues (map: Map<'a, 'b>): 'b list =
    map
    |> Map.toList
    |> List.map snd

let collectMapValuesLists (map: Map<'a, 'b list>) =
    map
    |> getValues
    |> List.collect id