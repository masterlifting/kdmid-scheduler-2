﻿[<RequireQualifiedAccess>]
module EA.Embassies.Russian.API

open Infrastructure
open EA.Embassies.Russian.Domain

let get service =
    match service with
    | Passport service ->
        match service with
        | IssueForeign(deps, request) -> service.Name |> request.Create |> Kdmid.Request.start deps request.TimeZone
        | CheckReadiness _ -> service.Name |> NotSupported |> Error |> async.Return
    | Notary service ->
        match service with
        | PowerOfAttorney(deps, request) -> service.Name |> request.Create |> Kdmid.Request.start deps request.TimeZone
    | Citizenship service ->
        match service with
        | CitizenshipRenunciation(deps, request) ->
            service.Name |> request.Create |> Kdmid.Request.start deps request.TimeZone