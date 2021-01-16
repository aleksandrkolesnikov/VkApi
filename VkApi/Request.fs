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
        member self.AsyncGetResponse () =
            task {
                use! response = self.GetResponseAsync ()
                use stream = response.GetResponseStream ()
                use reader = new StreamReader (stream)
                return! reader.ReadToEndAsync ()
            }

    let private performGet (url: string) =
        let request = WebRequest.CreateHttp url
        request.AsyncGetResponse ()

    let private performPost url filename (source: byte array) =
        let boundary = Guid.NewGuid ()
        let body =
            let (|FileExtension|) filename = Path.GetExtension filename
            let contentType = match filename with
                                | FileExtension ".jpg"
                                | FileExtension ".jpeg" -> "image/jpeg"
                                | FileExtension ".png" -> "image/png"
                                | FileExtension ".gif" -> "image/gif"
                                | FileExtension ".pdf" -> "application/pdf"
                                | FileExtension ".zip" -> "application/zip"
                                | _ -> "application/octet-stream"

            let header =
                $"--{boundary}\r\nContent-Disposition: form-data; name=file; filename=\"{filename}\"\r\nContent-Type:{contentType}\r\n\r\n"
                |> Encoding.UTF8.GetBytes
            let footer = $"\r\n--{boundary}--\r\n" |> Encoding.UTF8.GetBytes

            task {
                let buffer = Array.zeroCreate<byte> (header.Length + source.Length + footer.Length)
                let stream = new MemoryStream (buffer)
                do! stream.WriteAsync (header, 0, header.Length)
                do! stream.WriteAsync (source, 0, source.Length)
                do! stream.WriteAsync (footer, 0, footer.Length)

                return buffer
            }

        task {
            let request = url |> string |> WebRequest.CreateHttp
            let! body = body
            request.Method <- "POST"
            request.ContentType <- $"multipart/form-data; boundary={boundary}"
            use stream = request.GetRequestStream ()
            do! stream.WriteAsync (body, 0, body.Length)

            return! request.AsyncGetResponse ()
        }

    let perform<'Response> request =
        task {
            let! response = match request with
                            | Get url -> url |> performGet
                            | Post (url, filename, source) -> performPost url filename source

            return match response |> Parser.parse<'Response> with
                    | Error error -> raise (new VkException (error.Error))
                    | Ok response -> response
        }
