namespace VkApi


module internal Parser =

    open Newtonsoft.Json


    let parse<'Response> (json: string) =
        if json.Contains "error" then
            Result.Error <| JsonConvert.DeserializeObject<Error> json
        else
            Ok <| JsonConvert.DeserializeObject<'Response> json