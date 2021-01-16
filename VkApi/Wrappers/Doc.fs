namespace VkApi.Wrappers

open Newtonsoft.Json


[<NoEquality; NoComparison>]
type internal Doc<'T> =
    struct
        val Document: 'T

        [<JsonConstructor>]
        new doc = { Document = doc }
    end