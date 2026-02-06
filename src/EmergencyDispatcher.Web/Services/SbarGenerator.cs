using EmergencyDispatcher.Web.Models;

namespace EmergencyDispatcher.Web.Services;

public class SbarGenerator : ISbarGenerator
{
    private readonly string _serviceName;

    public SbarGenerator(IConfiguration config)
    {
        _serviceName = config["ServiceName"] ?? "Emergency Dispatch Service";
    }

    public string GenerateCallScript(Case emergencyCase, Member? member, ApplicationUser dispatcher)
    {
        return $"""
            S — Situation
            Good day, this is {dispatcher.FullName} from {_serviceName}.
            I am calling to pre-notify an incoming emergency patient.

            B — Background
            Patient: {emergencyCase.PatientName}, {emergencyCase.Age}, {emergencyCase.Sex}
            Known conditions: {member?.MedicalConditions ?? "None reported"}
            Medications: {member?.Medications ?? "None reported"}
            Allergies: {member?.Allergies ?? "None reported"}

            A — Assessment
            Current condition: {emergencyCase.EmergencyType}
            Transport method: {emergencyCase.TransportMethod ?? "Unknown"}
            Estimated arrival: {emergencyCase.EstimatedEta ?? 0} minutes
            Origin: {emergencyCase.LocationText ?? "Unknown"}

            R — Recommendation
            Requesting ER triage preparation.
            Contact: {dispatcher.PhoneNumber}
            """;
    }

    public string GenerateMessage(Case emergencyCase, Member? member, ApplicationUser dispatcher)
    {
        return $"""
            SBAR PRE-NOTIFICATION
            S: Incoming emergency patient
            Name: {emergencyCase.PatientName}
            Age/Sex: {emergencyCase.Age} / {emergencyCase.Sex}
            Condition: {emergencyCase.EmergencyType}

            B: History: {member?.MedicalConditions ?? "None reported"}
            Meds: {member?.Medications ?? "None reported"}
            Allergies: {member?.Allergies ?? "None reported"}

            A: Status: {emergencyCase.Notes ?? emergencyCase.EmergencyType}
            ETA: {emergencyCase.EstimatedEta} mins
            From: {emergencyCase.LocationText}

            R: Please prepare ER triage
            Dispatcher: {dispatcher.FullName}, {dispatcher.PhoneNumber}
            """;
    }

    public string GenerateEmail(Case emergencyCase, Member? member, ApplicationUser dispatcher)
    {
        return $"""
            Subject: SBAR Pre-Notification — Incoming Emergency Patient

            S — Incoming emergency patient arriving shortly.

            B — Background
            Patient: {emergencyCase.PatientName}, {emergencyCase.Age}, {emergencyCase.Sex}
            Known conditions: {member?.MedicalConditions ?? "None reported"}
            Medications: {member?.Medications ?? "None reported"}
            Allergies: {member?.Allergies ?? "None reported"}

            A — Assessment
            Current condition: {emergencyCase.EmergencyType}
            Transport: {emergencyCase.TransportMethod ?? "Unknown"}
            ETA: {emergencyCase.EstimatedEta ?? 0} minutes
            Origin: {emergencyCase.LocationText ?? "Unknown"}

            R — Request ER readiness and confirmation.
            Dispatcher: {dispatcher.FullName}, {dispatcher.PhoneNumber}
            """;
    }
}
