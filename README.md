# Veteran Analytics System

Veteran Analytics System is a .NET web application meant to provide a dashboard of analytics for veteran-assistance program data imported from Google Forms and Ragic.

## Local Setup

1. You will need the following in your user secrets
    ```
    {
      "RagicAPIKey": "[ApiKey]",
      "SendGridKey": "[ApiKey]",
      "FromEmail":  "[FromEmail]"
    }
    ```
1. Publish the database to your local sql server instance. The default database name is `VeteranAnalytics`
