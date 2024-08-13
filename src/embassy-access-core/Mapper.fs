[<RequireQualifiedAccess>]
module internal EmbassyAccess.Mapper

open System
open Infrastructure
open EmbassyAccess.Domain

module City =
    [<Literal>]
    let Belgrade = nameof City.Belgrade

    [<Literal>]
    let Berlin = nameof City.Berlin

    [<Literal>]
    let Budapest = nameof City.Budapest

    [<Literal>]
    let Sarajevo = nameof City.Sarajevo

    [<Literal>]
    let Podgorica = nameof City.Podgorica

    [<Literal>]
    let Tirana = nameof City.Tirana

    [<Literal>]
    let Paris = nameof City.Paris

    [<Literal>]
    let Rome = nameof City.Rome

    [<Literal>]
    let Dublin = nameof City.Dublin

    [<Literal>]
    let Bern = nameof City.Bern

    [<Literal>]
    let Helsinki = nameof City.Helsinki

    [<Literal>]
    let Hague = nameof City.Hague

    [<Literal>]
    let Ljubljana = nameof City.Ljubljana

    let toExternal city =
        let result = External.City()

        match city with
        | City.Belgrade -> result.Name <- Belgrade
        | City.Berlin -> result.Name <- Berlin
        | City.Budapest -> result.Name <- Budapest
        | City.Sarajevo -> result.Name <- Sarajevo
        | City.Podgorica -> result.Name <- Podgorica
        | City.Tirana -> result.Name <- Tirana
        | City.Paris -> result.Name <- Paris
        | City.Rome -> result.Name <- Rome
        | City.Dublin -> result.Name <- Dublin
        | City.Bern -> result.Name <- Bern
        | City.Helsinki -> result.Name <- Helsinki
        | City.Hague -> result.Name <- Hague
        | City.Ljubljana -> result.Name <- Ljubljana

        result

    let toInternal (city: External.City) =
        match city.Name with
        | Belgrade -> City.Belgrade |> Ok
        | Berlin -> City.Berlin |> Ok
        | Budapest -> City.Budapest |> Ok
        | Sarajevo -> City.Sarajevo |> Ok
        | Podgorica -> City.Podgorica |> Ok
        | Tirana -> City.Tirana |> Ok
        | Paris -> City.Paris |> Ok
        | Rome -> City.Rome |> Ok
        | Dublin -> City.Dublin |> Ok
        | Bern -> City.Bern |> Ok
        | Helsinki -> City.Helsinki |> Ok
        | Hague -> City.Hague |> Ok
        | Ljubljana -> City.Ljubljana |> Ok
        | _ -> Error <| NotSupported $"City {city.Name}."

module Country =
    [<Literal>]
    let Serbia = nameof Country.Serbia

    [<Literal>]
    let Germany = nameof Country.Germany

    [<Literal>]
    let Bosnia = nameof Country.Bosnia

    [<Literal>]
    let Montenegro = nameof Country.Montenegro

    [<Literal>]
    let Albania = nameof Country.Albania

    [<Literal>]
    let Hungary = nameof Country.Hungary

    [<Literal>]
    let Ireland = nameof Country.Ireland

    [<Literal>]
    let Switzerland = nameof Country.Switzerland

    [<Literal>]
    let Finland = nameof Country.Finland

    [<Literal>]
    let France = nameof Country.France

    [<Literal>]
    let Netherlands = nameof Country.Netherlands

    [<Literal>]
    let Slovenia = nameof Country.Slovenia

    let toExternal country =
        let result = External.Country()

        let city =
            match country with
            | Country.Serbia city ->
                result.Name <- Serbia
                city
            | Country.Germany city ->
                result.Name <- Germany
                city
            | Country.Bosnia city ->
                result.Name <- Bosnia
                city
            | Country.Montenegro city ->
                result.Name <- Montenegro
                city
            | Country.Albania city ->
                result.Name <- Albania
                city
            | Country.Hungary city ->
                result.Name <- Hungary
                city
            | Country.Ireland city ->
                result.Name <- Ireland
                city
            | Country.Switzerland city ->
                result.Name <- Switzerland
                city
            | Country.Finland city ->
                result.Name <- Finland
                city
            | Country.France city ->
                result.Name <- France
                city
            | Country.Netherlands city ->
                result.Name <- Netherlands
                city
            | Country.Slovenia city ->
                result.Name <- Slovenia
                city

        result.City <- city |> City.toExternal

        result

    let toInternal (country: External.Country) =
        country.City
        |> City.toInternal
        |> Result.bind (fun city ->
            match country.Name with
            | Serbia -> Country.Serbia city |> Ok
            | Germany -> Country.Germany city |> Ok
            | Bosnia -> Country.Bosnia city |> Ok
            | Montenegro -> Country.Montenegro city |> Ok
            | Albania -> Country.Albania city |> Ok
            | Hungary -> Country.Hungary city |> Ok
            | Ireland -> Country.Ireland city |> Ok
            | Switzerland -> Country.Switzerland city |> Ok
            | Finland -> Country.Finland city |> Ok
            | France -> Country.France city |> Ok
            | Netherlands -> Country.Netherlands city |> Ok
            | Slovenia -> Country.Slovenia city |> Ok
            | _ -> Error <| NotSupported $"Country {country.Name}.")

