namespace VkApi.TinyHttp

open System.IO


[<NoComparison>]
type Content = {
    Name: string
    Content: Stream
}

[<NoComparison>]
type HttpRequest =
    | Get of string
    | Post of string * Content

[<RequireQualifiedAccess; CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module internal HttpRequest =

    open System
    open System.Text
    open System.Net


    let private AsyncGet (url: string)  =
        async {
            let request = WebRequest.CreateHttp url
            let! response = request.AsyncGetResponse ()

            return response.GetResponseStream ()
        }

    let private AsyncPost (url: string) (content: Content) =
        let (|FileExtension|) (filename: string) = Path.GetExtension filename

        async {
            let boundary = Guid.NewGuid ()
            let contentType =
                match content.Name with
                | FileExtension ".jpg"
                | FileExtension ".jpeg" -> "image/jpeg"
                | FileExtension ".txt" -> "text/plain"
                | _ -> "application/octet-stream"

            let header =
                $"--{boundary}\r\nContent-Disposition: form-data; name=file; filename=\"{content.Name}\"\r\nContent-Type:{contentType}\r\n\r\n"
                |> Encoding.UTF8.GetBytes

            let footer =
                $"\r\n--{boundary}--\r\n"
                |> Encoding.UTF8.GetBytes

            let count = content.Content.Length |> int
            let! data = content.Content.AsyncRead count
            let body =
                seq { header; data; footer }
                |> Array.concat

            let request = WebRequest.Create url
            request.Method <- "POST"
            request.ContentType <- $"multipart/form-data; boundary={boundary}"
            let! stream = Async.FromBeginEnd (request.BeginGetRequestStream, request.EndGetRequestStream)
            do! stream.AsyncWrite (body, 0, body.Length)
            let! response = request.AsyncGetResponse ()

            return response.GetResponseStream ()
        }

    let AsyncMake request =
        match request with
        | Get url -> url |> AsyncGet
        | Post (url, content) -> AsyncPost url content
