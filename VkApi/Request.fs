namespace VkApi


[<RequireQualifiedAccess>]
module internal Request =
    
    open System
    open System.Net
    open System.IO
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


    let performGet (url: string) =
        let request = WebRequest.CreateHttp url
        request.AsyncGetResponse ()

    let performPost url filename =
        let boundary = Guid.NewGuid ()
        let body =
            let header = $"--{boundary}\r\nContent-Disposition: form-data; name=file; filename=\"{filename}\"\r\nContent-Type:application/octet-stream\r\n\r\n" |> Encoding.UTF8.GetBytes
            let footer = $"\r\n--{boundary}--\r\n" |> Encoding.UTF8.GetBytes
            let data =
                task {
                    use stream = File.OpenRead filename
                    let buffer = Array.zeroCreate<byte> (stream.Length |> int)
                    let! _ = stream.ReadAsync (buffer, 0, buffer.Length)

                    return buffer
                }

            task {
                let! data = data
                let buffer = Array.zeroCreate<byte> (header.Length + data.Length + footer.Length)
                let stream = new MemoryStream (buffer)
                do! stream.WriteAsync (header, 0, header.Length)
                do! stream.WriteAsync (data, 0, data.Length)
                do! stream.WriteAsync (footer, 0, footer.Length)

                return buffer
            }

        task {
            let request = url |> string |> WebRequest.CreateHttp
            let! d = body
            request.Method <- "POST"
            request.ContentType <- $"multipart/form-data; boundary={boundary}"
            use stream = request.GetRequestStream ()
            do! stream.WriteAsync (d, 0, d.Length)

            return! request.AsyncGetResponse ()
        }


    let perform<'Response> request =
        task {
            let! response = match request with
                            | Get url -> url |> performGet
                            | Post (url, filename) -> performPost url filename

            return match response |> Parser.parse<'Response> with
                    | Error error -> raise (new VkException (error.Error))
                    | Ok response -> response
        }

