namespace VkApi.Core


[<RequireQualifiedAccess>]
module internal ResponseConverter =

    open System.IO
    open Newtonsoft.Json
    open FSharp.Control.Tasks.V2


    let private ParseAsync<'Response> (stream: Stream) =
        task {
            use reader = new StreamReader (stream)
            let! text =
                ()
                |> reader.ReadToEndAsync

            // TODO: replace with match and active pattern
            let r = if text.Contains "error" then
                        Error <| JsonConvert.DeserializeObject<ErrorWrapper> text
                    else
                        Ok <| JsonConvert.DeserializeObject<'Response> text

            return r
        }

    let ConvertAsync<'Response> stream =
        task {
            let! response = ParseAsync<'Response> stream
            return match response with
                    | Error error -> failwith error.Error.Message
                    | Ok response -> response
        }
