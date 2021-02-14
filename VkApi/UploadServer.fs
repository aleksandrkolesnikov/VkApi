namespace VkApi

open Newtonsoft.Json


[<Struct; NoEquality; NoComparison>]
type internal UploadServer [<JsonConstructor>] (upload_url: string) =
    member _.Url = upload_url