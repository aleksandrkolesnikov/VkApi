namespace VkApi.Core

open Newtonsoft.Json


[<Struct; NoEquality; NoComparison>]
type internal Items<'T> [<JsonConstructor>] (items: seq<'T>) =
    member _.Items = items
