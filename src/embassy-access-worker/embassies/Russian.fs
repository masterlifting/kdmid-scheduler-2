﻿module internal EmbassyAccess.Worker.Embassies.Russian

open System
open Infrastructure
open Persistence.Domain
open Worker.Domain
open EmbassyAccess.Domain
open EmbassyAccess.Persistence
open EmbassyAccess.Embassies.Russian.Domain

let private createConfig (schedule: Schedule option) =
    { TimeShift = schedule |> Option.map _.TimeShift |> Option.defaultValue 0y }

let private processRequest notify ct config storage request =

    let processRequest ct config storage =
        (storage, config, ct)
        |> EmbassyAccess.Deps.Russian.processRequest
        |> EmbassyAccess.Api.processRequest

    processRequest ct config storage request
    |> ResultAsync.bind' notify
    |> ResultAsync.map (fun request -> request.State |> string)

let private run country getRequests processRequests =
    fun (_, schedule, ct) ->
        Persistence.Storage.create InMemory
        |> ResultAsync.wrap (fun storage ->
            storage
            |> getRequests ct country
            |> ResultAsync.bind' (processRequests ct schedule storage))

module private SearchAppointments =

    let private getRequests ct country storage =
        let filter = Russian country |> Filter.Request.SearchAppointments
        storage |> Repository.Query.Request.get ct filter

    let private processRequests ct schedule storage requests =

        let config = createConfig schedule

        let notifySubscribers request = async { return Ok request }

        let processRequest = processRequest notifySubscribers ct config storage

        let uniqueRequests = requests |> Seq.filter _.GroupBy.IsNone

        let groupedRequests =
            requests
            |> Seq.filter _.GroupBy.IsSome
            |> Seq.groupBy _.GroupBy.Value
            |> Map
            |> Map.map (fun _ requests -> requests |> Seq.truncate 5)

        let groupedRequests =
            match uniqueRequests |> Seq.isEmpty with
            | true -> groupedRequests
            | false -> groupedRequests |> Map.add "Unique" uniqueRequests

        let processGroupedRequests groups =

            let rec choose (errors: Error' list) requests =
                async {
                    match requests with
                    | [] ->
                        return
                            match errors.Length with
                            | 0 -> Ok None
                            | 1 -> Error errors[0].Message
                            | _ ->
                                let msg =
                                    errors
                                    |> List.mapi (fun i error -> $"%i{i + 1}.%s{error.Message}")
                                    |> String.concat Environment.NewLine

                                Error $"Multiple errors: %s{Environment.NewLine}%s{msg}"

                    | request :: requestsTail ->
                        match! request |> processRequest with
                        | Error error -> return! choose (errors @ [ error ]) requestsTail
                        | Ok result -> return Ok <| Some result
                }

            let processGroup key group =
                group
                |> Seq.toList
                |> choose []
                |> ResultAsync.map (fun result ->
                    match result with
                    | Some result -> $"'%s{key}': %s{result}"
                    | None -> $"'%s{key}'. No results.")
                |> ResultAsync.mapError (fun error -> $"'{key}': %s{error}")

            groups |> Map.map processGroup |> Map.values |> Async.Parallel

        let toWorkerResult (results: Result<string, string> array) =
            let msgs, errors = results |> Result.unzip

            match msgs, errors with
            | [], [] -> Ok <| Debug "No results."
            | [], errors ->

                Error
                <| Operation
                    { Message = Environment.NewLine + (errors |> String.concat Environment.NewLine)
                      Code = None }

            | msgs, [] ->

                Ok <| Info(msgs |> String.concat Environment.NewLine)

            | msgs, errors ->

                Ok
                <| Warn(Environment.NewLine + (msgs @ errors |> String.concat Environment.NewLine))

        async {
            let! results = groupedRequests |> processGroupedRequests
            return results |> toWorkerResult
        }

    let run country = run country getRequests processRequests

module private MakeAppointments =

    let private getRequests ct country storage =
        let filter = Russian country |> Filter.Request.MakeAppointments
        storage |> Repository.Query.Request.get ct filter

    let private processRequests ct schedule storage requests =

        let config = createConfig schedule

        let notifySubscribers request = async { return Ok request }

        let processRequest = processRequest notifySubscribers ct config storage

        let toWorkerResult (results: Result<string, Error'> array) =
            let msgs, errors = results |> Result.unzip

            match msgs, errors with
            | [], [] -> Ok <| Debug "No results."
            | [], errors ->

                Error
                <| Operation
                    { Message =
                        Environment.NewLine
                        + (errors |> List.map _.Message |> String.concat Environment.NewLine)
                      Code = None }
            | msgs, [] ->

                Ok <| Info(msgs |> String.concat Environment.NewLine)

            | msgs, errors ->

                Ok
                <| Warn(
                    Environment.NewLine
                    + (msgs @ (errors |> List.map _.Message) |> String.concat Environment.NewLine)
                )

        async {
            let! results = requests |> Seq.map processRequest |> Async.Sequential
            return results |> toWorkerResult
        }

    let run country = run country getRequests processRequests

let addTasks country =
    Graph.Node(
        { Name = "Russian"; Task = None },
        [ Graph.Node(
              { Name = "Search appointments"
                Task = Some <| SearchAppointments.run country },
              []
          )
          Graph.Node(
              { Name = "Make appointments"
                Task = Some <| MakeAppointments.run country },
              []
          ) ]
    )
