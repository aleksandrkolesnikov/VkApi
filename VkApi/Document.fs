namespace VkApi

open Newtonsoft.Json
open System


type Document [<JsonConstructor>] internal (id: uint64, owner_id: uint64, title: string, size: uint64, date: uint64, url: string) =
    let dateTime =
        let unixEpoch = DateTime (1970, 1, 1, 0, 0, 0, 0)
        date |> float |> unixEpoch.AddSeconds

    let mutable hash = Array.empty<byte>

    interface IEquatable<Document> with
        member self.Equals document = (self.Id = document.Id) && (self.OwnerId = document.OwnerId)

    override self.Equals obj =
        match obj with
        | :? IEquatable<Document> as doc -> doc.Equals self
        | _ -> false

    override self.GetHashCode () = (self.Id ^^^ self.OwnerId) |> int

    member _.Id = id
    member _.OwnerId = owner_id
    member _.Title = title
    member _.Size = size
    member _.Date = dateTime
    member _.Url = url
    member _.Hash
        with get () = hash
        and set (value) = hash <- value