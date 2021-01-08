open System.Threading.Tasks
open System.Collections.Generic;


let Add4 x = x + 4

let MultiplyBy3 x = x * 3

let Memoize func =
    let cache = new Dictionary<_, _> ()
    fun arg ->
        if cache.ContainsKey arg then cache.[arg]
        else
            let result = func arg
            cache.[arg] <- result
            result


let rec Fact n =
    match n with
    | 1 -> 1
    | _ -> n * Fact (n - 1)


[<EntryPoint>]
let main argv =
    let r = Fact 15
    let MemFact = Memoize Fact
    let mr = MemFact 15
    let mr2 = MemFact 15

    printfn "%d" r
    printfn "%d" mr

    0
