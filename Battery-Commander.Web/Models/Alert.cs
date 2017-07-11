namespace BatteryCommander.Web.Models
{
    public class Alert
    {
        // Id

        // SentAt

        // Recipients - IDs? - Pull Phone, Email from Soldier entity

        // Format - SMS? Email? Phone?

        // Message / Content

        // Status - Open, Closed -- Only one open at a time, all responses will be attributed to that alert
        // Responses - FROM, CONTENT

        // Request Ack T/F?
        // What about sending a short-URL to acknowledge receipt? https://abcd.red-leg-dev.com/Alert/{id}/Acknowledge
        // User enters name/identifying info, accessible from phones, can provide more context
        // OR, each user gets a custom URL based on receipient hash so that we don't even need more than a big ACK button?
    }
}