namespace VkApi.TinyHttp

open System.IO
open FSharp.Control.Tasks.V2


type Content = {
    Name: string
    Content: Stream
}

type HttpRequest =
    | Get of string
    | Post of string * Content

[<RequireQualifiedAccess; CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module internal HttpRequest =

    open System.Net.Http


    let private client = new HttpClient ()

    let private GetAsync (url: string) =
        url
        |> client.GetStreamAsync

    let private PostAsync (url:string) content =
        task {
            let httpContent = new MultipartFormDataContent ()
            use innerContent = new StreamContent (content.Content)
            httpContent.Add (innerContent, "file", content.Name)
            let! response =
                (url, httpContent)
                |> client.PostAsync

            return! response.Content.ReadAsStreamAsync ()
        }

    let SendAsync request =
        match request with
        | Get url -> url |> GetAsync
        | Post (url, content) -> PostAsync url content
