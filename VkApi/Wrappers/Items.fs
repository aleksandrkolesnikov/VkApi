namespace VkApi.Wrappers

open Newtonsoft.Json


[<NoEquality; NoComparison>]
type internal Items<'T> =
    struct
        val Items: seq<'T>

        [<JsonConstructor>]
        new items = { Items = items }
    end