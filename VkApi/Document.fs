namespace VkApi

open Newtonsoft.Json
open System


[<CustomEquality; NoComparison>]
type Document =
    struct
        val Id: uint64
        val OwnerId: uint64
        val Title: string
        val Size: uint64
        val Date: DateTime

        [<JsonConstructor>]
        internal new (id, owner_id, title, size, date) =
            let toDateTime date =
                let unixEpoch = DateTime (1970, 1, 1, 0, 0, 0, 0)
                date |> unixEpoch.AddSeconds
            {
                Id = id
                OwnerId = owner_id
                Title = title
                Size = size
                Date = date |> float |> toDateTime
            }
    end

    interface IEquatable<Document> with
        member self.Equals document = self.Id = document.Id

    override self.Equals obj =
        match obj with
        | :? IEquatable<Document> as doc -> doc.Equals <| self
        | _ -> false

    override self.GetHashCode () = int self.Id