namespace VkApi

open Newtonsoft.Json


type internal Doc<'T> =
    struct
        val Document: 'T

        [<JsonConstructor>]
        new doc = { Document = doc }
    end