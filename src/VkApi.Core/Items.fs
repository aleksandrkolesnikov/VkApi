namespace VkApi.Core

open Newtonsoft.Json


[<NoEquality; NoComparison>]
type internal Items<'T> [<JsonConstructor>] (items: seq<'T>) =
    member _.Items = items
