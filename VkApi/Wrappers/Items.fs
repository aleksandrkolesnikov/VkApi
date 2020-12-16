namespace VkApi.Wrappers

open Newtonsoft.Json
open System.Collections.Generic


[<NoComparison>]
type internal Items<'T> =
    struct
        val Items: IEnumerable<'T>

        [<JsonConstructor>]
        new items = { Items = items }
    end