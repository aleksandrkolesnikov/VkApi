namespace VkApi.Core

open Newtonsoft.Json


[<NoEquality; NoComparison>]
type internal Doc [<JsonConstructor>] (doc: Document) =
    member _.Document = doc
