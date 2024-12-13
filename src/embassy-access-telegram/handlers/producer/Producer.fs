﻿[<RequireQualifiedAccess>]
module EA.Telegram.Handlers.Producer.Core

open System
open Infrastructure.Domain
open Infrastructure.Prelude
open Web.Telegram.Producer
open Web.Telegram.Domain.Producer
open EA.Core.Domain
open EA.Core.DataAccess
open EA.Telegram.DataAccess
open EA.Telegram.Dependencies.Producer

let createAppointments (embassy: EmbassyNode, appointments: Set<Appointment>) =
    fun (deps: Core.Dependencies) ->
        deps.RequestStorage
        |> Request.Query.findManyByEmbassyName embassy.Name
        |> ResultAsync.map (Seq.map _.Id)
        |> ResultAsync.bindAsync (fun subscriptions ->
            deps.initChatStorage ()
            |> ResultAsync.wrap (Chat.Query.findManyBySubscriptions subscriptions))
        |> ResultAsync.map (
            Seq.map (fun chat ->
                let buttons =
                    appointments
                    |> Seq.map (fun appointment ->
                        (embassy.Id, appointment.Id)
                        |> EA.Telegram.Command.ChooseAppointments
                        |> EA.Telegram.Command.set,
                        appointment.Description)
                    |> Map

                (chat.Id, New)
                |> Buttons.create
                    { Name = $"Choose the appointment for '{embassy}'"
                      Columns = 1
                      Data = buttons })
        )

let createConfirmation (requestId: RequestId, embassy: EmbassyNode, confirmations: Set<Confirmation>) =
    fun (deps: Core.Dependencies) ->
        deps.initChatStorage ()
        |> ResultAsync.wrap (Chat.Query.findManyBySubscription requestId)
        |> ResultAsync.map (
            Seq.map (fun chat ->
                confirmations
                |> Seq.map (fun confirmation -> $"'{embassy.Name}'. Confirmation: {confirmation.Description}")
                |> String.concat "\n"
                |> fun msg -> (chat.Id, New) |> Text.create msg)
        )

let createError (requestId: RequestId, error: Error') =
    fun (deps: Core.Dependencies) ->
        deps.initChatStorage ()
        |> ResultAsync.wrap (Chat.Query.findManyBySubscription requestId)
        |> ResultAsync.map (Seq.map (fun chat -> Web.Telegram.Producer.Text.createError error chat.Id))
