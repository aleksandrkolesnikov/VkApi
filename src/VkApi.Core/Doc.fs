namespace VkApi.Core

open Newtonsoft.Json


[<Struct; NoEquality; NoComparison>]
type internal Doc [<JsonConstructor>] (doc: Document) =
    member _.Document = doc
