namespace VkApi.Core

open Newtonsoft.Json


[<Struct; NoEquality; NoComparison>]
type internal Response<'T> [<JsonConstructor>] (response: 'T) =
    member _.Response = response