module Embassy =
    [<Literal>]
    let Russian = nameof Embassy.Russian

    [<Literal>]
    let Spanish = nameof Embassy.Spanish

    [<Literal>]
    let Italian = nameof Embassy.Italian

    [<Literal>]
    let French = nameof Embassy.French

    [<Literal>]
    let German = nameof Embassy.German

    [<Literal>]
    let British = nameof Embassy.British

    let toExternal embassy =
        let result = External.Embassy()

        let country =
            match embassy with
            | Embassy.Russian country ->
                result.Name <- Russian
                country
            | Embassy.Spanish country ->
                result.Name <- Spanish
                country
            | Embassy.Italian country ->
                result.Name <- Italian
                country
            | Embassy.French country ->
                result.Name <- French
                country
            | Embassy.German country ->
                result.Name <- German
                country
            | Embassy.British country ->
                result.Name <- British
                country

        result.Country <- country |> Country.toExternal
        result

    let toInternal (embassy: External.Embassy) =
        embassy.Country
        |> Country.toInternal
        |> Result.bind (fun country ->
            match embassy.Name with
            | Russian -> Embassy.Russian country |> Ok
            | Spanish -> Embassy.Spanish country |> Ok
            | Italian -> Embassy.Italian country |> Ok
            | French -> Embassy.French country |> Ok
            | German -> Embassy.German country |> Ok
            | British -> Embassy.British country |> Ok
            | _ -> Error <| NotSupported $"Embassy {embassy.Name}.")

module Confirmation =
    let toExternal (confirmation: Confirmation option) =
        confirmation
        |> Option.map (fun x ->
            let result = External.Confirmation()

            result.Description <- x.Description

            result)

    let toInternal (confirmation: External.Confirmation option) =
        confirmation |> Option.map (fun x -> { Description = x.Description })

module Appointment =
    let toExternal (appointment: Appointment) =
        let result = External.Appointment()

        result.Value <- appointment.Value
        result.Confirmation <- appointment.Confirmation |> Confirmation.toExternal
        result.Description <- appointment.Description |> Option.defaultValue ""
        result.DateTime <- appointment.Date.ToDateTime(appointment.Time)

        result

    let toInternal (appointment: External.Appointment) =
        { Value = appointment.Value
          Date = DateOnly.FromDateTime(appointment.DateTime)
          Time = TimeOnly.FromDateTime(appointment.DateTime)
          Confirmation = appointment.Confirmation |> Confirmation.toInternal
          Description =
            match appointment.Description with
            | AP.IsString x -> Some x
            | _ -> None }

