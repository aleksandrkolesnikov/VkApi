namespace VkApi

open Newtonsoft.Json


[<NoEquality; NoComparison>]
type internal UploadServer =
    struct
        val Url: string

        [<JsonConstructor>]
        new upload_url = { Url = upload_url }
    end