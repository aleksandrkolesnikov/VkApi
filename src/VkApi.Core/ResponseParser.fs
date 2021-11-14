namespace VkApi.Core


[<RequireQualifiedAccess>]
module internal ResponseParser =

    open Newtonsoft.Json


    let Parse<'Response> (text: string) =
        let ToError = JsonConvert.DeserializeObject<ErrorWrapper> >> Error
        let ToResponse = JsonConvert.DeserializeObject<'Response> >> Ok

        if text.Contains "error" then
            text |> ToError
        else
            text |> ToResponse