module ConfirmationOption =
    [<Literal>]
    let FirstAvailable = nameof ConfirmationOption.FirstAvailable

    [<Literal>]
    let Range = nameof ConfirmationOption.DateTimeRange

    [<Literal>]
    let ForAppointment = nameof ConfirmationOption.ForAppointment

    let toExternal option =
        option
        |> Option.map (fun option ->

            let result = External.ConfirmationOption()

            match option with
            | ConfirmationOption.FirstAvailable -> result.Type <- FirstAvailable
            | ConfirmationOption.DateTimeRange(min, max) ->
                result.Type <- Range
                result.DateStart <- Nullable min
                result.DateEnd <- Nullable max
            | ConfirmationOption.ForAppointment appointment ->
                result.Type <- ForAppointment
                result.Appointment <- appointment |> Appointment.toExternal |> Some

            result)

    let toInternal (option: External.ConfirmationOption) =
        match option.Type with
        | FirstAvailable -> ConfirmationOption.FirstAvailable |> Ok
        | Range ->
            match option.DateStart |> Option.ofNullable, option.DateEnd |> Option.ofNullable with
            | Some min, Some max -> ConfirmationOption.DateTimeRange(min, max) |> Ok
            | _ -> Error <| NotFound "DateStart or DateEnd."
        | ForAppointment ->
            match option.Appointment with
            | Some appointment -> appointment |> Appointment.toInternal |> ConfirmationOption.ForAppointment |> Ok
            | _ -> Error <| NotFound "Appointment."
        | _ -> Error <| NotSupported $"ConfirmationOption {option.Type}."

module RequestState =
    [<Literal>]
    let Created = nameof RequestState.Created

    [<Literal>]
    let InProcess = nameof RequestState.InProcess

    [<Literal>]
    let Completed = nameof RequestState.Completed

    [<Literal>]
    let Failed = nameof RequestState.Failed

    let toExternal state =
        let result = External.RequestState()

        match state with
        | RequestState.Created -> result.Type <- Created
        | RequestState.InProcess -> result.Type <- InProcess
        | RequestState.Completed -> result.Type <- Completed
        | RequestState.Failed error ->
            result.Type <- Failed
            result.Error <- error |> Mapper.Error.toExternal |> Some

        result

    let toInternal (state: External.RequestState) =
        match state.Type with
        | Created -> RequestState.Created |> Ok
        | InProcess -> RequestState.InProcess |> Ok
        | Completed -> RequestState.Completed |> Ok
        | Failed ->
            match state.Error with
            | Some error -> error |> Mapper.Error.toInternal |> Result.map RequestState.Failed
            | None -> Error <| NotSupported "Failed state without error"
        | _ -> Error <| NotSupported $"Request state {state.Type}."

module Request =
    let toExternal request =
        let result = External.Request()

        result.Id <- request.Id.Value
        result.Payload <- request.Payload
        result.Embassy <- request.Embassy |> Embassy.toExternal
        result.State <- request.State |> RequestState.toExternal
        result.Attempt <- request.Attempt
        result.ConfirmationOption <- request.ConfirmationOption |> ConfirmationOption.toExternal
        result.Appointments <- request.Appointments |> Seq.map Appointment.toExternal |> Seq.toArray
        result.Description <- request.Description |> Option.defaultValue ""
        result.Modified <- request.Modified

        result

    let toInternal (request: External.Request) =
        request.Embassy
        |> Embassy.toInternal
        |> Result.bind (fun embassy ->
            request.State
            |> RequestState.toInternal
            |> Result.bind (fun state ->
                let result =
                    { Id = RequestId(request.Id)
                      Payload = request.Payload
                      Embassy = embassy
                      State = state
                      Attempt = request.Attempt
                      ConfirmationOption = None
                      Appointments = request.Appointments |> Seq.map Appointment.toInternal |> Set.ofSeq
                      Description =
                        match request.Description with
                        | AP.IsString x -> Some x
                        | _ -> None
                      Modified = request.Modified }

                match request.ConfirmationOption with
                | None -> Ok result
                | Some option ->
                    option
                    |> ConfirmationOption.toInternal
                    |> Result.map (fun x ->
                        { result with
                            ConfirmationOption = Some x })))
