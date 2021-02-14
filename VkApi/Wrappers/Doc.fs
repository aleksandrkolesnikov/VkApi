namespace VkApi.Wrappers

open Newtonsoft.Json
open VkApi


[<Struct; NoEquality; NoComparison>]
type internal Doc [<JsonConstructor>] (doc: Document) =
    member _.Document = doc