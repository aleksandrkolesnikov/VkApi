namespace VkApi.Core


[<RequireQualifiedAccess>]
module internal ResponseParser =

    open Newtonsoft.Json


    let private Parse<'Response> (text: string) =
        let ToError = JsonConvert.DeserializeObject<ErrorWrapper> >> Error
        let ToResponse = JsonConvert.DeserializeObject<'Response> >> Ok

        if text.Contains "error" then
            text |> ToError
        else
            text |> ToResponse


    let TryParse<'Response> text =
        match Parse<'Response> text with
        | Error error -> failwith "OK"
        | Ok response -> response
