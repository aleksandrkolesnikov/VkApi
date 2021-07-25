namespace VkApi.TinyHttp

open System


type HttpRequestBuilder () =
    let MakeGet = Get >> Some

    let MakePost = Post >> Some

    member _.Run value =
        match value with
        | None -> raise <| ArgumentException ("Empty Request")
        | Some request -> HttpRequest.SendAsync request

    member _.Yield _ = None

    [<CustomOperation "GET">]
    member _.Get (value, uri) =
        match value with
        | None ->
            uri |> MakeGet
        | Some request ->
            match request with
            | Get _ ->
                uri |> MakeGet
            | Post _ -> raise <| ArgumentException ("Could not change Get Reqeust to Post Reqeust")

    [<CustomOperation "POST">]
    member _.Post (value, uri) =
        match value with
        | None ->
            (uri, { Name = ""; Content = null })
            |> MakePost
        | Some request ->
            match request with
            | Get _ -> raise <| ArgumentException ("Could not change Post Reqeust to Get Reqeust")
            | Post (_, content) ->
                (uri, content)
                |> MakePost

    [<CustomOperation "Content">]
    member _.Content (value, content) =
        match value with
        | None ->
            ("", content)
            |> MakePost
        | Some request ->
            match request with
            | Get _ -> raise <| ArgumentException ("Could not use Get Request with Content")
            | Post (uri, _) ->
                (uri, content)
                |> MakePost

[<AutoOpen>]
module HttpRequestBuilderModule =
    let http = HttpRequestBuilder ()
