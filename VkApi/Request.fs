namespace VkApi


type internal Request =
    | Get of Url: string
    | Post of Url: string * FileName: string * Source: byte array


[<RequireQualifiedAccess>]
module internal Request =
    
    open System
    open System.IO
    open System.Net
    open System.Text
    open FSharp.Control.Tasks.V2


    type HttpWebRequest with
        member self.GetResponseAsync<'Response> () =
            task {
                use! response = self.GetResponseAsync ()
                use stream = response.GetResponseStream ()
                use reader = new StreamReader (stream)
                let! text = reader.ReadToEndAsync ()

                return match text |> Parser.parse<'Response> with
                        | Ok response -> response
                        | Result.Error error ->
                            match error.Error.Code with
                            | 6 -> raise (new TooManyRequestsPerSecondException (error.Error))
                            | 1151 -> raise (new AccessDeniedException (error.Error))       // this code does not mention in https://vk.com/dev/errors
                            | _ -> raise (new Exception ("Something went wrong"))
            }

        member self.WriteContentAsync content =
            task {
                use! stream = self.GetRequestStreamAsync ()
                do! stream.WriteAsync (content, 0, content.Length)
            }

    let private performGet<'Response> (url: string) =
        let request = WebRequest.CreateHttp url
        request.GetResponseAsync<'Response> ()

    let private performPost<'Response> (url: string) filename (source: byte array) =
        let boundary = Guid.NewGuid ()
        let body =
            let (|FileExtension|) (filename: string) = Path.GetExtension filename
            let contentType = match filename with
                                | FileExtension ".jpg"
                                | FileExtension ".jpeg" -> "image/jpeg"
                                | FileExtension ".png" -> "image/png"
                                | FileExtension ".gif" -> "image/gif"
                                | FileExtension ".pdf" -> "application/pdf"
                                | FileExtension ".zip" -> "application/zip"
                                | FileExtension ".txt" -> "text/plain"
                                | _ -> "application/octet-stream"

            let header =
                $"--{boundary}\r\nContent-Disposition: form-data; name=file; filename=\"{filename}\"\r\nContent-Type:{contentType}\r\n\r\n"
                |> Encoding.UTF8.GetBytes
            let footer = $"\r\n--{boundary}--\r\n" |> Encoding.UTF8.GetBytes

            seq { header; source; footer } |> Array.concat

        task {
            let request = url |> WebRequest.CreateHttp
            request.Method <- "POST"
            request.ContentType <- $"multipart/form-data; boundary={boundary}"
            do! request.WriteContentAsync body

            return! request.GetResponseAsync<'Response> ()
        }

    let perform<'Response> request =
        task {
            let! response = match request with
                            | Get url -> url |> performGet<'Response>
                            | Post (url, filename, source) -> performPost<'Response> url filename source

            return response
        }
