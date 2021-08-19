namespace VkApi.Core

open Newtonsoft.Json


[<NoEquality; NoComparison>]
type internal Response<'T> [<JsonConstructor>] (response: 'T) =
    member _.Response = response
