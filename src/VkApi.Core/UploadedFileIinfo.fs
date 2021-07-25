namespace VkApi.Core

open System
open Newtonsoft.Json


[<Struct; NoEquality; NoComparison>]
type internal UploadedFileInfo [<JsonConstructor>] (file: string) =
    member _.Info = file
    member _.Title = (file.Split ('|')).[7]
    member _.Hash = (file.Split ('|')).[8] |> Convert.FromHexString
